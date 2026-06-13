# Input System — 输入系统

> **Status**: Draft  
> **Author**: game-designer, ux-designer  
> **Last Updated**: 2026-06-13  
> **Last Verified**: 2026-06-13  
> **Implements Pillar**: 爽感 (Feel)  
> **Source**: `design/gdd/game-concept.md` §12

## Summary

输入系统是玩家与 SugarRush 交互的唯一入口。它统一处理键盘/鼠标和手柄输入，将原始按键转译为高层游戏动作（跳跃、翻滚、转向、使用道具、暂停），并向 Skiing Controller、Item System、Game Flow 等模块发送无歧义的动作事件。

> **Quick reference** — Layer: `Foundation` · Priority: `MVP` · Key deps: `None`

## Overview

SugarRush 的操作设计目标是「30 秒内上手」。玩家可以用左手键盘 + 右手鼠标单手操作，也可以完全用手柄游玩。输入系统负责：

1.  监听硬件输入（键盘、鼠标、手柄）。
2.  将输入映射为抽象游戏动作。
3.  处理动作优先级、冲突和缓冲（如跳跃缓冲、翻滚取消）。
4.  向其他系统发送动作事件，避免它们直接读取原始输入。

## Player Fantasy

玩家应该感觉「控制即反应」—— 按跳跃立刻跳，按翻滚立刻滚，没有任何迟滞。手柄和键鼠手感一致，不会因为换设备而重新学习。

## Detailed Design

### Core Rules

1. **抽象动作层**：所有下游系统只订阅抽象动作，不读取具体按键。
2. **双输入源**：键盘/鼠标 和 手柄 同时可用，任意时刻最后一个有效输入源决定 UI 提示图标。
3. **动作缓冲（Action Buffering）**：
   - 跳跃缓冲：玩家在落地前 0.12s 按下跳跃，落地后立即再次跳跃。
   - 翻滚缓冲：同理 0.12s。
4. **动作优先级**：
   - 暂停 > 使用道具 > 翻滚 > 跳跃 > 转向
   - 翻滚期间可以触发跳跃（翻滚跳跃 combo）。
   - 空中不能翻滚（落地后可用）。
5. **手柄死区**：左摇杆水平死区 0.15，避免漂移。
6. **UI 导航**：所有菜单必须支持键盘（方向键/Tab + Enter）和手柄（D-pad/左摇杆 + A）导航，禁止 hover-only 交互。
7. **可重映射（扩展）**：MVP 不做完整重映射，但所有动作名使用常量，便于后续扩展。

### Action Map

| Action | Keyboard/Mouse | Xbox | PlayStation | Context |
|--------|----------------|------|-------------|---------|
| Jump | Space / Left Mouse | A | × | Gameplay |
| Roll | Left Shift / Right Mouse | B | ○ | Gameplay |
| Steer Left | A / ← | Left Stick / D-pad ← | 同左 | Gameplay |
| Steer Right | D / → | Left Stick / D-pad → | 同右 | Gameplay |
| Use Item 1 | Q / 1 | LB | L1 | Gameplay |
| Use Item 2 | E / 2 | RB | R1 | Gameplay |
| Use Item 3 | R / 3 | Y | △ | Gameplay |
| Pause | Esc | Menu / Start | Options | Gameplay + UI |
| Confirm | Space / Enter / Left Mouse | A | × | UI |
| Cancel | Esc / Right Mouse | B | ○ | UI |
| Navigate Up | W / ↑ | D-pad ↑ | 同上 | UI |
| Navigate Down | S / ↓ | D-pad ↓ | 同上 | UI |
| Navigate Left | A / ← | D-pad ← | 同上 | UI |
| Navigate Right | D / → | D-pad → | 同上 | UI |

### Input Event Interface

```csharp
// 发送给订阅者的事件
public struct InputActionEvent
{
    public InputAction Action;      // Jump, Roll, SteerLeft, UseItem1, Pause, etc.
    public ActionPhase Phase;       // Started, Performed, Canceled
    public float Value;             // 模拟量（转向 -1..1）
    public InputDevice Device;      // KeyboardMouse, Gamepad
}
```

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| Gameplay Input | 进入游玩状态 | 进入暂停/菜单 | 跳跃/翻滚/转向/道具生效 |
| Menu Input | 进入菜单/暂停 | 返回游玩 | 仅导航/确认/取消 |
| Disabled | 关卡加载/结算 | 加载完成 | 不处理任何输入 |

### Interactions with Other Systems

| System | Interface | Data Flow |
|--------|-----------|-----------|
| **Skiing Controller** | 订阅 `Jump`, `Roll`, `SteerLeft`, `SteerRight` | Input → 动作指令 |
| **Item & Pickup System** | 订阅 `UseItem1/2/3` | Input → 使用道具 |
| **Game Flow** | 订阅 `Pause`, `Confirm`, `Cancel` | Input → 状态切换 |
| **HUD / Menus** | 订阅导航 + 确认/取消 | Input → UI 焦点移动 |
| **Audio System** | 订阅 `Jump`, `Roll`, 道具使用 | 触发音效 |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| 键鼠和手柄同时操作 | 最后一个产生输入的设备被识别为当前设备 | 避免输入冲突 |
| 空中按翻滚 | 忽略或缓冲到落地后执行 | 翻滚仅限地面 |
| 翻滚中按跳跃 | 执行翻滚跳跃 combo | 增加爽感 |
| 暂停中按跳跃 | 忽略 | 暂停时仅菜单导航有效 |
| 按住跳跃键不松 | 仅触发一次跳跃（除非落地） | 防止连跳失控 |
| 手柄断连 | 自动切换回键鼠提示；已按下的动作视为 Canceled | 优雅降级 |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Skiing Controller | Input depends on nothing / Skiing depends on Input | 动作事件消费 |
| Item & Pickup System | Depends on Input | 使用道具触发 |
| Game Flow | Depends on Input | 暂停/确认/取消 |
| HUD / Menus | Depends on Input | UI 导航 |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| `jumpBufferTime` | 0.12 s | 0.05 ~ 0.25 | 更宽容 | 更严格 |
| `rollBufferTime` | 0.12 s | 0.05 ~ 0.25 | 更宽容 | 更严格 |
| `gamepadDeadZone` | 0.15 | 0.05 ~ 0.30 | 更少漂移 | 更灵敏 |
| `steerAnalogSensitivity` | 1.0 | 0.5 ~ 2.0 | 转向更快 | 转向更慢 |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| 当前输入源切换 | 右下角图标切换（键鼠/手柄） | 无 | Vertical Slice |
| 跳跃 | 糖糖起跳动画 | 「咻」 | MVP |
| 翻滚 | 糖糖翻滚动画 | 「嗖」 | MVP |
| 道具使用 | 道具动画/光效 | 对应道具音 | MVP |
| 暂停 | 暂停菜单淡入 | 暂停音效 | MVP |

## Game Feel

### Feel Reference

像《Ski Safari》和《Alto's Odyssey》的结合：跳跃响应极快，翻滚有短无敌帧，转向顺滑但带一点惯性。按键反馈应在 1-2 帧内可见。

### Input Responsiveness

| Action | Max Input-to-Response Latency | Frame Budget (60fps) |
|--------|------------------------------|---------------------|
| Jump | ≤ 33 ms | ≤ 2 frames |
| Roll | ≤ 33 ms | ≤ 2 frames |
| Steer | ≤ 33 ms | ≤ 2 frames |
| Pause | ≤ 50 ms | ≤ 3 frames |
| UI Navigation | ≤ 50 ms | ≤ 3 frames |

### Weight and Responsiveness Profile

- **响应**：跳跃/翻滚即时，无启动延迟。
- **控制**：空中可微调落点，但受 Skiing Controller 物理约束。
- **失败纹理**：输入失败时（如空中翻滚），通过动画或音效告知玩家原因，而不是静默忽略。

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| 按键提示（跳跃/翻滚/道具） | 关卡内提示或 HUD 边缘 | 状态变化时 | 新手提示 |
| 当前输入源图标 | 设置/暂停菜单角落 | 输入源切换时 | 始终 |
| 手柄/键鼠按键图标 | 道具栏旁 | 输入源切换时 | Gameplay |

## Cross-References

| This Document References | Target GDD | Specific Element Referenced | Nature |
|--------------------------|-----------|----------------------------|--------|
| 跳跃/翻滚/转向动作 | `design/gdd/skiing-controller.md` | `Jump`, `Roll`, `Steer` 动作事件 | State trigger |
| 使用道具 | `design/gdd/item-pickup-system.md` | `UseItem1/2/3` 动作事件 | State trigger |
| 暂停/菜单 | `design/gdd/game-flow.md` | `Pause`, `Confirm`, `Cancel` 事件 | State trigger |

## Acceptance Criteria

- [ ] 键鼠和手柄都能完整操作游戏。
- [ ] 跳跃/翻滚/转向延迟 ≤ 2 帧。
- [ ] 跳跃和翻滚缓冲 0.12s 生效。
- [ ] 空中不能翻滚，翻滚中可接跳跃。
- [ ] 所有菜单支持键盘和手柄导航。
- [ ] 当前输入源切换时 UI 提示更新。
- [ ] 下游系统不直接读取原始按键，只订阅抽象动作。
- [ ] 无硬编码按键，动作名使用常量/枚举。

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| 是否支持完整按键重映射？ | ux-designer | W3 | MVP 不做，扩展规划 |
| 手柄震动是否加入（跳跃落地/道具生效）？ | game-designer | W3 | 依赖资源和时间 |
| 鼠标点击 UI 与 Gameplay 跳跃冲突如何处理？ | ux-designer | W2 | 暂停/菜单时禁用 gameplay 输入 |
