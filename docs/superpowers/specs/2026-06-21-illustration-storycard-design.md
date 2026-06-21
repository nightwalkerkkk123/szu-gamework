# 插画接入设计方案：StoryCard 叙事系统 + UI 背景

> 日期：2026-06-21
> 游戏：糖大冒险·雪山狂飙 / SugarRush
> 状态：设计已批准，待写实现计划

## 1. Overview（概述）

把一批现成的绘本式插画（12 张，主题完全契合"血糖控制·雪山跑酷"）接入游戏，
用于两件事：**关卡过渡叙事**与**UI 背景补充**。当前游戏场景切换是
`SceneManager.LoadScene()` 瞬切，无任何过渡、无叙事插页、UI 场景背景单调。

本方案产出两个**解耦子系统**，只共享同一批导入的 Sprite 资源：

1. **StoryCard 叙事覆盖层** —— 全屏故事卡，手动翻页，ESC 跳过，看过一次后不再自动弹（PlayerPrefs 持久化）。
2. **静态 UI 背景** —— 把插画铺到三个纯 UI 场景/面板作背景。

## 2. 决策记录（已与用户确认）

| 决策点 | 结论 | 理由 |
|--------|------|------|
| 过渡呈现形式 | **全屏故事卡·手动翻页**（Space/点击翻页） | 插画自带文案与编号，本就是"漫画页"，玩家自控阅读节奏 |
| 故事分配 | **序章集中 + 各关单页** | 教学/世界观集中在序章不重复；每关开场一张专属图 |
| 序章触发 | **只看一次 + ESC 可跳过** | 体验最好；用 PlayerPrefs 记"已看"，死亡重玩不重播 |
| 背景范围 | **仅 3 个纯 UI 处**：StartScene 封面、LevelSelect 装饰、ResultPanel 结算 | 不碰 L1/L2 关卡内渲染，避免与现有雪景/3D 冲突 |
| 失败结算图 | **红暴风雪图**（`720X720.PNG`） | 与胜利"冲过关了"形成红/暖对比 |
| 命名空间 | `SugarRush.Foundation.StoryCard` | 与现有 Foundation 层（SceneSwitcher/ResultPanel）一致 |

## 3. 素材清单与映射（代码事实）

源目录：`~/Desktop/用所选项目新建的文件夹/`（12 个文件），复制进
`unity-project/Assets/Art/Illustrations/`，导入为 Sprite（Texture Type =
Sprite 2D and UI，Max Size 2048）。

| 编号 | 源文件 | 内容 | 接入位置 |
|------|--------|------|---------|
| ① | `image_w1536_h864_picturebook_01_imbalance_day_16x.png` | 失衡之日·世界观 | 序章 |
| ② | `image_w1536_h864_picturebook_02_glucose_scale.png` | 你就是那杆秤·血糖刻度+键位 | 序章 |
| ③ | `image_w1536_h864_picturebook_03_snow_house_start.png` | 从雪屋出发 | L1 进入 |
| ④ | `image_w1536_h864_picturebook_04_jump_roll.png` | 跳/滚/稳住 | 序章 |
| ⑤ | `720X720.PNG` | 红色暴风雪·高糖 Boss | L2 进入 + 失败结算 |
| ⑥ | `20260621-182652.jpg` | NPC 与道具系统(6/12) | 序章 |
| ⑦ | `image_w1536_h864_picturebook_07_ice_cave.png` | 滑进冰窟·低血糖 | L3 进入 |
| — | `720X720 (1).PNG` | 胰岛素+冲过关了 | 胜利结算 |
| — | `20260621-182641.jpg` | 标题封面 | StartScene 背景 |
| — | `20260621-182648.jpg` | L1 流程图 | LevelSelect 背景 |
| — | `20260621-182656.jpg` | 红漩涡 Boss 全景 | 备用（暂不接，留作后续 L2 关内背景）|
| — | `20260621-182702.jpg` | 蓝冰窟全景 | 备用（暂不接，留作后续 L3 关内背景）|

> 注：`182656`/`182702` 两张全景图本轮不接（关卡内远景背景已被用户排除），保留在
> Illustrations 目录供下一轮使用。

### 场景映射（重要：文件名与关卡号相反）

沿用 `2026-06-20-l2-l3-snow-theme-design.md` 已确认的映射：

- **L1** = `Assets/Scenes/L1_HospitalChalet.unity`
- **L2** = `Assets/Scenes/L2_Fusion_Ours.unity`（自研 MonoBehaviour 3D）
- **L3** = `Assets/Scenes/L2_Fusion.unity`（第三方 Entitas ECS）

## 4. 设计：StoryCard 叙事系统（过渡）

三个文件，单一职责，边界清晰。放在
`Assets/Scripts/Foundation/StoryCard/`，命名空间 `SugarRush.Foundation.StoryCard`。

### 4.1 `StoryCardSequence.cs`（ScriptableObject — 数据）

```
[CreateAssetMenu(menuName = "SugarRush/Story/Card Sequence")]
public class StoryCardSequence : ScriptableObject
{
    public string PlayOnceKey;     // PlayerPrefs 键；空 = 每次都播
    public Sprite[] Pages;         // 有序页面
}
```

- 纯数据容器。无逻辑。
- 4 个资产实例存 `Assets/Data/Story/`：
  - `Seq_Prologue`（Pages = ①②④⑥，PlayOnceKey = `sr_story_prologue`）
  - `Seq_L1_Intro`（Pages = ③，PlayOnceKey = `sr_story_l1`）
  - `Seq_L2_Intro`（Pages = ⑤，PlayOnceKey = `sr_story_l2`）
  - `Seq_L3_Intro`（Pages = ⑦，PlayOnceKey = `sr_story_l3`）

### 4.2 `StoryProgress.cs`（纯 C# 类 — 逻辑，可单元测试）

负责回答"给定一组序列，哪些没看过、要拼成哪些页来播"。**不依赖 Unity 运行时
PlayerPrefs**，通过接口注入存档读写以便单测。

```
public interface IStoryProgressStore        // 抽象存档
{
    bool HasSeen(string key);
    void MarkSeen(string key);
}

public class PlayerPrefsStore : IStoryProgressStore { ... }   // 生产实现

public static class StoryProgress
{
    // 输入序列列表 → 输出"应播放的序列"子集（PlayOnceKey 为空或未看过的）
    public static IReadOnlyList<StoryCardSequence> ResolvePlayable(
        IReadOnlyList<StoryCardSequence> sequences, IStoryProgressStore store);

    // 把多个序列的页拼成一个连续页数组
    public static Sprite[] FlattenPages(IReadOnlyList<StoryCardSequence> sequences);
}
```

- `ResolvePlayable`：跳过 `PlayOnceKey` 已看过的序列；`PlayOnceKey` 为空的总是播。
- `FlattenPages`：把待播序列的 Pages 顺序拼接。
- 设计为静态纯函数 + 注入 store，便于单测，无单例依赖。

### 4.3 `StoryCardPlayer.cs`（MonoBehaviour — 覆盖层控制器）

挂在 `StoryCardOverlay.prefab` 的根 Canvas 上。

序列化字段：
- `StoryCardSequence[] _sequences` —— 本场景要播的序列（按序）
- `Image _pageImage` —— 显示当前页
- `GameObject _root` —— 整个覆盖层根（控制显隐）
- `GameObject _nextHint` —— "按空格继续"提示
- `bool _freezeGameplay = true` —— 是否 `Time.timeScale = 0`

行为（用 `IStoryProgressStore` = `PlayerPrefsStore`）：
1. `Start()`：调 `StoryProgress.ResolvePlayable` → `FlattenPages`。
   - 无可播页 → `_root.SetActive(false)`，立即 `Complete()`（不冻结）。
   - 有页 → 显示 `_root`，`_freezeGameplay` 时 `Time.timeScale = 0`，显示第 0 页。
2. `Update()`（不受 timeScale 影响）：
   - Space / 鼠标左键 / Enter → 下一页；最后一页再按 → `Complete()`。
   - ESC → 直接 `Complete()`（跳过剩余）。
3. `Complete()`：对所有刚播的序列调 `store.MarkSeen(PlayOnceKey)`；
   `Time.timeScale = 1`；`_root.SetActive(false)`；触发 `UnityEvent OnComplete`。

> 暂停式 UI 标准做法：`Update()` 不受 `timeScale` 影响，故 `timeScale=0` 时翻页输入仍正常。
> 图片用 `Image.preserveAspect = true` + 全屏 `RectTransform`，16:9 与方图都不变形。

### 4.4 Prefab：`StoryCardOverlay.prefab`

- 独立 Canvas（Screen Space - Overlay，`sortingOrder = 100`，盖过 HUD/ResultPanel）。
- 全屏黑底 Image（防止露出后面场景）+ 居中页面 Image（preserveAspect）+ 右下角 NextHint 文本。
- 根挂 `StoryCardPlayer`。
- 各场景实例化后只需在 Inspector 填 `_sequences`。

### 4.5 各场景接入

| 场景文件 | StoryCardPlayer `_sequences` | 首次进入播放 |
|----------|------------------------------|------------|
| `L1_HospitalChalet.unity` | `[Seq_Prologue, Seq_L1_Intro]` | ①②④⑥③（5 页）|
| `L2_Fusion_Ours.unity` | `[Seq_L2_Intro]` | ⑤（1 页）|
| `L2_Fusion.unity`（=L3） | `[Seq_L3_Intro]` | ⑦（1 页）|

- 死亡重玩同一场景：序列已 MarkSeen → 直接开打，不烦人。
- 合法过关 L1→L2：L2 序列首次触发 = 自然过场感。

## 5. 设计：静态 UI 背景（背景补充）

无新脚本（除 ResultPanel 小改）。各场景独立、无状态。

| 场景/面板 | 改动 | 用图 |
|-----------|------|------|
| `StartScene.unity` | 最底层加全屏 Image（preserveAspect 或 fill） | `182641` 封面 |
| `LevelSelect.unity` | 加背景 Image（按钮层之下） | `182648` L1 流程图 |
| `ResultPanel`（在 L1/L2 场景内） | 面板加背景 Image，按胜负换 Sprite | 胜=`720X720(1)`、负=`720X720` |

### 5.1 `ResultPanel.cs` 改动

新增字段：
- `[SerializeField] private Image _backgroundImage;`
- `[SerializeField] private Sprite _winBackground;`
- `[SerializeField] private Sprite _loseBackground;`

`ShowResult(bool win)` 内：
```
if (_backgroundImage != null)
    _backgroundImage.sprite = win ? _winBackground : _loseBackground;
```
其余逻辑不变，向后兼容（字段为空则不动背景）。

## 6. Edge Cases（边界情况）

- **PlayerPrefs 已脏**：开发期可通过菜单 `SugarRush/Story/Reset Progress`（可选小工具，或手动 `PlayerPrefs.DeleteKey`）重置。本轮先不做菜单，记为可选。
- **timeScale 泄漏**：`Complete()` 必恢复 `timeScale=1`；若玩家在故事卡播放中切场景，`SceneSwitcher.LoadScene` 已 `Time.timeScale = 1`，双保险。
- **空序列 / 空 Pages**：`ResolvePlayable` 过滤掉无页序列；全空时 Player 立即 Complete 不冻结。
- **图片缺失（Sprite 为 null）**：导入失败时 `_pageImage.sprite = null` 显示空白——实现时跳过 null 页。
- **ESC 与游戏内暂停冲突**：故事卡播放期间独占输入（覆盖层在最上层），暂不与其它 ESC 行为共存（本轮各关无 ESC 暂停菜单）。

## 7. Dependencies（依赖）

- 现有：`GameEvents`（ResultPanel 已用）、UGUI、`SceneManager`。
- 新增运行时依赖：无第三方包。仅用 `UnityEngine.UI.Image` + `PlayerPrefs`。
- 实现工具：**Unity MCP**（导入纹理、建 Prefab、连线场景、Play 模式截图）；Editor 需开启并连上 MCP。C# 脚本直接写文件后由 MCP `refresh_unity` 触发编译。

## 8. Tuning Knobs（可调项）

- 各 `StoryCardSequence` 的 `Pages` 顺序与 `PlayOnceKey`。
- `StoryCardPlayer._freezeGameplay`、翻页输入键。
- 背景图 `preserveAspect` vs 填满（按各场景分辨率定）。

## 9. Acceptance Criteria（验收标准）

**逻辑（BLOCKING — 单元测试）**：
- `StoryProgress.ResolvePlayable` 跳过已看序列、保留未看与空 key 序列。
- `StoryProgress.FlattenPages` 按序拼接页。
- ESC 跳过后所有播放序列被 MarkSeen。
- 测试用 fake `IStoryProgressStore`，确定性、无 PlayerPrefs/IO。

**视觉/交互（ADVISORY — Play 模式截图）**：
- 首进 L1 播 5 页，Space 翻页，最后一页进入 gameplay。
- 死亡重玩 L1 不再弹故事卡。
- L1→L2 过关后 L2 故事卡首次触发。
- StartScene/LevelSelect 背景正确显示、不变形。
- ResultPanel 胜/负显示对应背景图。

## 10. 文件清单

**新增**
- `Assets/Art/Illustrations/`（12 张 Sprite）
- `Assets/Scripts/Foundation/StoryCard/StoryCardSequence.cs`
- `Assets/Scripts/Foundation/StoryCard/StoryProgress.cs`
- `Assets/Scripts/Foundation/StoryCard/StoryCardPlayer.cs`
- `Assets/Data/Story/Seq_Prologue.asset` / `Seq_L1_Intro.asset` / `Seq_L2_Intro.asset` / `Seq_L3_Intro.asset`
- `Assets/Prefabs/UI/StoryCardOverlay.prefab`
- 单元测试：`Assets/Tests/EditMode/StoryProgressTests.cs`（含 asmdef）

**修改**
- `Assets/Scenes/L1_HospitalChalet.unity`（挂 Prefab + 配序列）
- `Assets/Scenes/L2_Fusion_Ours.unity`（挂 Prefab + 配序列）
- `Assets/Scenes/L2_Fusion.unity`（挂 Prefab + 配序列）
- `Assets/Scenes/StartScene.unity`（加背景）
- `Assets/Scenes/LevelSelect.unity`（加背景）
- `Assets/Scripts/Foundation/ResultPanel.cs`（背景 Sprite 切换）
- 各含 ResultPanel 的场景（连线背景 Image 与 Sprite 字段）
