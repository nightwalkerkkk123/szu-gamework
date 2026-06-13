# W2 L1 占位关卡搭建指南

## 前提

1. 切回 Unity Editor，让 Package Manager 自动解析 `manifest.json` 中的新包。
2. 确认 Console 无编译错误。

## 场景搭建步骤

### 1. 创建场景

- `File > New Scene`
- 保存为 `Assets/Scenes/L1_HospitalChalet.unity`

### 2. 创建 Config Assets

- `Assets > Create > SugarRush > Glucose Config` → 保存为 `Assets/Data/Configs/GlucoseConfig.asset`
- `Assets > Create > SugarRush > Skiing Config` → 保存为 `Assets/Data/Configs/SkiingConfig.asset`
- `Assets > Create > SugarRush > Level Data` → 保存为 `Assets/Data/Configs/L1_HospitalChalet.asset`
  - `Target Distance Meters`: `100`
  - `Par Time Seconds`: `75`

### 3. 玩家对象

创建空对象 `Player`，标签设为 `Player`，添加以下组件：

- `Transform`: Position `(0, 1, 0)`
- `Rigidbody2D`: Gravity Scale `2.5`, Freeze Rotation `Z`
- `BoxCollider2D`: Size `(0.8, 1.6)`
- `Sprite Renderer`: 临时用白色方块或占位 sprite
- `GlucoseSystem`: 拖拽 `GlucoseConfig`
- `SkiingController`: 拖拽 `GlucoseConfig`, `SugarRushInput`, `GlucoseSystem`
- `PlayerInventory`: 拖拽 `GlucoseSystem`, `SkiingController`, `SugarRushInput`

创建空子对象 `GroundCheck`，放到玩家脚底，用于地面检测可视化（可选）。

### 4. 输入

- 创建空对象 `Input`
- 添加 `SugarRushInput` 组件
- 将 `Assets/Data/SugarRush.inputactions` 拖到 `Input Asset`

### 5. 摄像机

- 主摄像机 `MainCamera` 保持默认
- 创建 `Cinemachine > 2D Camera`
- 添加 `CameraFollow` 组件
- `Target`: `Player`
- `Virtual Camera`: 刚创建的 CinemachineVirtualCamera

### 6. 地面 / 平台

- 创建多个 `Quad` 或 `Sprite` 对象，加 `BoxCollider2D`，Layer 设为 `Ground`
- 摆放约 100m 长的下坡路线
- 建议段落：
  1. 起始平地（0–10m）
  2. 教学跳跃段（10–30m）
  3. 教学翻滚段（30–50m）
  4. 道具段（50–70m）
  5. 综合段（70–90m）
  6. 终点冲刺（90–100m）

### 7. 障碍物

创建空对象 `Obstacles`，下面创建子对象：

- `StumbleRock`: 加 `BoxCollider2D`, `Obstacle` 组件，Type `Stumble`
- `CrashTree`: 加 `BoxCollider2D`, `Obstacle` 组件，Type `Crash`

复制若干摆放到跳跃/翻滚教学段后方。

### 8. 道具

创建空对象 `Items`，下面创建子对象：

- 创建 3 个 ItemEffect assets：
  - `Assets/Data/Items/InsulinSpray.asset`
  - `Assets/Data/Items/HypoglycemicPills.asset`
  - `Assets/Data/Items/HighSugarSnowflake.asset`
- 每个道具Pickup创建空对象，加 `CircleCollider2D` (Is Trigger), `PickupItem` 组件，拖拽对应 ItemEffect

### 9. Hazard 区（可选）

- 创建空对象 `HazardZone`，加 `BoxCollider2D` (Is Trigger), `HazardZone` 组件
- 拖到寒冷/强风路段

### 10. 终点线

- 创建空对象 `FinishLine`，加 `BoxCollider2D` (Is Trigger), `FinishLine` 组件
- 放到 x ≈ 100 位置

### 11. GameFlow

- 创建空对象 `GameFlow`
- 添加 `GameFlowManager`: 拖拽 `SugarRushInput`, `Player`, `Player/GlucoseSystem`
- 添加 `LevelManager`: 拖拽 `L1_HospitalChalet` asset, `Player`, 起点空对象 `StartPoint`

### 12. UI

- 创建 `Canvas` (Screen Space - Overlay)
- 创建 `Slider`，删除 Handle Slide Area
- 添加 `GlucoseUI` 组件，拖拽 `Player/GlucoseSystem`
- 可添加 TMP Text 显示距离、时间（可选）

## 验证清单

- [ ] 按空格跳跃
- [ ] 按左 Shift 翻滚
- [ ] 撞到 StumbleRock 会减速
- [ ] 撞到 CrashTree 会停止并失败
- [ ] 血糖持续下降，到低血糖区 3 秒后失败
- [ ] 拾取道具后按 E 使用
- [ ] 到达 FinishLine 触发胜利
- [ ] 按 ESC 暂停/继续
