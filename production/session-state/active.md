# Session State — 糖大冒险：雪山狂飙 / SugarRush

> 初始化时间：2026-06-13
> 目的：注册项目配置、恢复上下文、明确下一步

---

## Project

- **Game**: 糖大冒险：雪山狂飙 / SugarRush
- **ProjectConfig**: `docs/ProjectConfig.yaml` — Unity 2022.3.62f3c1, Built-in, UGUI, feature-based folders, ai_mode: guided
- **Current Phase**: Phase 1 — Pre-Production (W2 进行中)
- **Branch**: `main`
- **Review Mode**: lean

---

## Sprint State

**Sprint Goal**: W2 — 完成核心循环原型，验证滑行手感与血糖机制转译

| # | Feature / Task | Status | Notes |
|---|---------------|--------|-------|
| 1 | 创建 ProjectConfig.yaml | ✅ Done | 2026-06-13 初始化 |
| 2 | Glucose System GDD | ✅ Done | 2026-06-13 |
| 3 | Input System GDD | ✅ Done | 2026-06-13 |
| 4 | Skiing Controller GDD | ✅ Done | W2 最高风险，决定手感 |
| 5 | 3 个 MVP 道具实现 | ✅ Done | 脚本与 SO 结构完成 |
| 6 | L1 医院雪屋可玩 | 🟡 In Progress | 脚本完成，待按搭建指南在 Editor 中拼场景 |

---

## Completed This Session

- 项目注册初始化 — 创建 `docs/ProjectConfig.yaml`
- 更新会话状态 — 明确当前 Phase、Sprint Goal、任务列表
- 完成 Glucose System GDD 与 Input System GDD
- 合并 `docs/w2-gdds` → `main`
- 完成 Skiing Controller GDD
- 完成 Obstacle & Hazard GDD
- 完成 Item & Pickup System GDD
- 完成 Game Flow GDD
- 完成 Level Data & Progression GDD
- 完成 W2 核心 GDD 设计审阅（Lean）
- 修复审阅发现的关键问题：
  - `OnCrisisFailed` 事件在 Glucose System GDD 中显式定义
  - 高糖雪花速度加成明确为独立临时 bonus（与 Glucose modifier 相乘）
- 更新 `Packages/manifest.json`：新增 Input System 1.11.0、Cinemachine 2.10.0；升级 TextMeshPro 3.0.9
- Unity Editor 检测到 manifest 变更并自动解析完成：`packages-lock.json` 与 `ProjectSettings.asset` 已更新
- 完成 W2 核心循环原型全部脚本：
  - Core: `GameEvents`, `ObservableValue`
  - Foundation: `GlucoseConfig`, `GlucoseSystem`, `GlucoseUI`, `SugarRushInput`, `SugarRush.inputactions`
  - Gameplay: `SkiingConfig`, `SkiingController`, `CameraFollow`, `Obstacle`, `HazardZone`, `FinishLine`, `ItemEffect`, `PickupItem`, `PlayerInventory`, 3 个 MVP item effects
  - GameFlow: `GameFlowManager`, `LevelData`, `LevelManager`
- 编写 W2 实现计划与 L1 占位关卡搭建指南
- 解决 Unity MCP 包安装问题：
  - 发现 GitHub 直链下载反复失败
  - 改用本地已 clone 的 `E:/CODE/unity-mcp`
  - 更新 `manifest.json` 与 `packages-lock.json` 指向本地 `file:../../../unity-mcp/MCPForUnity`

---

## In Progress

- **Unity MCP 包解析与连接** — 已改成本地路径，等待切回 Unity Editor 后自动解析

---

## Pending Decisions

- [x] Skiing Controller 是否用 Rigidbody2D 物理驱动，还是纯 transform 插值？—— **本 GDD 推荐 Rigidbody2D**，待原型验证
- [ ] 是否启用 Unity MCP / GitHub MCP？（当前正在安装 Unity MCP）
- [ ] 是否使用 DOTween / PrimeTween 等 tweening 中间件？（当前 none）
- [ ] 文件夹策略是否保持 feature-based？现有 `Assets/` 下为 type-based 结构
- [ ] L3 低血糖区是否纳入 W2 还是 W3？

---

## Key Config Changes Since Last Session

- 新增 `docs/ProjectConfig.yaml`
- `ai_mode` 设为 `guided`
- 选定 `Built-in` 渲染管线、`UGUI`、`Cinemachine`、`Input System`
- `Packages/manifest.json` 更新：新增 Input System / Cinemachine，升级 TextMeshPro
- `ProjectSettings.asset` 中 `activeInputHandler` 改为 `2`（Input System package）
- 新增 `com.coplaydev.unity-mcp` 本地包引用：`file:../../../unity-mcp/MCPForUnity`

---

## Active TODOs in Code

```
// TODO: 切回 Unity Editor 让 Package Manager 解析本地 MCP 包
// TODO: 按 w2-l1-scene-setup.md 在 Unity Editor 中搭建 L1_HospitalChalet.unity
// TODO: 创建 GlucoseConfig / SkiingConfig / LevelData / 3 ItemEffect SO assets
// TODO: 验证跳跃、翻滚、障碍、道具、终点、暂停流程
// TODO: 调整滑雪手感参数（baseAcceleration, maxSpeed, jumpForce, slopeAngle）
```

---

## Notes for Next Session

- 项目已完成 W1 概念与系统拆分（`design/gdd/systems-index.md` 含 18 系统）
- 已完成 Foundation 层：Glucose System、Input System
- 已完成 Core 层：Skiing Controller、Obstacle & Hazard
- 已完成 Feature 层：Item & Pickup System
- 核心循环原型代码已全部提交到本地 `main`
- 远程推送因网络中断失败，已尝试多次；待网络恢复后可再次 `git push origin main`
- Unity MCP 已改成本地包路径，解析后即可通过 MCP 自动化场景搭建
- 本地 MCP 仓库位置：`E:/CODE/unity-mcp`（在工程外，其他机器需自行准备）
