using SugarRush.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Top-center banner that briefly appears with a colored message when the
    /// glucose zone changes. Fades out over 1s after a 0.5s delay.
    /// </summary>
    public class ZoneChangeBanner : MonoBehaviour
    {
        [SerializeField] private Canvas _parentCanvas;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        private Text _bannerText;
        private float _alpha = 0f;
        private float _fadeTimer = -1f; // -1 = idle
        private bool _isShowing;

        private const float DisplayDuration = 0.5f;
        private const float FadeOutDuration = 1f;

        private void Awake()
        {
            if (_glucoseSystem == null) _glucoseSystem = FindObjectOfType<GlucoseSystem>();
            EnsureUI();
        }

        private void OnEnable()
        {
            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged += OnZoneChanged;
            }
        }

        private void OnDisable()
        {
            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged -= OnZoneChanged;
            }
        }

        private void Update()
        {
            if (!_isShowing) return;

            _fadeTimer += Time.deltaTime;

            if (_fadeTimer >= DisplayDuration)
            {
                // Fade out
                float t = (_fadeTimer - DisplayDuration) / FadeOutDuration;
                _alpha = Mathf.Lerp(1f, 0f, t);
                if (_alpha <= 0.001f)
                {
                    _alpha = 0f;
                    _isShowing = false;
                    _fadeTimer = -1f;
                }
                ApplyAlpha();
            }
        }

        private void OnZoneChanged(GlucoseZone newZone)
        {
            ShowBanner(newZone);
        }

        private void ShowBanner(GlucoseZone zone)
        {
            _bannerText.text = ZoneLabel(zone);
            _bannerText.color = ZoneTextColor(zone);
            _alpha = 1f;
            _fadeTimer = 0f;
            _isShowing = true;
            ApplyAlpha();
        }

        private void ApplyAlpha()
        {
            if (_bannerText == null) return;
            var c = _bannerText.color;
            _bannerText.color = new Color(c.r, c.g, c.b, _alpha);
        }

        private static string ZoneLabel(GlucoseZone zone) => zone switch
        {
            GlucoseZone.LowCrisis => "进入低血糖危机区",
            GlucoseZone.LowWarning => "进入低血糖区",
            GlucoseZone.HighWarning => "进入高血糖区",
            GlucoseZone.HighCrisis => "进入高血糖危机区",
            GlucoseZone.Safe => "进入正常血糖区",
            _ => "血糖状态变化"
        };

        private static Color ZoneTextColor(GlucoseZone zone) => zone switch
        {
            GlucoseZone.LowCrisis => new Color(0.9f, 0.2f, 0.2f),    // red
            GlucoseZone.HighCrisis => new Color(0.85f, 0.2f, 0.85f), // magenta
            GlucoseZone.LowWarning => new Color(1f, 0.6f, 0f),       // orange
            GlucoseZone.HighWarning => new Color(1f, 0.6f, 0f),      // orange
            _ => Color.white
        };

        private void EnsureUI()
        {
            if (_bannerText != null) return;

            var canvas = _parentCanvas != null ? _parentCanvas : FindObjectOfType<Canvas>();

            var go = new GameObject("ZoneChangeBanner");
            go.transform.SetParent(canvas != null ? canvas.transform : transform, false);

            var bg = go.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0f); // transparent background

            _bannerText = go.AddComponent<Text>();
            _bannerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _bannerText.fontSize = 28;
            _bannerText.alignment = TextAnchor.MiddleCenter;
            _bannerText.color = Color.white;
            _bannerText.text = "";
            _bannerText.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.2f, 0.88f);
            rect.anchorMax = new Vector2(0.8f, 0.98f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}