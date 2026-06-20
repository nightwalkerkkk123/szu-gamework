using SugarRush.Foundation;
using System;
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

        public static event Action<PickupItem, ItemEffect, GameObject> OnItemPickedUp;

        private bool _consumed;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_consumed) return; // guard against double-trigger (multiple player colliders / same-frame re-entry)
            if (!other.CompareTag("Player")) return;
            if (_itemEffect == null) return;

            if (other.TryGetComponent<GlucoseSystem>(out var glucoseSystem) &&
                other.TryGetComponent<SkiingController>(out var skiingController))
            {
                _consumed = true;
                _itemEffect.Apply(glucoseSystem, skiingController);
                OnItemPickedUp?.Invoke(this, _itemEffect, other.gameObject);
                Debug.Log($"[PickupItem] Picked up {_itemEffect.DisplayName}.", this);

                if (_destroyOnPickup)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
