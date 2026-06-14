using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Trigger zone below the level that kills the player when they fall into a gap.
    /// </summary>
    public class GapDeathZone : MonoBehaviour
    {
        [SerializeField] private float _deathY = -20f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (other.transform.position.y >= _deathY) return;

            if (other.TryGetComponent<SkiingController>(out var controller))
            {
                controller.TriggerCrash();
            }
        }
    }
}
