using SugarRush.Core;
using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// 3D sphere controller for the fused endless-runner mode.
    /// Preserves SugarRush glucose mechanics while driving a rolling sphere.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class SpherePlayerController : MonoBehaviour, IPlayerController
    {
        [SerializeField] private GlucoseSystem _glucoseSystem;
        [SerializeField] private SugarRushInput _input;

        [Header("Movement")]
        [SerializeField] private float _baseAcceleration = 18f;
        [SerializeField] private float _maxSpeed = 25f;
        [SerializeField] private float _jumpForce = 8f;
        [Tooltip("Extra downward acceleration while airborne. Lowers the jump apex AND shortens hang time, turning the floaty default jump into a crisp arcade hop. Mass-independent (Acceleration mode).")]
        [SerializeField] private float _extraFallGravity = 16f;
        [SerializeField] private float _groundCheckDistance = 0.55f;
        [SerializeField] private LayerMask _groundMask;

        [Header("Glucose Impact")]
        [SerializeField] private float _minSpeedMultiplier = 0.5f;
        [SerializeField] private float _maxSpeedMultiplier = 1.25f;

        [Header("Fall Death")]
        [Tooltip("Seconds of continuous falling (airborne, descending) before the run fails. Replaces the fixed gap death zone on the downhill track.")]
        [SerializeField] private float _fallDeathSeconds = 2f;

        private Rigidbody _rb;
        private bool _isGrounded;
        private bool _isDead;
        private float _fallTimer;

        public bool IsGrounded => _isGrounded;
        public float Speed => _rb.velocity.magnitude;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;

            if (_glucoseSystem == null) _glucoseSystem = GetComponent<GlucoseSystem>();
            if (_input == null) _input = FindObjectOfType<SugarRushInput>();
            if (_input != null) _input.OnJumpPressed += TryJump;
            Debug.Log($"[SpherePlayerController-DIAG] Awake: _input={(_input != null ? _input.name : "NULL")}, subscribed={(_input != null)}");
        }

        private void OnDestroy()
        {
            if (_input != null) _input.OnJumpPressed -= TryJump;
        }

        private void FixedUpdate()
        {
            if (_isDead) return;

            _isGrounded = CheckGrounded();

            // Fall-to-death: if airborne and descending for too long, the player has
            // dropped into a gap (no fixed death plane works on a descending track).
            if (!_isGrounded && _rb.velocity.y < -1f)
            {
                _fallTimer += Time.fixedDeltaTime;
                if (_fallTimer > _fallDeathSeconds)
                {
                    Die();
                    return;
                }
            }
            else
            {
                _fallTimer = 0f;
            }

            float speedMod = CalculateSpeedModifier();
            float controlMod = _glucoseSystem != null ? _glucoseSystem.ControlModifier : 1f;

            // Forward acceleration along the slope / platform.
            _rb.AddForce(Vector3.right * _baseAcceleration * speedMod, ForceMode.Force);

            // Clamp horizontal speed.
            Vector3 velocity = _rb.velocity;
            if (velocity.x > _maxSpeed * speedMod)
            {
                velocity.x = _maxSpeed * speedMod;
                _rb.velocity = velocity;
            }

            // Reduced air control based on glucose.
            if (!_isGrounded)
            {
                float desiredX = _maxSpeed * speedMod * controlMod;
                float deltaX = desiredX - velocity.x;
                _rb.AddForce(Vector3.right * deltaX * 2f * controlMod, ForceMode.Force);

                // Extra fall gravity: crisper, less floaty jump arc.
                _rb.AddForce(Vector3.down * _extraFallGravity, ForceMode.Acceleration);
            }
        }

        private float CalculateSpeedModifier()
        {
            if (_glucoseSystem == null) return 1f;

            float t = _glucoseSystem.CurrentValue / 100f;
            return Mathf.Lerp(_minSpeedMultiplier, _maxSpeedMultiplier, t);
        }

        private void TryJump()
        {
            // Grounded check removed per user request: allow jump regardless of ground state (enables double-jump).
            if (!enabled || _isDead) return;

            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }

        private bool CheckGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundMask);
        }

        public void Die()
        {
            if (_isDead) return;
            _isDead = true;
            enabled = false;
            _rb.velocity = Vector3.zero;
            _rb.useGravity = false;
            GameEvents.RaiseGameFinished(false);
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (_rb != null)
            {
                _rb.velocity = enabled ? _rb.velocity : Vector3.zero;
                _rb.useGravity = enabled;
            }
        }
    }
}
