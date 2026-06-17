using SugarRush.Gameplay;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Player squash and stretch driven by jump, land and roll events.
    /// Modifies transform.localScale only — orthogonal to PlayerVisuals localPosition shake.
    /// </summary>
    public class PlayerSquashStretch : MonoBehaviour
    {
        [SerializeField] private SkiingController _skiingController;

        [Header("Jump Squash")]
        [SerializeField] private Vector3 _jumpScale = new Vector3(0.85f, 1.3f, 1f);
        [SerializeField] private float _jumpSquashDuration = 0.1f;

        [Header("Land Squash")]
        [SerializeField] private Vector3 _landScale = new Vector3(1.2f, 0.8f, 1f);
        [SerializeField] private float _landSquashDuration = 0.08f;

        [Header("Roll Squash")]
        [SerializeField] private Vector3 _rollScale = new Vector3(0.6f, 0.6f, 1f);

        private Vector3 _targetScale = Vector3.one;
        private Vector3 _currentScale = Vector3.one;
        private float _squashTimer;

        private void OnEnable()
        {
            if (_skiingController != null)
            {
                _skiingController.OnJumped += HandleJumped;
                _skiingController.OnLanded += HandleLanded;
                _skiingController.OnRollingChanged += HandleRollingChanged;
            }
        }

        private void OnDisable()
        {
            if (_skiingController != null)
            {
                _skiingController.OnJumped -= HandleJumped;
                _skiingController.OnLanded -= HandleLanded;
                _skiingController.OnRollingChanged -= HandleRollingChanged;
            }
        }

        private void Update()
        {
            // Lerp current scale toward target each frame.
            _currentScale = Vector3.Lerp(_currentScale, _targetScale, Time.deltaTime * 20f);
            transform.localScale = _currentScale;

            // Countdown for transient squash (jump / land). Rolling is sustained, no timer.
            if (_squashTimer > 0f)
            {
                _squashTimer -= Time.deltaTime;
                if (_squashTimer <= 0f)
                {
                    _targetScale = Vector3.one;
                }
            }
        }

        private void HandleJumped()
        {
            _targetScale = _jumpScale;
            _squashTimer = _jumpSquashDuration;
        }

        private void HandleLanded()
        {
            _targetScale = _landScale;
            _squashTimer = _landSquashDuration;
        }

        private void HandleRollingChanged(bool isRolling)
        {
            if (isRolling)
            {
                _squashTimer = 0f;
                _targetScale = _rollScale;
            }
            else
            {
                _targetScale = Vector3.one;
            }
        }
    }
}