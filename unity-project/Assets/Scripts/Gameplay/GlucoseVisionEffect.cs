using SugarRush.Core;
using SugarRush.Foundation;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Screen-space visual feedback for glucose zones.
    /// Uses a full-screen overlay image that tints/pulses in crisis states.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class GlucoseVisionEffect : MonoBehaviour
    {
        [SerializeField] private GlucoseSystem _glucoseSystem;

        [Header("Overlay Colors")]
        [SerializeField] private Color _normalColor = new Color(0f, 0f, 0f, 0f);
        [SerializeField] private Color _lowWarningColor = new Color(0.2f, 0.4f, 0.9f, 0.15f);
        [SerializeField] private Color _highWarningColor = new Color(0.9f, 0.6f, 0.2f, 0.15f);
        [SerializeField] private Color _lowCrisisColor = new Color(0.1f, 0.25f, 0.8f, 0.35f);
        [SerializeField] private Color _highCrisisColor = new Color(0.8f, 0.15f, 0.15f, 0.35f);

        [Header("Frost Overlay")]
        [SerializeField] private Image _frostImage;
        [SerializeField] private Color _lowCrisisFrostColor = new Color(0.7f, 0.9f, 1f, 0.25f);
        [SerializeField] private Color _highCrisisFrostColor = new Color(1f, 0.7f, 0.5f, 0.15f);
        [SerializeField] private float _frostLerpSpeed = 3f;

        [Header("Crisis Pulse")]
        [SerializeField] private float _pulseSpeed = 3f;
        [SerializeField] private float _pulseAlphaAmplitude = 0.1f;

        [Header("Timing")]
        [SerializeField] private float _colorLerpSpeed = 4f;

        private Image _image;
        private Color _targetColor;
        private Color _targetFrostColor;
        private bool _isCrisis;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = _normalColor;
            _targetColor = _normalColor;
        }

        private void Start()
        {
            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged += HandleZoneChanged;
                UpdateTargetColor(_glucoseSystem.CurrentZone);
            }
            else
            {
                GameEvents.OnGlucoseZoneChanged += HandleZoneChanged;
            }
        }

        private void OnDestroy()
        {
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
            Color target = _targetColor;

            if (_isCrisis)
            {
                float pulse = Mathf.Sin(Time.time * _pulseSpeed) * _pulseAlphaAmplitude;
                target.a = Mathf.Clamp01(target.a + pulse);
            }

            _image.color = Color.Lerp(_image.color, target, Time.deltaTime * _colorLerpSpeed);

            if (_frostImage != null)
            {
                _frostImage.color = Color.Lerp(_frostImage.color, _targetFrostColor, Time.deltaTime * _frostLerpSpeed);
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
                GlucoseZone.LowWarning => _lowWarningColor,
                GlucoseZone.HighWarning => _highWarningColor,
                _ => _normalColor
            };

            _targetFrostColor = zone switch
            {
                GlucoseZone.LowCrisis => _lowCrisisFrostColor,
                GlucoseZone.HighCrisis => _highCrisisFrostColor,
                _ => new Color(1f, 1f, 1f, 0f)
            };

            _isCrisis = zone == GlucoseZone.LowCrisis || zone == GlucoseZone.HighCrisis;
        }
    }
}
