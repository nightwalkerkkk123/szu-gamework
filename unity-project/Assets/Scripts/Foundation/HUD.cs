using SugarRush.Core;
using SugarRush.GameFlow;
using SugarRush.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// In-game HUD showing distance, speed, time, glucose bar, distance progress
    /// and stumble/shield status indicators. Resolves references automatically so
    /// it can be dropped into any scene.
    /// </summary>
    public class HUD : MonoBehaviour
    {
        [Header("Systems (auto-resolved if null)")]
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private MonoBehaviour _playerController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        [Header("Text elements")]
        [SerializeField] private Text _distanceText;
        [SerializeField] private Text _speedText;
        [SerializeField] private Text _timeText;
        [SerializeField] private Text _glucoseText;

        [Header("Bars")]
        [SerializeField] private Slider _glucoseSlider;
        [SerializeField] private Image _glucoseFill;
        [SerializeField] private Image _distanceProgressFill;

        [Header("Status icons")]
        [SerializeField] private GameObject _stumbleIndicator;
        [SerializeField] private GameObject _shieldIndicator;
        [SerializeField] private GameObject _rollIndicator;

        [Header("Glucose zone colors")]
        [SerializeField] private Color _safeColor = new Color(0.4f, 0.85f, 0.4f);
        [SerializeField] private Color _lowWarningColor = new Color(0.95f, 0.85f, 0.2f);
        [SerializeField] private Color _lowCrisisColor = new Color(0.9f, 0.2f, 0.2f);
        [SerializeField] private Color _highWarningColor = new Color(1f, 0.6f, 0f);
        [SerializeField] private Color _highCrisisColor = new Color(0.85f, 0.2f, 0.85f);

        private IPlayerController PlayerController => _playerController as IPlayerController;
        private SkiingController _skiingConcrete;

        private void Awake()
        {
            ResolveReferences();
            EnsureUI();
            SubscribeGlucose();
        }

        private void OnDestroy()
        {
            UnsubscribeGlucose();
            UnsubscribeSkiing();
        }

        private void ResolveReferences()
        {
            if (_levelManager == null) _levelManager = FindObjectOfType<LevelManager>();
            if (_playerController == null)
            {
                var bridge = FindObjectOfType<EntitasPlayerBridge>();
                if (bridge != null) _playerController = bridge;
            }
            if (_glucoseSystem == null) _glucoseSystem = FindObjectOfType<GlucoseSystem>();
            if (_playerController is SkiingController concrete) _skiingConcrete = concrete;
        }

        private void SubscribeGlucose()
        {
            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnValueChanged += UpdateGlucoseBar;
                _glucoseSystem.OnZoneChanged += UpdateGlucoseColor;
                UpdateGlucoseBar(_glucoseSystem.CurrentValue);
                UpdateGlucoseColor(_glucoseSystem.CurrentZone);
            }
        }

        private void UnsubscribeGlucose()
        {
            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnValueChanged -= UpdateGlucoseBar;
                _glucoseSystem.OnZoneChanged -= UpdateGlucoseColor;
            }
        }

        private void SubscribeSkiing()
        {
            // Status indicators wire up once the SkiingController reference is
            // available. Re-call after scene rebuilds if the player respawns.
            if (_skiingConcrete != null)
            {
                _skiingConcrete.OnRollingChanged += UpdateRollIcon;
                _skiingConcrete.OnStumbledChanged += UpdateStumbleIcon;
                UpdateRollIcon(_skiingConcrete.IsRolling);
                UpdateStumbleIcon(_skiingConcrete.IsStumbled);
            }
        }

        private void UnsubscribeSkiing()
        {
            if (_skiingConcrete != null)
            {
                _skiingConcrete.OnRollingChanged -= UpdateRollIcon;
                _skiingConcrete.OnStumbledChanged -= UpdateStumbleIcon;
            }
        }

        private void Start()
        {
            SubscribeSkiing();
        }

        private void EnsureUI()
        {
            if (_distanceText != null && _speedText != null && _timeText != null
                && _glucoseText != null && _glucoseSlider != null && _distanceProgressFill != null)
                return;

            var canvas = new GameObject("SugarRushHUD");
            canvas.transform.SetParent(transform, false);
            var cv = canvas.AddComponent<Canvas>();
            cv.renderMode = RenderMode.ScreenSpaceOverlay;
            cv.sortingOrder = 100;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            // Top status bar: distance / speed / time / glucose
            var topPanel = CreatePanel(canvas.transform, "TopPanel",
                anchorX: 0f, anchorY: 1f, height: 60f, topOffset: 0f,
                color: new Color(0f, 0f, 0f, 0.5f));

            _distanceText = CreateText(topPanel.transform, "DistanceText", "距离: --",
                anchorX: 0.02f, width: 0.18f, alignment: TextAnchor.MiddleLeft);
            _speedText = CreateText(topPanel.transform, "SpeedText", "速度: --",
                anchorX: 0.22f, width: 0.16f, alignment: TextAnchor.MiddleLeft);
            _timeText = CreateText(topPanel.transform, "TimeText", "时间: --",
                anchorX: 0.40f, width: 0.16f, alignment: TextAnchor.MiddleLeft);
            _glucoseText = CreateText(topPanel.transform, "GlucoseText", "血糖: --",
                anchorX: 0.58f, width: 0.18f, alignment: TextAnchor.MiddleLeft);

            // Glucose bar (slider) on top-right
            var glucoseBar = CreateBar(topPanel.transform, "GlucoseBar",
                anchorX: 0.78f, width: 0.20f,
                out _glucoseSlider, out _glucoseFill, 100f);
            _glucoseFill.color = _safeColor;

            // Bottom distance progress bar
            var bottomPanel = CreatePanel(canvas.transform, "BottomPanel",
                anchorX: 0f, anchorY: 0f, height: 12f, topOffset: 0f,
                color: new Color(0f, 0f, 0f, 0.4f));
            _distanceProgressFill = CreateHorizontalFill(bottomPanel.transform, "DistanceProgressFill",
                new Color(0.3f, 0.7f, 1f));

            // Status icons bottom-right: shield / roll / stumble
            var statusRow = CreatePanel(canvas.transform, "StatusRow",
                anchorX: 1f, anchorY: 0f, height: 56f, topOffset: 16f,
                color: new Color(0f, 0f, 0f, 0f));
            _shieldIndicator = CreateIcon(statusRow.transform, "ShieldIcon", new Color(0.85f, 0.85f, 1f), 0f);
            _rollIndicator = CreateIcon(statusRow.transform, "RollIcon", new Color(0.6f, 0.4f, 0.2f), 1f);
            _stumbleIndicator = CreateIcon(statusRow.transform, "StumbleIcon", new Color(0.9f, 0.2f, 0.2f), 2f);

            _shieldIndicator.SetActive(false);
            _rollIndicator.SetActive(false);
            _stumbleIndicator.SetActive(false);
        }

        private static GameObject CreatePanel(Transform parent, string name,
            float anchorX, float anchorY, float height, float topOffset, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var rect = go.GetComponent<RectTransform>();
            if (anchorY >= 0.5f)
            {
                rect.anchorMin = new Vector2(anchorX, 1f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 1f);
                rect.anchoredPosition = new Vector2(0f, -topOffset);
                rect.sizeDelta = new Vector2(0f, height);
            }
            else
            {
                rect.anchorMin = new Vector2(anchorX, 0f);
                rect.anchorMax = new Vector2(1f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(0f, topOffset);
                rect.sizeDelta = new Vector2(0f, height);
            }
            return go;
        }

        private static Text CreateText(Transform parent, string name, string defaultText,
            float anchorX, float width, TextAnchor alignment)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = alignment;
            text.text = defaultText;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(anchorX, 0f);
            rect.anchorMax = new Vector2(anchorX + width, 1f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.offsetMin = new Vector2(8f, 0f);
            rect.offsetMax = new Vector2(-8f, 0f);
            return text;
        }

        private static GameObject CreateBar(Transform parent, string name,
            float anchorX, float width,
            out Slider slider, out Image fillImage, float maxValue)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var bg = go.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.6f);
            var bgRect = go.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(anchorX, 0.15f);
            bgRect.anchorMax = new Vector2(anchorX + width, 0.85f);
            bgRect.pivot = new Vector2(0f, 0.5f);
            bgRect.offsetMin = new Vector2(0f, 0f);
            bgRect.offsetMax = new Vector2(0f, 0f);

            slider = go.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = maxValue;
            slider.value = maxValue;

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(go.transform, false);
            var faRect = fillArea.AddComponent<RectTransform>();
            faRect.anchorMin = Vector2.zero;
            faRect.anchorMax = Vector2.one;
            faRect.offsetMin = new Vector2(4f, 4f);
            faRect.offsetMax = new Vector2(-4f, -4f);

            var fillGO = new GameObject("Fill");
            fillGO.transform.SetParent(fillArea.transform, false);
            fillImage = fillGO.AddComponent<Image>();
            fillImage.color = Color.green;
            var fillRect = fillGO.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            slider.fillRect = fillRect;
            slider.targetGraphic = fillImage;
            slider.interactable = false;
            return go;
        }

        private static Image CreateHorizontalFill(Transform parent, string name, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Horizontal;
            img.fillOrigin = (int)Image.OriginHorizontal.Left;
            img.fillAmount = 0f;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return img;
        }

        private static GameObject CreateIcon(Transform parent, string name, Color color, int index)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = color;
            var rect = go.GetComponent<RectTransform>();
            float slot = 1f / 3f;
            rect.anchorMin = new Vector2(1f - (index + 1) * slot, 0f);
            rect.anchorMax = new Vector2(1f - index * slot, 1f);
            rect.offsetMin = new Vector2(4f, 4f);
            rect.offsetMax = new Vector2(-4f, -4f);
            return go;
        }

        private void Update()
        {
            // Distance / time
            if (_levelManager != null)
            {
                float distance = _levelManager.DistanceTraveled;
                float target = _levelManager.Data != null ? _levelManager.Data.TargetDistanceMeters : 0f;
                float progress = target > 0f ? Mathf.Clamp01(distance / target) * 100f : 0f;
                SetText(_distanceText, $"距离: {distance:F0}m / {target:F0}m ({progress:F0}%)");
                SetText(_timeText, $"时间: {_levelManager.ElapsedTime:F1}s");
                if (_distanceProgressFill != null)
                {
                    _distanceProgressFill.fillAmount = _levelManager.NormalizedProgress;
                }
            }
            else
            {
                SetText(_distanceText, "距离: --");
                SetText(_timeText, "时间: --");
                if (_distanceProgressFill != null) _distanceProgressFill.fillAmount = 0f;
            }

            // Speed
            if (PlayerController != null)
            {
                SetText(_speedText, $"速度: {PlayerController.Speed:F1}m/s");
            }
            else
            {
                SetText(_speedText, "速度: --");
            }

            // Glucose text (bar updates via event)
            if (_glucoseSystem != null)
            {
                SetText(_glucoseText, $"血糖: {_glucoseSystem.CurrentValue:F0} ({_glucoseSystem.CurrentZone})");
            }
            else
            {
                SetText(_glucoseText, "血糖: --");
            }

            // Shield indicator (no event for now — poll)
            if (_skiingConcrete != null && _shieldIndicator != null)
            {
                _shieldIndicator.SetActive(_skiingConcrete.ShieldActive);
            }
        }

        private void UpdateGlucoseBar(float value)
        {
            if (_glucoseSlider != null)
            {
                _glucoseSlider.value = Mathf.Clamp(value, 0f, _glucoseSlider.maxValue);
            }
        }

        private void UpdateGlucoseColor(GlucoseZone zone)
        {
            if (_glucoseFill == null) return;
            _glucoseFill.color = zone switch
            {
                GlucoseZone.LowCrisis => _lowCrisisColor,
                GlucoseZone.LowWarning => _lowWarningColor,
                GlucoseZone.Safe => _safeColor,
                GlucoseZone.HighWarning => _highWarningColor,
                GlucoseZone.HighCrisis => _highCrisisColor,
                _ => _safeColor,
            };
        }

        private void UpdateStumbleIcon(bool active)
        {
            if (_stumbleIndicator != null) _stumbleIndicator.SetActive(active);
        }

        private void UpdateRollIcon(bool active)
        {
            if (_rollIndicator != null) _rollIndicator.SetActive(active);
        }

        private static void SetText(Text text, string value)
        {
            if (text != null) text.text = value;
        }
    }
}
