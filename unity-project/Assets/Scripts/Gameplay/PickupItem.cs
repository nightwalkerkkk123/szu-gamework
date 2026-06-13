using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// World pickup that applies an item effect immediately to the player.
    /// </summary>
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private ItemEffect _itemEffect;
        [SerializeField] private bool _destroyOnPickup = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (_itemEffect == null) return;

            if (other.TryGetComponent<GlucoseSystem>(out var glucoseSystem) &&
                other.TryGetComponent<SkiingController>(out var skiingController))
            {
                _itemEffect.Apply(glucoseSystem, skiingController);
                Debug.Log($"[PickupItem] Picked up {_itemEffect.DisplayName}.", this);

                if (_destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
