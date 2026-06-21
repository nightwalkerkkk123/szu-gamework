using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// 3D world pickup for the fused endless-runner mode.
    /// The 2D <see cref="PickupItem"/> uses OnTriggerEnter2D and requires a SkiingController,
    /// so it never fires for the 3D sphere player. This variant uses a 3D trigger and applies
    /// the effect with a null SkiingController — glucose deltas still apply; skiing-only bonuses
    /// (e.g. speed boost) simply no-op for the sphere.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PickupItem3D : MonoBehaviour
    {
        [SerializeField] private ItemEffect _itemEffect;
        [SerializeField] private bool _destroyOnPickup = true;
        [Tooltip("Idle spin for readability (degrees/second). 0 = static.")]
        [SerializeField] private float _spinDegreesPerSecond = 90f;

        private bool _consumed;

        private void Update()
        {
            if (_spinDegreesPerSecond != 0f)
            {
                transform.Rotate(Vector3.up, _spinDegreesPerSecond * Time.deltaTime, Space.World);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_consumed || _itemEffect == null || !other.CompareTag("Player")) return;

            var glucose = other.GetComponentInParent<GlucoseSystem>();
            if (glucose == null) glucose = FindObjectOfType<GlucoseSystem>();
            if (glucose == null) return;

            _consumed = true;

            if (_itemEffect.PickupSfx != null)
            {
                AudioSource.PlayClipAtPoint(_itemEffect.PickupSfx, transform.position);
            }

            _itemEffect.Apply(glucose, null);
            Debug.Log($"[PickupItem3D] Picked up {_itemEffect.DisplayName}.", this);

            if (_destroyOnPickup) Destroy(gameObject);
        }
    }
}
