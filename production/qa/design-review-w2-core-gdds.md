# Design Review — W2 Core GDDs

> **Review Mode**: Lean
> **Scope**: 7 MVP GDDs completed on 2026-06-13
>   - `design/gdd/glucose-system.md`
>   - `design/gdd/input-system.md`
>   - `design/gdd/skiing-controller.md`
>   - `design/gdd/obstacle-hazard-system.md`
>   - `design/gdd/item-pickup-system.md`
>   - `design/gdd/game-flow.md`
>   - `design/gdd/level-data-progression.md`
> **Reviewer**: AI design-reviewer
> **Date**: 2026-06-13

---

## Executive Summary

7 份核心 GDD 已覆盖 SugarRush MVP 核心循环所需的最小设计集合。整体架构清晰，遵循「血糖 = 生存条」的 Mechanic Translation 原则，系统间依赖无循环。发现 3 个**应尽快修复**的关键问题、4 个**中等风险**的模糊点，以及若干**低优先级建议**。

**结论**：修复 C1–C3 后，可进入 Unity 原型实现阶段。

---

## Findings

### 🔴 Critical — 必须修复

| ID | Issue | Location | Impact | Recommended Fix |
|----|-------|----------|--------|-----------------|
| **C1** | `OnCrisisFailed` 事件未在 Glucose System GDD 中显式定义 | `glucose-system.md` §6, §8 | Game Flow 订阅了一个未命名的事件，实现时可能漏掉或命名不一致 | 在 Glucose System 的「Interactions」和「Events」章节增加 `OnCrisisFailed` / `OnCrisisEntered/Exited` 事件定义 |
| **C2** | 高糖雪花速度加成是「叠加」还是「覆盖」表述矛盾 | `item-pickup-system.md` §3.2, `ItemData` schema | 程序员可能实现为覆盖 Glucose modifier，导致高血糖时速度异常 | 明确为「独立临时加法 bonus（+0.15），与 Glucose modifier 相乘」；修改 `ItemData` 字段名或注释，移除 `overridesSpeedModifier` 歧义 |
| **C3** | 缺少 HUD、Camera Follow、Scoring GDD 但已被多处引用 | `systems-index.md`, cross-references in Skiing/GameFlow/LevelData | 原型阶段可占位实现，但缺少正式规范会导致 UI/相机行为不一致 | 进入开发前先写 1 页轻量 HUD GDD；Camera 可合并进 Skiing GDD 作为实现注释 |

### 🟡 Warning — 建议处理

| ID | Issue | Location | Impact | Recommended Fix |
|----|-------|----------|--------|-----------------|
| **W1** | `ApplyGlucoseDelta(float delta, Source source)` 中的 `Source` 枚举未定义 | `glucose-system.md`, `item-pickup-system.md`, `obstacle-hazard-system.md` | 不同系统可能传不一致的 source 值 | 在 Glucose System GDD 中定义 `GlucoseChangeSource` 枚举（Item, Zone, Passive） |
| **W2** | Skiing Controller 的 steer 公式使用 `velocity.y` 表示左右，但 2D 游戏中 Y 通常表示上下 | `skiing-controller.md` §3.3 | 可能导致代码中坐标轴理解错误 | 增加注释说明：本游戏采用横版视角，X 为前进，Y 为屏幕垂直方向（左右移动） |
| **W3** | GameOver/Victory 到 Result 的转换条件有歧义：「动画播放完成 或 按 Confirm」 | `game-flow.md` §4 | 玩家可能误触跳过动画，也可能必须等待 | 明确默认行为：动画播放期间按 Confirm 可跳过；否则自动在动画结束后进入 Result |
| **W4** | `ObstacleData.isFatal` 与 `type == Avoidable` 有重叠 | `obstacle-hazard-system.md` §2.2 | 数据配置时可能产生冲突 | 明确规则：Avoidable 类型强制 isFatal=true；或在工具中校验 |

### 🟢 Note — 低优先级建议

| ID | Issue | Location | Recommended Action |
|----|-------|----------|-------------------|
| **N1** | L1 数量参数为起点，未经过原型验证 | `level-data-progression.md` §3.3 | 在 W2 原型阶段快速迭代，记录调参版本 |
| **N2** | Game Flow 提到「启动 GlucoseSystem」但实际上系统已在 Boot 初始化 | `game-flow.md` §3 | 改为「启用/恢复 GlucoseSystem 更新」 |
| **N3** | 没有定义「帅气通过 combo」的具体奖励 | `obstacle-hazard-system.md` §5.2 | MVP 可仅发送事件不做奖励；VS 阶段再设计 |
| **N4** | `hazardDamagePerSecond` 命名偏向伤害而非血糖变化 | `obstacle-hazard-system.md` §2.4 | 建议改为 `glucoseDeltaPerSecond` 以匹配 Glucose System 术语 |
| **N5** | 缺乏色盲可访问性规范 | `glucose-system.md` §Open Questions | 在 HUD GDD 中规划形状/纹理冗余 |

---

## Cross-Cutting Consistency Check

### 接口命名一致性

| Concept | Used In | Status |
|---------|---------|--------|
| `ApplyGlucoseDelta` | Glucose, Item, Obstacle | ✅ 一致 |
| `ApplyBuffOverTime` | Glucose, Item | ✅ 一致 |
| `ApplyPassiveDelta` | Glucose, Obstacle, Zones | ✅ 一致 |
| `SpeedModifier` / `ControlModifier` | Glucose, Skiing, Item | ⚠️ C2 需澄清叠加规则 |
| `Stumble()` / `Crash()` | Skiing, Obstacle, GameFlow | ✅ 一致 |
| `OnCrisisFailed` | GameFlow | ❌ 未在 Glucose GDD 定义（C1） |
| `LoadLevel` / `Victory` / `GameOver` | GameFlow, LevelData | ✅ 一致 |

### 数值合理性快速检查

| Interaction | Calculation | Verdict |
|-------------|-------------|---------|
| 血糖 80 → 吃高糖雪花 | 80 + 25 = 105 → clamp 100 | 进入 High，crisis 倒计时开始 | ✅ |
| 血糖 95 → 用胰岛素 | 95 - 30 = 65 | 回到 Normal，立即解除 crisis | ✅ |
| 血糖 95 → 用降糖药 | 95 - 8*5 = 55 | 回到 Normal，持续下降 | ✅ |
| 低血糖 8 → 吃高糖雪花 | 8 + 25 = 33 | 脱离 Low crisis，进入 Below Normal | ✅ |
| 高血糖 + 高糖雪花 + 高血糖区 | 100 + 被动 + 25 = 100 | 已 clamp，但 crisis 计时继续 | ⚠️ 极端情况需测试 |

### 失败条件一致性

| Source | Failure Trigger | Handled By |
|--------|----------------|------------|
| 低血糖 crisis | value ≤ 10 持续 3s | Glucose System → GameFlow |
| 高血糖 crisis | value ≥ 95 持续 3s | Glucose System → GameFlow |
| 致命障碍 / Avoidable | 碰撞 | Obstacle → Skiing → GameFlow |
| Hazard threshold | 达到阈值 | Obstacle → GameFlow |
| 夹死 / soft-lock | 持续卡住 | Skiing Controller → GameFlow |

✅ 失败路径清晰，无多头管理。

---

## MVP Scope Compliance

| Requirement | Status |
|-------------|--------|
| 单人模式 | ✅ 已明确 |
| 3 道具（胰岛素/降糖药/高糖雪花） | ✅ 已定义 |
| 血糖机制转译 | ✅ 5 档抽象 + modifier |
| L1 医院雪屋 | ✅ 已设计 |
| 无多人 | ✅ 已砍掉 |
| 无程序化地形 | ✅ 改为手作 Chunk |
| 医学准确性底线 | ✅ 抽象数值 + 概念正确 |

---

## Recommendations

1. **立即修复 C1–C3** 后，可进入 Unity 开发。
2. **W2 开发优先级**：
   1. GlucoseSystem + 简单 HUD 血糖条
   2. SkiingController + Cinemachine 2D 占位
   3. InputSystem 动作映射
   4. Obstacle/Hazard 碰撞 + Stumble/Crash
   5. Item pickup + usage
   6. GameFlow 状态机
   7. LevelData L1 手搭
3. **W2 末必须内部试玩**，验证：
   - 30 秒内理解血糖条 = 生存条
   - 高血糖/低血糖能被身体感知
   - 道具使用时机有决策感

---

## Sign-off

- [ ] 所有 Critical 已修复
- [ ] 所有 Warning 已记录或修复
- [ ] 团队确认可进入 `/prototype`
