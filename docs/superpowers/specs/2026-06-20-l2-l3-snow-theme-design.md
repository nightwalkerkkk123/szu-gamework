# L2 / L3 雪山主题化设计方案

> 日期：2026-06-20
> 游戏：糖大冒险·雪山狂飙 / SugarRush
> 状态：设计已批准，待写实现计划

## 1. Overview（概述）

把两个 3D 跑酷原型场景各自打磨成正式关卡 L2、L3。当前两个场景都停在"逻辑能跑通"的
原型阶段：纯白虚空、无 3D 材质、无环境美术、与"雪山·血糖"主题脱节。本轮**不改操控**
（保留只跳），集中解决"和主题没关系"——补齐雪山美术氛围与血糖主题统一。

## 2. 决策记录（已与用户确认）

| 决策点 | 结论 | 理由 |
|--------|------|------|
| 操控方向 | **保留只跳**，本轮不加横向操控 | 优先补更大的缺口（视觉/主题脱节）；操控改善留下一轮 |
| 关卡分配 | **L2 = `L2_Fusion_Ours`**、**L3 = `L2_Fusion`** | Ours 已接血糖最近主题；Entitas 更复杂作高难高潮，难度 L1<L2<L3 顺势 |
| 美术程度 | **程序氛围包**（无新资产） | jam 节奏最现实，几何体调色即可出雪山感 |
| L3 血糖桥接 | **本轮一起做** | 用户确认 |

### 已知遗留张力
- 保留"只跳"操控后，玩家可能仍感"在看不在玩"。本轮接受此风险，操控改善列入下一轮。

## 3. 现状（代码事实）

- **L2 = `Assets/Scenes/L2_Fusion_Ours.unity`**：自研 MonoBehaviour 3D 跑酷。
  - `SpherePlayerController`（自动前冲 + 只跳，Z 轴锁死）、`ChunkSpawner3D`、`FollowCamera3D`、
    `GapDeathZone3D`、`Obstacle3D`
  - **已集成** `GlucoseSystem` + `GlucoseSystem3DAdapter`（按血糖区给球染色）+ `GlucoseUI` + `HUD` + `GameFlowManager` + `ResultPanel`
  - `SimpleParticleSpawner` 报警："Particle sprite not assigned"
- **L3 = `Assets/Scenes/L2_Fusion.unity`**：第三方 **Entitas ECS** EndlessRunner（`Assets/ThirdParty/EndlessRunner/`）。
  - `GameController`（ECS 驱动）、`CameraChaser`/`CameraView`、`DeathHeightListener`
  - 平台颜色来自 `GameController.Configure()` 第 68 行 `SetTypeColorTable`（蓝/橙/绿）
  - 调试覆盖层 = `GameController.OnGUI()`，受 `#if !ENTITAS_DISABLE_VISUAL_DEBUGGING` 守卫
  - **无血糖、无 HUD**
- **美术现状**：`Assets/Materials/` 有 0 个 `.mat`；`Assets/Art/Textures/` 仅 14 张 2D sprite
  （含 `SnowGround.png`、`Mountain_BG.png`、`Snowflake.png`，均为 L1 2D 用）。

## 4. 设计：三个模块

### 模块 A — 共享「程序氛围包」（一份资产，两关复用）

可复用的场景氛围设置，根治"白底虚空"：

1. **渐变天空盒**：程序化 skybox 材质（天顶冷白 `#EAF2FA` → 地平线淡蓝 `#9FC2E0`），
   赋给两个场景的 `RenderSettings.skybox` / Lighting。
2. **远景雾**：`RenderSettings.fog = true`，线性雾，冷蓝白色，start/end 调到恰好藏住
   `ChunkSpawner3D` 的生成边界（一石二鸟：氛围纵深 + 掩盖 pop-in）。
3. **方向光**：低角度冬日斜阳，冷色温（~6500K 偏冷），柔和阴影。
4. **材质库**（新建于 `Assets/Materials/`）：
   - `SnowWhite`（雪地）、`IceBlue`（冰面/平台）、`CandyPink`（玩家糖球）、`HazardRed`（危险/障碍）
   - Built-in Standard shader，纯色 + 适度 Smoothness。
5. **飘雪粒子**：给 `SimpleParticleSpawner` 赋 `Snowflake.png`，跟随相机持续飘雪。

> 落地形式：氛围包做成一个可复用的设置（Prefab "AtmosphereRig" 或一个 Editor 菜单
> `SugarRush/Setup/Apply Snow Atmosphere`，对当前场景一键套用 skybox/fog/light/snow）。
> 具体形式在实现计划阶段定。

### 模块 B — L2 = `L2_Fusion_Ours`（球体跑酷，已有血糖）

- 套用氛围包（A）。
- 地形 chunk → `SnowWhite`；玩家球 → `CandyPink`（与 `GlucoseSystem3DAdapter` 染色兼容：
  适配器在 `Update` 里覆盖 `material.color`，基础材质提供质感）。
- `Obstacle3D` → 冰块/拐杖糖外观（材质 `IceBlue`/`HazardRed`）；拾取物复用 L1 词汇
  （胰岛素 / 糖丸 / 雪花）。
- 定位**3D 入门关**：节奏平缓，教会玩家"3D 里也要管血糖"。
- 验证白底消失（skybox + fog 生效）。

### 模块 C — L3 = `L2_Fusion`（Entitas ECS，高潮高难）

- 套用氛围包（A）。
- ECS 平台 → 冰雪色：改 `GameController.Configure()` 第 68 行 `SetTypeColorTable` 为冰蓝/雪白系。
- **移除调试覆盖层**：在 Player Settings 加编译宏 `ENTITAS_DISABLE_VISUAL_DEBUGGING`
  （干净、不动第三方源码）。
- 🔴 **血糖桥接**（本模块硬骨头）：
  - 复用 `GlucoseSystem`（MonoBehaviour）挂到场景管理对象。
  - 新增 `GlucoseEntitasBridge`：每帧读取 Entitas 玩家状态（位置/速度/存活），驱动血糖
    漂移；血糖 crisis → 触发 Entitas 死亡路径（`KillPlayerService` 或既有死亡入口）。
  - 给 Entitas 玩家球的 `Renderer` 挂 `GlucoseSystem3DAdapter`（视图无关，可直接复用染色）。
  - 从 L2 场景复制 UGUI HUD Canvas（`GlucoseUI` + `HUD`），绑定到同一 `GlucoseSystem`。
- 难度爬升：更快、更多断崖（复用 `DeathHeightListener` / `SetDeathHeight`）、平台更窄。

## 5. Dependencies（依赖）

- `GlucoseSystem` / `GlucoseSystem3DAdapter` / `GlucoseUI` / `HUD`（已存在，L3 复用）
- Entitas EndlessRunner 框架（L3，仅加宏 + 改配置色 + 外挂桥，不改其核心源码）
- 现有 sprite `Snowflake.png`（粒子）

## 6. Out of Scope（本轮不做）

- 横向操控 / 变道 / 翻滚（操控保留只跳，列入下一轮）
- 新 3D 贴图生成（T2I）；本轮纯程序氛围 + 纯色材质
- L1 既有内容改动
- 关卡间流程串联（L1→L2→L3 切换）的新建（若已有则复用，不新设计）

## 7. Acceptance Criteria（验收标准）

1. L2、L3 进入 Play 后**不再是白底虚空**：有天空盒、雾、雪地材质、飘雪。
2. L2 玩家球随血糖区变色（既有功能不回归），HUD 正常显示血糖/速度/距离。
3. L3 **不再显示** ECS 调试文字覆盖层。
4. L3 出现血糖 HUD，且血糖随游玩漂移、crisis 会导致失败。
5. L3 平台为冰雪色，整体难度高于 L2（更快 / 更多断崖）。
6. 两关 `read_console` 无新增 error；`SimpleParticleSpawner` 不再报 sprite 未赋值。
