using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay.Items
{
    /// <summary>
    /// 磁铁:即时生效。在玩家身上挂载 <see cref="MagnetField"/> 组件,
    /// 在持续时间内将范围内所有 PickupItem 拉向玩家。
    /// </summary>
    [CreateAssetMenu(fileName = "Magnet", menuName = "SugarRush/Items/Magnet")]
    public class MagnetEffect : ItemEffect
    {
        [SerializeField, Min(1f)] private float _radius = 5f;
        [SerializeField, Min(0.5f)] private float _duration = 3f;
        [SerializeField, Min(1f)] private float _pullSpeed = 12f;

        public override void Apply(GlucoseSystem glucoseSystem, SkiingController skiingController)
        {
            if (skiingController == null) return;

            var go = skiingController.gameObject;
            var existing = go.GetComponent<MagnetField>();
            if (existing == null)
            {
                existing = go.AddComponent<MagnetField>();
            }

            existing.Configure(_radius, _duration, _pullSpeed);
            Debug.Log($"[Item] Magnet active for {_duration:F1}s, radius {_radius:F1}m, pullSpeed {_pullSpeed:F1}.", go);
        }
    }
}
