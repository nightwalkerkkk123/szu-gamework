using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay.Items
{
    /// <summary>
    /// 降糖药瓶：5 秒内每秒降低 8 点血糖。
    /// </summary>
    [CreateAssetMenu(fileName = "HypoglycemicPills", menuName = "SugarRush/Items/Hypoglycemic Pills")]
    public class HypoglycemicPillsEffect : ItemEffect
    {
        [SerializeField, Range(-100f, 0f)] private float _deltaPerSecond = -8f;
        [SerializeField, Min(0.1f)] private float _duration = 5f;

        public override void Apply(GlucoseSystem glucoseSystem, SkiingController skiingController)
        {
            glucoseSystem?.ApplyBuffOverTime(_deltaPerSecond, _duration);
            Debug.Log($"[Item] Hypoglycemic Pills applied: {_deltaPerSecond:F1}/s for {_duration:F1}s.", skiingController);
        }
    }
}
