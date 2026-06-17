using SugarRush.Core;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Full-screen vignette overlay that fades in on game end.
    /// Win = golden, Lose = dark red. Fades out when the result panel takes over.
    /// </summary>
    public class HudVignette : MonoBehaviour
    {
        [SerializeField] private Canvas _parentCanvas;

        private Image _vignetteImage;
        private float _targetAlpha;
        private float _currentAlpha;
        private bool _isWin;

        private const float FadeSpeed = 2.5f; // alpha units per second (0.4s fade ≈ 1.0 / 2.5)

        private void Awake()
        {
            EnsureUI();
        }

        private void OnEnable()
        {
            GameEvents.OnGameFinished += OnGameFinished;
        }

        private void OnDisable()
        {
            GameEvents.OnGameFinished -= OnGameFinished;
        }

        private void Update()
        {
            if (Mathf.Abs(_currentAlpha - _targetAlpha) < 0.001f) return;
            _currentAlpha = Mathf.MoveTowards(_currentAlpha, _targetAlpha, FadeSpeed * Time.deltaTime);
            SetAlpha(_currentAlpha);
        }

        private void OnGameFinished(bool win)
        {
            _isWin = win;
            _targetAlpha = 1f;
            _vignetteImage.color = win
                ? new Color(1f, 0.85f, 0.3f, 0f)   // golden
                : new Color(0.6f, 0.1f, 0.1f, 0f); // dark red
            _currentAlpha = 0f;
            SetAlpha(0f);
        }

        /// <summary>
        /// Called externally when the result panel is shown — fades vignette out.
        /// </summary>
        public void HideVignette()
        {
            _targetAlpha = 0f;
        }

        private void SetAlpha(float alpha)
        {
            if (_vignetteImage == null) return;
            var c = _vignetteImage.color;
            _vignetteImage.color = new Color(c.r, c.g, c.b, alpha);
        }

        private void EnsureUI()
        {
            if (_vignetteImage != null) return;

            var canvas = _parentCanvas != null ? _parentCanvas : FindObjectOfType<Canvas>();

            var go = new GameObject("HudVignette");
            go.transform.SetParent(canvas != null ? canvas.transform : transform, false);
            _vignetteImage = go.AddComponent<Image>();
            _vignetteImage.color = new Color(1f, 0.85f, 0.3f, 0f);
            _vignetteImage.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}