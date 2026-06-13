# Skiing Controller — 滑雪控制器

> **Status**: Draft  
> **Author**: game-designer, gameplay-programmer  
> **Last Updated**: 2026-06-13  
> **Last Verified**: 2026-06-13  
> **Implements Pillar**: 爽感 (Feel) + 教育 (Education)  
> **Source**: `design/gdd/game-concept.md` §12, §14

## Summary

Skiing Controller 是 SugarRush 的**手感核心**。它把输入动作（跳跃、翻滚、转向）转化为糖糖在雪山上的物理运动，同时读取 Glucose System 的速度/操控修饰符，让血糖状态直接改变滑行体验。控制器的目标是：30 秒上手、一次按键一次爽快反馈、血糖高低能被玩家「身体感知」而非只看 UI。

> **Quick reference** — Layer: `Core` · Priority: `MVP` · Key deps: `Input System`, `Glucose System`

## Overview

糖糖以恒定前向速度在 2D 横版场景中自动下滑。玩家只能控制三件事：

1. **跳跃**：越过障碍、拾取高空道具。
2. **翻滚**：穿过低矮障碍、获得短无敌帧。
3. **转向**：微调横向位置，选择路线（健康区/道具/捷径）。

前向速度由基础速度和血糖 `SpeedModifier` 共同决定；转向灵敏度由 `ControlModifier` 缩放。高血糖让玩家更快但也更难控制，低血糖让玩家沉重迟滞——这些变化通过物理参数直接体现。

## Player Fantasy

玩家应该感觉自己在**操控一个会自己往前冲的雪球**：

- 正常血糖：顺滑、跟手、想跳就跳、想滚就滚。
- 高血糖：像踩了火箭滑板——速度飙升、转向过度、必须提前预判。
- 低血糖：像雪板陷进糖浆——动作变慢、跳跃无力、需要更长的提前量。

## Detailed Design

### Core Rules

1. **自动前滑**：糖糖始终受到向右的恒定水平推力，玩家不能直接刹车或倒车。
2. **物理驱动**：使用 `Rigidbody2D` + 力/速度控制，保证碰撞、斜坡、跳跃弧线自然。
3. **输入 → 动作映射**（来自 Input System）：
   - `Jump`： grounded 时施加垂直冲量；空中可再次微调水平速度。
   - `Roll`： grounded 时触发；持续 0.5s，期间碰撞箱降低并进入无敌。
   - `SteerLeft/Right`：水平施加侧向力/目标速度，模拟滑雪转向。
4. **血糖修饰符只读**：从 `GlucoseSystem` 读取 `SpeedModifier` 与 `ControlModifier`，不反向写血糖。
5. ** grounded 判定**：角色底部射线检测，距离地面 ≤ 0.1 单位视为 grounded；离开地面后 0.08s 内仍允许跳跃（coyote time）。
6. **翻滚规则**：
   - 仅 grounded 时可触发。
   - 持续 0.5s，期间无法再次翻滚。
   - 翻滚中按跳跃 → **翻滚跳跃 combo**：保持低碰撞箱起跳，水平速度小幅提升。
   - 翻滚可穿越高度 ≤ 角色蹲伏高度的障碍物。
7. **转向限制**：
   - 水平速度有上限，防止飞出屏幕。
   - 空中转向效率为地面的 60%。
   - 高血糖时转向灵敏度降低（`ControlModifier < 1`），但最大前向速度提升。

### Movement Model

#### Horizontal (Forward)

```
targetForwardSpeed = baseForwardSpeed * SpeedModifier
forwardForce       = (targetForwardSpeed - currentVelocity.x) * acceleration
```

| Parameter | Default | Safe Range | Description |
|-----------|---------|------------|-------------|
| `baseForwardSpeed` | 8.0 m/s | 5 ~ 14 | 正常血糖下的目标前向速度 |
| `forwardAcceleration` | 20.0 | 10 ~ 40 | 达到目标速度的力度 |
| `maxForwardSpeed` | 14.0 m/s | 10 ~ 22 | 绝对上限（防止高血糖无限加速） |

#### Vertical (Jump)

```
if grounded or coyoteTimeActive:
    velocity.y = jumpForce
```

| Parameter | Default | Safe Range | Description |
|-----------|---------|------------|-------------|
| `jumpForce` | 12.0 | 8 ~ 18 | 起跳垂直速度 |
| `gravityScale` | 2.5 | 1.5 ~ 4.0 | 下落曲率 |
| `coyoteTime` | 0.08 s | 0.05 ~ 0.15 | 离开地面后仍可跳 |
| `jumpBufferTime` | 0.12 s | 0.05 ~ 0.25 | 落地前预输入 |

#### Horizontal (Steer)

```
steerInput        = Input.GetAxis("Steer")  // -1..1
steerAcceleration = baseSteerAcceleration * ControlModifier
targetSteerSpeed  = steerInput * maxSteerSpeed * ControlModifier
steerForce        = (targetSteerSpeed - currentVelocity.y) * steerAcceleration
```

> 注：横版视角中，屏幕水平为前进方向，屏幕垂直为左右转向方向。为与 `Rigidbody2D.velocity` 一致，记 `velocity.x` 为前进，`velocity.y` 为横向。

| Parameter | Default | Safe Range | Description |
|-----------|---------|------------|-------------|
| `maxSteerSpeed` | 5.0 m/s | 3 ~ 8 | 最大横向移动速度 |
| `baseSteerAcceleration` | 25.0 | 15 ~ 45 | 横向响应力度 |
| `airSteerFactor` | 0.6 | 0.3 ~ 1.0 | 空中转向效率 |

#### Roll

```
on Roll Started:
    isRolling = true
    rollTimer = rollDuration
    hitbox.height *= rollHeightRatio
    invincible = true

on Roll Ended:
    isRolling = false
    hitbox.height /= rollHeightRatio
    invincible = false
```

| Parameter | Default | Safe Range | Description |
|-----------|---------|------------|-------------|
| `rollDuration` | 0.5 s | 0.3 ~ 0.8 | 翻滚持续时间 |
| `rollHeightRatio` | 0.5 | 0.4 ~ 0.7 | 翻滚时碰撞箱高度比例 |
| `rollJumpBoost` | 1.15 | 1.0 ~ 1.3 | 翻滚跳跃水平速度加成 |
| `rollCooldown` | 0.3 s | 0.1 ~ 0.5 | 翻滚结束后再次翻滚冷却 |

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Grounded | 底部射线命中地面 | 离开地面 > coyoteTime | 可跳、可滚；转向 100% 生效 |
| Airborne | 离开地面 | 落地 | 可再次按跳微调；不可滚；转向效率 60% |
| Rolling |  grounded + Roll 输入 | rollDuration 结束或落地前保持 | 无敌；低碰撞箱；可接翻滚跳跃 |
| Roll-Jump | Rolling 中 Jump 输入 | 落地 | 低碰撞箱起跳；水平速度 +15% |
| Stumbled | 撞到不可穿越障碍 | 0.6s 后自动恢复 | 速度短暂归零；无法控制；播放摔倒动画 |
| Crashed | 撞到致命障碍或血糖危机 | 关卡失败 | 触发失败流程 |

### Interactions with Other Systems

| System | Interface | Data Flow |
|--------|-----------|-----------|
| **Input System** | 订阅 `Jump`, `Roll`, `SteerLeft`, `SteerRight` | Input → 动作触发 |
| **Glucose System** | 只读 `SpeedModifier`, `ControlModifier` | Glucose → 物理参数缩放 |
| **Obstacle & Hazard** | 碰撞事件 `OnHit(ObstacleType type, bool isRollable)` | 摔倒/死亡判定 |
| **Camera Follow** | 发送 `OnJumpPeak`, `OnRollStarted`, `OnStumbled` 事件 | Controller → 相机震动/FOV |
| **Game Feel & Feedback** | 发送 `OnJump`, `OnLand`, `OnRoll`, `OnStumble` | Controller → 粒子/音效 |
| **Scoring & Rating** | 查询跳跃次数、翻滚次数、摔倒次数 | 评分统计 |

**所有权规则**：Skiing Controller 不直接修改血糖；运动本身不改变血糖（避免循环依赖）。任何「运动降糖」效果由 Environmental Zones 或 Item System 通过 Glucose API 触发。

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| 高血糖时撞到障碍 | 正常触发摔倒；速度高=容错窗口更小 | 风险与奖励并存 |
| 低血糖时尝试翻滚跳跃 | 速度 modifier 低，combo 距离变短 | 状态真实可感 |
| 连续按翻滚 | 受 rollCooldown 限制，不能无限无敌 | 防止滥用 |
| 从斜坡起跳 | 斜坡法线影响起跳方向；使用 `Rigidbody2D` 自然处理 | 物理一致性 |
| 被障碍物夹住 | 物理挤压；若持续卡住触发 Crashed | 避免 soft-lock |
| 暂停游戏 | 保存当前速度；恢复后继续 | 公平性 |
| 关卡结束胜利 | 速度逐渐降到 0，播放胜利动画 | 结算仪式感 |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Input System | Skiing depends on Input | 订阅动作事件 |
| Glucose System | Skiing depends on Glucose | 读取 SpeedModifier / ControlModifier |
| Obstacle & Hazard | Mutual | 碰撞事件；摔倒/死亡判定 |
| Camera Follow | Skiing → Camera | 发送相机事件 |
| Game Feel & Feedback | Skiing → Feedback | 发送动作事件 |
| Scoring & Rating | Scoring depends on Skiing | 读取动作统计 |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| `baseForwardSpeed` | 8.0 m/s | 5 ~ 14 | 整体节奏更快 | 整体节奏更慢 |
| `maxForwardSpeed` | 14.0 m/s | 10 ~ 22 | 高血糖上限更高 | 更可控 |
| `forwardAcceleration` | 20.0 | 10 ~ 40 | 更快达到目标速度 | 更拖沓 |
| `jumpForce` | 12.0 | 8 ~ 18 | 跳得更高 | 跳得更低 |
| `gravityScale` | 2.5 | 1.5 ~ 4.0 | 下落更快 | 飘得更久 |
| `maxSteerSpeed` | 5.0 m/s | 3 ~ 8 | 左右移动更快 | 更 sluggish |
| `baseSteerAcceleration` | 25.0 | 15 ~ 45 | 转向更跟手 | 转向更滑 |
| `airSteerFactor` | 0.6 | 0.3 ~ 1.0 | 空中更好修正 | 空中更难控制 |
| `coyoteTime` | 0.08 s | 0.05 ~ 0.15 | 更宽容 | 更严格 |
| `rollDuration` | 0.5 s | 0.3 ~ 0.8 | 无敌窗口更长 | 更短 |
| `rollCooldown` | 0.3 s | 0.1 ~ 0.5 | 更难连续滚 | 更容易 spam |
| `stumbleDuration` | 0.6 s | 0.4 ~ 1.0 | 惩罚更重 | 更轻 |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| 跳跃 | 起跳扬尘粒子；糖糖压缩 → 伸展动画 | 「咻」 | MVP |
| 落地 | 落地小烟；糖糖缓冲动画 | 「噗」 | MVP |
| 翻滚 | 糖糖缩成球滚动；拖尾粒子 | 「嗖」+ 雪地摩擦 | MVP |
| 翻滚跳跃 combo | 起跳时球弹开；强化扬尘 | 「嘣」 | MVP |
| 摔倒 | 糖糖翻滚倒地；星星转圈；0.6s 后站起 | 「哎哟」卡通音 | MVP |
| 高血糖滑行 | 身后拖尾变长变红；速度线增强 | 风声加剧 | MVP |
| 低血糖滑行 | 动作变慢；糖糖呼出白气；拖尾变短 | 低沉环境音 | MVP |

## Game Feel

### Feel Reference

- **Primary reference**: 《Ski Safari》—— 自动前冲、跳跃翻滚组合、爽快节奏。
- **Secondary reference**: 《Alto's Odyssey》—— 顺滑转向和优雅的落地感。
- **Anti-reference**: 不要像《Temple Run》那样镜头向后；不要像平台跳跃游戏那样需要精确像素操作。

### Input Responsiveness

| Action | Max Input-to-Motion Latency | Frame Budget (60fps) |
|--------|----------------------------|---------------------|
| Jump | ≤ 33 ms | ≤ 2 frames |
| Roll | ≤ 33 ms | ≤ 2 frames |
| Steer | ≤ 50 ms | ≤ 3 frames |

### Weight and Responsiveness Profile

- **Normal**: 轻盈、跟手、跳跃弧线中等；转向阻尼低。
- **High glucose**: 前向速度接近上限；转向有轻微 overshoot；落地后滑行距离变长。
- **Low glucose**: 起跳无力；下落更快；转向迟缓；整体像「拖着沙袋」。

### Feel Acceptance Criteria

- [ ] 测试玩家 30 秒内能稳定跳过 3 个连续障碍。
- [ ] 高血糖时玩家会主动说「好快，要提前跳」。
- [ ] 低血糖时玩家会说「跳不起来」而不是「按键没反应」。
- [ ] 翻滚过障碍后有明显正反馈（无敌感 + 速度维持）。
- [ ] 摔倒惩罚清晰可见，但不会让玩家觉得「被系统耍了」。

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| 速度感提示 | 屏幕边缘速度线 | 每帧 | 速度 > 90% max |
| 跳跃/翻滚可用提示 | 糖糖脚底小箭头 | 状态变化 | grounded / roll-ready |
| 摔倒恢复倒计时 | 糖糖头顶小进度环 | 每帧 | Stumbled 状态 |

## Cross-References

| This Document References | Target GDD | Specific Element Referenced | Nature |
|--------------------------|-----------|----------------------------|--------|
| 跳跃/翻滚/转向输入 | `design/gdd/input-system.md` | `Jump`, `Roll`, `Steer` 事件 | Data dependency |
| 血糖修饰符 | `design/gdd/glucose-system.md` | `SpeedModifier`, `ControlModifier` | Data dependency |
| 障碍碰撞 | `design/gdd/obstacle-hazard-system.md` | 碰撞事件、可翻滚判定 | Data dependency |
| 相机跟随 | `design/gdd/camera-follow.md` | `OnJumpPeak`, `OnStumble` | Event trigger |
| 动作统计 | `design/gdd/scoring-rating.md` | 跳跃/翻滚/摔倒次数 | Data dependency |

## Acceptance Criteria

- [ ] 糖糖自动前向滑行，玩家可通过跳跃/翻滚/转向控制。
- [ ] 跳跃 ≤ 2 帧响应；翻滚 ≤ 2 帧响应。
- [ ] grounded 判定 + coyote time + jump buffer 全部生效。
- [ ] 翻滚期间无敌，可穿越低矮障碍，可接翻滚跳跃 combo。
- [ ] 高血糖速度提升、操控下降；低血糖速度下降、操控沉重。
- [ ] 撞到不可穿越障碍触发 0.6s 摔倒，期间无法控制。
- [ ] 所有物理参数可从 ScriptableObject/Data 配置，无硬编码。
- [ ] 暂停/胜利时运动正确冻结或减速。

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| 是否使用 `Rigidbody2D` 还是纯 transform 插值？ | gameplay-programmer | W2 初 | 本 GDD 推荐 Rigidbody2D，需原型验证斜坡和碰撞 |
| 斜坡对跳跃方向的影响是否需要特殊处理？ | gameplay-programmer | W2 中 | 依赖关卡 Tilemap 实际斜坡角度 |
| 是否需要「二段跳」扩展？ | game-designer | W3 | MVP 不做；运动鞋道具可扩展 |
| 翻滚无敌帧是否对 Boss/巨型障碍也有效？ | game-designer | W2 中 | 待 Obstacle & Hazard GDD 定义障碍分类 |
