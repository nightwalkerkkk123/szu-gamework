# Obstacle & Hazard System — 障碍与危险系统

> **Status**: Draft  
> **Author**: game-designer, level-designer  
> **Last Updated**: 2026-06-13  
> **Last Verified**: 2026-06-13  
> **Implements Pillar**: 爽感 (Feel) + 教育 (Education)  
> **Source**: `design/gdd/game-concept.md` §12, §14

## Summary

障碍与危险系统为 SugarRush 的滑行提供**决策压力**。它将雪山上的物体分为可跳跃、可翻滚、致命、环境陷阱等类型，并通过与 Skiing Controller 的碰撞事件决定玩家是安全通过、摔倒惩罚还是直接失败。所有障碍参数数据驱动，便于关卡设计师快速迭代。

> **Quick reference** — Layer: `Core` · Priority: `MVP` · Key deps: `Skiing Controller`, `Glucose System`

## Overview

关卡中出现的物体分为两类：

- **Obstacle（障碍）**：有实体碰撞，玩家必须通过跳跃/翻滚/路线选择来应对。
- **Hazard（危险/环境陷阱）**：不一定要有实体碰撞，通过区域伤害或强制效果影响玩家（如冰刺坑、暴风雪区、低血糖迷雾）。

系统核心职责：

1. 定义障碍分类与响应规则。
2. 处理碰撞检测，向 Skiing Controller 发送事件。
3. 根据玩家状态（是否翻滚、是否无敌）决定结果。
4. 触发视觉、音效、血糖等反馈。

## Player Fantasy

玩家应该把障碍看作**雪山上的「选择题」**：

- 看到石头 → 跳过去。
- 看到低矮的冰栅栏 → 滚过去。
- 看到悬崖/巨树 → 必须提前避开。
- 血糖异常时，同样的障碍变得更难——因为速度/操控变了，容错窗口也变了。

## Detailed Design

### Core Rules

1. **障碍只读玩家状态**：障碍查询 Skiing Controller 的 `IsRolling`、`IsInvincible`、`IsAirborne`，不直接读取输入或血糖。
2. **碰撞结果三档**：
   - **Safe**：玩家处于正确应对状态（跳跃中越过、翻滚中穿过），无障碍效果。
   - **Stumble**：玩家撞到可碰撞障碍但非致命，进入 0.6s 摔倒状态。
   - **Crash**：致命障碍或 Hazard 触发，关卡失败。
3. **数据驱动**：每个障碍实例引用一个 `ObstacleData` ScriptableObject，定义类型、高度、是否致命、视觉/音效等。
4. **Hazard 区域**：使用触发器（Trigger2D），持续或一次性影响玩家，可与血糖系统联动。
5. **美术隐喻**：障碍设计避免真实医疗图像；用雪山元素（冰、雪、岩石、糖果/甜品意象）表达风险。

### Obstacle Classification

| Type | Iconic Example | Player Response | Collision Result |
|------|---------------|-----------------|------------------|
| **Jumpable** | 雪石、小雪堆、横木 | 跳跃越过 | 跳跃中碰撞 → Safe；否则 Stumble |
| **Rollable** | 冰栅栏、低矮雪洞 | 翻滚穿过 | 翻滚中碰撞 → Safe；跳跃中撞到低高度 → Safe；否则 Stumble |
| **Avoidable** | 巨岩、冰裂缝、大树 | 必须转向避开 | 任何碰撞 → Crash |
| **Hazard Zone** | 冰刺坑、暴风雪带、低血糖迷雾 | 避开或快速通过 | 进入区域触发持续伤害/效果；达到阈值 → Crash |

### ObstacleData Schema

```csharp
[CreateAssetMenu(fileName = "ObstacleData", menuName = "SugarRush/ObstacleData")]
public class ObstacleData : ScriptableObject
{
    public string obstacleId;           // 唯一 ID
    public ObstacleType type;           // Jumpable / Rollable / Avoidable / Hazard
    public float height;                // 用于判定跳跃/翻滚阈值（单位：米）
    public bool isFatal;                // 是否直接 Crash
    public bool requiresRoll;           // 是否必须用翻滚通过
    public float stumbleDuration;       // Stumble 持续时间（默认 0.6s）
    public GameObject prefab;           // 关卡预制体
    public AudioClip hitSound;          // 撞击音效
    public ParticleSystem hitEffect;    // 撞击粒子
    // Hazard 专用
    public float hazardDamagePerSecond; // 每秒伤害/血糖影响（ Hazard 用）
    public float hazardThreshold;       // 进入区域多久或多少值后触发 Crash
}
```

### Collision Resolution Flow

```
OnTriggerEnter2D / OnCollisionEnter2D:
    if obstacle.isFatal:
        → Crash()
    else if player.IsRolling && obstacle.type == Rollable:
        → Safe (可穿越)
    else if player.IsAirborne && obstacle.height <= player.jumpClearance:
        → Safe (跳越)
    else:
        → Stumble(obstacle.stumbleDuration)
```

### Hazard Zone Rules

| Hazard Type | Effect | Exit Condition |
|-------------|--------|----------------|
| **Ice Spike Pit** | 进入即 Crash | 不可停留，必须跳过/绕过 |
| **Blizzard Band** | 每秒 `-2` 操控 modifier（视觉干扰） | 离开区域后 0.5s 恢复 |
| **Hypoglycemia Mist** | 每秒 `-5` 血糖 | 离开区域或血糖危机前脱离 |
| **Hyperglycemia Heat** | 每秒 `+5` 血糖 | 离开区域或血糖危机前脱离 |

> 注：血糖相关的 Hazard 最终通过 `GlucoseSystem.ApplyPassiveDelta()` 写入，Obstacle & Hazard System 不直接修改血糖。

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Safe | 正确应对障碍 | 离开碰撞体 | 无惩罚；可触发「帅气通过」小奖励（combo） |
| Stumbled | 非致命障碍碰撞 | stumbleDuration 结束 | 玩家无法控制；速度短暂归零；播放摔倒动画 |
| Crashed | 致命障碍 / Hazard 阈值 / 血糖危机 | 关卡失败 | 触发失败流程；紫色谢幕画面 |

### Interactions with Other Systems

| System | Interface | Data Flow |
|--------|-----------|-----------|
| **Skiing Controller** | 读取 `IsRolling`, `IsAirborne`, `Velocity`；发送 `Stumble()`, `Crash()` | 双向事件 |
| **Glucose System** | 对 Hazard Zone 调用 `ApplyPassiveDelta(float deltaPerSecond)` | Hazard → 血糖变化 |
| **Game Feel & Feedback** | 发送 `OnObstacleHit`, `OnHazardEntered/Exited` | 粒子/音效/屏幕震动 |
| **Audio System** | 订阅碰撞/区域事件 | 触发对应音效 |
| **Scoring & Rating** | 查询摔倒次数、帅气通过次数、进入 Hazard 次数 | 评分统计 |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| 翻滚结束时仍在障碍内 | 判定为 Stumble | 翻滚无敌只覆盖进入瞬间，不能卡在障碍里 |
| 跳跃最高点撞到障碍顶部 | 若高度 ≤ jumpClearance 则 Safe，否则 Stumble | 跳跃高度决定 clearance |
| 同时撞到 Jumpable 和 Rollable 判定区 | 按对玩家最有利的结果处理（Safe） | 避免惩罚过度 |
| 玩家在 Hazard Zone 内触发血糖危机 | 血糖系统负责失败判定；Hazard 只持续施加 delta | 单一权威源原则 |
| 障碍物被道具破坏（扩展） | MVP 不做；后续可作为「运动靴」等道具效果 | 控制范围 |
| 关卡加载时玩家出生在障碍内 | 强制触发 Stumble 或移动到安全点 | 防止 unfair death |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Skiing Controller | Mutual | 读取玩家状态；触发摔倒/失败 |
| Glucose System | Hazard depends on Glucose | 调用 `ApplyPassiveDelta` |
| Game Feel & Feedback | Obstacle → Feedback | 碰撞/区域事件 |
| Audio System | Obstacle → Audio | 碰撞/区域音效 |
| Scoring & Rating | Scoring depends on Obstacle | 读取统计 |
| Level Data & Progression | Obstacle placed by Level | 关卡配置引用 `ObstacleData` |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| `stumbleDuration` | 0.6 s | 0.4 ~ 1.2 | 惩罚更重 | 更宽容 |
| `jumpClearance` | 1.2 m | 0.8 ~ 2.0 | 更容易跳越 | 更难 |
| `rollHeightRatio` | 0.5 | 0.4 ~ 0.7 | 翻滚可穿更低障碍 | 翻滚优势降低 |
| `hazardTickRate` | 1.0 s | 0.2 ~ 2.0 | Hazard 效果更频繁 | 更稀疏 |
| `blizzardControlDrain` | -2 /s | -5 ~ 0 | 暴风雪中更难控 | 更宽容 |
| `mistGlucoseDrain` | -5 /s | -10 ~ -2 | 低血糖迷雾更危险 | 更安全 |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| 安全跳过障碍 | 障碍被越过时小粒子；糖糖自信表情 | 轻快「嗖」 | MVP |
| 安全翻滚穿过 | 障碍碎片/雪花飞溅 | 「嗖嗖」摩擦声 | MVP |
| 摔倒 | 糖糖翻滚倒地；星星转圈；0.6s 进度环 | 「哎哟」卡通音 | MVP |
| 致命碰撞 | 画面定格 → 紫色谢幕 | 重击 + 失败旋律 | MVP |
| 进入 Hazard Zone | 区域边缘高亮；屏幕滤镜变化 | 环境音效增强 | MVP |
| 持续 Hazard 伤害 | 每秒红色/蓝色脉冲 | 心跳/警报节奏 | MVP |

## Game Feel

### Feel Reference

- **Primary reference**: 《Ski Safari》—— 障碍节奏紧凑，翻滚和跳跃有明确分工。
- **Secondary reference**: 《Alto's Odyssey》—— 障碍与地形融合，不突兀。
- **Anti-reference**: 不要像《Flappy Bird》那样障碍物纯粹是节奏考验；障碍应该与血糖状态互动，让管理血糖成为路线选择的一部分。

### Hazard as Education

障碍和危险是**血糖机制的「物理化翻译」**：

- 低血糖迷雾 = 身体无力、反应变慢 → 玩家必须避开或快速吃糖。
- 高血糖暴风雪 = 情绪焦躁、视野变窄 → 玩家必须减速/降糖。
- 冰刺坑 = 直接风险 → 必须提前决策，不能靠反应硬闯。

### Feel Acceptance Criteria

- [ ] 玩家能在 3 次尝试内学会「跳高、滚低、避致命」。
- [ ] 摔倒时玩家觉得「我刚才按错了」而不是「游戏针对我」。
- [ ] 进入 Hazard Zone 后玩家会主动想「我得快点离开」。
- [ ] 高血糖状态下通过同一障碍，玩家能明显感觉到容错窗口变小。

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| 致命障碍预警（可选） | 障碍前小感叹号 | 进入屏幕时 | Avoidable / Hazard 前 |
| 摔倒恢复进度 | 糖糖头顶小进度环 | 每帧 | Stumbled 状态 |
| Hazard Zone 名称 | 屏幕边缘短暂提示 | 进入区域时 | 首次进入新区域 |
| 帅气通过 combo 计数 | 屏幕角落 | 连续安全通过时 | Vertical Slice |

## Cross-References

| This Document References | Target GDD | Specific Element Referenced | Nature |
|--------------------------|-----------|----------------------------|--------|
| 玩家翻滚/跳跃状态 | `design/gdd/skiing-controller.md` | `IsRolling`, `IsAirborne`, `Stumble()`, `Crash()` | Data dependency |
| Hazard 改变血糖 | `design/gdd/glucose-system.md` | `ApplyPassiveDelta` | Data dependency |
| 障碍放置与关卡节奏 | `design/gdd/level-data-progression.md` | 关卡片段配置 | Data dependency |
| 失败流程 | `design/gdd/game-flow.md` | Crash → Failure state | State trigger |
| 摔倒/通过统计 | `design/gdd/scoring-rating.md` | 动作/碰撞统计 | Data dependency |

## Acceptance Criteria

- [ ] 障碍分为 Jumpable / Rollable / Avoidable / Hazard 四类，数据驱动配置。
- [ ] 跳跃中越过 Jumpable 障碍不惩罚；翻滚中穿过 Rollable 障碍不惩罚。
- [ ] 撞到非致命障碍触发 0.6s 摔倒；撞到致命障碍或 Hazard 阈值触发失败。
- [ ] Hazard Zone 通过 `GlucoseSystem.ApplyPassiveDelta` 影响血糖，不直接写血糖。
- [ ] 摔倒/失败触发对应视觉、音效、动画反馈。
- [ ] 障碍参数可从 `ObstacleData` ScriptableObject 配置，无硬编码。
- [ ] 关卡加载时玩家不会出生在障碍内导致 unfair death。

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| 是否需要「帅气通过」combo 系统 MVP 就做？ | game-designer | W2 中 | 可先预留事件，数值/奖励 VS 阶段做 |
| L1 医院雪屋具体有哪些 Hazard Zone？ | level-designer | W2 中 | 待 Level Data GDD |
| Avoidable 障碍是否允许偶尔用无敌帧穿越？ | game-designer | W2 中 | 本 GDD 建议不允许，保持清晰规则 |
| 障碍是否需要动态生成（难度曲线）？ | level-designer | W2 末 | MVP 手作关卡，扩展可程序化 |
