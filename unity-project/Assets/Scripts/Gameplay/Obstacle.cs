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
    /// </list>
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
        }

        [SerializeField] private ObstacleType _type = ObstacleType.Stumble;

        [Tooltip("For Jumpable: required clearance in meters. 0 = any jump clears. >0 = player must jump high enough to clear this height; otherwise stumble.")]
        [SerializeField] private float _heightMeters = 0f;

        public ObstacleType Type => _type;

        public float HeightMeters => _heightMeters;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Player")) return;

            if (!collision.collider.TryGetComponent<SkiingController>(out var controller))
                return;

            // Avoidable bypasses every invuln — always crash on contact, even while
            // rolling (force: true overrides the roll invulnerability window).
            if (_type == ObstacleType.Avoidable)
            {
                ApplyHitFeedback(collision, controller);
                controller.TriggerCrash(force: true);
                return;
            }

            // Roll invuln applies to Stumble/Crash/Jumpable/Rollable uniformly.
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
            }
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
        /// Height clearance check: the obstacle is cleared when its required
        /// clearance is within the player's max jump height. The mass-aware
        /// projectile math lives on <see cref="SkiingController.MaxJumpHeight"/>
        /// so it stays correct even when the player's Rigidbody2D mass != 1.
        /// </summary>
        private bool ClearsHeight(SkiingController controller)
        {
            if (_heightMeters <= 0f) return true;
            return _heightMeters <= controller.MaxJumpHeight;
        }
    }
}
