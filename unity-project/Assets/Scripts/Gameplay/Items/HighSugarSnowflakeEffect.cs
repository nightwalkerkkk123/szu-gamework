using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay.Items
{
    /// <summary>
    /// 高糖雪花：立即 +25 血糖，并在 3 秒内提供临时速度加成（与 Glucose modifier 相乘）。
    /// </summary>
    [CreateAssetMenu(fileName = "HighSugarSnowflake", menuName = "SugarRush/Items/High Sugar Snowflake")]
    public class HighSugarSnowflakeEffect : ItemEffect
    {
        [SerializeField, Range(0f, 100f)] private float _instantDelta = 25f;
        [SerializeField, Min(0.1f)] private float _speedBoostDuration = 3f;
        [SerializeField] private float _speedBoostMultiplier = 1.15f;

        public override void Apply(GlucoseSystem glucoseSystem, SkiingController skiingController)
        {
            glucoseSystem?.ApplyDelta(_instantDelta);
            // Request a temporary speed bonus through the controller's interface.
            // It raises the speed cap + downhill accel for the duration — unlike
            // poking Rigidbody2D.velocity, which the speed limiter clamps away in a
            // single frame and which leaks into the vertical (jump) velocity.
            skiingController?.ApplySpeedMultiplierBonus(_speedBoostMultiplier, _speedBoostDuration);
            Debug.Log($"[Item] High Sugar Snowflake applied: +{_instantDelta:F1} glucose, speed boost x{_speedBoostMultiplier:F2} for {_speedBoostDuration:F1}s.", skiingController);
        }
    }
}
