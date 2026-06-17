using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay.Items
{
    /// <summary>
    /// 防御护盾：使用后激活一次性免疫,下次 Stumble 或 Crash 触发前消耗。
    /// 玩家拾起 → equip 到单槽 inventory → 按 use 键激活。
    /// </summary>
    [CreateAssetMenu(fileName = "Shield", menuName = "SugarRush/Items/Shield")]
    public class ShieldEffect : ItemEffect
    {
        public override void Apply(GlucoseSystem glucoseSystem, SkiingController skiingController)
        {
            if (skiingController == null) return;
            skiingController.ActivateShield();
            Debug.Log("[Item] Shield activated — next Stumble/Crash is blocked.", skiingController);
        }
    }
}
