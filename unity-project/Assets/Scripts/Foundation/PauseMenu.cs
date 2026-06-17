using SugarRush.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Pause overlay panel. Subscribes to GameEvents.OnGamePaused / OnGameResumed
    /// and toggles Time.timeScale + visibility. Provides Resume, Restart and Quit
    /// buttons. Round 1 MVP — Settings (audio / fullscreen) is a follow-up.
    /// </summary>
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI (auto-built if null)")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;

        private bool _subscribed;

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            if (_subscribed) return;
            GameEvents.OnGamePaused += Show;
            GameEvents.OnGameResumed += Hide;
            _subscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_subscribed) return;
            GameEvents.OnGamePaused -= Show;
            GameEvents.OnGameResumed -= Hide;
            _subscribed = false;
        }

        private void Awake()
        {
            EnsureUI();
            if (_panel != null) _panel.SetActive(false);
        }

        private void Start()
        {
            BindButtons();
        }

        private void EnsureUI()
        {
            if (_panel != null && _resumeButton != null && _restartButton != null && _quitButton != null)
                return;

            // Build onto the HUD canvas if it exists, else create our own.
            var hud = FindObjectOfType<HUD>();
            Transform parent = hud != null ? hud.transform : transform;

            // Panel: full-screen semi-transparent overlay
            _panel = new GameObject("PausePanel");
            _panel.transform.SetParent(parent, false);
            var bg = _panel.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.7f);
            var bgRect = _panel.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            // Title
            var titleGO = new GameObject("Title");
            titleGO.transform.SetParent(_panel.transform, false);
            var titleText = titleGO.AddComponent<Text>();
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.text = "PAUSED";
            titleText.fontSize = 56;
            titleText.color = Color.white;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.fontStyle = FontStyle.Bold;
            var titleRect = titleGO.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0.55f);
            titleRect.anchorMax = new Vector2(1f, 0.85f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;

            // Buttons stacked vertically
            _resumeButton = CreateButton(_panel.transform, "ResumeButton", "继续", 0.40f, 0.50f);
            _restartButton = CreateButton(_panel.transform, "RestartButton", "重新开始", 0.30f, 0.40f);
            _quitButton = CreateButton(_panel.transform, "QuitButton", "退出", 0.20f, 0.30f);
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

            // Label child
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
            if (_resumeButton != null)
            {
                _resumeButton.onClick.AddListener(OnResumeClicked);
            }
            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(OnRestartClicked);
            }
            if (_quitButton != null)
            {
                _quitButton.onClick.AddListener(OnQuitClicked);
            }
        }

        private void Show()
        {
            if (_panel != null) _panel.SetActive(true);
            // Don't override an existing pause (e.g. system pause).
            if (Time.timeScale > 0f) Time.timeScale = 0f;
        }

        private void Hide()
        {
            if (_panel != null) _panel.SetActive(false);
            if (Time.timeScale == 0f) Time.timeScale = 1f;
        }

        private void OnResumeClicked()
        {
            // Resume is just a raised event — PauseMenu listens and hides itself.
            // Time.timeScale is restored by Hide().
            GameEvents.RaiseGameResumed();
        }

        private void OnRestartClicked()
        {
            // Round 1 MVP: hard scene reload. Round 2 will swap this for a
            // runtime reset so obstacles and pooled objects also respawn.
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
    }
}
