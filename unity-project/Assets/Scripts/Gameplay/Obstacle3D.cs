using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// 3D obstacle collision handler for the fused endless-runner mode.
    /// </summary>
    public class Obstacle3D : MonoBehaviour
    {
        public enum ObstacleType { Stumble, Crash }

        [SerializeField] private ObstacleType _type = ObstacleType.Crash;

        private void OnCollisionEnter(Collision collision)
        {
            if (!collision.collider.CompareTag("Player")) return;

            if (collision.collider.TryGetComponent<SpherePlayerController>(out var controller))
            {
                switch (_type)
                {
                    case ObstacleType.Crash:
                        controller.Die();
                        break;
                }
            }
        }
    }
}
