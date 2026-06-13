# Glucose System — 血糖系统

> **Status**: Draft  
> **Author**: game-designer, systems-designer  
> **Last Updated**: 2026-06-13  
> **Last Verified**: 2026-06-13  
> **Implements Pillar**: 教育 (Education) + 爽感 (Feel)  
> **Source**: `design/gdd/game-concept.md` §7, §11, §15

## Summary

血糖系统是 SugarRush 的核心生存变量。它把「血糖水平」转译为一条始终可见的状态条，玩家的所有操作（吃高糖雪花、使用胰岛素/降糖药、穿越不同区域）都会实时改变它。系统向其他模块输出速度、操控、视野三类修饰符，让血糖从「UI 数字」变成玩家能直接感受到的物理状态。

> **Quick reference** — Layer: `Foundation` · Priority: `MVP` · Key deps: `None` (bottleneck system，6+ 系统依赖它)

## Overview

玩家在滑雪过程中，血糖值会在 0–100 的抽象区间内波动。区间被划分为 5 档：低、偏低、正常、偏高、高。系统每秒根据时间、区域、道具效果更新血糖值，并据此发布 `SpeedModifier`、`ControlModifier`、`VisionModifier` 给其他系统。血糖进入「偏高/高」或「偏低/低」时，通过颜色、粒子、音效、屏幕效果多通道提醒玩家，但始终使用卡通漫画语言，避免真实医疗恐慌感。

## Player Fantasy

玩家应该把血糖条当成**自己在雪山上生存的能量温度计**：

- 正常时：滑行顺畅、画面明亮、心情轻快。
- 偏高时：身体像「喝了太多能量饮料」—— 速度变快但有点失控，屏幕边缘泛红、心跳加速。
- 高时：红色狂暴风雪压顶，视野变窄，必须立刻找胰岛素喷雾或降糖药。
- 偏低时：像「低血糖后的无力感」—— 动作变沉、屏幕发蓝发黏，必须吃块高糖雪花或冲进健康区。

**绝不给人的感觉**：被说教、在看医疗仪器、因为血糖数字失败。

## Detailed Design

### Core Rules

1. **血糖值范围**：抽象值 0–100，不在 UI 显示具体 mmol/L。
2. **五档状态**：
   - **低 (Low)**：0–15
   - **偏低 (Below Normal)**：16–35
   - **正常 (Normal)**：36–65
   - **偏高 (Above Normal)**：66–85
   - **高 (High)**：86–100
3. **状态条显示**：HUD 弧形弹药计，从深蓝（低）→ 翠绿（正常）→ 红紫（高）渐变。
4. **被动漂移**：
   - 在正常区间（36–65）：每秒 `-0.5` 点（轻微向低偏移，鼓励玩家主动管理）。
   - 在偏低区间（16–35）：每秒 `-0.3` 点（减速下坠，给玩家反应时间）。
   - 在低区间（0–15）：每秒 `-0.2` 点（危机但可逆）。
   - 在偏高区间（66–85）：每秒 `+0.8` 点（较快向高恶化）。
   - 在高区间（86–100）：每秒 `+1.2` 点（快速恶化，必须干预）。
5. **状态转换滞后**：进入新区间后，至少保持 0.5 秒才允许再次切换状态，防止边界抖动。
6. ** clamp**：所有计算结果 clamp 到 [0, 100]。
7. **危机判定**：
   - **低血糖危机**：血糖 ≤ 10，持续 3 秒以上 → 失败。
   - **高血糖危机**：血糖 ≥ 95，持续 3 秒以上 → 失败。
8. **失败不是死亡**：画面变成卡通谢幕（紫色背景、糖糖挥手、「血糖大冒险结束啦！」），强调再来一局。

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Low | value ≤ 15 | value > 15 | ControlModifier = 0.65；SpeedModifier = 0.75；蓝色脉冲边框；轻微操控阻尼 |
| Below Normal | 16 ≤ value ≤ 35 | value < 16 或 value > 35 | ControlModifier = 0.85；SpeedModifier = 0.90；蓝色警示粒子密度低 |
| Normal | 36 ≤ value ≤ 65 | value < 36 或 value > 65 | ControlModifier = 1.0；SpeedModifier = 1.0；无负面修饰；UI 绿色稳定填充 |
| Above Normal | 66 ≤ value ≤ 85 | value < 66 或 value > 85 | ControlModifier = 0.90；SpeedModifier = 1.15；红色警示粒子；屏幕轻微红晕 |
| High | value ≥ 86 | value < 86 | ControlModifier = 0.75；SpeedModifier = 1.35；红色暴风雪粒子；强 vignette；画面轻微抖动 |
| Crisis (Low) | value ≤ 10 持续 3s | 干预后 value > 10 | 漫画分镜框 + ❗❗❗ + 蓝色速度线；3 秒内不脱离则失败 |
| Crisis (High) | value ≥ 95 持续 3s | 干预后 value < 95 | 漫画分镜框 + ❗❗❗ + 红色速度线；3 秒内不脱离则失败 |

### Interactions with Other Systems

| System | Interface | Data Flow |
|--------|-----------|-----------|
| **Skiing Controller** | 只读修饰符 | Glucose System → `SpeedModifier`, `ControlModifier` |
| **Camera Follow** | 只读修饰符 | Glucose System → `VisionModifier`（影响视野/抖动） |
| **Item & Pickup System** | 写入 API | Item System → `ApplyGlucoseDelta(float delta, Source source)` |
| **Environmental Zones** | 写入 API | Zone System → `ApplyPassiveDelta(float deltaPerSecond)` |
| **HUD** | 订阅事件 | Glucose System → `OnGlucoseChanged`, `OnStateEntered/Exited` |
| **Audio System** | 订阅事件 | Glucose System → `OnStateEntered` 触发心跳/警报音 |
| **Game Feel & Feedback** | 订阅事件 | Glucose System → `OnStateEntered` 触发粒子/抖动/漫画框 |
| **Game Flow** | 订阅事件 | Glucose System → `OnCrisisFailed(bool isLow)` 触发关卡失败 |
| **Scoring & Rating** | 查询接口 | Scoring 读取历史最高/最低血糖、进入危机次数 |

**所有权规则**：Glucose System 是血糖值的唯一权威源。任何系统想改变血糖，必须通过 `ApplyGlucoseDelta` 或 `ApplyBuffOverTime`，不能直接写 `_currentGlucose`。

## Formulas

### Current Glucose Update

```
newValue = Clamp(
    currentValue
    + passiveDrift * deltaTime
    + zoneDelta * deltaTime
    + sum(activeBuffs.deltaPerSecond) * deltaTime,
    0, 100
)
```

| Variable | Type | Range | Source | Description |
|----------|------|-------|--------|-------------|
| `currentValue` | float | 0–100 | runtime | 上一帧血糖值 |
| `passiveDrift` | float | -0.5 ~ +1.2 /s | data table | 由当前状态决定 |
| `zoneDelta` | float | -5 ~ +8 /s | Environmental Zones | 区域被动效果 |
| `activeBuffs` | List<Buff> | — | Item System | 持续型道具效果 |
| `deltaTime` | float | — | Unity Time | 帧间隔 |

### Modifier Outputs

```
SpeedModifier  = Lerp(1.0, targetSpeedModifier,  transitionCurve)
ControlModifier = Lerp(1.0, targetControlModifier, transitionCurve)
VisionModifier = Lerp(1.0, targetVisionModifier, transitionCurve)
```

| Modifier | Low | Below Normal | Normal | Above Normal | High |
|----------|-----|--------------|--------|--------------|------|
| `targetSpeedModifier` | 0.75 | 0.90 | 1.00 | 1.15 | 1.35 |
| `targetControlModifier` | 0.65 | 0.85 | 1.00 | 0.90 | 0.75 |
| `targetVisionModifier` | 0.85 | 0.95 | 1.00 | 0.95 | 0.80 |

`transitionCurve`：状态切换时使用 0.3 秒平滑插值（ease-in-out），避免突变。

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| 道具把血糖从 8 瞬间推到 60 | 立即退出低血糖危机；恢复 normal 修饰符 | 干预有效，给玩家正反馈 |
| 多个道具同时生效 | Buff 效果叠加；同名道具刷新持续时间不叠加数值 | 避免指数爆炸 |
| 玩家在高血糖区使用高糖雪花 | 两者叠加可能瞬间进入 crisis | 双刃剑设计意图 |
| 暂停游戏 | 血糖系统暂停更新；deltaTime = 0 | 公平性 |
| 关卡结束/胜利 | 血糖系统停止更新；结算时读取历史状态 | 评分需要 |
| 血糖正好在边界 15/16、65/66、85/86 | 状态转换滞后 0.5s；最终按更新后的值判定 | 防止边界抖动 |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Item & Pickup System | Depends on Glucose System | 调用 `ApplyGlucoseDelta` / `ApplyBuffOverTime` |
| Environmental Zones | Depends on Glucose System | 调用 `ApplyPassiveDelta` |
| Skiing Controller | Depends on Glucose System | 读取 `SpeedModifier`, `ControlModifier` |
| Camera Follow | Depends on Glucose System | 读取 `VisionModifier` |
| HUD | Depends on Glucose System | 订阅状态事件 |
| Audio System | Depends on Glucose System | 订阅状态事件 |
| Game Feel & Feedback | Depends on Glucose System | 订阅状态事件 |
| Game Flow | Depends on Glucose System | 订阅 `OnCrisisFailed` |
| Scoring & Rating | Depends on Glucose System | 查询历史血糖数据 |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| `passiveDriftNormal` | -0.5 /s | -2 ~ 0 | 玩家必须更频繁补充/干预 | 血糖更稳定，游戏更简单 |
| `passiveDriftHigh` | +1.2 /s | 0.5 ~ 3 | 高血糖危机更快 | 更宽容 |
| `lowCrisisThreshold` | 10 | 5 ~ 20 | 更难进入低血糖危机 | 更容易失败 |
| `highCrisisThreshold` | 95 | 90 ~ 98 | 更难进入高血糖危机 | 更容易失败 |
| `crisisDuration` | 3.0 s | 2 ~ 5 | 更多反应时间 | 更紧张 |
| `stateTransitionHysteresis` | 0.5 s | 0 ~ 1.5 | 更少状态抖动 | 更多抖动 |
| `modifierBlendTime` | 0.3 s | 0 ~ 1.0 | 更平滑 | 更敏感 |
| `speedModifierHigh` | 1.35 | 1.1 ~ 1.6 | 高血糖更刺激但更难控 | 更平缓 |
| `controlModifierLow` | 0.65 | 0.4 ~ 0.8 | 低血糖更沉重 | 更轻松 |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| 进入 Low | 屏幕边缘蓝色脉冲；糖糖出现哈气粒子 | 低沉心跳（60 BPM） | MVP |
| 进入 Below Normal | 蓝色警示粒子密度低 | 轻微低音提示 | MVP |
| 进入 Normal | UI 绿色稳定填充；绿色治愈涟漪 | 悦耳「叮」 | MVP |
| 进入 Above Normal | 屏幕边缘红色脉冲；黄色警示粒子 | 心跳加速（90 BPM） | MVP |
| 进入 High | 红色暴风雪粒子；强 vignette；轻微抖动 | 心跳急促（120 BPM）+ 警报 | MVP |
| 进入 Crisis (Low/High) | 漫画分镜框 + ❗❗❗ + 速度线 + 像素抽动 | 低频脉冲 + 警报 | MVP |
| 道具生效（胰岛素/降糖药） | 蓝色/绿色光芒扩散环 | 「嘶」/「咕嘟」 | MVP |
| 吃高糖雪花 | 红色糖晶爆裂 | 「咻」加速音 | MVP |
| 失败 | 紫色谢幕背景；糖糖挥手；「血糖大冒险结束啦！」 | 失败旋律 | MVP |

## Game Feel

### Feel Reference

应该像《Ski Safari》中「吃到加速道具后的失控快感」，但加上「必须自我管理」的紧张感。不是《暗黑地牢》的压迫，也不是《Alto's Odyssey》的禅意——而是**「卡通过山车 + 生存温度计」**。

**Anti-reference**：不要像真实医疗 App 那样冰冷读数；不要像硬核生存游戏那样一次失误就绝望。

### Input Responsiveness

血糖修饰符必须在玩家触发道具/进入新区间后 **≤ 100ms** 内开始生效（6 帧 @ 60fps）。状态事件订阅者必须在同一帧收到通知。

### Weight and Responsiveness Profile

- **Weight**：血糖正常时轻快感；低血糖时沉重黏稠；高血糖时刺激失控。
- **Player control**：正常时高控制；危机时控制被压缩，但玩家仍有干预手段（道具）。
- **Snap quality**：状态切换用 0.3s 插值，感觉「顺滑但明确」。
- **Acceleration model**：修饰符即时响应输入（道具/区域），没有启动延迟。
- **Failure texture**：失败前有 3 秒 crisis 缓冲，玩家知道「我没及时用道具」，而不是「突然死亡」。

### Feel Acceptance Criteria

- [ ] 测试玩家能在不看教程的情况下，3 秒内意识到「血糖条是我的生存条」。
- [ ] 高血糖时玩家会说「好快，要失控了」而不是「我看不懂数字」。
- [ ] 低血糖时玩家会说「动作变沉了」而不是「游戏卡顿」。
- [ ] 没有玩家用「说教」「医疗课」描述血糖系统。

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| 当前血糖值（0–100 抽象条） | 屏幕左上角 HUD | 每帧 | 始终可见 |
| 当前状态文字（低/正常/高等） | 血糖条下方或内部 | 状态切换时 | 始终可见 |
| 危机倒计时（ crisis 状态） | 血糖条旁或屏幕中央 | 每帧 | crisis 状态 |
| 血糖变化弹窗（+15 / -20） | 糖糖头顶或血糖条旁 | 触发时 | 道具/区域生效 |
| 状态颜色（蓝/绿/红） | 血糖条填充色 + 屏幕边缘 | 每帧 | 随状态变化 |

## Cross-References

| This Document References | Target GDD | Specific Element Referenced | Nature |
|--------------------------|-----------|----------------------------|--------|
| 道具改变血糖 | `design/gdd/item-pickup-system.md` | `ApplyGlucoseDelta` API | Data dependency |
| 区域被动影响血糖 | `design/gdd/environmental-zones.md` | `ApplyPassiveDelta` API | Data dependency |
| 血糖影响滑行速度/操控 | `design/gdd/skiing-controller.md` | `SpeedModifier`, `ControlModifier` | Data dependency |
| 血糖影响视野/抖动 | `design/gdd/camera-follow.md` | `VisionModifier` | Data dependency |
| 危机/失败判定 | `design/gdd/game-flow.md` | `OnCrisisFailed` event | State trigger |
| 血糖数据用于评分 | `design/gdd/scoring-rating.md` | Historical glucose stats | Data dependency |

> 注：当前引用的 GDD 可能尚未撰写，但接口命名已在 `systems-index.md` 中约定。

## Acceptance Criteria

- [ ] 血糖值在 0–100 范围内，状态切换不抖动。
- [ ] 5 档状态各自对应不同的 Speed/Control/Vision 修饰符。
- [ ] 道具和区域只能通过 `ApplyGlucoseDelta` / `ApplyBuffOverTime` / `ApplyPassiveDelta` 改变血糖。
- [ ] 低血糖 ≤ 10 / 高血糖 ≥ 95 持续 3 秒触发失败。
- [ ] HUD 血糖条颜色随状态从蓝→绿→红平滑过渡。
- [ ] 危机状态触发漫画分镜框、❗❗❗、速度线，但不去饱和/不模拟真实症状。
- [ ] 暂停时血糖系统停止更新。
- [ ] 所有数值可从 ScriptableObject/Data 配置，无硬编码。

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| 是否需要色盲模式下的血糖条形状冗余？ | ux-designer | W2 中 | 待 art-bible §色盲安全落地 |
| 高血糖区的 `zoneDelta` 具体数值由谁平衡？ | game-designer | W2 末 | 待 Environmental Zones GDD |
| 道具叠加规则是否允许「胰岛素 + 降糖药」同时生效？ | game-designer | W2 中 | 待 Item System GDD |
| 是否需要存档玩家最近一次的血糖历史？ | systems-designer | W3 | 待 Save System 决策 |
