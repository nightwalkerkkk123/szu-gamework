using SugarRush.Core;
using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Visual ski trail behind the player. Responds to grounded state, roll, stumble and glucose zone.
    /// </summary>
    [RequireComponent(typeof(TrailRenderer))]
    public class SkiTrail : MonoBehaviour
    {
        [SerializeField] private SkiingController _skiingController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        [Header("Trail Settings")]
        [SerializeField] private float _groundedTime = 0.4f;
        [SerializeField] private float _groundedWidthMultiplier = 0.25f;
        [SerializeField] private float _airTime = 0.1f;
        [SerializeField] private float _airWidthMultiplier = 0.05f;
        [SerializeField] private float _rollTime = 0.15f;
        [SerializeField] private float _rollWidthMultiplier = 0.5f;

        [Header("Colors")]
        [SerializeField] private Color _normalColor = new Color(0.85f, 0.95f, 1f, 0.7f);
        [SerializeField] private Color _lowCrisisColor = new Color(0.3f, 0.5f, 1f, 0.8f);
        [SerializeField] private Color _highCrisisColor = new Color(1f, 0.35f, 0.35f, 0.8f);
        [SerializeField] private Color _warningColorTint = new Color(0.9f, 0.9f, 0.7f, 0.75f);
        [SerializeField] private float _colorLerpSpeed = 8f;

        private TrailRenderer _trail;
        private Color _targetColor;

        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
            _trail.emitting = false;
            _targetColor = _normalColor;
        }

        private void Start()
        {
            if (_skiingController != null)
            {
                _skiingController.OnGroundedChanged += HandleGroundedChanged;
                _skiingController.OnRollingChanged += HandleRollingChanged;
                _skiingController.OnStumbledChanged += HandleStumbledChanged;
            }

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged += HandleZoneChanged;
                UpdateTargetColor(_glucoseSystem.CurrentZone);
            }
            else
            {
                GameEvents.OnGlucoseZoneChanged += HandleZoneChanged;
            }

            UpdateTrailAppearance();
        }

        private void OnDestroy()
        {
            if (_skiingController != null)
            {
                _skiingController.OnGroundedChanged -= HandleGroundedChanged;
                _skiingController.OnRollingChanged -= HandleRollingChanged;
                _skiingController.OnStumbledChanged -= HandleStumbledChanged;
            }

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged -= HandleZoneChanged;
            }
            else
            {
                GameEvents.OnGlucoseZoneChanged -= HandleZoneChanged;
            }
        }

        private void Update()
        {
            _trail.startColor = Color.Lerp(_trail.startColor, _targetColor, Time.deltaTime * _colorLerpSpeed);
            _trail.endColor = new Color(_trail.startColor.r, _trail.startColor.g, _trail.startColor.b, 0f);
        }

        private void HandleGroundedChanged(bool isGrounded)
        {
            UpdateTrailAppearance();
        }

        private void HandleRollingChanged(bool isRolling)
        {
            UpdateTrailAppearance();
        }

        private void HandleStumbledChanged(bool isStumbled)
        {
            if (isStumbled)
            {
                _trail.emitting = false;
                _trail.Clear();
            }
            else
            {
                UpdateTrailAppearance();
            }
        }

        private void HandleZoneChanged(GlucoseZone zone)
        {
            UpdateTargetColor(zone);
        }

        private void UpdateTargetColor(GlucoseZone zone)
        {
            _targetColor = zone switch
            {
                GlucoseZone.LowCrisis => _lowCrisisColor,
                GlucoseZone.HighCrisis => _highCrisisColor,
                GlucoseZone.LowWarning or GlucoseZone.HighWarning => _warningColorTint,
                _ => _normalColor
            };
        }

        private void UpdateTrailAppearance()
        {
            if (_skiingController == null) return;

            if (_skiingController.IsStumbled)
            {
                _trail.emitting = false;
                return;
            }

            if (_skiingController.IsRolling)
            {
                _trail.time = _rollTime;
                _trail.widthMultiplier = _rollWidthMultiplier;
                _trail.emitting = true;
                return;
            }

            if (_skiingController.IsGrounded)
            {
                _trail.time = _groundedTime;
                _trail.widthMultiplier = _groundedWidthMultiplier;
                _trail.emitting = true;
            }
            else
            {
                _trail.time = _airTime;
                _trail.widthMultiplier = _airWidthMultiplier;
                _trail.emitting = true;
            }
        }
    }
}
