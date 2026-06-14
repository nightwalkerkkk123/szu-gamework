using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// 3D variant of the gap death zone for the fused endless-runner mode.
    /// </summary>
    public class GapDeathZone3D : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (other.TryGetComponent<SpherePlayerController>(out var controller))
            {
                controller.Die();
            }
        }
    }
}
