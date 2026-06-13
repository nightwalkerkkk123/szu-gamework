using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Environmental hazard that applies a continuous glucose delta while the player stays inside.
    /// </summary>
    public class HazardZone : MonoBehaviour
    {
        [SerializeField] private GlucoseSystem _glucoseSystem;
        [SerializeField] private float _deltaPerSecond = -3f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (_glucoseSystem != null)
            {
                _glucoseSystem.SetPassiveDelta(_deltaPerSecond);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            if (_glucoseSystem != null)
            {
                _glucoseSystem.ResetPassiveDelta();
            }
        }
    }
}
