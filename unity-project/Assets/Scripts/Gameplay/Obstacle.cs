using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Obstacle collision handler. Dispatches outcome based on <see cref="ObstacleType"/>
    /// and the player's current airborne/rolling state.
    /// </summary>
    /// <remarks>
    /// Type semantics:
    /// <list type="bullet">
    /// <item><b>Stumble</b> — always stumbles; jump and roll do not help.</item>
    /// <item><b>Crash</b> — always crashes; jump and roll do not help.</item>
    /// <item><b>Jumpable</b> — safe if the player is airborne at contact. If the
    /// obstacle has a non-zero <see cref="HeightMeters"/>, the player must also
    /// clear that height; otherwise the contact is treated as a stumble.</item>
    /// <item><b>Rollable</b> — safe if the player is rolling at contact. No-op
    /// otherwise (stumble).</item>
    /// <item><b>Avoidable</b> — any contact crashes; no jump or roll invuln applies.</item>
    /// <item><b>Ramp</b> — trigger only. Launches the player upward with a vertical
    /// impulse (jumpForce × 1.2) for a positive setpiece moment.</item>
    /// <item><b>Spike</b> — safe if rolling; otherwise <see cref="SkiingController.TriggerCrash"/>.
    /// Stricter than Rollable (which only stumbles on miss).</item>
    /// </list>
    /// Ramp is the only type that uses <see cref="OnTriggerEnter2D"/>; the
    /// builder must set its <see cref="BoxCollider2D.isTrigger"/> to true.
    /// </remarks>
    public class Obstacle : MonoBehaviour
    {
        public enum ObstacleType
        {
            Stumble,
            Crash,
            Jumpable,
            Rollable,
            Avoidable,
            Ramp,
            Spike,
        }

        [SerializeField] private ObstacleType _type = ObstacleType.Stumble;

        [Tooltip("For Jumpable: required clearance in meters. 0 = any jump clears. >0 = player must jump high enough to clear this height; otherwise stumble.")]
        [SerializeField] private float _heightMeters = 0f;

        [Tooltip("Ramp launch impulse multiplier vs SkiingConfig.jumpForce. Only used by Ramp type.")]
        [SerializeField, Min(0.5f)] private float _rampImpulseMultiplier = 1.2f;

        public ObstacleType Type => _type;

        public float HeightMeters => _heightMeters;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Player")) return;

            if (!collision.collider.TryGetComponent<SkiingController>(out var controller))
                return;

            // Avoidable bypasses every invuln — always crash on contact.
            if (_type == ObstacleType.Avoidable)
            {
                ApplyHitFeedback(collision, controller);
                controller.TriggerCrash();
                return;
            }

            // Roll invuln applies to Stumble/Crash/Jumpable/Rollable/Spike uniformly.
            if (controller.IsRolling)
                return;

            switch (_type)
            {
                case ObstacleType.Stumble:
                    ApplyHitFeedback(collision, controller);
                    controller.TriggerStumble();
                    return;

                case ObstacleType.Crash:
                    ApplyHitFeedback(collision, controller);
                    controller.TriggerCrash();
                    return;

                case ObstacleType.Jumpable:
                    if (!controller.IsGrounded && ClearsHeight(controller))
                        return;
                    ApplyHitFeedback(collision, controller);
                    controller.TriggerStumble();
                    return;

                case ObstacleType.Rollable:
                    ApplyHitFeedback(collision, controller);
                    controller.TriggerStumble();
                    return;

                case ObstacleType.Spike:
                    // Stricter than Rollable: missing the roll = Crash, not Stumble.
                    ApplyHitFeedback(collision, controller);
                    controller.TriggerCrash();
                    return;
            }
        }

        // Ramp uses a trigger collider (set by L1SceneBuilder) so the player passes through
        // while still receiving the upward impulse. All other types stay on collision.
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_type != ObstacleType.Ramp) return;
            if (!other.CompareTag("Player")) return;
            if (!other.TryGetComponent<SkiingController>(out var controller)) return;
            if (controller.Config == null) return;

            controller.ApplyExternalImpulse(Vector2.up * controller.Config.jumpForce * _rampImpulseMultiplier);
        }

        private void ApplyHitFeedback(Collision2D collision, SkiingController controller)
        {
            var contact = collision.GetContact(0);

            if (collision.collider.TryGetComponent<SimpleParticleSpawner>(out var spawner))
            {
                spawner.SpawnHitParticles(contact.point);
            }

            if (collision.collider.TryGetComponent<PlayerVisuals>(out var visuals))
            {
                visuals.TriggerHurtFlash();
            }
        }

        /// <summary>
        /// Cheap first-cut height clearance check using the projectile formula
        /// h = v² / (2g) against <see cref="SkiingConfig.jumpForce"/> and
        /// <see cref="SkiingConfig.gravityScale"/>. Returns true when the obstacle's
        /// required clearance is within the player's max jump height.
        /// </summary>
        private bool ClearsHeight(SkiingController controller)
        {
            if (_heightMeters <= 0f) return true;
            var cfg = controller.Config;
            if (cfg == null) return true; // fail-open: if config missing, assume any jump clears

            float g = Mathf.Abs(Physics2D.gravity.y) * cfg.gravityScale;
            if (g <= 0f) return true;

            float maxHeight = (cfg.jumpForce * cfg.jumpForce) / (2f * g);
            return _heightMeters <= maxHeight;
        }
    }
}
