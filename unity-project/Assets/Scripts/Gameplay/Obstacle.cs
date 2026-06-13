using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Obstacle collision handler. Supports stumble and crash outcomes.
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        public enum ObstacleType { Stumble, Crash }

        [SerializeField] private ObstacleType _type = ObstacleType.Stumble;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Player")) return;

            if (collision.collider.TryGetComponent<SkiingController>(out var controller))
            {
                if (controller.IsRolling) return;

                var contact = collision.GetContact(0);

                if (collision.collider.TryGetComponent<SimpleParticleSpawner>(out var spawner))
                {
                    spawner.SpawnHitParticles(contact.point);
                }

                if (collision.collider.TryGetComponent<PlayerVisuals>(out var visuals))
                {
                    visuals.TriggerHurtFlash();
                }

                switch (_type)
                {
                    case ObstacleType.Stumble:
                        controller.TriggerStumble();
                        break;
                    case ObstacleType.Crash:
                        controller.TriggerCrash();
                        break;
                }
            }
        }
    }
}
