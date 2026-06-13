using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay.Items
{
    /// <summary>
    /// 胰岛素喷雾：立即降低血糖 30 点。
    /// </summary>
    [CreateAssetMenu(fileName = "InsulinSpray", menuName = "SugarRush/Items/Insulin Spray")]
    public class InsulinSprayEffect : ItemEffect
    {
        [SerializeField, Range(-100f, 0f)] private float _instantDelta = -30f;

        public override void Apply(GlucoseSystem glucoseSystem, SkiingController skiingController)
        {
            glucoseSystem?.ApplyDelta(_instantDelta);
            Debug.Log($"[Item] Insulin Spray applied: {_instantDelta:F1} glucose.", skiingController);
        }
    }
}
