using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Trigger zone below the level that kills the player when they fall into a gap.
    /// </summary>
    public class GapDeathZone : MonoBehaviour
    {
        // The zone's GameObject follows the camera (see scene setup), staying a fixed distance
        // BELOW the player — so entering the trigger already means a genuine fall. There is NO
        // absolute-y gate: the old "y >= -20" check fought the downhill track (which descends to
        // ~-350) and killed the player mid-slope without any visible obstacle.
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            if (other.TryGetComponent<SkiingController>(out var controller))
            {
                controller.TriggerCrash();
            }
        }
    }
}
