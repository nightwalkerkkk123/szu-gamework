using SugarRush.Core;
using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Spawns simple 2D particles for landing, pickups, hits, death, and game-feel VFX.
    /// Creates particles on the fly using a shared white sprite.
    ///
    /// Events wired:
    ///   SkiingController.OnJumped             -> Jump dust
    ///   SkiingController.OnRollingChanged     -> Roll start dust / roll stop (no-op)
    ///   SkiingController.OnExternalImpulse    -> Ramp launch burst
    ///   SkiingController.OnStumbledChanged    -> Stumble spark
    ///   SkiingController.OnShieldActivated    -> Shield activation ring
    ///   SkiingController.OnShieldConsumed     -> Shield consumed flash
    ///   PickupItem.OnItemPickedUp             -> Pickup sparkle (color-improved)
    ///   GameEvents.OnGameFinished             -> Finish line confetti (win only)
    /// </summary>
    public class SimpleParticleSpawner : MonoBehaviour
    {
        [SerializeField] private Sprite _particleSprite;

        [Header("Landing Snow")]
        [SerializeField] private int _landingParticleCount = 8;
        [SerializeField] private float _landingParticleLife = 0.4f;
        [SerializeField] private float _landingParticleSpeed = 3f;
        [SerializeField] private Color _landingColor = new Color(0.9f, 0.95f, 1f, 0.8f);

        [Header("Pickup Sparkle")]
        [SerializeField] private int _pickupParticleCount = 12;
        [SerializeField] private float _pickupParticleLife = 0.5f;
        [SerializeField] private float _pickupParticleSpeed = 4f;

        [Header("Hit Burst")]
        [SerializeField] private int _hitParticleCount = 10;
        [SerializeField] private float _hitParticleLife = 0.35f;
        [SerializeField] private float _hitParticleSpeed = 5f;
        [SerializeField] private Color _hitColor = new Color(1f, 0.2f, 0.2f, 0.9f);

        [Header("Death Burst")]
        [SerializeField] private int _deathParticleCount = 24;
        [SerializeField] private float _deathParticleLife = 0.7f;
        [SerializeField] private float _deathParticleSpeed = 6f;

        [Header("Jump Dust")]
        [SerializeField] private int _jumpDustCount = 7;
        [SerializeField] private float _jumpDustLife = 0.3f;
        [SerializeField] private float _jumpDustSpeed = 2.5f;
        [SerializeField] private Color _jumpDustColor = new Color(0.85f, 0.9f, 0.95f, 0.8f);

        [Header("Roll Start Dust")]
        [SerializeField] private int _rollDustCount = 6;
        [SerializeField] private float _rollDustLife = 0.4f;
        [SerializeField] private float _rollDustSpeed = 2f;
        [SerializeField] private Color _rollDustColor = new Color(0.9f, 0.95f, 1f, 0.7f);

        [Header("Ramp Launch Burst")]
        [SerializeField] private int _rampBurstCount = 14;
        [SerializeField] private float _rampBurstLife = 0.5f;
        [SerializeField] private float _rampBurstSpeed = 5f;
        [SerializeField] private Color _rampBurstColor = new Color(0.5f, 0.95f, 1f, 0.9f);

        [Header("Stumble Spark")]
        [SerializeField] private int _stumbleSparkCount = 14;
        [SerializeField] private float _stumbleSparkLife = 0.35f;
        [SerializeField] private float _stumbleSparkSpeed = 4.5f;
        [SerializeField] private Color _stumbleSparkColor = new Color(1f, 0.25f, 0.15f, 0.9f);

        [Header("Shield Activation Ring")]
        [SerializeField] private int _shieldRingCount = 20;
        [SerializeField] private float _shieldRingLife = 0.4f;
        [SerializeField] private float _shieldRingSpeed = 3f;
        [SerializeField] private Color _shieldRingColor = new Color(0.75f, 0.8f, 0.85f, 0.9f);

        [Header("Shield Consumed Flash")]
        [SerializeField] private int _shieldFlashCount = 10;
        [SerializeField] private float _shieldFlashLife = 0.3f;
        [SerializeField] private float _shieldFlashSpeed = 4f;
        [SerializeField] private Color _shieldFlashColor = new Color(1f, 0.85f, 0.2f, 0.9f);

        [Header("Finish Confetti")]
        [SerializeField] private int _confettiCount = 30;
        [SerializeField] private float _confettiLife = 0.8f;
        [SerializeField] private float _confettiSpeed = 5f;

        [SerializeField] private float _gravity = -10f;

        // Pre-allocated arrays to avoid allocation in hot paths
        private readonly Vector2[] _ringDirections = new Vector2[20];

        private void Awake()
        {
            // Pre-build a evenly-spaced ring pattern (used by shield ring and confetti)
            for (int i = 0; i < _ringDirections.Length; i++)
            {
                float angle = (360f / _ringDirections.Length) * i * Mathf.Deg2Rad;
                _ringDirections[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            }
        }

        private void OnEnable()
        {
            var skiing = GetComponent<SkiingController>();
            if (skiing != null)
            {
                skiing.OnGroundedChanged += HandleGroundedChanged;
                skiing.OnJumped += HandleJumped;
                skiing.OnRollingChanged += HandleRollingChanged;
                skiing.OnExternalImpulse += HandleExternalImpulse;
                skiing.OnStumbledChanged += HandleStumbledChanged;
                skiing.OnShieldActivated += HandleShieldActivated;
                skiing.OnShieldConsumed += HandleShieldConsumed;
            }

            PickupItem.OnItemPickedUp += HandleItemPickedUp;
            GameEvents.OnGameFinished += HandleGameFinished;
        }

        private void OnDisable()
        {
            var skiing = GetComponent<SkiingController>();
            if (skiing != null)
            {
                skiing.OnGroundedChanged -= HandleGroundedChanged;
                skiing.OnJumped -= HandleJumped;
                skiing.OnRollingChanged -= HandleRollingChanged;
                skiing.OnExternalImpulse -= HandleExternalImpulse;
                skiing.OnStumbledChanged -= HandleStumbledChanged;
                skiing.OnShieldActivated -= HandleShieldActivated;
                skiing.OnShieldConsumed -= HandleShieldConsumed;
            }

            PickupItem.OnItemPickedUp -= HandleItemPickedUp;
            GameEvents.OnGameFinished -= HandleGameFinished;
        }

        private void OnDestroy()
        {
            // Handled in OnDisable to ensure symmetric subscription
        }

        #region Public Spawn Methods

        /// <summary>Jump dust — fires on SkiingController.OnJumped when player is grounded at fire time.</summary>
        public void SpawnJumpDust(Vector3 position)
        {
            for (int i = 0; i < _jumpDustCount; i++)
            {
                // Fan downward: angles between 60-120 degrees (pointing mostly down)
                float angle = Random.Range(60f, 120f) * Mathf.Deg2Rad;
                float speed = Random.Range(_jumpDustSpeed * 0.5f, _jumpDustSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _jumpDustLife, _jumpDustColor, 0.1f, 0.03f);
            }
        }

        /// <summary>Roll start dust — fires on SkiingController.OnRollingChanged(true). Snow puff trail behind player.</summary>
        public void SpawnRollDust(Vector3 position)
        {
            for (int i = 0; i < _rollDustCount; i++)
            {
                // Slightly behind and below: angle 100-140 degrees (behind-left to behind-right)
                float angle = Random.Range(100f, 140f) * Mathf.Deg2Rad;
                float speed = Random.Range(_rollDustSpeed * 0.4f, _rollDustSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _rollDustLife, _rollDustColor, 0.12f, 0.04f);
            }
        }

        /// <summary>Ramp launch burst — fires on SkiingController.OnExternalImpulse. Launch dust + cyan ring.</summary>
        public void SpawnRampBurst(Vector3 position)
        {
            // Dust burst at feet
            for (int i = 0; i < _rampBurstCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_rampBurstSpeed * 0.3f, _rampBurstSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _rampBurstLife, _rampBurstColor, 0.14f, 0.04f);
            }

            // Bright cyan ring expanding outward
            SpawnRingBurst(position, _rampBurstCount, _rampBurstLife * 0.8f, _rampBurstSpeed * 1.2f, Color.cyan);
        }

        /// <summary>Stumble spark — fires on SkiingController.OnStumbledChanged(true). Radial red sparks at feet.</summary>
        public void SpawnStumbleSpark(Vector3 position)
        {
            for (int i = 0; i < _stumbleSparkCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_stumbleSparkSpeed * 0.4f, _stumbleSparkSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _stumbleSparkLife, _stumbleSparkColor, 0.12f, 0.03f);
            }
        }

        /// <summary>Shield activation ring — fires on SkiingController.OnShieldActivated. Silver ring expanding from player.</summary>
        public void SpawnShieldRing(Vector3 position)
        {
            SpawnRingBurst(position, _shieldRingCount, _shieldRingLife, _shieldRingSpeed, _shieldRingColor);
        }

        /// <summary>Shield consumed flash — fires on SkiingController.OnShieldConsumed. Golden shockwave outward.</summary>
        public void SpawnShieldFlash(Vector3 position)
        {
            SpawnRingBurst(position, _shieldFlashCount, _shieldFlashLife, _shieldFlashSpeed, _shieldFlashColor);
        }

        /// <summary>Finish confetti — fires on GameEvents.OnGameFinished(true). Multicolor burst at player.</summary>
        public void SpawnConfetti(Vector3 position)
        {
            Color[] confettiColors = new Color[]
            {
                new Color(1f, 0.3f, 0.3f, 1f),
                new Color(0.3f, 1f, 0.3f, 1f),
                new Color(0.3f, 0.5f, 1f, 1f),
                new Color(1f, 0.85f, 0.2f, 1f),
                new Color(1f, 0.5f, 0.9f, 1f),
                Color.white
            };

            for (int i = 0; i < _confettiCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_confettiSpeed * 0.3f, _confettiSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                Color color = confettiColors[i % confettiColors.Length];
                SpawnParticle(position, vel, _confettiLife, color, 0.15f, 0.04f);
            }
        }

        // Existing methods kept as-is

        public void SpawnLandingParticles(Vector3 position)
        {
            for (int i = 0; i < _landingParticleCount; i++)
            {
                float angle = Random.Range(80f, 100f) * Mathf.Deg2Rad;
                float speed = Random.Range(_landingParticleSpeed * 0.5f, _landingParticleSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _landingParticleLife, _landingColor, 0.12f, 0.04f);
            }
        }

        public void SpawnPickupParticles(Vector3 position, Color color)
        {
            for (int i = 0; i < _pickupParticleCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_pickupParticleSpeed * 0.3f, _pickupParticleSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _pickupParticleLife, color, 0.15f, 0.05f);
            }
        }

        public void SpawnHitParticles(Vector3 position)
        {
            for (int i = 0; i < _hitParticleCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_hitParticleSpeed * 0.3f, _hitParticleSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _hitParticleLife, _hitColor, 0.14f, 0.05f);
            }
        }

        public void SpawnDeathParticles(Vector3 position, Color color)
        {
            for (int i = 0; i < _deathParticleCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_deathParticleSpeed * 0.3f, _deathParticleSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _deathParticleLife, color, 0.2f, 0.05f);
            }
        }

        #endregion

        #region Private Helpers

        private void SpawnParticle(Vector3 position, Vector2 velocity, float life, Color color, float startScale, float endScale)
        {
            var go = new GameObject("Particle");
            go.transform.position = position;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _particleSprite;
            sr.color = color;
            sr.sortingOrder = 10;

            var particle = go.AddComponent<SimpleParticle>();
            particle.Init(velocity, life, _gravity, color, startScale, endScale);
        }

        /// <summary>Spawns a ring of particles evenly distributed in a circle, all moving outward.</summary>
        private void SpawnRingBurst(Vector3 position, int count, float life, float speed, Color color)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 vel = _ringDirections[i % _ringDirections.Length] * speed;
                float lifeVar = life * Random.Range(0.85f, 1.15f);
                SpawnParticle(position, vel, lifeVar, color, 0.12f, 0.0f);
            }
        }

        #endregion

        #region Event Handlers

        private void HandleGroundedChanged(bool isGrounded)
        {
            if (isGrounded)
            {
                SpawnLandingParticles(transform.position);
            }
        }

        private void HandleJumped()
        {
            // Only fire if currently grounded (jump dust under feet makes sense mid-ground)
            var skiing = GetComponent<SkiingController>();
            if (skiing != null && skiing.IsGrounded)
            {
                SpawnJumpDust(transform.position);
            }
        }

        private void HandleRollingChanged(bool isRolling)
        {
            if (isRolling)
            {
                SpawnRollDust(transform.position);
            }
        }

        private void HandleExternalImpulse(Vector2 impulse)
        {
            SpawnRampBurst(transform.position);
        }

        private void HandleStumbledChanged(bool isStumbled)
        {
            if (isStumbled)
            {
                SpawnStumbleSpark(transform.position);
            }
        }

        private void HandleShieldActivated()
        {
            SpawnShieldRing(transform.position);
        }

        private void HandleShieldConsumed()
        {
            SpawnShieldFlash(transform.position);
        }

        private void HandleItemPickedUp(PickupItem pickup, ItemEffect effect, GameObject player)
        {
            if (player != gameObject) return;
            Color color = GetPickupColor(effect);
            SpawnPickupParticles(player.transform.position, color);
        }

        private void HandleGameFinished(bool win)
        {
            if (win)
            {
                SpawnConfetti(transform.position);
            }
            // On false (lose): death particles already fire from Obstacle hit; skip
        }

        /// <summary>Maps pickup ItemEffect to a particle color. Extends the original logic with shield/magnet.</summary>
        private Color GetPickupColor(ItemEffect effect)
        {
            if (effect == null) return Color.white;

            string name = effect.DisplayName.ToLowerInvariant();
            if (name.Contains("shield")) return new Color(0.75f, 0.8f, 0.85f, 1f);   // silver
            if (name.Contains("magnet")) return new Color(0.85f, 0.3f, 1f, 1f);    // purple
            if (name.Contains("insulin")) return Color.blue;
            if (name.Contains("pill")) return Color.green;
            if (name.Contains("snowflake")) return Color.yellow;

            return Color.white;
        }

        #endregion
    }
}