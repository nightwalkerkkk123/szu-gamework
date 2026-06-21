# 插画接入（StoryCard 叙事系统 + UI 背景）Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 把 12 张绘本插画接入游戏，做成全屏故事卡过渡（序章+各关单页，只看一次可跳过）与三处 UI 背景。

**Architecture:** 两个解耦子系统共享导入的 Sprite。叙事系统 = 独立 asmdef 里的纯逻辑 `StoryProgress`（TDD 单测）+ 数据 `StoryCardSequence`（SO）+ 覆盖层 `StoryCardPlayer`（MonoBehaviour）+ `StoryCardOverlay` Prefab，挂到各关卡场景。背景系统 = StartScene/LevelSelect 加全屏 Image + `ResultPanel.cs` 按胜负切换背景 Sprite。

**Tech Stack:** Unity 2022.3.62f3c1（Built-in、UGUI）、C#、Unity Test Framework 1.1.33（EditMode NUnit）、Unity MCP（资源导入/Prefab/场景连线/Play 验收）。

## Global Constraints

- 引擎：Unity 2022.3.62f3c1 LTS，Built-in Render Pipeline，UGUI。
- 命名：类 PascalCase、私有字段 `_camelCase`、文件名匹配类名。
- 命名空间：`SugarRush.Foundation.StoryCard`。
- 数据驱动：序列内容用 ScriptableObject 资产，不硬编码。
- 依赖注入优于单例：`StoryProgress` 通过 `IStoryProgressStore` 注入存档，保证可单测。
- 测试确定性：单测不碰 `PlayerPrefs`/IO，用 fake store。
- 场景映射（**文件名与关卡号相反**）：L1=`L1_HospitalChalet.unity`、L2=`L2_Fusion_Ours.unity`、L3=`L2_Fusion.unity`。
- 素材源目录：`/Users/bytedance/Desktop/用所选项目新建的文件夹/`。
- 提交：每个 Task 末尾提交；提交信息引用本计划。
- Unity MCP 前置：Editor 必须开着 `unity-project/` 并连上 MCP；改脚本后用 `refresh_unity` 触发编译，`read_console` 查编译错误。

---

### Task 1: StoryCard 运行时模块 + 纯逻辑单测（TDD）

建立独立 asmdef，写 `StoryCardSequence`（数据）与 `StoryProgress`（纯逻辑），用 EditMode 单测红→绿。

**Files:**
- Create: `unity-project/Assets/Scripts/Foundation/StoryCard/SugarRush.StoryCard.asmdef`
- Create: `unity-project/Assets/Scripts/Foundation/StoryCard/StoryCardSequence.cs`
- Create: `unity-project/Assets/Scripts/Foundation/StoryCard/StoryProgress.cs`
- Create: `unity-project/Assets/Tests/EditMode/SugarRush.StoryCard.Tests.asmdef`
- Test: `unity-project/Assets/Tests/EditMode/StoryProgressTests.cs`

**Interfaces:**
- Produces:
  - `class StoryCardSequence : ScriptableObject { public string PlayOnceKey; public Sprite[] Pages; }`
  - `interface IStoryProgressStore { bool HasSeen(string key); void MarkSeen(string key); }`
  - `class PlayerPrefsStore : IStoryProgressStore`
  - `static class StoryProgress`：
    - `IReadOnlyList<StoryCardSequence> ResolvePlayable(IReadOnlyList<StoryCardSequence> sequences, IStoryProgressStore store)`
    - `Sprite[] FlattenPages(IReadOnlyList<StoryCardSequence> sequences)`

- [ ] **Step 1: 建运行时 asmdef**

Create `unity-project/Assets/Scripts/Foundation/StoryCard/SugarRush.StoryCard.asmdef`:

```json
{
    "name": "SugarRush.StoryCard",
    "references": ["UnityEngine.UI"],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "autoReferenced": true
}
```

- [ ] **Step 2: 写数据类 StoryCardSequence.cs**

```csharp
using UnityEngine;

namespace SugarRush.Foundation.StoryCard
{
    /// <summary>
    /// Ordered set of full-screen story illustration pages, optionally shown only once.
    /// </summary>
    [CreateAssetMenu(menuName = "SugarRush/Story/Card Sequence", fileName = "Seq_NewStory")]
    public class StoryCardSequence : ScriptableObject
    {
        [Tooltip("PlayerPrefs 键；留空 = 每次都播")]
        public string PlayOnceKey;

        [Tooltip("有序页面")]
        public Sprite[] Pages;
    }
}
```

- [ ] **Step 3: 写逻辑桩 StoryProgress.cs（先返回错误值，制造红）**

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace SugarRush.Foundation.StoryCard
{
    /// <summary>Abstraction over persistent "已看过" storage so logic stays testable.</summary>
    public interface IStoryProgressStore
    {
        bool HasSeen(string key);
        void MarkSeen(string key);
    }

    /// <summary>Production store backed by UnityEngine.PlayerPrefs.</summary>
    public class PlayerPrefsStore : IStoryProgressStore
    {
        public bool HasSeen(string key)
        {
            return !string.IsNullOrEmpty(key) && PlayerPrefs.GetInt(key, 0) == 1;
        }

        public void MarkSeen(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Pure functions deciding which sequences to play and flattening their pages.</summary>
    public static class StoryProgress
    {
        // STUB — returns wrong values so tests fail red first.
        public static IReadOnlyList<StoryCardSequence> ResolvePlayable(
            IReadOnlyList<StoryCardSequence> sequences, IStoryProgressStore store)
        {
            return new List<StoryCardSequence>();
        }

        public static Sprite[] FlattenPages(IReadOnlyList<StoryCardSequence> sequences)
        {
            return new Sprite[0];
        }
    }
}
```

- [ ] **Step 4: 建测试 asmdef**

Create `unity-project/Assets/Tests/EditMode/SugarRush.StoryCard.Tests.asmdef`:

```json
{
    "name": "SugarRush.StoryCard.Tests",
    "references": ["SugarRush.StoryCard", "UnityEngine.UI"],
    "optionalUnityReferences": ["TestAssemblies"],
    "includePlatforms": ["Editor"],
    "excludePlatforms": []
}
```

- [ ] **Step 5: 写失败测试 StoryProgressTests.cs**

```csharp
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using SugarRush.Foundation.StoryCard;

namespace SugarRush.Tests
{
    public class StoryProgressTests
    {
        private class FakeStore : IStoryProgressStore
        {
            public readonly HashSet<string> Seen = new HashSet<string>();
            public bool HasSeen(string key) => !string.IsNullOrEmpty(key) && Seen.Contains(key);
            public void MarkSeen(string key) { if (!string.IsNullOrEmpty(key)) Seen.Add(key); }
        }

        private static StoryCardSequence MakeSequence(string key, int pageCount)
        {
            var seq = ScriptableObject.CreateInstance<StoryCardSequence>();
            seq.PlayOnceKey = key;
            seq.Pages = new Sprite[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                var tex = new Texture2D(1, 1);
                seq.Pages[i] = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
            }
            return seq;
        }

        [Test]
        public void ResolvePlayable_UnseenKeyedSequence_IsIncluded()
        {
            var seq = MakeSequence("k1", 1);
            var result = StoryProgress.ResolvePlayable(new[] { seq }, new FakeStore());
            Assert.AreEqual(1, result.Count);
            Assert.AreSame(seq, result[0]);
        }

        [Test]
        public void ResolvePlayable_SeenKeyedSequence_IsSkipped()
        {
            var seq = MakeSequence("k1", 1);
            var store = new FakeStore();
            store.MarkSeen("k1");
            var result = StoryProgress.ResolvePlayable(new[] { seq }, store);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void ResolvePlayable_EmptyKey_AlwaysIncluded()
        {
            var seq = MakeSequence("", 1);
            var result = StoryProgress.ResolvePlayable(new[] { seq }, new FakeStore());
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ResolvePlayable_NullSequence_IsSkipped()
        {
            var seq = MakeSequence("k1", 1);
            var result = StoryProgress.ResolvePlayable(
                new StoryCardSequence[] { null, seq }, new FakeStore());
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void FlattenPages_ConcatenatesInOrder()
        {
            var a = MakeSequence("a", 2);
            var b = MakeSequence("b", 1);
            var pages = StoryProgress.FlattenPages(new[] { a, b });
            Assert.AreEqual(3, pages.Length);
            Assert.AreSame(a.Pages[0], pages[0]);
            Assert.AreSame(a.Pages[1], pages[1]);
            Assert.AreSame(b.Pages[0], pages[2]);
        }

        [Test]
        public void FlattenPages_SkipsNullPages()
        {
            var seq = MakeSequence("a", 2);
            seq.Pages[0] = null;
            var pages = StoryProgress.FlattenPages(new[] { seq });
            Assert.AreEqual(1, pages.Length);
        }
    }
}
```

- [ ] **Step 6: 跑测试确认红**

用 MCP `refresh_unity` 编译，`read_console` 确认无编译错误，再 MCP `run_tests`（mode=EditMode，filter=`SugarRush.Tests.StoryProgressTests`）。
Expected: 6 个测试 FAIL（桩返回空集，Count 断言不符）。

- [ ] **Step 7: 实现真逻辑（替换 StoryProgress 两个方法体）**

```csharp
        public static IReadOnlyList<StoryCardSequence> ResolvePlayable(
            IReadOnlyList<StoryCardSequence> sequences, IStoryProgressStore store)
        {
            var result = new List<StoryCardSequence>();
            if (sequences == null) return result;
            foreach (var seq in sequences)
            {
                if (seq == null) continue;
                if (string.IsNullOrEmpty(seq.PlayOnceKey) || !store.HasSeen(seq.PlayOnceKey))
                {
                    result.Add(seq);
                }
            }
            return result;
        }

        public static Sprite[] FlattenPages(IReadOnlyList<StoryCardSequence> sequences)
        {
            var pages = new List<Sprite>();
            if (sequences == null) return pages.ToArray();
            foreach (var seq in sequences)
            {
                if (seq == null || seq.Pages == null) continue;
                foreach (var page in seq.Pages)
                {
                    if (page != null) pages.Add(page);
                }
            }
            return pages.ToArray();
        }
```

- [ ] **Step 8: 跑测试确认绿**

MCP `refresh_unity` → `read_console`（无错）→ `run_tests`（EditMode）。
Expected: 6 个测试全 PASS。

- [ ] **Step 9: 提交**

```bash
git add unity-project/Assets/Scripts/Foundation/StoryCard/ unity-project/Assets/Tests/EditMode/
git commit -m "feat(story): StoryCardSequence 数据 + StoryProgress 纯逻辑 + EditMode 单测

计划 docs/superpowers/plans/2026-06-21-illustration-storycard.md Task 1

Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>"
```

---

### Task 2: StoryCardPlayer 覆盖层控制器

写 MonoBehaviour 控制器（输入/翻页/冻结/标记已看）。无单测（依赖 Update/Input/timeScale），靠编译通过 + 后续 Play 验收。

**Files:**
- Create: `unity-project/Assets/Scripts/Foundation/StoryCard/StoryCardPlayer.cs`

**Interfaces:**
- Consumes: `StoryProgress.ResolvePlayable`、`StoryProgress.FlattenPages`、`PlayerPrefsStore`、`StoryCardSequence`（Task 1）。
- Produces: `class StoryCardPlayer : MonoBehaviour`，序列化字段 `_sequences`(StoryCardSequence[])、`_root`(GameObject)、`_pageImage`(Image)、`_nextHint`(GameObject)、`_freezeGameplay`(bool)；公开 `UnityEvent OnComplete`。

- [ ] **Step 1: 写 StoryCardPlayer.cs**

```csharp
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SugarRush.Foundation.StoryCard
{
    /// <summary>
    /// Full-screen story-card overlay. On Start it plays unseen sequences page by page,
    /// freezing gameplay until the player advances through them (or skips with ESC).
    /// </summary>
    public class StoryCardPlayer : MonoBehaviour
    {
        [SerializeField] private StoryCardSequence[] _sequences;
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _pageImage;
        [SerializeField] private GameObject _nextHint;
        [SerializeField] private bool _freezeGameplay = true;

        public UnityEvent OnComplete;

        private readonly IStoryProgressStore _store = new PlayerPrefsStore();
        private Sprite[] _pages;
        private IReadOnlyList<StoryCardSequence> _playing;
        private int _index;
        private bool _active;

        private void Start()
        {
            _playing = StoryProgress.ResolvePlayable(_sequences, _store);
            _pages = StoryProgress.FlattenPages(_playing);

            if (_pages.Length == 0)
            {
                if (_root != null) _root.SetActive(false);
                if (_freezeGameplay) Time.timeScale = 1f;
                OnComplete?.Invoke();
                return;
            }

            if (_root != null) _root.SetActive(true);
            if (_freezeGameplay) Time.timeScale = 0f;
            _index = 0;
            _active = true;
            ShowPage(0);
        }

        // Update runs regardless of Time.timeScale, so input works while frozen.
        private void Update()
        {
            if (!_active) return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Complete();
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space)
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetMouseButtonDown(0))
            {
                Advance();
            }
        }

        private void Advance()
        {
            _index++;
            if (_index >= _pages.Length)
            {
                Complete();
            }
            else
            {
                ShowPage(_index);
            }
        }

        private void ShowPage(int i)
        {
            if (_pageImage != null)
            {
                _pageImage.sprite = _pages[i];
                _pageImage.preserveAspect = true;
            }
            if (_nextHint != null) _nextHint.SetActive(true);
        }

        private void Complete()
        {
            if (_active && _playing != null)
            {
                foreach (var seq in _playing)
                {
                    if (seq != null) _store.MarkSeen(seq.PlayOnceKey);
                }
            }

            _active = false;
            if (_freezeGameplay) Time.timeScale = 1f;
            if (_root != null) _root.SetActive(false);
            OnComplete?.Invoke();
        }
    }
}
```

- [ ] **Step 2: 编译检查**

MCP `refresh_unity` → `read_console`（filter errors）。
Expected: 无编译错误，`StoryCardPlayer` 类型可用。

- [ ] **Step 3: 提交**

```bash
git add unity-project/Assets/Scripts/Foundation/StoryCard/StoryCardPlayer.cs
git commit -m "feat(story): StoryCardPlayer 全屏故事卡覆盖层控制器

计划 Task 2

Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>"
```

---

### Task 3: 导入 12 张插画为 Sprite

复制源图进项目并设为 Sprite 类型。

**Files:**
- Create: `unity-project/Assets/Art/Illustrations/`（12 张图 + .meta）

- [ ] **Step 1: 复制图片**

```bash
mkdir -p unity-project/Assets/Art/Illustrations
cp "/Users/bytedance/Desktop/用所选项目新建的文件夹/"*.png "/Users/bytedance/Desktop/用所选项目新建的文件夹/"*.PNG "/Users/bytedance/Desktop/用所选项目新建的文件夹/"*.jpg unity-project/Assets/Art/Illustrations/
ls unity-project/Assets/Art/Illustrations/
```
Expected: 12 个文件（5 picturebook png、2 720x720 PNG、5 jpg）。

- [ ] **Step 2: 导入并设 Sprite 类型**

MCP `refresh_unity` 先触发导入，再 MCP `execute_code` 跑：

```csharp
using UnityEditor;
using UnityEngine;
string dir = "Assets/Art/Illustrations";
foreach (var guid in AssetDatabase.FindAssets("t:Texture2D", new[] { dir }))
{
    string path = AssetDatabase.GUIDToAssetPath(guid);
    var importer = AssetImporter.GetAtPath(path) as TextureImporter;
    if (importer == null) continue;
    importer.textureType = TextureImporterType.Sprite;
    importer.spriteImportMode = SpriteImportMode.Single;
    importer.maxTextureSize = 2048;
    importer.SaveAndReimport();
    Debug.Log("[StoryImport] Sprite: " + path);
}
AssetDatabase.Refresh();
```
Expected: console 打印 12 行 `[StoryImport] Sprite:`。

- [ ] **Step 3: 验证**

MCP `read_console` 确认 12 张全部转 Sprite、无导入错误。

- [ ] **Step 4: 提交**

```bash
git add unity-project/Assets/Art/Illustrations/
git commit -m "assets(story): 导入 12 张绘本插画为 Sprite

计划 Task 3

Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>"
```

---

### Task 4: 创建 4 个序列资产 + StoryCardOverlay Prefab

用 MCP `execute_code` 程序化创建（符合项目 `L1SceneBuilder` 程序化传统）。

**Files:**
- Create: `unity-project/Assets/Data/Story/Seq_Prologue.asset`
- Create: `unity-project/Assets/Data/Story/Seq_L1_Intro.asset`
- Create: `unity-project/Assets/Data/Story/Seq_L2_Intro.asset`
- Create: `unity-project/Assets/Data/Story/Seq_L3_Intro.asset`
- Create: `unity-project/Assets/Prefabs/UI/StoryCardOverlay.prefab`

**Interfaces:**
- Consumes: `StoryCardSequence`、`StoryCardPlayer`（Task 1/2）；Illustrations Sprites（Task 3）。
- Produces: 4 个序列资产 + Overlay Prefab（含 `StoryCardPlayer`、全屏黑底、居中页面 Image、右下 NextHint）。

- [ ] **Step 1: 创建 4 个序列资产**

MCP `execute_code`：

```csharp
using UnityEditor;
using UnityEngine;
using SugarRush.Foundation.StoryCard;
using System.IO;

Sprite Load(string file)
{
    string p = "Assets/Art/Illustrations/" + file;
    var s = AssetDatabase.LoadAssetAtPath<Sprite>(p);
    if (s == null) Debug.LogError("[Story] 找不到 Sprite: " + p);
    return s;
}

Directory.CreateDirectory("Assets/Data/Story");

StoryCardSequence MakeSeq(string assetName, string key, string[] files)
{
    var seq = ScriptableObject.CreateInstance<StoryCardSequence>();
    seq.PlayOnceKey = key;
    var pages = new System.Collections.Generic.List<Sprite>();
    foreach (var f in files) { var s = Load(f); if (s != null) pages.Add(s); }
    seq.Pages = pages.ToArray();
    AssetDatabase.CreateAsset(seq, "Assets/Data/Story/" + assetName + ".asset");
    return seq;
}

MakeSeq("Seq_Prologue", "sr_story_prologue", new[] {
    "image_w1536_h864_picturebook_01_imbalance_day_16x.png",
    "image_w1536_h864_picturebook_02_glucose_scale.png",
    "image_w1536_h864_picturebook_04_jump_roll.png",
    "20260621-182652.jpg"
});
MakeSeq("Seq_L1_Intro", "sr_story_l1", new[] { "image_w1536_h864_picturebook_03_snow_house_start.png" });
MakeSeq("Seq_L2_Intro", "sr_story_l2", new[] { "720X720.PNG" });
MakeSeq("Seq_L3_Intro", "sr_story_l3", new[] { "image_w1536_h864_picturebook_07_ice_cave.png" });

AssetDatabase.SaveAssets();
AssetDatabase.Refresh();
Debug.Log("[Story] 4 序列资产已建");
```

> `LoadAssetAtPath<Sprite>` 需带扩展名的完整路径，上面 `files` 已含真实后缀（注意 `720X720.PNG` 是大写 `.PNG`）。Expected: console `[Story] 4 序列资产已建`，无 `找不到 Sprite` 报错。

- [ ] **Step 2: 创建 StoryCardOverlay Prefab**

MCP `execute_code`：

```csharp
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using SugarRush.Foundation.StoryCard;
using System.IO;

Directory.CreateDirectory("Assets/Prefabs/UI");

var rootGO = new GameObject("StoryCardOverlay",
    typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(StoryCardPlayer));
var canvas = rootGO.GetComponent<Canvas>();
canvas.renderMode = RenderMode.ScreenSpaceOverlay;
canvas.sortingOrder = 100;
var scaler = rootGO.GetComponent<CanvasScaler>();
scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
scaler.referenceResolution = new Vector2(1920, 1080);

// 黑底
var bg = new GameObject("Backdrop", typeof(Image));
bg.transform.SetParent(rootGO.transform, false);
var bgImg = bg.GetComponent<Image>();
bgImg.color = Color.black;
var bgRt = bg.GetComponent<RectTransform>();
bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one;
bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;

// 页面图
var page = new GameObject("PageImage", typeof(Image));
page.transform.SetParent(rootGO.transform, false);
var pageImg = page.GetComponent<Image>();
pageImg.preserveAspect = true;
var pageRt = page.GetComponent<RectTransform>();
pageRt.anchorMin = Vector2.zero; pageRt.anchorMax = Vector2.one;
pageRt.offsetMin = Vector2.zero; pageRt.offsetMax = Vector2.zero;

// 下一页提示
var hint = new GameObject("NextHint", typeof(Text));
hint.transform.SetParent(rootGO.transform, false);
var hintTxt = hint.GetComponent<Text>();
hintTxt.text = "空格/点击 继续    ESC 跳过";
hintTxt.alignment = TextAnchor.LowerRight;
hintTxt.fontSize = 28;
hintTxt.color = Color.white;
hintTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
var hintRt = hint.GetComponent<RectTransform>();
hintRt.anchorMin = new Vector2(0.5f, 0f); hintRt.anchorMax = new Vector2(1f, 0.12f);
hintRt.offsetMin = Vector2.zero; hintRt.offsetMax = new Vector2(-40, 0);

// 连线 StoryCardPlayer 私有 SerializeField（用 SerializedObject）
var player = rootGO.GetComponent<StoryCardPlayer>();
var so = new SerializedObject(player);
so.FindProperty("_root").objectReferenceValue = rootGO;
so.FindProperty("_pageImage").objectReferenceValue = pageImg;
so.FindProperty("_nextHint").objectReferenceValue = hint;
so.FindProperty("_freezeGameplay").boolValue = true;
so.ApplyModifiedPropertiesWithoutUndo();

string prefabPath = "Assets/Prefabs/UI/StoryCardOverlay.prefab";
PrefabUtility.SaveAsPrefabAsset(rootGO, prefabPath);
Object.DestroyImmediate(rootGO);
AssetDatabase.Refresh();
Debug.Log("[Story] StoryCardOverlay Prefab 已建: " + prefabPath);
```

Expected: console `[Story] StoryCardOverlay Prefab 已建`，`read_console` 无错。

> `LegacyRuntime.ttf` 在 2022.3 是内置 Legacy 字体名（旧版为 `Arial.ttf`）。若取字体报 null，改用 `"Arial.ttf"`。

- [ ] **Step 3: 提交**

```bash
git add unity-project/Assets/Data/Story/ unity-project/Assets/Prefabs/UI/StoryCardOverlay.prefab*
git commit -m "feat(story): 4 序列资产 + StoryCardOverlay Prefab

计划 Task 4

Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>"
```

---

### Task 5: 把 StoryCardOverlay 挂进 L1/L2/L3 三场景

每个场景实例化 Prefab 并填该场景的 `_sequences`。

**Files:**
- Modify: `unity-project/Assets/Scenes/L1_HospitalChalet.unity`
- Modify: `unity-project/Assets/Scenes/L2_Fusion_Ours.unity`
- Modify: `unity-project/Assets/Scenes/L2_Fusion.unity`

**Interfaces:**
- Consumes: `StoryCardOverlay.prefab`、4 序列资产（Task 4）。

- [ ] **Step 1: 三场景各挂一个实例并配序列**

对每个场景重复：MCP `manage_scene`(action=load, name=场景名) → MCP `execute_code`：

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SugarRush.Foundation.StoryCard;

// —— 参数：每个场景改这两行 ——
string sceneName = "L1_HospitalChalet"; // 或 "L2_Fusion_Ours" / "L2_Fusion"
string[] seqAssets = {                  // 该场景序列
    "Assets/Data/Story/Seq_Prologue.asset",
    "Assets/Data/Story/Seq_L1_Intro.asset"
};
// L2_Fusion_Ours: { "Assets/Data/Story/Seq_L2_Intro.asset" }
// L2_Fusion:      { "Assets/Data/Story/Seq_L3_Intro.asset" }

var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/StoryCardOverlay.prefab");
var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
instance.name = "StoryCardOverlay";

var player = instance.GetComponent<StoryCardPlayer>();
var so = new SerializedObject(player);
var arr = so.FindProperty("_sequences");
arr.arraySize = seqAssets.Length;
for (int i = 0; i < seqAssets.Length; i++)
{
    arr.GetArrayElementAtIndex(i).objectReferenceValue =
        AssetDatabase.LoadAssetAtPath<StoryCardSequence>(seqAssets[i]);
}
so.ApplyModifiedPropertiesWithoutUndo();

EditorSceneManager.MarkSceneDirty(instance.scene);
EditorSceneManager.SaveOpenScenes();
Debug.Log("[Story] 已挂 StoryCardOverlay 到 " + sceneName);
```

Expected: 三场景各打印一次 `[Story] 已挂 StoryCardOverlay 到 …`。

- [ ] **Step 2: 提交**

```bash
git add unity-project/Assets/Scenes/L1_HospitalChalet.unity unity-project/Assets/Scenes/L2_Fusion_Ours.unity unity-project/Assets/Scenes/L2_Fusion.unity
git commit -m "feat(story): L1/L2/L3 场景挂载 StoryCardOverlay 并配序列

计划 Task 5

Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>"
```

---

### Task 6: ResultPanel 背景按胜负切换

**Files:**
- Modify: `unity-project/Assets/Scripts/Foundation/ResultPanel.cs`
- Modify: `unity-project/Assets/Scenes/L1_HospitalChalet.unity`、`L2_Fusion_Ours.unity`（含 ResultPanel 的场景，连线背景 Image 与 Sprite）

**Interfaces:**
- Produces: `ResultPanel` 新增 `[SerializeField] Image _backgroundImage; Sprite _winBackground; Sprite _loseBackground;`，`ShowResult` 内按 win 切换。

- [ ] **Step 1: 改 ResultPanel.cs — 加字段**

在 `ResultPanel` 类 `_loseColor` 字段之后加：

```csharp
        [Header("Background")]
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Sprite _winBackground;
        [SerializeField] private Sprite _loseBackground;
```

- [ ] **Step 2: 改 ShowResult — 切换背景**

在 `ShowResult(bool win)` 方法体开头（`if (_panel != null)` 之前）加：

```csharp
            if (_backgroundImage != null)
            {
                Sprite bg = win ? _winBackground : _loseBackground;
                if (bg != null)
                {
                    _backgroundImage.sprite = bg;
                    _backgroundImage.enabled = true;
                }
            }
```

- [ ] **Step 3: 编译检查**

MCP `refresh_unity` → `read_console`。Expected: 无错。

- [ ] **Step 4: 连线两场景的 ResultPanel 背景**

对 `L1_HospitalChalet`、`L2_Fusion_Ours` 各执行：`manage_scene`(load) → MCP `execute_code`：

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using SugarRush.Foundation;

var win = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Illustrations/720X720 (1).PNG");
var lose = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Illustrations/720X720.PNG");

var panel = Object.FindObjectOfType<ResultPanel>(true);
if (panel == null) { Debug.LogError("[Story] 场景无 ResultPanel"); }
else
{
    var so = new SerializedObject(panel);
    var panelGO = so.FindProperty("_panel").objectReferenceValue as GameObject;

    // 在 _panel 下找/建一个最底层 Background Image
    Image bgImg = null;
    if (panelGO != null)
    {
        var t = panelGO.transform.Find("ResultBackground");
        if (t != null) bgImg = t.GetComponent<Image>();
        if (bgImg == null)
        {
            var go = new GameObject("ResultBackground", typeof(Image));
            go.transform.SetParent(panelGO.transform, false);
            go.transform.SetAsFirstSibling();
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            bgImg = go.GetComponent<Image>();
            bgImg.preserveAspect = true;
        }
    }

    so.FindProperty("_backgroundImage").objectReferenceValue = bgImg;
    so.FindProperty("_winBackground").objectReferenceValue = win;
    so.FindProperty("_loseBackground").objectReferenceValue = lose;
    so.ApplyModifiedPropertiesWithoutUndo();

    EditorSceneManager.MarkSceneDirty(panel.gameObject.scene);
    EditorSceneManager.SaveOpenScenes();
    Debug.Log("[Story] ResultPanel 背景已连线");
}
```

Expected: 两场景各打印 `[Story] ResultPanel 背景已连线`。

- [ ] **Step 5: 提交**

```bash
git add unity-project/Assets/Scripts/Foundation/ResultPanel.cs unity-project/Assets/Scenes/L1_HospitalChalet.unity unity-project/Assets/Scenes/L2_Fusion_Ours.unity
git commit -m "feat(story): ResultPanel 胜负背景切换 + 场景连线

计划 Task 6

Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>"
```

---

### Task 7: StartScene / LevelSelect 静态背景

**Files:**
- Modify: `unity-project/Assets/Scenes/StartScene.unity`
- Modify: `unity-project/Assets/Scenes/LevelSelect.unity`

- [ ] **Step 1: 给两场景加最底层全屏背景 Image**

对每个场景：`manage_scene`(load) → MCP `execute_code`（改 `sceneName`/`spriteFile`）：

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

string sceneName = "StartScene";                         // 或 "LevelSelect"
string spriteFile = "Assets/Art/Illustrations/20260621-182641.jpg"; // LevelSelect 用 20260621-182648.jpg

var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spriteFile);
if (sprite == null) { Debug.LogError("[Story] 找不到背景: " + spriteFile); }

// 找现有 Canvas（UI 场景必有）；没有则建一个
var canvas = Object.FindObjectOfType<Canvas>();
if (canvas == null)
{
    var cg = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
    cg.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
    canvas = cg.GetComponent<Canvas>();
}

// 已存在则复用
var existing = canvas.transform.Find("SceneBackground");
GameObject bgGO = existing != null ? existing.gameObject
    : new GameObject("SceneBackground", typeof(Image));
bgGO.transform.SetParent(canvas.transform, false);
bgGO.transform.SetAsFirstSibling();   // 置于所有按钮之下
var img = bgGO.GetComponent<Image>();
img.sprite = sprite;
img.preserveAspect = false;           // 填满全屏；如要不裁切改 true
var rt = bgGO.GetComponent<RectTransform>();
rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;

EditorSceneManager.MarkSceneDirty(canvas.gameObject.scene);
EditorSceneManager.SaveOpenScenes();
Debug.Log("[Story] " + sceneName + " 背景已加");
```

Expected: 两场景各打印 `[Story] … 背景已加`。

- [ ] **Step 2: 提交**

```bash
git add unity-project/Assets/Scenes/StartScene.unity unity-project/Assets/Scenes/LevelSelect.unity
git commit -m "feat(story): StartScene 封面 + LevelSelect 背景

计划 Task 7

Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>"
```

---

### Task 8: Play 模式人工验收（ADVISORY）

**Files:** 无（验收记录写入 `production/qa/evidence/`）。

- [ ] **Step 1: 重置进度以验证首播**

MCP `execute_code`：
```csharp
UnityEngine.PlayerPrefs.DeleteKey("sr_story_prologue");
UnityEngine.PlayerPrefs.DeleteKey("sr_story_l1");
UnityEngine.PlayerPrefs.DeleteKey("sr_story_l2");
UnityEngine.PlayerPrefs.DeleteKey("sr_story_l3");
UnityEngine.PlayerPrefs.Save();
UnityEngine.Debug.Log("[Story] 进度已重置");
```

- [ ] **Step 2: 逐项验收并截图**

`manage_scene`(load=L1_HospitalChalet) → `manage_editor`(action=play) → 用 MCP 截图，确认：
1. 首进 L1 弹 5 页（①②④⑥③），Space 翻页，第 5 页后进入 gameplay 且时间恢复。
2. 死亡/重玩 L1 不再弹故事卡（PlayerPrefs 已记）。
3. L1 通关 → L2，L2 故事卡（⑤）首次触发。
4. L2 → L3（`L2_Fusion`）故事卡（⑦）首次触发。
5. StartScene 封面、LevelSelect 背景显示正常、不严重变形。
6. ResultPanel 胜=冲过关图、负=红暴风雪图。
`manage_editor`(action=stop) 退出 Play。

- [ ] **Step 3: 记录验收**

把截图与逐项结论写入 `production/qa/evidence/2026-06-21-storycard.md`，提交：

```bash
git add production/qa/evidence/2026-06-21-storycard.md
git commit -m "test(story): StoryCard + 背景 Play 模式验收记录

计划 Task 8

Co-Authored-By: Claude Opus 4.8 (1M context) <noreply@anthropic.com>"
```

---

## Self-Review

**Spec coverage：**
- §4 StoryCard 系统 → Task 1（数据+逻辑+单测）、Task 2（Player）、Task 4（序列资产+Prefab）、Task 5（场景挂载）✅
- §5 UI 背景 → Task 6（ResultPanel）、Task 7（StartScene/LevelSelect）✅
- §3 素材导入 → Task 3 ✅
- §6 边界（空序列/timeScale/null 页）→ Task 1 测试 `NullSequence`/`SkipsNullPages` + Task 2 空页 Complete 分支 ✅
- §9 验收（逻辑 BLOCKING + 视觉 ADVISORY）→ Task 1 单测 + Task 8 Play 验收 ✅

**Placeholder scan：** 无 TBD/TODO；所有代码步骤含完整代码；扩展名风险点已在 Task 4 显式说明修正方法。

**Type consistency：** `ResolvePlayable`/`FlattenPages`/`PlayOnceKey`/`Pages`/`_sequences`/`_backgroundImage`/`_winBackground`/`_loseBackground` 在定义任务与消费任务间一致；序列 PlayerPrefs 键 `sr_story_*` 在 Task 4 资产、Task 8 重置间一致。
