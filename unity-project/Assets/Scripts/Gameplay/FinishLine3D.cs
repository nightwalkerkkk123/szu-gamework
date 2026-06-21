using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// 3D variant of the finish line for the fused endless-runner mode.
    /// The 2D <see cref="FinishLine"/> uses OnTriggerEnter2D and never fires for the
    /// 3D sphere player, so the fused track needs this 3D trigger to raise the win event.
    /// Raises the win event exactly once when the player enters the trigger.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class FinishLine3D : MonoBehaviour
    {
        [SerializeField] private bool _destroyOnTrigger = false;

        private bool _triggered;

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered || !other.CompareTag("Player")) return;
            _triggered = true;

            Debug.Log("[FinishLine3D] Level finished — victory.", this);
            SugarRush.Core.GameEvents.RaiseGameFinished(true);

            if (_destroyOnTrigger) Destroy(gameObject);
        }
    }
}
