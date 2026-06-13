# Level Data & Progression — 关卡数据与进度系统

> **Status**: Draft  
> **Author**: level-designer, game-designer  
> **Last Updated**: 2026-06-13  
> **Last Verified**: 2026-06-13  
> **Implements Pillar**: 爽感 (Feel) + 教育 (Education)  
> **Source**: `design/gdd/game-concept.md` §8, §9

## Summary

Level Data & Progression 负责 SugarRush 中所有手作关卡的定义、加载、拼接和进度跟踪。MVP 聚焦 **L1 医院雪屋**，通过数据驱动的方式配置关卡长度、障碍布局、道具放置、终点位置和教学节奏。系统采用「关卡片段（Chunk）」机制，既保留手作关卡的精确控制，又支持片段复用降低制作成本。

> **Quick reference** — Layer: `Feature` · Priority: `MVP` · Key deps: `Skiing Controller`, `Obstacle & Hazard`, `Item & Pickup`, `Environmental Zones`, `Game Flow`

## Overview

每个关卡由一系列按顺序排列的 **Chunk** 组成：

- **Chunk**：一段 10–30 米长的关卡片段，包含地形、障碍、道具、区域修饰和可选的提示点。
- **Level**：一个 ScriptableObject，定义关卡使用的 Chunk 序列、总长度、终点位置、背景音乐、初始血糖值。
- **终点（Hospital Station）**：触发 `Victory` 的特殊区域。

MVP 只做 L1；L2/L3 在 W3 扩展，但数据结构预留。

## Player Fantasy

玩家应该感觉自己在**穿越一段有起伏、有节奏的雪山路**：

- 开头轻松，熟悉操作。
- 中段出现第一个「必须翻滚」的低栅栏和「必须跳跃」的石头。
- 后段节奏加快，道具和障碍交错，需要连续决策。
- 最后看到医院降糖站的绿色十字，冲刺抵达。

## Detailed Design

### Core Rules

1. **手作关卡，非程序化生成**：MVP 每个 Chunk 由关卡设计师手动摆放，确保节奏可控。
2. **Chunk 复用**：同一段 Chunk 可在同一关卡内或不同关卡间复用，降低工作量。
3. **数据驱动**：所有 Chunk、障碍、道具、区域均通过 ScriptableObject / Prefab 配置。
4. **距离进度**：关卡内有 `Progress` 值（0.0–1.0），用于 HUD 显示和评分。
5. **终点触发**：玩家进入终点触发器即触发 `Victory`，通知 Game Flow。
6. **出生点安全**：玩家出生位置及周围 5 米内不得有障碍或 Hazard。

### LevelData Schema

```csharp
[CreateAssetMenu(fileName = "LevelData", menuName = "SugarRush/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelId;              // "L1_Hospital_SnowHouse"
    public string displayName;          // "医院雪屋"
    public string description;          // 关卡简介
    public Sprite thumbnail;
    public AudioClip bgm;
    
    [Header("Chunks")]
    public List<ChunkData> chunks;      // 按顺序排列的片段
    public float totalLength;           // 总长度（米），由 chunks 累加或覆盖
    
    [Header("Glucose")]
    public float startGlucose = 50f;    // 初始血糖
    
    [Header("Progression")]
    public string nextLevelId;          // 下一关 ID（MVP 为空）
    public bool lockedByDefault = false;
}
```

### ChunkData Schema

```csharp
[CreateAssetMenu(fileName = "ChunkData", menuName = "SugarRush/ChunkData")]
public class ChunkData : ScriptableObject
{
    public string chunkId;
    public float length = 20f;          // 片段长度（米）
    public GameObject chunkPrefab;      // 包含地形、装饰的预制体
    
    [Header("Spawns")]
    public List<ObstacleSpawn> obstacles;
    public List<ItemSpawn> items;
    public List<HazardSpawn> hazards;
    public List<ZoneSpawn> zones;
    public List<HintTrigger> hints;
}

[System.Serializable]
public class ObstacleSpawn
{
    public ObstacleData obstacleData;
    public float positionX;             // 在 chunk 内的水平位置
    public float positionY;             // 垂直位置
}

[System.Serializable]
public class ItemSpawn
{
    public ItemData itemData;
    public float positionX;
    public float positionY;
}
```

### L1 医院雪屋 — MVP 设计

**目标**：让玩家在 60–90 秒内熟悉跳跃、翻滚、道具使用，并理解「血糖 = 生存条」。

| Segment | 距离 | 内容 | 教学点 |
|---------|------|------|--------|
| **Intro** | 0–15m | 平坦雪地，无障碍，轻微下坡 | 熟悉自动前滑和转向 |
| **First Jump** | 15–25m | 1 个 Jumpable 雪石 | 学习跳跃 |
| **First Roll** | 25–40m | 2 个 Rollable 冰栅栏 | 学习翻滚 |
| **Item Tutorial** | 40–55m | 1 个胰岛素喷雾 + 1 个高糖雪花 | 学习道具使用 |
| **Mixed Section** | 55–80m | 跳跃/翻滚交替，1 个降糖药瓶 | 综合操作 |
| **Final Sprint** | 80–100m | 障碍密度增加，终点医院可见 | 紧张冲刺 |
| **Hospital Station** | 100m | 终点触发器 | 抵达胜利 |

**L1 参数起点**：

| Parameter | Value |
|-----------|-------|
| 总长度 | 100 m |
| 预计通关时间 | 60–90 s |
| 初始血糖 | 50 |
| 胰岛素喷雾 | 2 个 |
| 降糖药瓶 | 1 个 |
| 高糖雪花 | 2 个 |
| Jumpable 障碍 | 4 个 |
| Rollable 障碍 | 5 个 |
| Avoidable 障碍 | 1 个（后段可选） |
| Hazard Zone | 0 个（L1 不含区域修饰） |

> 注：以上数量为起点，原型验证后调整。

### Progress Tracking

```
progress = playerPositionX / totalLength
```

- `progress` 范围 0.0–1.0。
- HUD 显示为进度条或距离剩余。
- 评分系统读取 `progress` 判断是否正常通关。

### Finish Line / Hospital Station

- 物理表现：一个带有绿色十字的大型触发器区域，视觉上为医院雪屋建筑。
- 逻辑：玩家进入触发器后，Game Flow 进入 `Victory` 状态。
- 安全区：终点前 10 米内无致命障碍，允许玩家冲刺进入。

### States and Transitions

| State | Entry Condition | Exit Condition | Behavior |
|-------|----------------|----------------|----------|
| **Unloaded** | 关卡未加载 | GameFlow 调用 LoadLevel | 准备资源 |
| **Loading** | 开始加载 | 所有 Chunk 实例化完成 | 显示加载提示 |
| **Ready** | 加载完成 | GameFlow 进入 Playing | 玩家出生点就位 |
| **In Progress** | Playing 状态 | 玩家到达终点或失败 | 更新进度 |
| **Completed** | 玩家到达终点 | 进入 Result | 停止跟踪 |

### Interactions with Other Systems

| System | Interface | Data Flow |
|--------|-----------|-----------|
| **Game Flow** | `LoadLevel(levelId)`, `UnloadCurrentLevel()` | GameFlow → Level |
| **Skiing Controller** | 读取玩家 `positionX` | 计算进度 |
| **Obstacle & Hazard** | Chunk 中放置 `ObstacleData` / Hazard | Level → Obstacle |
| **Item & Pickup System** | Chunk 中放置 `ItemData` | Level → Item |
| **Environmental Zones** | Chunk 中放置 `ZoneData` | Level → Zones |
| **HUD** | 发送 `OnProgressUpdated` | 进度条显示 |
| **Scoring & Rating** | 提供 `LevelRunData`（进度、时间、血糖历史） | 评分计算 |
| **Camera Follow** | 提供关卡边界 | 相机约束 |

## Edge Cases

| Scenario | Expected Behavior | Rationale |
|----------|------------------|-----------|
| 玩家速度过快冲出终点后段 | 终点触发器有足够深度；超过总长度视为胜利 | 防止冲过头不触发 |
| 玩家在终点前失败 | 正常进入 GameOver，不触发 Victory | 状态清晰 |
| 关卡加载时缺失 prefab | 记录错误，跳过该对象，继续加载 | 健壮性 |
| 玩家倒退到关卡起点之前 | 限制玩家位置 x ≥ 0 | 防止跑出世界 |
| Chunk 复用导致道具重复刷新 | 每次重玩重新实例化 Chunk | 每次游戏体验一致 |
| 关卡数据 totalLength 与 chunks 累加不符 | 以 chunks 累加为准，LogWarning | 数据一致性 |

## Dependencies

| System | Direction | Nature of Dependency |
|--------|-----------|---------------------|
| Game Flow | Level depends on GameFlow | 加载/卸载调用 |
| Skiing Controller | Level reads Skiing | 进度计算 |
| Obstacle & Hazard | Level places Obstacles | 数据配置 |
| Item & Pickup System | Level places Items | 数据配置 |
| Environmental Zones | Level places Zones | 数据配置 |
| HUD | Level → HUD | 进度显示 |
| Scoring & Rating | Scoring depends on Level | 通关数据 |
| Camera Follow | Camera depends on Level | 边界约束 |

## Tuning Knobs

| Parameter | Current Value | Safe Range | Effect of Increase | Effect of Decrease |
|-----------|--------------|------------|-------------------|-------------------|
| `L1 totalLength` | 100 m | 60 ~ 200 | 关卡更长 | 更短 |
| `L1 startGlucose` | 50 | 30 ~ 70 | 初始更安全 | 更紧张 |
| `chunkLength` | 20 m | 10 ~ 50 | 片段更长、节奏更慢 | 更细碎 |
| `finishZoneDepth` | 5 m | 3 ~ 10 | 终点容错更大 | 更小 |
| `safeSpawnRadius` | 5 m | 3 ~ 10 | 出生更安全 | 更紧凑 |

## Visual/Audio Requirements

| Event | Visual Feedback | Audio Feedback | Priority |
|-------|----------------|---------------|----------|
| 关卡加载 | 加载进度条 / 雪山小动画 | 环境风声 | MVP |
| 关卡开始 | 3s 倒计时或「出发！」漫画框 | 出发哨音 | MVP |
| 进度更新 | HUD 距离条推进 | 无 | MVP |
| 终点可见 | 医院绿色十字发光 | BGM 渐强 | MVP |
| 抵达终点 | 医院大门打开；糖糖冲入 | 胜利旋律 | MVP |
| 新区域进入 | 屏幕色调微调（L2/L3） | 区域氛围音 | W3 |

## UI Requirements

| Information | Display Location | Update Frequency | Condition |
|-------------|-----------------|-----------------|-----------|
| 关卡名称 | 关卡开始动画顶部 | 关卡加载时 | LevelIntro |
| 距离进度 | HUD 顶部/底部进度条 | 每帧 | Playing |
| 剩余距离 | 进度条旁数字（可选） | 每帧 | Playing |
| 终点提示 | 屏幕右侧箭头/医院图标 | 终点进入视野时 | 最后 20m |

## Cross-References

| This Document References | Target GDD | Specific Element Referenced | Nature |
|--------------------------|-----------|----------------------------|--------|
| 关卡加载/胜利/失败 | `design/gdd/game-flow.md` | `LoadLevel`, `Victory`, `GameOver` | Data dependency |
| 障碍配置 | `design/gdd/obstacle-hazard-system.md` | `ObstacleData`, `ObstacleSpawn` | Data dependency |
| 道具配置 | `design/gdd/item-pickup-system.md` | `ItemData`, `ItemSpawn` | Data dependency |
| 区域配置 | `design/gdd/environmental-zones.md` | `ZoneData`, `ZoneSpawn` | Data dependency |
| 进度显示 | `design/gdd/hud.md` | `OnProgressUpdated` | Data dependency |
| 评分数据 | `design/gdd/scoring-rating.md` | `LevelRunData` | Data dependency |

## Acceptance Criteria

- [ ] 存在 `LevelData` 和 `ChunkData` ScriptableObject 定义 L1。
- [ ] L1 总长度约 100m，预计通关时间 60–90s。
- [ ] L1 包含跳跃、翻滚、道具使用的教学节奏。
- [ ] 玩家抵达终点触发 `Victory`。
- [ ] 出生点周围 5m 内无障碍和 Hazard。
- [ ] 关卡数据可配置障碍、道具、区域、提示的生成位置。
- [ ] 进度条实时更新，准确反映玩家位置。
- [ ] 关卡重开时正确重置所有对象和玩家位置。
- [ ] MVP 只做 L1，但数据结构支持 L2/L3 扩展。

## Open Questions

| Question | Owner | Deadline | Resolution |
|----------|-------|----------|-----------|
| L1 是否加入 1 个简单 Hazard Zone 作为预告？ | level-designer | W2 中 | 当前 GDD 建议 L1 不加，降低复杂度 |
| Chunk 复用是否需要随机化障碍位置？ | level-designer | W2 末 | MVP 固定；扩展可随机 |
| 终点医院是否需要一段短动画？ | game-designer | W2 中 | VS 阶段加美术，MVP 先触发胜利 |
| L2 高血糖区是否使用独立 Tilemap 或材质覆盖？ | level-designer | W3 | 待 Environmental Zones GDD |
