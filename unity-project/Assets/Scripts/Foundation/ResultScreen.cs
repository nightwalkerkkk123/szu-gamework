using SugarRush.Core;
using SugarRush.GameFlow;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// End-of-level result overlay. Subscribes to GameEvents.OnGameFinished and
    /// shows Win / Lose copy with run stats and a Retry button. Round 1 MVP —
    /// Runtime reset (no scene reload) is a round 2 follow-up.
    /// </summary>
    public class ResultScreen : MonoBehaviour
    {
        [Header("UI (auto-built if null)")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _headline;
        [SerializeField] private Text _distanceStat;
        [SerializeField] private Text _timeStat;
        [SerializeField] private Text _parTimeStat;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _quitButton;

        [Header("Style")]
        [SerializeField] private Color _winColor = new Color(0.4f, 0.9f, 0.4f);
        [SerializeField] private Color _loseColor = new Color(0.9f, 0.3f, 0.3f);

        private LevelManager _levelManager;
        private bool _subscribed;

        private void OnEnable() => Subscribe();
        private void OnDisable() => Unsubscribe();
        private void OnDestroy() => Unsubscribe();

        private void Subscribe()
        {
            if (_subscribed) return;
            GameEvents.OnGameFinished += Show;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed) return;
            GameEvents.OnGameFinished -= Show;
            _subscribed = false;
        }

        private void Awake()
        {
            EnsureUI();
            if (_panel != null) _panel.SetActive(false);
        }

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();
            BindButtons();
        }

        private void EnsureUI()
        {
            if (_panel != null && _headline != null && _retryButton != null) return;

            var hud = FindObjectOfType<HUD>();
            Transform parent = hud != null ? hud.transform : transform;

            _panel = new GameObject("ResultPanel");
            _panel.transform.SetParent(parent, false);
            var bg = _panel.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.75f);
            var bgRect = _panel.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            _headline = CreateText(_panel.transform, "Headline", "胜利!",
                new Vector2(0f, 0.70f), new Vector2(1f, 0.90f), 64, FontStyle.Bold, _winColor);

            _distanceStat = CreateText(_panel.transform, "DistanceStat", "距离: --",
                new Vector2(0.20f, 0.55f), new Vector2(0.80f, 0.65f), 28, FontStyle.Normal, Color.white);

            _timeStat = CreateText(_panel.transform, "TimeStat", "时间: --",
                new Vector2(0.20f, 0.45f), new Vector2(0.80f, 0.55f), 28, FontStyle.Normal, Color.white);

            _parTimeStat = CreateText(_panel.transform, "ParTimeStat", "目标: --",
                new Vector2(0.20f, 0.35f), new Vector2(0.80f, 0.45f), 24, FontStyle.Italic, new Color(0.85f, 0.85f, 0.85f));

            _retryButton = CreateButton(_panel.transform, "RetryButton", "再来一次", 0.20f, 0.30f);
            _quitButton = CreateButton(_panel.transform, "QuitButton", "退出", 0.08f, 0.18f);
        }

        private static Text CreateText(Transform parent, string name, string text,
            Vector2 anchorMin, Vector2 anchorMax, int fontSize, FontStyle style, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.text = text;
            t.fontSize = fontSize;
            t.color = color;
            t.alignment = TextAnchor.MiddleCenter;
            t.fontStyle = style;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return t;
        }

        private static Button CreateButton(Transform parent, string name, string label,
            float anchorMinY, float anchorMaxY)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0.2f, 0.2f, 0.25f, 0.95f);
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.3f, anchorMinY);
            rect.anchorMax = new Vector2(0.7f, anchorMaxY);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var btn = go.AddComponent<Button>();
            btn.targetGraphic = img;

            var labelGO = new GameObject("Label");
            labelGO.transform.SetParent(go.transform, false);
            var txt = labelGO.AddComponent<Text>();
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.text = label;
            txt.fontSize = 28;
            txt.color = Color.white;
            txt.alignment = TextAnchor.MiddleCenter;
            var labelRect = labelGO.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            return btn;
        }

        private void BindButtons()
        {
            if (_retryButton != null) _retryButton.onClick.AddListener(OnRetryClicked);
            if (_quitButton != null) _quitButton.onClick.AddListener(OnQuitClicked);
        }

        private void Show(bool win)
        {
            if (_panel != null) _panel.SetActive(true);
            if (_headline != null)
            {
                _headline.text = win ? "胜利!" : "失败";
                _headline.color = win ? _winColor : _loseColor;
            }

            if (_levelManager == null) _levelManager = FindObjectOfType<LevelManager>();
            if (_levelManager != null && _levelManager.Data != null)
            {
                var data = _levelManager.Data;
                float parTime = data.ParTimeSeconds;
                float elapsed = _levelManager.ElapsedTime;
                float distance = _levelManager.DistanceTraveled;
                float target = data.TargetDistanceMeters;
                SetText(_distanceStat, $"距离: {distance:F0}m / {target:F0}m");
                SetText(_timeStat, $"时间: {elapsed:F1}s");
                SetText(_parTimeStat, parTime > 0f
                    ? (elapsed <= parTime
                        ? $"目标时间: {parTime:F0}s — 提前 {parTime - elapsed:F1}s"
                        : $"目标时间: {parTime:F0}s — 超时 {elapsed - parTime:F1}s")
                    : "目标时间: --");
            }
            else
            {
                SetText(_distanceStat, "距离: --");
                SetText(_timeStat, "时间: --");
                SetText(_parTimeStat, "目标: --");
            }

            // Hard pause — LevelManager already early-returns on timeScale<=0
            // and SkiingController stays alive, so the player visually freezes.
            if (Time.timeScale > 0f) Time.timeScale = 0f;
        }

        private void OnRetryClicked()
        {
            Time.timeScale = 1f;
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        private void OnQuitClicked()
        {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private static void SetText(Text text, string value)
        {
            if (text != null) text.text = value;
        }
    }
}
