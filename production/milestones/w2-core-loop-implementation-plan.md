# W2 核心循环原型实现计划

## 目标

在 Unity 中搭建可运行的最小核心循环：糖糖在雪坡上自动下滑，玩家通过跳跃/翻滚躲避障碍、拾取道具，血糖系统实时影响速度与控制，最终到达终点或失败。

## 原则

- **最小可玩**：占位美术、占位音效，先验证机制。
- **SO-first**：配置数据用 `ScriptableObject`，运行时状态用 MonoBehaviour。
- **事件驱动**：GlucoseSystem 发布数值/修饰器事件，其他系统订阅。
- **Input System**：所有玩家输入走新 Input System，支持键盘 + 手柄占位。

## 文件结构

```
unity-project/Assets/
├── Scripts/
│   ├── Core/                 # 跨系统通用
│   │   ├── GameEvents.cs
│   │   ├── ObservableValue.cs
│   │   └── Tags.cs
│   ├── Foundation/           # Glucose / Input
│   │   ├── GlucoseSystem.cs
│   │   ├── GlucoseConfig.cs
│   │   ├── GlucoseUI.cs
│   │   └── SugarRushInput.cs (Input System generated + wrapper)
│   ├── Gameplay/             # Skiing / Obstacle / Item
│   │   ├── SkiingController.cs
│   │   ├── SkiingConfig.cs
│   │   ├── Obstacle.cs
│   │   ├── HazardZone.cs
│   │   ├── PickupItem.cs
│   │   ├── ItemEffect.cs
│   │   ├── ItemLibrary.cs
│   │   ├── FinishLine.cs
│   │   └── CameraFollow.cs (Cinemachine 2D 占位)
│   └── GameFlow/             # GameFlow / LevelData
│       ├── GameFlowManager.cs
│       ├── LevelData.cs
│       └── LevelManager.cs
├── Data/
│   └── Configs/              # SO assets
├── Prefabs/
│   ├── Player/
│   ├── Obstacles/
│   ├── Items/
│   └── UI/
└── Scenes/
    └── L1_HospitalChalet.unity
```

## 实现顺序

1. **GlucoseSystem + Config + UI**
   - 5 档血糖状态（Safe / Low Warning / Low Crisis / High Warning / High Crisis）
   - 实时衰减、delta 应用、持续 buff
   - 发布 SpeedModifier / ControlModifier / VisionModifier / OnCrisisFailed
   - 占位血糖条 UI

2. **SkiingController + Config**
   - Rigidbody2D 物理驱动
   - 自动向右下滑（恒定基础力 + 斜坡投影）
   - 读取 GlucoseSystem 的速度/控制修饰器
   - 跳跃、翻滚、落地状态机

3. **Input System**
   - Input Actions asset：Jump、Roll、UseItem、Pause
   - 生成 C# wrapper class
   - SkiingController 订阅输入事件

4. **Obstacle / Hazard**
   - Obstacle：碰撞触发 stumble / crash（按 tag/type）
   - HazardZone：持续 passive delta

5. **Item Pickup / Usage**
   - PickupItem：触发器碰撞，调用 ItemEffect
   - 3 个 MVP ItemEffect SO
   - 道具槽 + UseItem 输入

6. **GameFlow**
   - Intro → Playing → Paused → Result（Win / Fail）
   - 处理低血糖/高血糖/致命碰撞失败、终点胜利

7. **L1 占位关卡**
   - 100m 长平台，基础跳跃/翻滚教学段
   - 终点线 + 简单障碍物/道具摆放
   - Cinemachine 2D 跟随

## 验收标准

- [ ] 按空格/手柄 A 能跳跃，按左 Shift/手柄 B 能翻滚。
- [ ] 血糖值会随时间下降，拾取道具后正确变化。
- [ ] 低血糖/高血糖持续 3 秒触发失败。
- [ ] 撞到致命障碍物触发失败。
- [ ] 到达终点触发胜利。
- [ ] 游戏状态切换正确，暂停/继续可用。

## 风险

- 包解析被 Unity Editor 占用阻塞，需手动切回 Unity 触发解析。
- 手感参数需要多次迭代；先保留大量可配置字段。
