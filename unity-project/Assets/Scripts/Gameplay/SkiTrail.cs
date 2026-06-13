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

        [Header("Speed/Glucose Boost")]
        [SerializeField] private float _speedForMaxTrail = 18f;
        [SerializeField] private float _maxWidthBoost = 0.35f;
        [SerializeField] private float _maxTimeBoost = 0.3f;
        [SerializeField] private float _glucoseBoostLerpSpeed = 4f;

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

        private float _currentBoost;

        private void Update()
        {
            _trail.startColor = Color.Lerp(_trail.startColor, _targetColor, Time.deltaTime * _colorLerpSpeed);
            _trail.endColor = new Color(_trail.startColor.r, _trail.startColor.g, _trail.startColor.b, 0f);

            UpdateSpeedGlucoseBoost();
        }

        private void UpdateSpeedGlucoseBoost()
        {
            float speed = _skiingController != null ? _skiingController.Velocity.magnitude : 0f;
            float speedBoost = Mathf.Lerp(0f, 1f, Mathf.Clamp01(speed / _speedForMaxTrail));

            float glucoseBoost = 0f;
            if (_glucoseSystem != null)
            {
                var zone = _glucoseSystem.CurrentZone;
                if (zone == GlucoseZone.HighCrisis || zone == GlucoseZone.HighWarning)
                {
                    glucoseBoost = 1f;
                }
            }

            float targetBoost = Mathf.Max(speedBoost, glucoseBoost);
            _currentBoost = Mathf.Lerp(_currentBoost, targetBoost, Time.deltaTime * _glucoseBoostLerpSpeed);

            // Width/time are updated in UpdateTrailAppearance; we re-apply each frame to account for boost.
            UpdateTrailAppearance();
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

            float widthBoost = _currentBoost * _maxWidthBoost;
            float timeBoost = _currentBoost * _maxTimeBoost;

            if (_skiingController.IsRolling)
            {
                _trail.time = _rollTime + timeBoost * 0.5f;
                _trail.widthMultiplier = _rollWidthMultiplier + widthBoost;
                _trail.emitting = true;
                return;
            }

            if (_skiingController.IsGrounded)
            {
                _trail.time = _groundedTime + timeBoost;
                _trail.widthMultiplier = _groundedWidthMultiplier + widthBoost;
                _trail.emitting = true;
            }
            else
            {
                _trail.time = _airTime + timeBoost * 0.3f;
                _trail.widthMultiplier = _airWidthMultiplier + widthBoost * 0.5f;
                _trail.emitting = true;
            }
        }
    }
}
