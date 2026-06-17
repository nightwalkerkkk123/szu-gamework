using SugarRush.Core;
using SugarRush.GameFlow;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Result screen shown on win or lose. Subscribes to global game finished events.
    /// Round 1: distance / time / par-time stats, Quit button. Round 2 will
    /// add a runtime reset (no scene reload) and replace the hard load.
    /// </summary>
    public class ResultPanel : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _messageText;
        [SerializeField] private Text _distanceStat;
        [SerializeField] private Text _timeStat;
        [SerializeField] private Text _parTimeStat;

        [Header("Buttons")]
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _quitButton;

        [Header("Colors")]
        [SerializeField] private Color _winColor = new Color(0.2f, 0.8f, 0.3f);
        [SerializeField] private Color _loseColor = new Color(0.9f, 0.2f, 0.2f);

        private LevelManager _levelManager;

        private void OnEnable()
        {
            GameEvents.OnGameFinished += ShowResult;
        }

        private void OnDisable()
        {
            GameEvents.OnGameFinished -= ShowResult;
        }

        private void Start()
        {
            _levelManager = FindObjectOfType<LevelManager>();

            if (_panel != null) _panel.SetActive(false);

            if (_restartButton != null) _restartButton.onClick.AddListener(RestartLevel);
            if (_quitButton != null) _quitButton.onClick.AddListener(QuitGame);
        }

        private void OnDestroy()
        {
            if (_restartButton != null) _restartButton.onClick.RemoveListener(RestartLevel);
            if (_quitButton != null) _quitButton.onClick.RemoveListener(QuitGame);
        }

        private void ShowResult(bool win)
        {
            if (_panel != null) _panel.SetActive(true);

            if (_titleText != null)
            {
                _titleText.text = win ? "胜利!" : "失败";
                _titleText.color = win ? _winColor : _loseColor;
            }

            if (_messageText != null)
            {
                _messageText.text = win
                    ? "你成功抵达了医院雪屋。"
                    : "血糖失控或撞到了障碍，再试一次吧。";
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

            // Hard pause — LevelManager early-returns on timeScale<=0 already.
            if (Time.timeScale > 0f) Time.timeScale = 0f;
        }

        private void RestartLevel()
        {
            // Round 2: runtime reset. Hide the result panel first so the player
            // doesn't see a flicker, then reset run state in place.
            if (_panel != null) _panel.SetActive(false);
            RuntimeResetter.Reset();
        }

        private void QuitGame()
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
