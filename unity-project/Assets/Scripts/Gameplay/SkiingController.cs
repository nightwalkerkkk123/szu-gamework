using SugarRush.Core;
using SugarRush.Foundation;
using System;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Core player skiing controller. Physics-driven with jump, roll, stumble and glucose-aware modifiers.
    /// Includes coyote time, jump buffering and landing feedback.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class SkiingController : MonoBehaviour
    {
        [SerializeField] private SkiingConfig _config;
        [SerializeField] private GlucoseSystem _glucoseSystem;
        [SerializeField] private SugarRushInput _input;
        [SerializeField] private BoxCollider2D _hitbox;

        private Rigidbody2D _rb;
        private TimerHandle _rollTimer;
        private TimerHandle _rollCooldownTimer;
        private TimerHandle _stumbleTimer;
        private bool _isGrounded;
        private bool _wasGrounded;
        private bool _isRolling;
        private bool _isStumbled;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private Vector2 _standingHitboxSize;
        private Vector2 _standingHitboxOffset;

        public bool IsGrounded => _isGrounded;
        public bool IsRolling => _isRolling;
        public bool IsStumbled => _isStumbled;
        public Vector2 Velocity => _rb.velocity;

        public event Action<bool> OnGroundedChanged;
        public event Action<bool> OnRollingChanged;
        public event Action<bool> OnStumbledChanged;
        public event Action OnJumped;
        public event Action OnLanded;

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
                _input.OnJumpPressed += QueueJump;
                _input.OnRollPressed += TryRoll;
            }

            SetupTimers();

            if (_hitbox == null)
            {
                _hitbox = GetComponent<BoxCollider2D>();
            }

            if (_hitbox != null)
            {
                _standingHitboxSize = _hitbox.size;
                _standingHitboxOffset = _hitbox.offset;
            }
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
                _input.OnJumpPressed -= QueueJump;
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

        private void Update()
        {
            if (_coyoteTimer > 0f) _coyoteTimer -= Time.deltaTime;
            if (_jumpBufferTimer > 0f) _jumpBufferTimer -= Time.deltaTime;

            // Try buffered jump if now grounded or within coyote time.
            if (_jumpBufferTimer > 0f && CanJump())
            {
                ExecuteJump();
                _jumpBufferTimer = 0f;
            }
        }

        private void FixedUpdate()
        {
            _wasGrounded = _isGrounded;
            _isGrounded = CheckGrounded();

            if (_isGrounded && !_wasGrounded)
            {
                OnGroundedChanged?.Invoke(true);
                ApplyLandingBuffer();
                OnLanded?.Invoke();
            }
            else if (!_isGrounded && _wasGrounded)
            {
                OnGroundedChanged?.Invoke(false);
                _coyoteTimer = _config.coyoteTime;
            }

            if (_isStumbled) return;

            float speedMod = _glucoseSystem != null ? _glucoseSystem.SpeedModifier : 1f;
            float controlMod = _glucoseSystem != null ? _glucoseSystem.ControlModifier : 1f;

            ApplyDownhillForce(speedMod);
            ApplySpeedLimit(speedMod);

            if (_isRolling)
            {
                // Strong forward boost and reduced gravity during roll.
                _rb.AddForce(Vector2.right * _config.rollSpeedBoost * speedMod, ForceMode2D.Force);
                _rb.gravityScale = _config.gravityScale * 0.5f;
            }
            else
            {
                _rb.gravityScale = _config.gravityScale;
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

        private void ApplyLandingBuffer()
        {
            if (_config.landingBounce <= 0f) return;

            Vector2 vel = _rb.velocity;
            if (vel.y < -1f)
            {
                vel.y = Mathf.Min(vel.y * -0.3f, _config.landingBounce);
                _rb.velocity = vel;
            }
        }

        private bool CheckGrounded()
        {
            Vector2 origin = new Vector2(transform.position.x, transform.position.y - _config.groundCheckOffset);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, _config.groundCheckDistance, _config.groundMask);
            Debug.DrawRay(origin, Vector2.down * _config.groundCheckDistance, hit ? Color.green : Color.red);
            return hit.collider != null;
        }

        private void QueueJump()
        {
            if (!enabled || _isStumbled || _isRolling) return;
            _jumpBufferTimer = _config.jumpBufferTime;

            if (CanJump())
            {
                ExecuteJump();
                _jumpBufferTimer = 0f;
            }
        }

        private bool CanJump()
        {
            return _isGrounded || _coyoteTimer > 0f;
        }

        private void ExecuteJump()
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0f);
            _rb.AddForce(Vector2.up * _config.jumpForce, ForceMode2D.Impulse);
            _coyoteTimer = 0f;
            OnJumped?.Invoke();
        }

        private void TryRoll()
        {
            if (!enabled || _isStumbled || _isRolling || _rollCooldownTimer.IsRunning) return;

            _isRolling = true;
            OnRollingChanged?.Invoke(true);
            _rollTimer.Reset(_config.rollDuration);
            _rollCooldownTimer.Reset(_config.rollDuration + _config.rollCooldown);

            if (_hitbox != null)
            {
                _hitbox.size = new Vector2(_standingHitboxSize.x, _standingHitboxSize.y * _config.rollHitboxHeightScale);
                _hitbox.offset = new Vector2(_standingHitboxOffset.x, _standingHitboxOffset.y - _standingHitboxSize.y * (1f - _config.rollHitboxHeightScale) * 0.5f);
            }

            // Small instant forward kick.
            Vector2 vel = _rb.velocity;
            vel.x = Mathf.Min(vel.x + _config.rollInitialBoost, _config.maxSpeed * 1.2f);
            _rb.velocity = vel;

            Debug.Log("[SkiingController] Roll started.", this);
        }

        private void EndRoll()
        {
            _isRolling = false;
            OnRollingChanged?.Invoke(false);

            if (_hitbox != null)
            {
                _hitbox.size = _standingHitboxSize;
                _hitbox.offset = _standingHitboxOffset;
            }

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
            vel.y = Mathf.Abs(vel.y) * 0.5f + 2f;
            _rb.velocity = vel;

            // Tumble rotation visual (physics stays upright).
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-25f, 25f));

            Debug.Log("[SkiingController] Stumbled.", this);
        }

        public void TriggerCrash()
        {
            if (_isRolling) return; // invulnerable

            if (TryGetComponent<SimpleParticleSpawner>(out var spawner))
            {
                Color color = TryGetComponent<SpriteRenderer>(out var sr) ? sr.color : Color.cyan;
                spawner.SpawnDeathParticles(transform.position, color);
            }

            if (TryGetComponent<PlayerVisuals>(out var visuals))
            {
                visuals.TriggerDeathVisual();
            }

            enabled = false;
            _rb.velocity = Vector2.zero;
            _rb.gravityScale = 0f;
            Debug.Log("[SkiingController] Crashed.", this);
        }

        private void EndStumble()
        {
            _isStumbled = false;
            transform.rotation = Quaternion.identity;
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
