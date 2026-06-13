# Systems Index: 糖大冒险：雪山狂飙 (SugarRush)

> **Status**: Draft — 待团队审阅
> **Created**: 2026-06-12
> **Last Updated**: 2026-06-12
> **Source Concept**: design/gdd/game-concept.md
> **Review Mode**: Lean（TD-SYSTEM-BOUNDARY / PR-SCOPE / CD-SYSTEMS 跳过）

---

## Overview

《糖大冒险：雪山狂飙》是一款 **2D 横版滑雪跑酷 Serious Game**，核心循环为：滑行 → 血糖实时变化 → 玩家决策（操作/道具/路线）→ 多通道反馈 → 抵达医院降糖站 → 评分重玩。

机械范围相对聚焦：无多人、无经济、无复杂叙事。MVP 围绕 **滑行手感**、**血糖机制转译**、**3 个核心道具**、**1–3 个手作关卡** 和 **基础 UI/反馈** 展开。三大设计支柱（幽默 / 教育 / 爽感）要求每个系统都服务于「血糖 = 生存条」这一核心变量，而非独立 mini-game。

目标人群已决：**成人大众健康教育**。主角已决：**糖糖 — 中性 mascot**。

---

## Systems Enumeration

| # | System Name | Category | Priority | Status | Design Doc | Depends On |
|---|-------------|----------|----------|--------|------------|------------|
| 1 | Input System | Core | MVP | Designed | design/gdd/input-system.md | — |
| 2 | Game Flow | Core | MVP | Not Started | — | Input System |
| 3 | Skiing Controller | Gameplay | MVP | In Design | design/gdd/skiing-controller.md | Input System, Obstacle & Hazard |
| 4 | Camera Follow (inferred) | Core | MVP | Not Started | — | Skiing Controller |
| 5 | Glucose System | Gameplay | MVP | Designed | design/gdd/glucose-system.md | — |
| 6 | Obstacle & Hazard | Gameplay | MVP | Not Started | — | Skiing Controller |
| 7 | Item & Pickup System | Gameplay | MVP | Not Started | — | Glucose System, Obstacle & Hazard |
| 8 | Environmental Zones | Gameplay | MVP | Not Started | — | Glucose System, Level Data |
| 9 | Level Data & Progression | Gameplay | MVP | Not Started | — | Skiing Controller, Obstacle & Hazard, Environmental Zones, Item System |
| 10 | Scoring & Rating | Progression | MVP | Not Started | — | Glucose System, Level Data |
| 11 | HUD | UI | MVP | Not Started | — | Glucose System, Level Data, Item System |
| 12 | Menus & Pause | UI | MVP | Not Started | — | Game Flow |
| 13 | Result Screen | UI | MVP | Not Started | — | Scoring & Rating, Game Flow |
| 14 | In-Level Hints (inferred) | Meta | MVP | Not Started | — | Level Data, Glucose System |
| 15 | Game Feel & Feedback (inferred) | Meta | Vertical Slice | Not Started | — | Skiing Controller, Glucose System, Item System |
| 16 | Audio System (inferred) | Audio | Vertical Slice | Not Started | — | Game Flow, Item System, Glucose System |
| 17 | Glucose Meter Item (扩展) | Gameplay | Alpha | Not Started | — | Item System |
| 18 | Sport Boots Item (扩展) | Gameplay | Alpha | Not Started | — | Item System, Skiing Controller |
| 19 | Achievement & Collection (扩展) | Meta | Full Vision | Not Started | — | Scoring & Rating |

**合计**：18 个系统（15 MVP + 3 扩展）。对象池、存档等实现细节归入对应系统 GDD 或 ADR，不单独拆系统。

---

## Categories

| Category | Description | 本项目涉及 |
|----------|-------------|-----------|
| **Core** | 输入、流程、相机等基础设施 | Input, Game Flow, Camera |
| **Gameplay** | 核心玩法与关卡 | Skiing, Glucose, Items, Zones, Levels, Obstacles |
| **Progression** | 成长与评价 | Scoring & Rating |
| **Economy** | 资源与交易 | ❌ MVP 不做 |
| **Persistence** | 存档与设置 | 音量等轻量设置（并入 Menus GDD） |
| **UI** | HUD、菜单、结算 | HUD, Menus, Result |
| **Audio** | 音效与音乐 | Audio System |
| **Narrative** | 剧情与对话 | ❌ MVP 不做 |
| **Meta** | 教程、反馈、成就 | In-Level Hints, Game Feel, Achievement |

---

## Priority Tiers

| Tier | Definition | 本项目 |
|------|------------|--------|
| **MVP** | 核心循环可玩、机制转译可验证、L1 可通关 | #1–14 |
| **Vertical Slice** | 完整体验打磨（手感、音效、视觉反馈） | #15–16 |
| **Alpha** | W3 弹性道具、L3 完整双向体验 | #17–18 |
| **Full Vision** | 成就、图鉴、多人等扩展 | #19 + 概念文档 §19 |

---

## Dependency Map

### Foundation Layer（无依赖 — 最先设计）

1. **Input System** — 键鼠/手柄统一输入层，所有操作系统的入口
2. **Glucose System** — 核心生存变量；定义 5 档状态、被动变化规则、警戒/危机阈值与输出修饰符（速度倍率、操控阻尼）

### Core Layer（依赖 Foundation）

1. **Game Flow** — depends on: Input System — 主菜单 → 关卡 → 暂停 → 结算的状态机
2. **Skiing Controller** — depends on: Input System — 滑、跳、翻滚、转向；读取 Glucose 输出修饰符
3. **Obstacle & Hazard** — depends on: Skiing Controller — 碰撞检测、摔倒判定、障碍物交互
4. **Camera Follow** — depends on: Skiing Controller — 横版跟随（Cinemachine 2D 或等效方案）

### Feature Layer（依赖 Core）

1. **Item & Pickup System** — depends on: Glucose System, Obstacle & Hazard — 胰岛素喷雾、降糖药瓶、高糖雪花；拾取触发与效果应用
2. **Environmental Zones** — depends on: Glucose System, Level Data — 高血糖区/低血糖区/健康区的被动血糖与物理修饰
3. **Level Data & Progression** — depends on: Skiing Controller, Obstacle & Hazard, Environmental Zones, Item System — 手作关卡序列、终点医院降糖站、距离进度
4. **Scoring & Rating** — depends on: Glucose System, Level Data — S/A/B/C 评级、完美通关条件（血糖不失控、无摔倒、翻滚次数）

### Presentation Layer（依赖 Feature）

1. **HUD** — depends on: Glucose System, Level Data, Item System — 血糖条、距离、警戒提示、道具栏
2. **Menus & Pause** — depends on: Game Flow — 主菜单、暂停、音量设置
3. **Result Screen** — depends on: Scoring & Rating, Game Flow — 结算评分、重玩/下一关
4. **In-Level Hints** — depends on: Level Data, Glucose System — 关内情境提示（非独立教程关）

### Polish Layer

1. **Game Feel & Feedback** — depends on: Skiing Controller, Glucose System, Item System — 屏幕抖动、粒子、漫画化危机特效、数值弹出
2. **Audio System** — depends on: Game Flow, Item System, Glucose System — 跳跃/收集/警戒/失败音效；区域氛围音

---

## Recommended Design Order

| Order | System | Priority | Layer | Agent(s) | Est. Effort | Why |
|-------|--------|----------|-------|----------|-------------|-----|
| 1 | Glucose System | MVP | Foundation | game-designer, systems-designer | M | 核心生存条 — 无它则「机制转译」不成立；定义输出契约供滑行/道具/区域读取 |
| 2 | Input System | MVP | Foundation | game-designer, ux-designer | S | 成人向操作需支持键鼠+手柄；所有交互系统的入口 |
| 3 | Skiing Controller | MVP | Core | game-designer, gameplay-programmer | L | 跑酷爽感支柱 — W1/W2 最高风险，决定「好不好玩」 |
| 4 | Obstacle & Hazard | MVP | Core | game-designer, level-designer | M | 摔倒与碰撞让决策有后果；L1 关卡骨架依赖 |
| 5 | Game Flow | MVP | Core | game-designer | S | 菜单→游玩→暂停→结算的骨架；解锁后续 UI 设计 |
| 6 | Item & Pickup System | MVP | Feature | game-designer, systems-designer | M | 3 个 MVP 道具构成「血糖三角」；教育目标直接载体 |
| 7 | Level Data & Progression | MVP | Feature | level-designer, game-designer | L | 手作关卡 + 终点；L1 垂直切片载体 |
| 8 | Environmental Zones | MVP | Feature | level-designer, systems-designer | M | L2/L3 的视觉化医学隐喻；高血糖区是答辩亮点 |
| 9 | Scoring & Rating | MVP | Feature | game-designer | S | 完美通关条件强化教育 KPI；结算有成就感 |
| 10 | HUD | MVP | Presentation | ux-designer, game-designer | M | 血糖条必须显眼 — Art Bible 原则 3；成人受众需信息清晰 |
| 11 | Menus & Pause | MVP | Presentation | ux-designer | S | 主菜单 + 暂停 + 键盘导航（技术约束） |
| 12 | Result Screen | MVP | Presentation | ux-designer | S | 评分展示 + 重玩循环闭环 |
| 13 | Camera Follow | MVP | Core | game-designer | S | 可与 #3 并行实现；横版跟随是跑酷基础体验 |
| 14 | In-Level Hints | MVP | Polish | game-designer, writer | S | 关内轻提示替代独立教程关；jam 节奏友好 |
| 15 | Game Feel & Feedback | VS | Polish | game-designer, technical-artist | M | W3 打磨；危机漫画化特效对齐 Art Bible |
| 16 | Audio System | VS | Polish | sound-designer, audio-director | M | 警戒心跳音是教育反馈关键通道 |

**并行建议**：#3 Skiing Controller 与 #1 Glucose System 可两人并行，但 GDD 需先定 Glucose 输出接口。#13 Camera 可在 Skiing GDD 中作为子章节，不必单独开会。

---

## Circular Dependencies

| 关系 | 描述 | 解决方案 |
|------|------|-----------|
| Glucose ↔ Skiing Controller | 血糖影响速度/操控；滑行（运动）可能降低血糖 | **单向数据流**：Glucose System 发布 `SpeedModifier`、`ControlModifier`、`VisionModifier`；Skiing Controller 只读。运动降糖由 Item/Zone 系统通过 Glucose API 触发，不由 Controller 直接写血糖 |
| Item ↔ Glucose | 道具改变血糖；血糖状态可能限制道具使用 | Glucose 为权威状态源；Item System 通过 `ApplyGlucoseDelta()` / `ApplyBuff()` 接口写入；危机状态可定义道具冷却规则 |

无不可解循环。Glucose System 是 **瓶颈系统**（6+ 系统依赖它）。

---

## High-Risk Systems

| System | Risk Type | Risk Description | Mitigation |
|--------|-----------|-----------------|------------|
| Skiing Controller | Design | 手感不爽则整个游戏失败，与 Ski Safari 对标压力大 | W2 初 `/prototype` 验证；Cinemachine 2D；先占位 sprite 后换美术 |
| Glucose System | Design | 机制转译不直观 → 变成「读条游戏」而非「自然理解」 | 5 档分级（不用 mmol/L）；警戒/危机多通道反馈；W2 末内部试玩 + 5 题问卷 |
| Environmental Zones | Scope | L2/L3 视觉+玩法耦合，美术与关卡工作量叠加 | L1 先不含区域修饰；L2 只做高血糖区；L3 弹性 |
| Level Data & Progression | Scope | 手作关卡比程序化更可控但仍耗时 | MVP 仅 L1 必做 + L2 强推；Tilemap 或预制片段复用 |
| Item & Pickup System | Design | 3 道具平衡难 — 高糖雪花双刃剑易被滥用或忽略 | 数值在 GDD 用表格 + 试玩迭代；胰岛素作为「安全网」锚定难度 |

---

## Progress Tracker

| Metric | Count |
|--------|-------|
| Total systems identified | 18 |
| Design docs started | 3 |
| Design docs reviewed | 0 |
| Design docs approved | 0 |
| MVP systems designed | 3 / 14 |
| Vertical Slice systems designed | 0 / 2 |

---

## Next Steps

- [x] 系统枚举与依赖映射（2026-06-12）
- [x] 完成 Glucose System GDD（2026-06-13）
- [x] 完成 Input System GDD（2026-06-13）
- [ ] 完成 Skiing Controller GDD（进行中，2026-06-13）
- [ ] 审阅并批准本索引
- [ ] 对 Glucose + Skiing 尽早 `/prototype` 验证手感与机制转译
- [ ] 每份 GDD 完成后 `/design-review`
- [ ] 全部 MVP GDD 完成后 `/gate-check pre-production`
