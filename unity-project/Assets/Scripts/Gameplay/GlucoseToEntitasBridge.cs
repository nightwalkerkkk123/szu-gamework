using Entitas;
using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Maps our GlucoseSystem speed modifier to the reference Entitas runner's
    /// maximum velocity, so blood-sugar directly controls how fast the player runs.
    /// </summary>
    public class GlucoseToEntitasBridge : MonoBehaviour
    {
        [SerializeField] private GlucoseSystem _glucoseSystem;

        private Vector2 _baseMaxVelocity;
        private bool _resolved;

        private void Start()
        {
            if (_glucoseSystem == null)
                _glucoseSystem = FindObjectOfType<GlucoseSystem>();
        }

        private void Update()
        {
            if (_glucoseSystem == null) return;

            var state = Contexts.sharedInstance?.gameState;
            if (state == null || !state.hasCurrentMaxVelocity) return;

            if (!_resolved)
            {
                _baseMaxVelocity = state.currentMaxVelocity.Value;
                _resolved = true;
            }

            var modifier = Mathf.Max(0.1f, _glucoseSystem.SpeedModifier);
            state.ReplaceCurrentMaxVelocity(_baseMaxVelocity * modifier);
        }
    }
}
