using SugarRush.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Placeholder UI for glucose bar. Wires to a Slider and optional zone color image.
    /// </summary>
    public class GlucoseUI : MonoBehaviour
    {
        [SerializeField] private GlucoseSystem _glucoseSystem;
        [SerializeField] private Slider _slider;
        [SerializeField] private Image _fillImage;

        [Header("Zone Colors")]
        [SerializeField] private Color _safeColor = Color.green;
        [SerializeField] private Color _lowWarningColor = Color.yellow;
        [SerializeField] private Color _lowCrisisColor = Color.red;
        [SerializeField] private Color _highWarningColor = new Color(1f, 0.6f, 0f);
        [SerializeField] private Color _highCrisisColor = Color.magenta;

        private void OnEnable()
        {
            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnValueChanged += UpdateBar;
                _glucoseSystem.OnZoneChanged += UpdateColor;
            }
        }

        private void OnDisable()
        {
            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnValueChanged -= UpdateBar;
                _glucoseSystem.OnZoneChanged -= UpdateColor;
            }
        }

        private void Start()
        {
            if (_glucoseSystem == null)
            {
                Debug.LogWarning("[GlucoseUI] GlucoseSystem not assigned.", this);
                enabled = false;
                return;
            }

            if (_slider == null) _slider = GetComponent<Slider>();
            UpdateBar(_glucoseSystem.CurrentValue);
            UpdateColor(_glucoseSystem.CurrentZone);
        }

        private void UpdateBar(float value)
        {
            if (_slider != null)
            {
                _slider.value = Mathf.Clamp01(value / 100f);
            }
        }

        private void UpdateColor(GlucoseZone zone)
        {
            if (_fillImage == null) return;

            _fillImage.color = zone switch
            {
                GlucoseZone.LowCrisis => _lowCrisisColor,
                GlucoseZone.LowWarning => _lowWarningColor,
                GlucoseZone.Safe => _safeColor,
                GlucoseZone.HighWarning => _highWarningColor,
                GlucoseZone.HighCrisis => _highCrisisColor,
                _ => _safeColor
            };
        }
    }
}
