using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// In-game settings overlay: master volume slider, fullscreen toggle, close button.
    /// Hosted as a child of PauseMenu's pause panel — toggles via Show()/Hide().
    /// </summary>
    public class SettingsPanel : MonoBehaviour
    {
        [Header("UI (auto-built if null)")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Text _masterVolumeLabel;
        [SerializeField] private Toggle _fullscreenToggle;
        [SerializeField] private Button _closeButton;

        private void Awake()
        {
            EnsureUI();
            if (_panel != null) _panel.SetActive(false);
        }

        private void Start()
        {
            BindControls();
            SyncFromService();
        }

        private void OnEnable()
        {
            SyncFromService();
        }

        private void EnsureUI()
        {
            if (_panel != null && _masterVolumeSlider != null && _fullscreenToggle != null && _closeButton != null)
                return;

            // Build as a child of the host PausePanel so it inherits the overlay.
            // If no PausePanel found, fall back to a fresh full-screen canvas.
            var pauseMenu = FindObjectOfType<PauseMenu>();
            Transform parent = pauseMenu != null
                ? pauseMenu.transform.Find("PausePanel")
                : transform;

            if (parent == null) parent = transform;

            _panel = new GameObject("SettingsSubPanel");
            _panel.transform.SetParent(parent, false);
            var bg = _panel.AddComponent<Image>();
            bg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);
            var bgRect = _panel.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0.2f, 0.15f);
            bgRect.anchorMax = new Vector2(0.8f, 0.65f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var title = CreateText(_panel.transform, "SettingsTitle", "设置",
                new Vector2(0f, 0.78f), new Vector2(1f, 0.95f), 36, FontStyle.Bold);
            title.alignment = TextAnchor.MiddleCenter;

            // Master volume slider
            var sliderGO = new GameObject("MasterVolumeSlider");
            sliderGO.transform.SetParent(_panel.transform, false);
            var sliderRect = sliderGO.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.1f, 0.55f);
            sliderRect.anchorMax = new Vector2(0.9f, 0.70f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;

            _masterVolumeSlider = CreateSlider(sliderGO, out _masterVolumeLabel);

            // Fullscreen toggle row
            var toggleGO = new GameObject("FullscreenToggle");
            toggleGO.transform.SetParent(_panel.transform, false);
            var toggleRect = toggleGO.AddComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.1f, 0.35f);
            toggleRect.anchorMax = new Vector2(0.9f, 0.50f);
            toggleRect.offsetMin = Vector2.zero;
            toggleRect.offsetMax = Vector2.zero;

            _fullscreenToggle = CreateToggle(toggleGO, "全屏");

            // Close button
            var closeGO = new GameObject("CloseButton");
            closeGO.transform.SetParent(_panel.transform, false);
            var closeImg = closeGO.AddComponent<Image>();
            closeImg.color = new Color(0.2f, 0.2f, 0.25f, 0.95f);
            var closeRect = closeGO.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0.30f, 0.05f);
            closeRect.anchorMax = new Vector2(0.70f, 0.20f);
            closeRect.offsetMin = Vector2.zero;
            closeRect.offsetMax = Vector2.zero;
            _closeButton = closeGO.AddComponent<Button>();
            _closeButton.targetGraphic = closeImg;
            var closeLabel = CreateText(closeGO.transform, "CloseLabel", "关闭",
                Vector2.zero, Vector2.one, 24, FontStyle.Normal);
            closeLabel.alignment = TextAnchor.MiddleCenter;
        }

        private static Text CreateText(Transform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax, int fontSize, FontStyle style)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.text = text;
            t.fontSize = fontSize;
            t.color = Color.white;
            t.fontStyle = style;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return t;
        }

        private static Slider CreateSlider(GameObject host, out Text label)
        {
            var slider = host.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;
            slider.minValue = 0f;
            slider.maxValue = 1f;

            var bg = new GameObject("Background");
            bg.transform.SetParent(host.transform, false);
            var bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var bgRect = bg.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(host.transform, false);
            var faRect = fillArea.AddComponent<RectTransform>();
            faRect.anchorMin = Vector2.zero;
            faRect.anchorMax = Vector2.one;
            faRect.offsetMin = new Vector2(5f, 5f);
            faRect.offsetMax = new Vector2(-5f, -5f);

            var fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            var fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(0.4f, 0.7f, 1f);
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            var handleArea = new GameObject("Handle Slide Area");
            handleArea.transform.SetParent(host.transform, false);
            var haRect = handleArea.AddComponent<RectTransform>();
            haRect.anchorMin = Vector2.zero;
            haRect.anchorMax = Vector2.one;
            haRect.offsetMin = Vector2.zero;
            haRect.offsetMax = Vector2.zero;

            var handle = new GameObject("Handle");
            handle.transform.SetParent(handleArea.transform, false);
            var handleImg = handle.AddComponent<Image>();
            handleImg.color = Color.white;
            var handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20f, 0f);

            slider.fillRect = fillRect;
            slider.handleRect = handleRect;
            slider.targetGraphic = handleImg;

            // Floating label on the right
            var labelGO = new GameObject("VolumeLabel");
            labelGO.transform.SetParent(host.transform, false);
            var labelRect = labelGO.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.85f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            label = labelGO.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = "100%";
            label.fontSize = 18;
            label.color = Color.white;
            label.alignment = TextAnchor.MiddleRight;
            return slider;
        }

        private static Toggle CreateToggle(GameObject host, string labelText)
        {
            var bgGO = new GameObject("Background");
            bgGO.transform.SetParent(host.transform, false);
            var bgImg = bgGO.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            var bgRect = bgGO.GetComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0f, 0.2f);
            bgRect.anchorMax = new Vector2(0.06f, 0.8f);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            var checkGO = new GameObject("Checkmark");
            checkGO.transform.SetParent(bgGO.transform, false);
            var checkImg = checkGO.AddComponent<Image>();
            checkImg.color = new Color(0.4f, 0.85f, 0.4f);
            var checkRect = checkGO.GetComponent<RectTransform>();
            checkRect.anchorMin = new Vector2(0.1f, 0.1f);
            checkRect.anchorMax = new Vector2(0.9f, 0.9f);
            checkRect.offsetMin = Vector2.zero;
            checkRect.offsetMax = Vector2.zero;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(host.transform, false);
            var labelRect = labelGO.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.08f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var label = labelGO.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.text = labelText;
            label.fontSize = 24;
            label.color = Color.white;
            label.alignment = TextAnchor.MiddleLeft;

            var toggle = host.AddComponent<Toggle>();
            toggle.targetGraphic = bgImg;
            toggle.graphic = checkImg;
            return toggle;
        }

        private void BindControls()
        {
            if (_masterVolumeSlider != null)
            {
                _masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            }
            if (_fullscreenToggle != null)
            {
                _fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggled);
            }
            if (_closeButton != null)
            {
                _closeButton.onClick.AddListener(Hide);
            }
        }

        private void SyncFromService()
        {
            if (_masterVolumeSlider != null) _masterVolumeSlider.value = SettingsService.MasterVolume;
            if (_masterVolumeLabel != null) _masterVolumeLabel.text = $"{Mathf.RoundToInt(SettingsService.MasterVolume * 100f)}%";
            if (_fullscreenToggle != null) _fullscreenToggle.isOn = SettingsService.Fullscreen;
        }

        private void OnMasterVolumeChanged(float value)
        {
            SettingsService.MasterVolume = value;
            if (_masterVolumeLabel != null) _masterVolumeLabel.text = $"{Mathf.RoundToInt(value * 100f)}%";
            SettingsService.Save();
        }

        private void OnFullscreenToggled(bool isOn)
        {
            SettingsService.Fullscreen = isOn;
            SettingsService.Save();
        }

        public void Show()
        {
            if (_panel != null) _panel.SetActive(true);
            SyncFromService();
        }

        public void Hide()
        {
            if (_panel != null) _panel.SetActive(false);
        }
    }
}
