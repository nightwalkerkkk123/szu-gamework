using SugarRush.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Result screen shown on win or lose. Subscribes to global game finished events.
    /// </summary>
    public class ResultPanel : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _messageText;
        [SerializeField] private Button _restartButton;

        [Header("Colors")]
        [SerializeField] private Color _winColor = new Color(0.2f, 0.8f, 0.3f);
        [SerializeField] private Color _loseColor = new Color(0.9f, 0.2f, 0.2f);

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
            if (_panel != null)
            {
                _panel.SetActive(false);
            }

            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(RestartLevel);
            }
        }

        private void OnDestroy()
        {
            if (_restartButton != null)
            {
                _restartButton.onClick.RemoveListener(RestartLevel);
            }
        }

        private void ShowResult(bool win)
        {
            if (_panel != null)
            {
                _panel.SetActive(true);
            }

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
        }

        private void RestartLevel()
        {
            StartCoroutine(RestartLevelAfterFrame());
        }

        private System.Collections.IEnumerator RestartLevelAfterFrame()
        {
            yield return null;

            // Resume time before reloading so the load and subsequent gameplay run normally.
            Time.timeScale = 1f;

            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Debug.Log($"[ResultPanel] Restarting scene: {sceneName}");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}