using SugarRush.Core;
using SugarRush.Foundation;
using System.Collections;
using UnityEngine;
using Cinemachine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Camera feel effects: speed-based zoom, landing shake, glucose crisis offset.
    /// </summary>
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class CameraEffects : MonoBehaviour
    {
        [SerializeField] private SkiingController _skiingController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        [Header("Speed Zoom")]
        [SerializeField] private float _baseOrthoSize = 6f;
        [SerializeField] private float _maxOrthoSize = 7.5f;
        [SerializeField] private float _zoomSpeed = 2f;
        [SerializeField] private float _speedForMaxZoom = 18f;

        [Header("Landing Shake")]
        [SerializeField] private float _shakeDuration = 0.15f;
        [SerializeField] private float _shakeAmplitude = 0.3f;
        [SerializeField] private float _minFallSpeedForShake = 5f;

        [Header("Glucose Crisis Offset")]
        [SerializeField] private float _crisisOffsetAmount = 0.25f;
        [SerializeField] private float _crisisRotationAmount = 2f;
        [SerializeField] private float _crisisLerpSpeed = 5f;

        [Header("Roll Zoom Burst")]
        [SerializeField] private float _rollZoomAmount = 0.4f;
        [SerializeField] private float _rollZoomDuration = 0.25f;

        private CinemachineVirtualCamera _vcam;
        private CinemachineBasicMultiChannelPerlin _noise;
        private float _zoomOffset;
        private float _rollZoomTimer;
        private Vector3 _crisisOffset;
        private float _crisisRotation;

        private void Awake()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();
            _noise = _vcam.AddCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            _noise.m_AmplitudeGain = 0f;
            _noise.m_FrequencyGain = 1f;
        }

        private void Start()
        {
            if (_skiingController != null)
            {
                _skiingController.OnGroundedChanged += HandleGroundedChanged;
                _skiingController.OnRollingChanged += HandleRollingChanged;
            }

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged += HandleZoneChanged;
            }
            else
            {
                SugarRush.Core.GameEvents.OnGlucoseZoneChanged += HandleZoneChanged;
            }
        }

        private void OnDestroy()
        {
            if (_skiingController != null)
            {
                _skiingController.OnGroundedChanged -= HandleGroundedChanged;
                _skiingController.OnRollingChanged -= HandleRollingChanged;
            }

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged -= HandleZoneChanged;
            }
            else
            {
                SugarRush.Core.GameEvents.OnGlucoseZoneChanged -= HandleZoneChanged;
            }
        }

        private void Update()
        {
            UpdateSpeedZoom();
            UpdateRollZoom();
            UpdateCrisisOffset();
        }

        private void UpdateSpeedZoom()
        {
            float speed = _skiingController != null ? _skiingController.Velocity.magnitude : 0f;
            float targetZoom = Mathf.Lerp(0f, _maxOrthoSize - _baseOrthoSize, Mathf.Clamp01(speed / _speedForMaxZoom));
            targetZoom += _rollZoomTimer > 0f ? _rollZoomAmount : 0f;

            _zoomOffset = Mathf.Lerp(_zoomOffset, targetZoom, Time.deltaTime * _zoomSpeed);
            _vcam.m_Lens.OrthographicSize = _baseOrthoSize + _zoomOffset;
        }

        private void UpdateRollZoom()
        {
            if (_rollZoomTimer > 0f)
            {
                _rollZoomTimer -= Time.deltaTime;
            }
        }

        private void UpdateCrisisOffset()
        {
            Vector3 targetOffset = Vector3.zero;
            float targetRotation = 0f;

            if (_glucoseSystem != null)
            {
                var zone = _glucoseSystem.CurrentZone;
                if (zone == GlucoseZone.LowCrisis || zone == GlucoseZone.HighCrisis)
                {
                    float t = Mathf.Sin(Time.time * 3f) * 0.5f + 0.5f;
                    targetOffset = new Vector3(
                        (Mathf.PerlinNoise(Time.time * 2f, 0f) - 0.5f) * 2f * _crisisOffsetAmount,
                        (Mathf.PerlinNoise(0f, Time.time * 2f) - 0.5f) * 2f * _crisisOffsetAmount,
                        0f);
                    targetRotation = (Mathf.PerlinNoise(Time.time * 1.5f, Time.time * 1.5f) - 0.5f) * 2f * _crisisRotationAmount;
                }
            }

            _crisisOffset = Vector3.Lerp(_crisisOffset, targetOffset, Time.deltaTime * _crisisLerpSpeed);
            _crisisRotation = Mathf.Lerp(_crisisRotation, targetRotation, Time.deltaTime * _crisisLerpSpeed);

            var transposer = _vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
            if (transposer != null)
            {
                transposer.m_TrackedObjectOffset = _crisisOffset;
            }

            transform.localRotation = Quaternion.Euler(0f, 0f, _crisisRotation);
        }

        private void HandleGroundedChanged(bool isGrounded)
        {
            if (isGrounded && _skiingController != null)
            {
                float fallSpeed = Mathf.Abs(_skiingController.Velocity.y);
                if (fallSpeed >= _minFallSpeedForShake)
                {
                    StartCoroutine(ShakeCoroutine(_shakeAmplitude * Mathf.Clamp01(fallSpeed / 15f)));
                }
            }
        }

        private void HandleRollingChanged(bool isRolling)
        {
            if (isRolling)
            {
                _rollZoomTimer = _rollZoomDuration;
            }
        }

        private void HandleZoneChanged(GlucoseZone zone)
        {
            // Offset updates are handled in Update.
        }

        private IEnumerator ShakeCoroutine(float amplitude)
        {
            _noise.m_AmplitudeGain = amplitude;
            float elapsed = 0f;
            while (elapsed < _shakeDuration)
            {
                elapsed += Time.deltaTime;
                _noise.m_AmplitudeGain = Mathf.Lerp(amplitude, 0f, elapsed / _shakeDuration);
                yield return null;
            }
            _noise.m_AmplitudeGain = 0f;
        }
    }
}
