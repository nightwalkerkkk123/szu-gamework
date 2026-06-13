using System;
using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Single-slot consumable inventory. Equipping a new item replaces the current one.
    /// </summary>
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private GlucoseSystem _glucoseSystem;
        [SerializeField] private SkiingController _skiingController;
        [SerializeField] private SugarRushInput _input;

        public ItemEffect EquippedItem { get; private set; }

        public event Action<ItemEffect> OnItemEquipped;
        public event Action OnItemUsed;

        private void Awake()
        {
            if (_glucoseSystem == null) _glucoseSystem = GetComponent<GlucoseSystem>();
            if (_skiingController == null) _skiingController = GetComponent<SkiingController>();

            if (_input != null)
            {
                _input.OnUseItemPressed += UseEquippedItem;
            }
        }

        private void OnDestroy()
        {
            if (_input != null)
            {
                _input.OnUseItemPressed -= UseEquippedItem;
            }
        }

        public void Equip(ItemEffect item)
        {
            EquippedItem = item;
            OnItemEquipped?.Invoke(EquippedItem);
        }

        public void UseEquippedItem()
        {
            if (EquippedItem == null) return;

            EquippedItem.Apply(_glucoseSystem, _skiingController);
            OnItemUsed?.Invoke();
            EquippedItem = null;
            OnItemEquipped?.Invoke(null);
        }

        public void Clear()
        {
            EquippedItem = null;
            OnItemEquipped?.Invoke(null);
        }
    }
}
