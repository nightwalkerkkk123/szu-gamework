using SugarRush.GameFlow;
using SugarRush.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// In-game HUD showing distance, speed, time and glucose value.
    /// Resolves references automatically so it can be dropped into any scene.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private MonoBehaviour _playerController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        private IPlayerController PlayerController => _playerController as IPlayerController;

        [SerializeField] private Text _distanceText;
        [SerializeField] private Text _speedText;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _glucoseText;

        private void Awake()
        {
            ResolveReferences();
            EnsureUI();
        }

        private void ResolveReferences()
        {
            if (_levelManager == null) _levelManager = FindObjectOfType<LevelManager>();
            if (_playerController == null)
            {
                _playerController = FindObjectOfType<SkiingController>();
            }
            if (_glucoseSystem == null) _glucoseSystem = FindObjectOfType<GlucoseSystem>();
        }

        private void EnsureUI()
        {
            if (_distanceText != null && _speedText != null && _timeText != null && _glucoseText != null)
                return;

            var canvas = new GameObject("SugarRushHUD");
            canvas.transform.SetParent(transform, false);
            var cv = canvas.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cv.sortingOrder = 100;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("Panel");
            panel.transform.SetParent(canvas.transform, false);
            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.5f);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.anchoredPosition = new Vector2(0f, -30f);
            panelRect.sizeDelta = new Vector2(0f, 60f);

            _distanceText = CreateText(panel.transform, "DistanceText", "距离: --", new Vector2(0.02f, 0.5f), TextAnchor.MiddleLeft);
            _speedText = CreateText(panel.transform, "SpeedText", "速度: --", new Vector2(0.30f, 0.5f), TextAnchor.MiddleLeft);
            _timeText = CreateText(panel.transform, "TimeText", "时间: --", new Vector2(0.55f, 0.5f), TextAnchor.MiddleLeft);
            _glucoseText = CreateText(panel.transform, "GlucoseText", "血糖: --", new Vector2(0.80f, 0.5f), TextAnchor.MiddleLeft);
        }

        private static Text CreateText(Transform parent, string name, string defaultText, Vector2 anchorX, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 22;
            text.color = Color.white;
            text.alignment = alignment;
            text.text = defaultText;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(anchorX.x, 0f);
            rect.anchorMax = new Vector2(anchorX.x + 0.18f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return text;
        }

        private void Update()
        {
            if (_levelManager != null)
            {
                float distance = _levelManager.DistanceTraveled;
                float target = _levelManager.Data != null ? _levelManager.Data.TargetDistanceMeters : 0f;
                float progress = target > 0f ? Mathf.Clamp01(distance / target) * 100f : 0f;
                SetText(_distanceText, $"距离: {distance:F0}m / {target:F0}m ({progress:F0}%)");
                SetText(_timeText, $"时间: {_levelManager.ElapsedTime:F1}s");
            }
            else
            {
                SetText(_distanceText, "距离: --");
                SetText(_timeText, "时间: --");
            }

            if (PlayerController != null)
            {
                SetText(_speedText, $"速度: {PlayerController.Speed:F1}m/s");
            }
            else
            {
                SetText(_speedText, "速度: --");
            }

            if (_glucoseSystem != null)
            {
                SetText(_glucoseText, $"血糖: {_glucoseSystem.CurrentValue:F0} ({_glucoseSystem.CurrentZone})");
            }
            else
            {
                SetText(_glucoseText, "血糖: --");
            }
        }

        private static void SetText(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
