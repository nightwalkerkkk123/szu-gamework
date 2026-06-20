using SugarRush.Foundation;
using System.Reflection;
using UnityEngine;

namespace SugarRush.Gameplay.Items
{
    /// <summary>
    /// 加速道具：短时间内提升滑行速度。
    /// </summary>
    [CreateAssetMenu(fileName = "SpeedBoost", menuName = "SugarRush/Items/Speed Boost")]
    public class SpeedBoostEffect : ItemEffect
    {
        [SerializeField, Range(0.1f, 5f)] private float _speedMultiplier = 1.8f;
        [SerializeField, Range(1f, 15f)] private float _durationSeconds = 5f;

        private static readonly FieldInfo s_timerField = typeof(SkiingController)
            .GetField("_speedBonusTimer", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo s_multiplierField = typeof(SkiingController)
            .GetField("_speedBonusMultiplier", BindingFlags.NonPublic | BindingFlags.Instance);

        public override void Apply(GlucoseSystem glucoseSystem, SkiingController skiingController)
        {
            if (skiingController == null) return;
            s_timerField?.SetValue(skiingController, _durationSeconds);
            s_multiplierField?.SetValue(skiingController, _speedMultiplier);
            Debug.Log($"[Item] Speed Boost applied: {_speedMultiplier}x for {_durationSeconds}s.");
        }
    }
}
