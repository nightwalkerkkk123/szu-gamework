using SugarRush.Core;
using SugarRush.Foundation;
using System.Collections;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Player visual feedback: color by glucose zone, hurt flash, roll flash, stumble shake.
    /// </summary>
    public class PlayerVisuals : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _bodyRenderer;
        [SerializeField] private SkiingController _skiingController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        [Header("Glucose Colors")]
        [SerializeField] private Color _normalColor = Color.cyan;
        [SerializeField] private Color _lowWarningColor = new Color(0.5f, 0.75f, 1f, 1f);
        [SerializeField] private Color _lowCrisisColor = new Color(0.25f, 0.45f, 0.9f, 1f);
        [SerializeField] private Color _highWarningColor = new Color(1f, 0.75f, 0.4f, 1f);
        [SerializeField] private Color _highCrisisColor = new Color(0.9f, 0.25f, 0.25f, 1f);

        [Header("Flash")]
        [SerializeField] private float _flashDuration = 0.15f;
        [SerializeField] private float _flashSpeed = 20f;
        [SerializeField] private Color _rollFlashColor = Color.white;
        [SerializeField] private Color _hurtFlashColor = Color.red;
        [SerializeField] private Color _pickupFlashColor = Color.yellow;

        [Header("Shake")]
        [SerializeField] private float _stumbleShakeAmount = 0.15f;
        [SerializeField] private float _stumbleShakeSpeed = 30f;
        [SerializeField] private float _crisisShakeAmount = 0.05f;

        private Color _baseColor;
        private float _flashTimer;
        private Color _flashColor;
        private bool _isRolling;
        private bool _isStumbling;
        private float _stumbleTimer;

        private void Start()
        {
            if (_bodyRenderer == null)
            {
                _bodyRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            _baseColor = _normalColor;

            if (_skiingController != null)
            {
                _skiingController.OnRollingChanged += HandleRollingChanged;
                _skiingController.OnStumbledChanged += HandleStumbledChanged;
            }

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged += HandleZoneChanged;
                UpdateBaseColor(_glucoseSystem.CurrentZone);
            }
            else
            {
                GameEvents.OnGlucoseZoneChanged += HandleZoneChanged;
            }

            PickupItem.OnItemPickedUp += HandleItemPickedUp;
        }

        private void OnDestroy()
        {
            if (_skiingController != null)
            {
                _skiingController.OnRollingChanged -= HandleRollingChanged;
                _skiingController.OnStumbledChanged -= HandleStumbledChanged;
            }

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged -= HandleZoneChanged;
            }
            else
            {
                GameEvents.OnGlucoseZoneChanged -= HandleZoneChanged;
            }

            PickupItem.OnItemPickedUp -= HandleItemPickedUp;
        }

        public void TriggerHurtFlash()
        {
            Flash(_hurtFlashColor);
        }

        public void TriggerDeathVisual()
        {
            if (_bodyRenderer != null)
            {
                _bodyRenderer.color = new Color(_bodyRenderer.color.r, _bodyRenderer.color.g, _bodyRenderer.color.b, 0f);
            }
        }

        private void Update()
        {
            ApplyVisuals();
        }

        private void ApplyVisuals()
        {
            if (_bodyRenderer == null) return;

            Color targetColor = _baseColor;

            if (_flashTimer > 0f)
            {
                _flashTimer -= Time.deltaTime;
                float flash = Mathf.PingPong(Time.time * _flashSpeed, 1f);
                targetColor = Color.Lerp(_baseColor, _flashColor, flash);
            }

            _bodyRenderer.color = Color.Lerp(_bodyRenderer.color, targetColor, Time.deltaTime * 15f);

            Vector3 pos = transform.localPosition;
            float shake = 0f;

            if (_isStumbling)
            {
                _stumbleTimer -= Time.deltaTime;
                if (_stumbleTimer <= 0f) _isStumbling = false;
                shake = Mathf.Sin(Time.time * _stumbleShakeSpeed) * _stumbleShakeAmount;
            }
            else if (_glucoseSystem != null && (_glucoseSystem.CurrentZone == GlucoseZone.LowCrisis || _glucoseSystem.CurrentZone == GlucoseZone.HighCrisis))
            {
                shake = Mathf.Sin(Time.time * 20f) * _crisisShakeAmount;
            }

            pos.x += shake;
            transform.localPosition = pos;
        }

        private void HandleRollingChanged(bool isRolling)
        {
            _isRolling = isRolling;
            if (isRolling)
            {
                Flash(_rollFlashColor);
            }
        }

        private void HandleStumbledChanged(bool isStumbled)
        {
            _isStumbling = isStumbled;
            if (isStumbled)
            {
                _stumbleTimer = 0.5f;
                Flash(_hurtFlashColor);
            }
        }

        private void HandleZoneChanged(GlucoseZone zone)
        {
            UpdateBaseColor(zone);
        }

        private void HandleItemPickedUp(PickupItem pickup, ItemEffect effect, GameObject player)
        {
            if (player != gameObject) return;
            Flash(_pickupFlashColor);
        }

        private void UpdateBaseColor(GlucoseZone zone)
        {
            _baseColor = zone switch
            {
                GlucoseZone.LowCrisis => _lowCrisisColor,
                GlucoseZone.LowWarning => _lowWarningColor,
                GlucoseZone.HighWarning => _highWarningColor,
                GlucoseZone.HighCrisis => _highCrisisColor,
                _ => _normalColor
            };
        }

        private void Flash(Color color)
        {
            _flashColor = color;
            _flashTimer = _flashDuration;
        }
    }
}
