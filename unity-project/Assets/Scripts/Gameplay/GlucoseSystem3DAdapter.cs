using SugarRush.Core;
using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Adapts the existing 2D GlucoseSystem visual feedback to a 3D player sphere.
    /// </summary>
    [RequireComponent(typeof(GlucoseSystem))]
    public class GlucoseSystem3DAdapter : MonoBehaviour
    {
        [SerializeField] private Renderer _bodyRenderer;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        [Header("Colors")]
        [SerializeField] private Color _normalColor = Color.cyan;
        [SerializeField] private Color _lowWarningColor = new Color(0.5f, 0.75f, 1f);
        [SerializeField] private Color _lowCrisisColor = new Color(0.25f, 0.45f, 0.9f);
        [SerializeField] private Color _highWarningColor = new Color(1f, 0.75f, 0.4f);
        [SerializeField] private Color _highCrisisColor = new Color(0.9f, 0.25f, 0.25f);

        private void Awake()
        {
            if (_glucoseSystem == null) _glucoseSystem = GetComponent<GlucoseSystem>();
            if (_bodyRenderer == null) _bodyRenderer = GetComponentInChildren<Renderer>();
        }

        private void Update()
        {
            if (_bodyRenderer == null || _glucoseSystem == null) return;

            _bodyRenderer.material.color = GetColorForZone(_glucoseSystem.CurrentZone);
        }

        private Color GetColorForZone(GlucoseZone zone)
        {
            return zone switch
            {
                GlucoseZone.LowCrisis => _lowCrisisColor,
                GlucoseZone.LowWarning => _lowWarningColor,
                GlucoseZone.HighWarning => _highWarningColor,
                GlucoseZone.HighCrisis => _highCrisisColor,
                _ => _normalColor,
            };
        }
    }
}
