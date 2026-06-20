using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay.Items
{
    /// <summary>
    /// 糖果血包：立即恢复血糖值，相当于补充血量。
    /// </summary>
    [CreateAssetMenu(fileName = "Candy", menuName = "SugarRush/Items/Candy Health Pack")]
    public class CandyEffect : ItemEffect
    {
        [SerializeField, Range(0f, 100f)] private float _instantDelta = 15f;

        public override void Apply(GlucoseSystem glucoseSystem, SkiingController skiingController)
        {
            glucoseSystem?.ApplyDelta(_instantDelta);
            Debug.Log($"[Item] Candy Health Pack applied: +{_instantDelta:F1} glucose.", skiingController);
        }
    }
}