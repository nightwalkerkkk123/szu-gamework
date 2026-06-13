using SugarRush.Foundation;
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
        private float _rollTimer;
        private float _rollCooldownTimer;
        private float _stumbleTimer;
        private bool _isGrounded;
        private bool _isRolling;
        private bool _isStumbled;

        public bool IsGrounded => _isGrounded;
        public bool IsRolling => _isRolling;
        public bool IsStumbled => _isStumbled;
        public Vector2 Velocity => _rb.velocity;

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
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.OnJumpPressed -= TryJump;
                _input.OnRollPressed -= TryRoll;
            }
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if (_rollTimer > 0f)
            {
                _rollTimer -= dt;
                if (_rollTimer <= 0f) EndRoll();
            }

            if (_rollCooldownTimer > 0f)
            {
                _rollCooldownTimer -= dt;
            }

            if (_stumbleTimer > 0f)
            {
                _stumbleTimer -= dt;
                if (_stumbleTimer <= 0f) EndStumble();
            }
        }

        private void FixedUpdate()
        {
            _isGrounded = CheckGrounded();

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

            // Minimal air steer placeholder; can be expanded with directional input.
            Vector2 vel = _rb.velocity;
            vel.x = Mathf.Lerp(vel.x, _config.maxSpeed * controlMod, _config.airControl * Time.fixedDeltaTime);
            _rb.velocity = vel;
        }

        private void ApplyFriction()
        {
            if (!_isGrounded) return;

            Vector2 vel = _rb.velocity;
            vel.x = Mathf.MoveTowards(vel.x, 0f, _config.groundFriction * Time.fixedDeltaTime);
            _rb.velocity = vel;
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
            if (!enabled || _isStumbled || _isRolling || _rollCooldownTimer > 0f) return;

            _isRolling = true;
            _rollTimer = _config.rollDuration;
            _rollCooldownTimer = _config.rollDuration + _config.rollCooldown;

            // Shrink hitbox placeholder: could animate scale or disable main collider and enable roll collider.
            Debug.Log("[SkiingController] Roll started.", this);
        }

        private void EndRoll()
        {
            _isRolling = false;
            Debug.Log("[SkiingController] Roll ended.", this);
        }

        public void TriggerStumble()
        {
            if (_isRolling) return; // invulnerable

            _isStumbled = true;
            _stumbleTimer = _config.stumbleDuration;

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
