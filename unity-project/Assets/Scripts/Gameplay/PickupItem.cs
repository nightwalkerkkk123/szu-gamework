using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// World pickup that grants an item effect to the player's inventory.
    /// </summary>
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private ItemEffect _itemEffect;
        [SerializeField] private bool _destroyOnPickup = true;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (_itemEffect == null) return;

            if (other.TryGetComponent<PlayerInventory>(out var inventory))
            {
                inventory.Equip(_itemEffect);
                Debug.Log($"[PickupItem] Picked up {_itemEffect.DisplayName}.", this);

                if (_destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
