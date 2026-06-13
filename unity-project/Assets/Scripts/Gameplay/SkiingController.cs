using SugarRush.Core;
using SugarRush.Foundation;
using System;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Core player skiing controller. Physics-driven with jump, roll, stumble and glucose-aware modifiers.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class SkiingController : MonoBehaviour
    {
        [SerializeField] private SkiingConfig _config;
        [SerializeField] private GlucoseSystem _glucoseSystem;
        [SerializeField] private SugarRushInput _input;

        private Rigidbody2D _rb;
        private TimerHandle _rollTimer;
        private TimerHandle _rollCooldownTimer;
        private TimerHandle _stumbleTimer;
        private bool _isGrounded;
        private bool _isRolling;
        private bool _isStumbled;

        public bool IsGrounded => _isGrounded;
        public bool IsRolling => _isRolling;
        public bool IsStumbled => _isStumbled;
        public Vector2 Velocity => _rb.velocity;

        public event Action<bool> OnGroundedChanged;
        public event Action<bool> OnRollingChanged;
        public event Action<bool> OnStumbledChanged;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = _config.gravityScale;
            _rb.freezeRotation = true;

            if (_config == null)
            {
                Debug.LogError("[SkiingController] SkiingConfig not assigned.", this);
                enabled = false;
                return;
            }

            if (_glucoseSystem == null)
            {
                Debug.LogWarning("[SkiingController] GlucoseSystem not assigned; modifiers default to 1.", this);
            }

            if (_input == null)
            {
                Debug.LogWarning("[SkiingController] SugarRushInput not assigned; input disabled.", this);
            }
            else
            {
                _input.OnJumpPressed += TryJump;
                _input.OnRollPressed += TryRoll;
            }

            SetupTimers();
        }

        private void OnEnable()
        {
            TimerService.Instance.Register(_rollTimer);
            TimerService.Instance.Register(_rollCooldownTimer);
            TimerService.Instance.Register(_stumbleTimer);
        }

        private void OnDisable()
        {
            if (TimerService.Instance != null)
            {
                TimerService.Instance.Unregister(_rollTimer);
                TimerService.Instance.Unregister(_rollCooldownTimer);
                TimerService.Instance.Unregister(_stumbleTimer);
            }
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.OnJumpPressed -= TryJump;
                _input.OnRollPressed -= TryRoll;
            }
        }

        private void SetupTimers()
        {
            _rollTimer = new TimerHandle(0f);
            _rollTimer.OnExpired += EndRoll;

            _rollCooldownTimer = new TimerHandle(0f);

            _stumbleTimer = new TimerHandle(0f);
            _stumbleTimer.OnExpired += EndStumble;
        }

        private void FixedUpdate()
        {
            bool wasGrounded = _isGrounded;
            _isGrounded = CheckGrounded();
            if (wasGrounded != _isGrounded)
            {
                OnGroundedChanged?.Invoke(_isGrounded);
            }

            if (_isStumbled) return;

            float speedMod = _glucoseSystem != null ? _glucoseSystem.SpeedModifier : 1f;
            float controlMod = _glucoseSystem != null ? _glucoseSystem.ControlModifier : 1f;

            ApplyDownhillForce(speedMod);
            ApplySpeedLimit(speedMod);

            if (_isRolling)
            {
                // Slight forward boost, reduced gravity
                _rb.AddForce(Vector2.right * _config.rollSpeedBoost * speedMod, ForceMode2D.Force);
            }
            else
            {
                ApplyAirControl(controlMod);
                ApplyFriction();
            }
        }

        private void ApplyDownhillForce(float speedMod)
        {
            Vector2 slopeDir = _config.SlopeDirection;
            float accel = _config.baseAcceleration * speedMod;
            _rb.AddForce(slopeDir * accel, ForceMode2D.Force);
        }

        private void ApplySpeedLimit(float speedMod)
        {
            float max = _config.maxSpeed * speedMod;
            Vector2 vel = _rb.velocity;
            if (vel.x > max)
            {
                vel.x = max;
                _rb.velocity = vel;
            }
        }

        private void ApplyAirControl(float controlMod)
        {
            if (_isGrounded) return;

            // Steer toward target horizontal speed using force rather than direct velocity assignment.
            float desiredX = _config.maxSpeed * controlMod;
            float deltaX = desiredX - _rb.velocity.x;
            float forceX = deltaX * _config.airControl * _rb.mass;
            _rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Force);
        }

        private void ApplyFriction()
        {
            if (!_isGrounded) return;

            // Apply horizontal drag as force instead of directly modifying velocity.
            float frictionForce = -_rb.velocity.x * _config.groundFriction * _rb.mass;
            _rb.AddForce(new Vector2(frictionForce, 0f), ForceMode2D.Force);
        }

        private bool CheckGrounded()
        {
            Vector2 origin = new Vector2(transform.position.x, transform.position.y - _config.groundCheckOffset);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, _config.groundCheckDistance, _config.groundMask);
            Debug.DrawRay(origin, Vector2.down * _config.groundCheckDistance, hit ? Color.green : Color.red);
            return hit.collider != null;
        }

        private void TryJump()
        {
            if (!enabled || _isStumbled || _isRolling || !_isGrounded) return;

            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            _rb.AddForce(Vector2.up * _config.jumpForce, ForceMode2D.Impulse);
        }

        private void TryRoll()
        {
            if (!enabled || _isStumbled || _isRolling || _rollCooldownTimer.IsRunning) return;

            _isRolling = true;
            OnRollingChanged?.Invoke(true);
            _rollTimer.Reset(_config.rollDuration);
            _rollCooldownTimer.Reset(_config.rollDuration + _config.rollCooldown);

            // Shrink hitbox placeholder: could animate scale or disable main collider and enable roll collider.
            Debug.Log("[SkiingController] Roll started.", this);
        }

        private void EndRoll()
        {
            _isRolling = false;
            OnRollingChanged?.Invoke(false);
            Debug.Log("[SkiingController] Roll ended.", this);
        }

        public void TriggerStumble()
        {
            if (_isRolling) return; // invulnerable

            _isStumbled = true;
            OnStumbledChanged?.Invoke(true);
            _stumbleTimer.Reset(_config.stumbleDuration);

            Vector2 vel = _rb.velocity;
            vel.x *= _config.stumbleSpeedFactor;
            _rb.velocity = vel;

            Debug.Log("[SkiingController] Stumbled.", this);
        }

        public void TriggerCrash()
        {
            if (_isRolling) return; // invulnerable

            enabled = false;
            _rb.velocity = Vector2.zero;
            Debug.Log("[SkiingController] Crashed.", this);
        }

        private void EndStumble()
        {
            _isStumbled = false;
            OnStumbledChanged?.Invoke(false);
            Debug.Log("[SkiingController] Stumble ended.", this);
        }

        public void SetEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (!enabled && _rb != null)
            {
                _rb.velocity = Vector2.zero;
            }
        }
    }
}
