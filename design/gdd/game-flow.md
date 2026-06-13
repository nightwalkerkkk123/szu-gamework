# Game Flow — 游戏流程系统

> **Status**: Draft  
> **Author**: game-designer, ux-designer  
> **Last Updated**: 2026-06-13  
> **Last Verified**: 2026-06-13  
> **Implements Pillar**: 爽感 (Feel) + 教育 (Education)  
> **Source**: `design/gdd/game-concept.md` §12, §14

## Summary

Game Flow 是 SugarRush 的**状态机骨架**。它管理玩家从启动游戏到主菜单、进入关卡、游玩、暂停、失败/胜利、结算、返回的完整生命周期。所有与「当前游戏处于什么阶段」相关的决策都集中在这里，避免各个系统各自维护状态导致不一致。

> **Quick reference** — Layer: `Core` · Priority: `MVP` · Key deps: `Input System`

## Overview

游戏流程采用有限状态机（FSM），核心状态包括：

| State | 说明 |
|-------|------|
| **Boot** | 启动加载：显示 Logo、初始化系统、读取设置 |
| **MainMenu** | 主菜单：开始游戏、选择关卡、设置、退出 |
| **LevelIntro** | 关卡开始前的轻量介绍（可选） |
| **Playing** | 正式游玩：输入、物理、血糖系统全部激活 |
| **Paused** | 暂停：时间缩放为 0，显示暂停菜单 |
| **GameOver** | 关卡失败（血糖危机 / 致命碰撞） |
| **Victory** | 关卡成功抵达医院降糖站 |
| **Result** | 结算界面：显示评分、统计、重玩/下一关/返回菜单 |

## Player Fantasy

玩家应该感觉流程像**一次完整的滑雪旅程**：从营地准备（菜单）→ 出发（关卡开始）→ 滑行（游玩）→ 中途休息（暂停）→ 抵达医院或遗憾结束（结算）。没有突兀的跳转，失败也不是终点而是再来一局的理由。

## Detailed Design

### Core Rules

1. **单一状态源**：`GameFlowManager` 是唯一持有当前游戏状态的权威源。
2. **状态驱动时间缩放**：
   - `Playing`：`Time.timeScale = 1`
   - `Paused`：`Time.timeScale = 0`
   - `GameOver/Victory`：先保持 `timeScale = 1` 播放失败/胜利动画，再进入 `Result` 时定格
3. **输入上下文切换**：Game Flow 通知 Input System 当前处于 Gameplay / Menu / Disabled 哪种输入模式。
4. **关卡可重玩**：失败或胜利后，玩家可一键重玩当前关卡或返回主菜单。
5. **轻量持久化**：音量、最佳分数、最近关卡存储在本地；不处理复杂存档。
6. **MVP 不做关卡选择**：主菜单直接进入 L1，L2/L3 在后续迭代中解锁。

### State Machine

```
                    +---------+
                    |  Boot   |
                    +----+----+
                         |
                         v
                  +-------------+
                  |  MainMenu   |<-------------------------+
                  +------+------+                          |
                         |                                 |
              +----------+----------+                      |
              |                     |                      |
              v                     v                      |
        +-----------+        +-------------+               |
        | Settings  |        |  LevelIntro |               |
        +-----------+        +------+------+               |
                                      |                    |
                                      v                    |
                               +-------------+             |
                               |   Playing   |             |
                               +------+------+             |
                                      |                    |
          +---------------------------+----------------+   |
          |                           |                |   |
          v                           v                v   |
    +-----------+              +------------+    +---------+
    |  Paused   |              |  GameOver  |    | Victory |
    +-----+-----+              +-----+------+    +----+----+
          |                          |                |
          | Resume                   |                |
          v                          v                v
    +-----------+              +------------+    +---------+
    |  Playing  |              |   Result   |<-->| Result  |
    +-----------+              +-----+------+    +---------+
                                     |
                    +----------------+----------------+
                    |                |                |
                    v                v                v
              +-----------+    +------------+   +-----------+
              | MainMenu  |    | ReplayLevel|   | NextLevel |
              +-----------+    +------------+   +-----------+
```

### State Definitions

| State | Entry Actions | Update | Exit Actions |
|-------|--------------|--------|-------------|
| **Boot** | 显示 splash；初始化 GlucoseSystem、InputSystem 等；加载设置 | 等待初始化完成 | 进入 MainMenu |
| **MainMenu** | 显示主菜单；播放 BGM；重置游戏数据 | 监听菜单导航/选择 | 进入 LevelIntro 或 Settings |
| **LevelIntro** | 显示 L1 名称与简短提示（如「避开红色暴风雪，到达医院！」） | 3s 后或按确认进入 Playing | 预加载关卡场景 |
| **Playing** | 恢复 timeScale=1；激活玩家输入；启动 GlucoseSystem | 各系统正常更新 | 进入 Paused/GameOver/Victory |
| **Paused** | timeScale=0；显示暂停菜单；禁用 gameplay 输入 | 监听 Resume/Restart/Quit | 恢复 Playing 或跳转 |
| **GameOver** | 播放失败动画；timeScale 保持 1 直到动画结束 | 等待动画/确认 | 进入 Result（失败版） |
| **Victory** | 播放胜利动画；timeScale 保持 1 | 等待动画/确认 | 进入 Result（胜利版） |
| **Result** | 显示评分、统计、按钮 | 监听导航/选择 | 返回 MainMenu / Replay / NextLevel |

### Transitions and Triggers

| From | To | Trigger | Condition |
|------|----|---------|-----------|
| Boot | MainMenu | 初始化完成 | — |
| MainMenu | LevelIntro | 选择「开始游戏」 | — |
| LevelIntro | Playing | 计时结束 或 按 Confirm | — |
| Playing | Paused | 按 Pause | 非已结束状态 |
| Paused | Playing | 按 Resume / 再按 Pause | — |
| Paused | MainMenu | 选择「退出到菜单」 | — |
| Paused | Playing | 选择「重新开始」 | 重新加载当前关卡 |
| Playing | GameOver | `GlucoseSystem.OnCrisisFailed` 或 `SkiingController.OnCrash` | — |
| Playing | Victory | 玩家触发终点医院区域 | — |
| GameOver | Result | 失败动画播放完成 或 按 Confirm | — |
| Victory | Result | 胜利动画播放完成 或 按 Confirm | — |
| Result | MainMenu | 选择「返回菜单」 | — |
| Result | Playing | 选择「重玩本关」 | 重新加载当前关卡 |
| Result | LevelIntro | 选择「下一关」 | 加载下一关卡（MVP 禁用） |

### Interactions with Other Systems

| System | Interface | Data Flow |
|--------|-----------|-----------|
| **Input System** | 发送 `SetInputMode(Gameplay/Menu/Disabled)` | GameFlow → Input |
| **Glucose System** | 订阅 `OnCrisisFailed` | Glucose → GameFlow（GameOver） |
| **Skiing Controller** | 订阅 `OnCrash` | Skiing → GameFlow（GameOver） |
| **Level Data & Progression** | 调用 `LoadLevel(levelId)` | GameFlow → Level |
| **Scoring & Rating** | 调用 `EvaluateRun(runData)` | GameFlow → Scoring |
| **HUD / Menus / Result** | 发送 `OnStateChanged` 事件 | GameFlow → UI |
| **Audio System** | 订阅状态变化 | 切换 BGM/Ambience |
| **Item & Pickup System** | 重置库存 | 关卡加载/重玩时清空 |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| 暂停时血糖系统 | 停止更新（timeScale=0） | 公平性 |
| 暂停时玩家仍按住跳跃键 | 恢复游戏后，按键视为新的 Press | 避免意外触发 |
| 失败动画播放中按暂停 | 忽略；等进入 Result 后再显示菜单 | 状态清晰 |
| 快速连续失败/胜利 | 必须等当前状态过渡完成才响应 | 防止状态机抖动 |
| 关卡加载失败 | 显示错误提示，返回 MainMenu | 健壮性 |
| 从 Result 重玩 | 重新实例化关卡，重置血糖、库存、分数 | 干净重启 |
| 后台切出（Windows Alt+Tab） | 自动进入 Paused（可配置） | 桌面端体验 |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Input System | GameFlow depends on Input | 暂停/确认/取消输入 |
| Glucose System | GameFlow subscribes to Glucose | Crisis 失败事件 |
| Skiing Controller | GameFlow subscribes to Skiing | Crash 失败事件 |
| Level Data & Progression | GameFlow calls Level | 关卡加载 |
| Scoring & Rating | GameFlow calls Scoring | 结算评分 |
| HUD / Menus / Result | GameFlow → UI | 状态驱动 UI |
| Audio System | GameFlow → Audio | BGM/Ambience 切换 |
| Item & Pickup System | GameFlow resets Item | 关卡重开时清空 |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| `levelIntroDuration` | 3.0 s | 0 ~ 5 | 介绍更长 | 更快进入 |
| `gameOverFreezeDelay` | 0.5 s | 0 ~ 1.5 | 失败动画前停顿更长 | 更快进入动画 |
| `resultAutoAdvanceDelay` | 1.5 s | 0 ~ 3 | 结算界面自动出现更晚 | 更快 |
| `pauseBackgroundDim` | 0.7 | 0.5 ~ 0.9 | 暂停背景更暗 | 更亮 |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| Boot | Logo 淡入淡出 | 开场音效 | MVP |
| MainMenu | 雪山背景；按钮高亮 | BGM | MVP |
| LevelIntro | 关卡名 + 小提示 + 漫画分镜 | 出发哨音 | MVP |
| Pause | 背景变暗；菜单面板滑入 | 环境音减弱 | MVP |
| GameOver | 紫色谢幕背景；糖糖挥手 | 失败旋律 | MVP |
| Victory | 医院降糖站亮起；糖糖庆祝 | 胜利旋律 | MVP |
| Result | 评分面板 + 统计 + 按钮 | 结算 BGM | MVP |

## UI Requirements

| Screen | Elements | Navigation | Priority |
|--------|----------|-----------|----------|
| **MainMenu** | 开始游戏、设置、退出 | 键盘/手柄/鼠标 | MVP |
| **Settings** | 音量滑块、返回 | 键盘/手柄/鼠标 | MVP |
| **Pause** | 继续、重新开始、退出到菜单 | 键盘/手柄/鼠标 | MVP |
| **Result** | 评分（S/A/B/C）、血糖统计、重玩、返回菜单、下一关（MVP 禁用） | 键盘/手柄/鼠标 | MVP |

> 所有菜单必须支持键盘（方向键/Tab + Enter）和手柄（D-pad/左摇杆 + A）导航，已在 Input System GDD 中约束。

## Cross-References

| This Document References | Target GDD | Specific Element Referenced | Nature |
|--------------------------|-----------|----------------------------|--------|
| 暂停/确认/取消输入 | `design/gdd/input-system.md` | `Pause`, `Confirm`, `Cancel` | Data dependency |
| 血糖危机失败 | `design/gdd/glucose-system.md` | `OnCrisisFailed` | Event subscription |
| 碰撞失败 | `design/gdd/skiing-controller.md` | `OnCrash` | Event subscription |
| 关卡加载 | `design/gdd/level-data-progression.md` | `LoadLevel(levelId)` | Data dependency |
| 评分结算 | `design/gdd/scoring-rating.md` | `EvaluateRun(runData)` | Data dependency |
| 菜单 UI | `design/gdd/menus-pause.md` / `design/gdd/result-screen.md` | 暂停/结算界面 | UI dependency |

## Acceptance Criteria

- [ ] 存在单一 `GameFlowManager` 管理所有状态转换。
- [ ] `Playing` 时 timeScale=1，`Paused` 时 timeScale=0。
- [ ] 按 Pause 进入暂停，再按 Resume 或 Pause 返回游玩。
- [ ] 血糖危机或致命碰撞触发 `GameOver` → `Result`。
- [ ] 抵达终点触发 `Victory` → `Result`。
- [ ] 从 Result 可重玩当前关卡或返回主菜单。
- [ ] 状态切换时通知 Input System 切换输入模式。
- [ ] 关卡重开时重置血糖、库存、分数、玩家位置。
- [ ] 音量设置持久化到本地。

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| 是否需要关卡选择界面 MVP 就做？ | game-designer | W2 中 | 当前 GDD 建议 MVP 只有 L1，主菜单直接开始 |
| 失败/胜利动画由谁负责？ | game-designer | W2 中 | 可先占位，VS 阶段加美术 |
| Result 界面是否显示医学问卷入口？ | game-designer | W2 末 | 课程要求 5 题问卷，可放在主菜单或结果后 |
| Boot 阶段是否需要显示健康提示/免责声明？ | game-designer | W2 末 | 医学准确性要求，建议简短提示 |
