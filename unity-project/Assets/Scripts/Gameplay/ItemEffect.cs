using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Base class for consumable item effects. Create concrete subclasses via Assets > Create > SugarRush > Items.
    /// </summary>
    public abstract class ItemEffect : ScriptableObject
    {
        [field: SerializeField] public string DisplayName { get; private set; } = "Item";
        [field: SerializeField, TextArea] public string Description { get; private set; } = "";
        [field: SerializeField] public Sprite Icon { get; private set; }

        /// <summary>
        /// Applies the effect. Called when the player uses the item.
        /// </summary>
        public abstract void Apply(GlucoseSystem glucoseSystem, SkiingController skiingController);
    }
}
