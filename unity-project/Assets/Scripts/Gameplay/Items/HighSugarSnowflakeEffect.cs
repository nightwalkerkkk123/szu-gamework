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
            skiingController?.StartCoroutine(SpeedBoostRoutine(skiingController));
            Debug.Log($"[Item] High Sugar Snowflake applied: +{_instantDelta:F1} glucose, speed boost x{_speedBoostMultiplier:F2}.", skiingController);
        }

        private System.Collections.IEnumerator SpeedBoostRoutine(SkiingController skiingController)
        {
            // Placeholder: direct velocity manipulation. Replace with a proper buff system later.
            var rb = skiingController.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity *= _speedBoostMultiplier;
            }

            yield return new WaitForSeconds(_speedBoostDuration);

            if (rb != null)
            {
                rb.velocity /= _speedBoostMultiplier;
            }
        }
    }
}
