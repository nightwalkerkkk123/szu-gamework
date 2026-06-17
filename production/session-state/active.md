# Session State — 糖大冒险：雪山狂飙 / SugarRush

> 初始化时间：2026-06-13
> 目的：注册项目配置、恢复上下文、明确下一步
> 最近更新：2026-06-17 — L1 扩内容方案实施

---

## Project

- **Game**: 糖大冒险：雪山狂飙 / SugarRush
- **ProjectConfig**: `docs/ProjectConfig.yaml` — Unity 2022.3.62f3c1, Built-in, UGUI, feature-based folders, ai_mode: guided
- **Current Phase**: Phase 1 — Pre-Production (W2 完成 → W2.5 L1 扩内容)
- **Branch**: `main` (扩内容改动在 `worktree-l1-expansion` worktree)
- **Review Mode**: lean

---

## Sprint State

**Sprint Goal**: L1 扩内容 — 5 分钟可玩时长 + 障碍类型扩充 + 血糖配置对齐 GDD

| # | Feature / Task | Status | Notes |
|---|---------------|--------|-------|
| 1-5 | W2 全部 GDD 与脚本 | ✅ Done | 见下方 "Completed This Session" |
| 6 | L1 医院雪屋 5 分钟可玩 | 🟡 In Progress | 代码已改,待 Editor 重生成场景验证 |
| 7 | Obstacle 类型扩充 (Jumpable/Rollable/Avoidable) | ✅ Done | 2026-06-17 |
| 8 | 血糖配置对齐 GDD (per-zone drift) | ✅ Done | 2026-06-17 |
| 9 | L1 7 段 2000m 重新设计 | ✅ Done | 2026-06-17,代码层 |
| 10 | Editor 场景重生成与手动验证 | ⏳ Pending | 用户需在 Editor 中运行 SugarRush/Setup/Build L1 Scene |

---

## Completed This Session (2026-06-17)

### L1 扩内容 (W2.5)
- **新 worktree**: `worktree-l1-expansion` 分支,在 `.claude/worktrees/l1-expansion/`
- **Obstacle 重构** (`unity-project/Assets/Scripts/Gameplay/Obstacle.cs`):
  - `Obstacle.ObstacleType` 从 2 值扩展为 5 值(Stumble/Crash/**Jumpable/Rollable/Avoidable**)
  - 重写 `OnCollisionEnter2D`:Jumpable 需离地、Rollable 需翻滚、Avoidable 不可豁免
  - Jumpable 高度判定:用物理公式 `h = v²/(2g)` 与 `SkiingConfig.jumpForce` 比较
- **数据扩展** (`LevelSegmentData.cs`):`SegmentObstacle` 加 `HeightMeters` 字段
- **血糖配置对齐 GDD §4.4** (`GlucoseConfig.cs` + `.asset`):
  - 旧 `passiveDecayPerSecond` 标记 `[Obsolete]`
  - 新增 5 个 per-zone 漂移字段:`driftLowCrisis=-0.2`、`driftLowWarning=-0.3`、`driftNormal=-0.5`、`driftHighWarning=0.8`、`driftHighCrisis=1.2`
  - `GlucoseConfig.asset` 更新:`passiveDecayPerSecond` 1.2 → 0.5,5 个新字段写入
  - `GlucoseSystem.cs` 加 `ResolveZoneDrift()` 私有方法,替换单值衰减
- **L1 重设计**:
  - 7 段、2000m、总可玩 5 分钟(平均 8 m/s 时 250s,慢节奏 ~5 min)
  - 12 个障碍:4 Jumpable + 5 Rollable + 2 Stumble + 1 Avoidable
  - 5 个道具:2 Insulin + 1 Pills + 2 Snowflake
  - 2 个 ColdWind 危险区(Mixed `-3/s` + FinalSprint `-2/s`)
  - Slope 节奏:8° → 12° → 12° → 12° → 10° → 16° → 8°(紧张-高潮-释放)
- **资产更新**:
  - `L1_HospitalChalet.asset`:`TargetDistanceMeters` 180→2000,`ParTimeSeconds` 60→240,`Segments` 列表清空(由 L1SceneBuilder 重写)

### 之前会话(2026-06-13)
- 项目注册初始化、`docs/ProjectConfig.yaml`
- 7 个核心 GDD(Glucose/Input/Skiing/Obstacle/Item/GameFlow/Level)
- W2 核心循环全部脚本(Core/Foundation/Gameplay/GameFlow)
- 3 个 MVP 道具实现
- Unity MCP 本地包安装

---

## In Progress

- **L1 扩内容 Editor 验证**:所有代码与资产已就位,需在 Unity Editor 2022.3 中:
  1. 切换到 `worktree-l1-expansion` 分支(已在 `E:\CODE\MyGame\.claude\worktrees\l1-expansion`)
  2. 打开 `unity-project/`
  3. 运行菜单 `SugarRush > Setup > Build L1 Scene`
  4. 打开 `L1_HospitalChalet.unity` 进入 Play 模式验证
  5. 完成 plan 验证清单(7 项)

---

## Pending Decisions

- [x] Skiing Controller 物理 vs transform? — **Rigidbody2D** ✅
- [ ] L2 3D 融合场景如何与 L1 共存?(`L2_Fusion.unity` 与 `L2_Fusion_Ours.unity` 双场景策略待定)
- [ ] L3 低血糖区是否纳入 W2 还是 W3?
- [ ] 速度 buff 重构(HighSugarSnowflake 直接改 velocity 是 hack,需 `SkiingController.ApplySpeedMultiplierBonus` 接口)— 列入下轮
- [ ] 多槽位 `PlayerInventory` 重构 — 列入下轮
- [ ] 单元测试体系搭建 — 用户决定本轮不做

---

## Key Config Changes Since Last Session

### L1 扩内容(本轮)
- `Obstacle.cs`:新增 3 个 enum 值、Jumpable 高度判定、ApplyHitFeedback 抽取
- `LevelSegmentData.cs`:`SegmentObstacle.HeightMeters` 字段
- `GlucoseConfig.cs`:5 个 per-zone drift 字段、被动衰减 obsolete
- `GlucoseConfig.asset`:passiveDecay 1.2→0.5、5 个新字段
- `GlucoseSystem.cs`:ResolveZoneDrift() 方法
- `L1SceneBuilder.cs`:EnsureSegments 重写为 7 段
- `L1_HospitalChalet.asset`:TargetDistance 180→2000、ParTime 60→240、Segments 清空

### W2(之前)
- 新增 `docs/ProjectConfig.yaml`
- `ai_mode` = `guided`
- 选定 `Built-in` 渲染、`UGUI`、`Cinemachine`、`Input System`
- `Packages/manifest.json`:Input System 1.11、Cinemachine 2.10、TextMeshPro 3.0.9、Unity MCP 本地包
- `ProjectSettings.asset`:activeInputHandler = 2

---

## Active TODOs in Code

```
// TODO(playtest): align glucose zone boundaries to GDD 35/85/95 (lowWarning 30→35, highWarning 75→85)
// TODO(speed-buff): refactor HighSugarSnowflakeEffect to call SkiingController.ApplySpeedMultiplierBonus instead of mutating Rigidbody2D.velocity
// TODO(inventory): expand PlayerInventory to 3 slots per GDD
// TODO(mcp): verify Unity MCP local package resolves on next Editor open
// TODO(verify): run SugarRush/Setup/Build L1 Scene in Editor to regenerate L1_HospitalChalet.unity
// TODO(verify): playtest L1 5 minutes — confirm 12 obstacles + 5 pickups + 2 hazards all spawn correctly
```

---

## Notes for Next Session

- L1 扩内容所有代码/资产改动都在 `worktree-l1-expansion` 分支;验证通过后合并到 `main`
- 完整方案见 `C:\Users\王子豪\.claude\plans\shimmering-swimming-whale.md`
- 场景重生成是 idempotent 的(`L1SceneBuilder` 每次都 delete + recreate segments),可重复运行
- `L1_HospitalChalet.asset` 当前 `Segments` 列表为空,Editor 第一次跑 `Build L1 Scene` 后会写回 7 个新 GUID 引用
- 之前活跃的 L2 融合分支(Entitas ECS)未受影响,仍在 `main` 领先 5 个 commit
- 远程推送因网络中断尚未执行,本轮 L1 扩内容完成后可一并推送

