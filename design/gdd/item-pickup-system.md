# Item & Pickup System — 道具与拾取系统

> **Status**: Draft  
> **Author**: game-designer, systems-designer  
> **Last Updated**: 2026-06-13  
> **Last Verified**: 2026-06-13  
> **Implements Pillar**: 教育 (Education) + 爽感 (Feel)  
> **Source**: `design/gdd/game-concept.md` §11, §15

## Summary

道具与拾取系统是 SugarRush 中玩家主动管理血糖的核心手段。MVP 包含 3 个道具：胰岛素喷雾（快速降糖急救）、降糖药瓶（持续降糖）、高糖雪花（短期加速但升血糖的双刃剑）。所有道具通过 `GlucoseSystem` 的标准 API 生效，系统本身不直接写血糖；拾取与使用均由数据驱动配置。

> **Quick reference** — Layer: `Feature` · Priority: `MVP` · Key deps: `Glucose System`, `Input System`, `Skiing Controller`

## Overview

玩家在关卡中会遇到漂浮或地面上的道具，靠近即自动拾取。拾取后道具进入道具栏，玩家通过 Input System 映射的 `UseItem1/2/3` 使用。

系统核心职责：

1. 定义 MVP 3 道具的效果、持续时长、冷却/堆叠规则。
2. 处理拾取触发、道具栏管理、使用触发。
3. 通过 `GlucoseSystem.ApplyGlucoseDelta` / `ApplyBuffOverTime` 应用效果。
4. 提供视觉、音效、HUD 反馈。

## Player Fantasy

玩家应该把道具看作**雪山上的急救包和兴奋剂**：

- 血糖飙红时，按一下胰岛素喷雾 → 画面从红转绿，松一口气。
- 血糖慢慢偏高时，嗑一瓶降糖药 → 稳妥下降，不用慌。
- 眼看要赶不上节奏，吃一片高糖雪花 → 眼前一爽，但血糖条开始往上爬，必须马上找下一个健康区或胰岛素。

## Detailed Design

### Core Rules

1. **自动拾取**：玩家碰撞到道具触发器即拾取，无需额外按键。
2. **即时使用道具**：拾取后进入道具栏，按对应按键立即使用。
3. **被动拾取道具（可选扩展）**：部分道具拾取即生效，不进入道具栏。MVP 只有主动使用道具。
4. **效果通过 Glucose API 应用**：Item System 只能调用 `ApplyGlucoseDelta` / `ApplyBuffOverTime`，不能直接写血糖。
5. **同名效果不叠加数值，刷新持续时间**：防止指数爆炸。
6. **道具栏容量**：MVP 为 3 个槽位，每个槽位固定绑定一种道具类型。
7. **道具映射到输入**（来自 Input System）：
   - `UseItem1` → 槽位 1（默认胰岛素喷雾）
   - `UseItem2` → 槽位 2（默认降糖药瓶）
   - `UseItem3` → 槽位 3（默认高糖雪花）
8. **视觉语言一致**：注射器=胰岛素，药瓶=口服药，红色雪花=高糖食物。

### MVP Item Table

| Item | Type | Effect | Duration | Cooldown | Stack/Refresh |
|------|------|--------|----------|----------|---------------|
| **Insulin Spray** 胰岛素喷雾 | Active | 立即 `-30` 血糖 | Instant | 3.0 s | 不叠加，使用后进入冷却 |
| **Hypoglycemic Pill** 降糖药瓶 | Active / Buff | 每秒 `-8` 血糖，持续 5s | 5.0 s | 8.0 s | 刷新持续时间，不叠加每秒数值 |
| **Sugar Snowflake** 高糖雪花 | Active / Buff | 立即 `+25` 血糖；同时 3s 内 `SpeedModifier +0.15` | 3.0 s | 5.0 s | 刷新持续时间，不叠加速度加成 |

> 注：以上数值为起点，需通过原型验证和内部试玩调整。

### ItemData Schema

```csharp
[CreateAssetMenu(fileName = "ItemData", menuName = "SugarRush/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemId;
    public string displayName;
    public Sprite icon;
    public GameObject pickupPrefab;
    public GameObject useEffectPrefab;
    public AudioClip useSound;
    
    [Header("Glucose")]
    public float instantDelta;          // 立即生效的血糖变化（可正可负）
    public float deltaPerSecond;        // 持续每秒血糖变化（可正可负）
    public float duration;              // 持续效果时长
    
    [Header("Modifiers")]
    public bool overridesSpeedModifier; // 是否覆盖速度修饰
    public float speedModifierBonus;    // 额外速度加成（加法）
    public bool overridesControlModifier;
    public float controlModifierBonus;
    
    [Header("Cooldown")]
    public float cooldown;              // 使用冷却
    
    [Header("Usage")]
    public bool useOnPickup;            // 拾取即使用（MVP 为 false）
    public int maxCharges;              // 最大堆叠次数（MVP 均为 1）
}
```

### Usage Flow

```
On UseItemN Input:
    slot = inventory.GetSlot(N)
    if slot.IsEmpty or slot.IsOnCooldown:
        → ignore / play error feedback
    else:
        item = slot.item
        slot.StartCooldown(item.cooldown)
        GlucoseSystem.ApplyGlucoseDelta(item.instantDelta, Source.Item)
        if item.deltaPerSecond != 0:
            GlucoseSystem.ApplyBuffOverTime(item.deltaPerSecond, item.duration, Source.Item)
        if item.speedModifierBonus != 0:
            SkiingController.ApplyTemporarySpeedBonus(item.speedModifierBonus, item.duration)
        Spawn(item.useEffectPrefab)
        Play(item.useSound)
        EventBus.Publish(ItemUsedEvent)
```

### Pickup Flow

```
OnTriggerEnter2D (ItemPickup):
    if inventory.CanAccept(item):
        inventory.Add(item)
        Spawn(pickupEffectPrefab)
        Play(pickupSound)
        Destroy(item.gameObject)
    else:
        → 槽位已满，道具保持原地或弹提示
```

### Sugar Snowflake — 双刃剑设计

| Aspect | Design |
|--------|--------|
| 立即效果 | 血糖 `+25`，进入 Above Normal 或 High |
| 速度加成 | 3s 内 `SpeedModifier +0.15`，叠加到 Glucose 的 modifier 上 |
| 风险 | 高血糖时再吃可能触发 crisis；需要配合胰岛素或降糖药 |
| 教学点 | 对应「高糖食物短期爽、长期风险」 |
| 使用场景 | 需要冲刺越过长障碍带；或低血糖急救（但会过头） |

### Inventory Slots

```csharp
public class InventorySlot
{
    public ItemData Item { get; private set; }
    public int Charges { get; private set; }
    public float CooldownRemaining { get; private set; }
    public bool IsOnCooldown => CooldownRemaining > 0;
    public bool IsEmpty => Item == null;
}
```

MVP 规则：
- 每个槽位绑定一种道具类型，不可替换为其他类型。
- 拾取同类型道具时，`Charges` 增加（上限 3）。
- 使用消耗 1 个 Charge，同时进入冷却。
- 冷却期间不可使用该槽位，但可继续拾取堆叠 Charges。

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Available | 道具在关卡中未拾取 | 玩家触发拾取 | 漂浮/旋转待机动画；轻微发光 |
| In Inventory | 拾取成功 | 使用或关卡结束 | 显示在 HUD 道具栏；显示可用数量 |
| On Cooldown | 使用道具后 | 冷却结束 | HUD 显示冷却环；输入被忽略 |
| Effect Active | 使用持续型道具 | Buff 结束 | 血糖持续变化；视觉/音效反馈 |

### Interactions with Other Systems

| System | Interface | Data Flow |
|--------|-----------|-----------|
| **Glucose System** | 调用 `ApplyGlucoseDelta` / `ApplyBuffOverTime` | Item → 血糖变化 |
| **Input System** | 订阅 `UseItem1/2/3` | Input → 使用道具 |
| **Skiing Controller** | 调用 `ApplyTemporarySpeedBonus` | 高糖雪花 → 临时速度加成 |
| **HUD** | 发送 `OnItemPickedUp`, `OnItemUsed`, `OnCooldownUpdated` | 道具栏/冷却显示 |
| **Game Feel & Feedback** | 发送道具使用/拾取事件 | 粒子/音效/屏幕光效 |
| **Audio System** | 订阅道具事件 | 播放对应音效 |
| **Scoring & Rating** | 查询道具使用次数、高糖雪花使用次数 | 评分统计 |

**所有权规则**：Item System 只能通过 Glucose API 写血糖；不能反向读取血糖来决定是否允许使用道具（避免循环依赖）。是否使用由玩家决策，HUD 仅作提示。

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| 低血糖危机时使用高糖雪花 | 血糖立即回升，可脱离 crisis | 双刃剑也能救命 |
| 高血糖危机时使用胰岛素 | 立即大幅下降，脱离 crisis | 急救设计意图 |
| 连续使用胰岛素喷雾 | 受 3s 冷却限制，不能连喷 | 防止瞬间回满 |
| 道具栏已满时拾取同类型道具 | Charges 增加（上限 3） | 鼓励收集 |
| 道具栏已满时拾取不同类型道具 | 忽略或替换当前槽位（MVP 忽略） | 槽位固定类型 |
| 暂停时使用道具 | 忽略 | 暂停时不处理 gameplay 输入 |
| 关卡结束时 Buff 仍在持续 | 停止更新，结算时读取最终状态 | 公平性 |
| 胰岛素把血糖从 95 降到 65 | 立即退出 High 状态，modifier 平滑过渡 | 给玩家清晰正反馈 |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Glucose System | Item depends on Glucose | 调用血糖 API |
| Input System | Item depends on Input | 使用道具输入 |
| Skiing Controller | Item depends on Skiing | 临时速度加成 |
| HUD | Item → HUD | 道具栏/冷却显示 |
| Game Feel & Feedback | Item → Feedback | 粒子/音效 |
| Audio System | Item → Audio | 道具音效 |
| Scoring & Rating | Scoring depends on Item | 使用统计 |
| Level Data & Progression | Items placed by Level | 关卡配置 |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| `insulinInstantDelta` | -30 | -50 ~ -15 | 急救更强 | 更弱 |
| `insulinCooldown` | 3.0 s | 1.5 ~ 6.0 | 使用更稀疏 | 更频繁 |
| `pillDeltaPerSecond` | -8 /s | -15 ~ -3 | 降糖更快 | 更慢 |
| `pillDuration` | 5.0 s | 3 ~ 10 | 持续更久 | 更短 |
| `pillCooldown` | 8.0 s | 5 ~ 15 | 使用更稀疏 | 更频繁 |
| `snowflakeInstantDelta` | +25 | +15 ~ +40 | 升糖更猛 | 更温和 |
| `snowflakeSpeedBonus` | +0.15 | 0.05 ~ 0.30 | 加速更明显 | 更弱 |
| `snowflakeDuration` | 3.0 s | 2 ~ 6 | 加速窗口更长 | 更短 |
| `snowflakeCooldown` | 5.0 s | 3 ~ 10 | 使用更稀疏 | 更频繁 |
| `maxChargesPerSlot` | 3 | 1 ~ 5 | 更宽容 | 更紧张 |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| 拾取胰岛素 | 蓝色光球吸入道具栏 | 「叮」 | MVP |
| 拾取降糖药 | 绿色光球吸入道具栏 | 「叮」 | MVP |
| 拾取高糖雪花 | 红色糖晶爆裂吸入 | 「咻」 | MVP |
| 使用胰岛素 | 蓝色喷雾环扩散；血糖条快速下降 | 「嘶——」 | MVP |
| 使用降糖药 | 绿色药水涟漪；持续发光 | 「咕嘟」 | MVP |
| 使用高糖雪花 | 红色糖晶环绕；身后拖尾变长 | 「咻——加速音」 | MVP |
| 道具冷却中 | HUD 槽位灰色 + 旋转冷却环 | 低沉「叮」（不可用） | MVP |
| 道具栏已满 | 道具原地弹跳提示 | 无 | MVP |

## Game Feel

### Feel Reference

- **Primary reference**: 《Ski Safari》—— 道具拾取瞬间有明确正反馈，使用时机影响节奏。
- **Secondary reference**: 《Hades》—— 道具效果清晰、冷却可见、堆叠直观。
- **Anti-reference**: 不要像 RPG 那样打开背包选道具； SugarRush 是跑酷，必须一键即用。

### Input Responsiveness

| Action | Max Input-to-Effect Latency | Frame Budget (60fps) |
|--------|----------------------------|---------------------|
| 使用道具 | ≤ 33 ms | ≤ 2 frames |
| 拾取道具反馈 | ≤ 50 ms | ≤ 3 frames |

### Weight and Responsiveness Profile

- **胰岛素**：瞬间反馈强，像「救命按钮」；冷却防止滥用。
- **降糖药**：平稳、可预期，适合预防性使用。
- **高糖雪花**：爽感峰值高，但之后有紧张感；鼓励玩家做风险决策。

### Feel Acceptance Criteria

- [ ] 玩家能在 1 次失误后理解「高血糖 → 胰岛素」。
- [ ] 玩家会主动在高血糖前使用降糖药，而不是等 crisis。
- [ ] 玩家吃完高糖雪花后会说「我现在得快点多用胰岛素/找健康区」。
- [ ] 道具冷却可见，玩家能规划使用节奏。

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| 道具图标 | 屏幕右下角 3 槽位 | 拾取/使用时 | 始终可见 |
| 道具数量/Charge | 图标右下角小数字 | 拾取/使用时 | 有 Charge 时 |
| 冷却环 | 图标上方覆盖 | 每帧 | 冷却期间 |
| 当前道具效果 Buff 图标 | 血糖条旁 | 每帧 | Buff 持续期间 |
| 拾取提示 | 糖糖头顶或道具栏 | 拾取瞬间 | 拾取时 |

## Cross-References

| This Document References | Target GDD | Specific Element Referenced | Nature |
|--------------------------|-----------|----------------------------|--------|
| 使用道具输入 | `design/gdd/input-system.md` | `UseItem1/2/3` | Data dependency |
| 应用血糖变化 | `design/gdd/glucose-system.md` | `ApplyGlucoseDelta`, `ApplyBuffOverTime` | Data dependency |
| 临时速度加成 | `design/gdd/skiing-controller.md` | `ApplyTemporarySpeedBonus` | Data dependency |
| 道具在关卡中放置 | `design/gdd/level-data-progression.md` | 关卡道具配置 | Data dependency |
| 道具栏显示 | `design/gdd/hud.md` | 道具槽位 UI | Data dependency |

## Acceptance Criteria

- [ ] 3 个 MVP 道具均有明确效果：胰岛素（快速降糖）、降糖药（持续降糖）、高糖雪花（加速+升糖）。
- [ ] 道具通过 `GlucoseSystem.ApplyGlucoseDelta` / `ApplyBuffOverTime` 生效，不直接写血糖。
- [ ] 玩家碰撞道具自动拾取；按对应按键使用。
- [ ] 同名效果刷新持续时间，不叠加数值。
- [ ] 道具使用有冷却，冷却期间无法使用。
- [ ] HUD 显示道具图标、数量、冷却状态。
- [ ] 所有道具参数可从 `ItemData` ScriptableObject 配置，无硬编码。
- [ ] 暂停时道具输入被忽略。

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| 道具数值是否需要在原型阶段大幅调整？ | game-designer | W2 末 | 依赖内部试玩反馈 |
| 是否允许「胰岛素 + 降糖药」同时使用？ | game-designer | W2 中 | 当前 GDD 允许，但叠加效果需谨慎 |
| 高糖雪花的速度加成是否叠加到 Glucose modifier 还是独立？ | systems-designer | W2 中 | 本 GDD 建议独立临时 bonus，最终与 Skiing Controller 协商 |
| 道具栏是否支持键盘/手柄快速切换焦点？ | ux-designer | W2 中 | 当前固定 3 槽位，无需切换 |
