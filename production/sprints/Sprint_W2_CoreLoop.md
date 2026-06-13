---
title: "糖大冒险：雪山狂飙 — Sprint W2"
sprint_number: 2
start_date: "2026-06-15"
end_date: "2026-06-21"
goal: "完成核心循环原型：滑行手感 + 血糖系统 + 3 个 MVP 道具 + L1 医院雪屋可玩"
status: "Planning"
---

# Sprint W2: 核心循环原型

## Goal

验证「机制转译」是否成立：玩家能在滑雪、跳跃、收集、决策中自然理解血糖平衡。

## Tasks

| # | Task | Size | Depends On | Skill / Command | Status | Linear ID |
|---|------|------|-----------|-----------------|--------|-----------|
| 1 | Glucose System GDD | M | — | `/design-system glucose-system` | ⬜ | |
| 2 | Input System GDD | S | — | `/design-system input-system` | ⬜ | |
| 3 | Skiing Controller GDD | L | Input System | `/design-system skiing-controller` | ⬜ | |
| 4 | Obstacle & Hazard GDD | M | Skiing Controller | `/design-system obstacle-hazard` | ⬜ | |
| 5 | Item & Pickup System GDD | M | Glucose, Obstacle | `/design-system item-pickup` | ⬜ | |
| 6 | Level Data & Progression GDD | L | Skiing, Obstacle, Item | `/design-system level-data` | ⬜ | |
| 7 | Camera Follow 方案 | S | Skiing Controller | unity-specialist | ⬜ | |
| 8 | 核心循环原型 `/prototype` | L | GDDs above | `/prototype core-loop` | ⬜ | |
| 9 | L1 医院雪屋初版场景 | M | Level Data | level-designer | ⬜ | |
| 10 | 内部试玩 + 5 题问卷 | S | L1 可玩 | producer / qa-tester | ⬜ | |

**Status Legend**: ⬜ To Do | 🔄 In Progress | ✅ Done | ❌ Blocked

## Acceptance Criteria

- [ ] 糖糖能在 L1 场景中滑行、跳跃、翻滚
- [ ] 血糖条实时变化，5 档状态可读
- [ ] 3 个 MVP 道具（胰岛素喷雾 / 降糖药瓶 / 高糖雪花）有效果
- [ ] 抵达终点「医院降糖站」可触发结算
- [ ] 无阻断崩溃，帧率稳定在 60 fps
- [ ] 试玩玩家 ≥ 70% 能正确回答 5 题中的 4 题

## Risks

| Risk | Mitigation |
|------|-----------|
| 滑行手感不爽 | W2 初即 `/prototype`；占位 sprite；Cinemachine 2D |
| 血糖机制不直观 | 5 档分级 + 多通道反馈 + 试玩问卷 |
| 范围超出 | L3 低血糖区移入 W3 弹性；只做 L1 + L2 视觉区 |

## Retrospective

### What went well
<!-- Fill after sprint completion -->

### What could improve
<!-- Fill after sprint completion -->

### Action items for next sprint
<!-- Fill after sprint completion -->
