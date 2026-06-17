using SugarRush.Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Full-screen flash overlay driven by crash and pickup events.
    /// Self-builds a full-screen Canvas + Image on first flash.
    /// </summary>
    public class ScreenFlash : MonoBehaviour
    {
        [Header("Crash Flash")]
        [SerializeField] private Color _crashColor = new Color(1f, 0f, 0f, 0.6f);
        [SerializeField] private float _crashFadeDuration = 0.4f;

        [Header("Pickup Flash")]
        [SerializeField] private Color _pickupColor = new Color(1f, 1f, 1f, 0.3f);
        [SerializeField] private float _pickupFadeDuration = 0.2f;

        private Canvas _canvas;
        private Image _image;
        private float _fadeTimer;
        private float _fadeDuration;
        private Color _targetColor;

        private void OnEnable()
        {
            SkiingController.OnCrashed += HandleCrashed;
            PickupItem.OnItemPickedUp += HandlePickup;
        }

        private void OnDisable()
        {
            SkiingController.OnCrashed -= HandleCrashed;
            PickupItem.OnItemPickedUp -= HandlePickup;
        }

        private void Update()
        {
            if (_image == null) return;

            if (_fadeTimer > 0f)
            {
                _fadeTimer -= Time.deltaTime;
                float t = Mathf.Clamp01(_fadeTimer / _fadeDuration);
                Color c = _targetColor;
                c.a = Mathf.Lerp(0f, _targetColor.a, t);
                _image.color = c;
            }
        }

        public void Flash(Color color, float alpha, float fadeDuration)
        {
            EnsureFlashImage();

            _targetColor = color;
            _targetColor.a = alpha;
            _fadeDuration = fadeDuration;
            _fadeTimer = fadeDuration;

            Color c = _targetColor;
            c.a = alpha;
            _image.color = c;
        }

        private void HandleCrashed()
        {
            Flash(_crashColor, _crashColor.a, _crashFadeDuration);
        }

        private void HandlePickup(PickupItem pickup, ItemEffect effect, GameObject player)
        {
            Flash(_pickupColor, _pickupColor.a, _pickupFadeDuration);
        }

        private void EnsureFlashImage()
        {
            if (_image != null) return;

            var go = new GameObject("ScreenFlashCanvas", typeof(Canvas), typeof(GraphicRaycaster));
            _canvas = go.GetComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 9999;

            var imgGO = new GameObject("ScreenFlashImage", typeof(Image));
            imgGO.transform.SetParent(_canvas.transform, false);
            _image = imgGO.GetComponent<Image>();
            _image.color = Color.clear;
            _image.raycastTarget = false;

            // Cover full screen.
            var rect = _image.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }
    }
}