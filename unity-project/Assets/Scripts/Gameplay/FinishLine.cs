using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Trigger at the end of a level. Raises the finish event via GameEvents.
    /// </summary>
    public class FinishLine : MonoBehaviour
    {
        [SerializeField] private bool _destroyOnTrigger = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            Debug.Log("[FinishLine] Level finished.", this);
            SugarRush.Core.GameEvents.RaiseGameFinished(true);

            if (_destroyOnTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
