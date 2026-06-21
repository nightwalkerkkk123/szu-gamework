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

        [Header("Background")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Sprite _winBackground;
        [SerializeField] private Sprite _loseBackground;

        [Header("Flow")]
        [Tooltip("胜利后跳转的场景名；留空 = 重玩本关。L2_Fusion_Ours 设为 LevelSelect。")]
        [SerializeField] private string _winNextScene = "";

        private bool _lastWin;

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
            if (_backgroundImage != null)
            {
                Sprite bg = win ? _winBackground : _loseBackground;
                if (bg != null)
                {
                    _backgroundImage.sprite = bg;
                    _backgroundImage.enabled = true;
                }
            }

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

            _lastWin = win;
            if (_restartButton != null)
            {
                var label = _restartButton.GetComponentInChildren<Text>();
                if (label != null)
                {
                    label.text = (win && !string.IsNullOrEmpty(_winNextScene))
                        ? "返回关卡选择"
                        : "再试一次";
                }
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

            string sceneName = (_lastWin && !string.IsNullOrEmpty(_winNextScene))
                ? _winNextScene
                : UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Debug.Log($"[ResultPanel] Loading scene: {sceneName} (win={_lastWin})");
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
    }
}