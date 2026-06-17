using SugarRush.Gameplay;
using SugarRush.Gameplay.Items;
using UnityEngine;
using UnityEngine.UI;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Floating "+ItemName" text that rises from the player's screen position
    /// when a pickup is collected. Self-pooling via simple enable/disable.
    /// </summary>
    public class PickupFloater : MonoBehaviour
    {
        [SerializeField] private Canvas _parentCanvas;

        private Text _floatText;
        private float _lifeTimer = -1f;
        private float _startY;

        private const float FloatDuration = 0.8f;
        private const float FloatRisePx = 50f;

        private static readonly System.Collections.Generic.Dictionary<System.Type, Color> ItemColors =
            new()
            {
                { typeof(InsulinSprayEffect),    new Color(0.4f,  0.75f, 1f)    }, // insulin blue
                { typeof(HypoglycemicPillsEffect), new Color(0.4f,  0.9f,  0.4f)  }, // pills green
                { typeof(HighSugarSnowflakeEffect), new Color(1f,   0.9f,  0.2f)  }, // snowflake yellow
                { typeof(ShieldEffect),          new Color(0.85f, 0.85f, 0.9f)  }, // shield silver
                { typeof(MagnetEffect),          new Color(0.85f, 0.4f,  1f)    }, // magnet purple
            };

        private void Awake()
        {
            EnsureUI();
        }

        private void OnEnable()
        {
            PickupItem.OnItemPickedUp += OnPickedUp;
        }

        private void OnDisable()
        {
            PickupItem.OnItemPickedUp -= OnPickedUp;
        }

        private void Update()
        {
            if (_lifeTimer < 0f) return;

            _lifeTimer += Time.deltaTime;
            float t = Mathf.Clamp01(_lifeTimer / FloatDuration);

            // Rise
            var pos = _floatText.rectTransform.anchoredPosition;
            pos.y = _startY + FloatRisePx * t;
            _floatText.rectTransform.anchoredPosition = pos;

            // Fade
            _floatText.color = new Color(_floatText.color.r, _floatText.color.g, _floatText.color.b, 1f - t);

            if (t >= 1f)
            {
                _floatText.gameObject.SetActive(false);
                _lifeTimer = -1f;
            }
        }

        private void OnPickedUp(PickupItem item, ItemEffect effect, GameObject player)
        {
            if (_lifeTimer >= 0f) return; // Already showing one — skip

            _floatText.text = "+" + effect.DisplayName;
            _floatText.color = LookupColor(effect);

            // Snap to player's screen-space position (top-center of player)
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(
                Camera.main, player.transform.position);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas != null ? _parentCanvas.transform as RectTransform : null,
                screenPos, null, out var localPos);

            _floatText.rectTransform.anchoredPosition = new Vector2(localPos.x, localPos.y);
            _startY = localPos.y;

            _floatText.gameObject.SetActive(true);
            _lifeTimer = 0f;
        }

        private static Color LookupColor(ItemEffect effect)
        {
            foreach (var kv in ItemColors)
            {
                if (kv.Key.IsAssignableFrom(effect.GetType()))
                    return kv.Value;
            }
            return Color.white;
        }

        private void EnsureUI()
        {
            if (_floatText != null) return;

            var canvas = _parentCanvas != null ? _parentCanvas : FindObjectOfType<Canvas>();

            var go = new GameObject("PickupFloater");
            go.transform.SetParent(canvas != null ? canvas.transform : transform, false);

            _floatText = go.AddComponent<Text>();
            _floatText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _floatText.fontSize = 22;
            _floatText.alignment = TextAnchor.MiddleCenter;
            _floatText.color = Color.white;
            _floatText.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(200f, 40f);
            rect.anchoredPosition = Vector2.zero;

            _floatText.gameObject.SetActive(false);
        }
    }
}