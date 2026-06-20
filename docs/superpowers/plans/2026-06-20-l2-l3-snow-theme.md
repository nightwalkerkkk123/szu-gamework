# L2 / L3 雪山主题化 Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 把 L2(`L2_Fusion_Ours`) 与 L3(`L2_Fusion`) 两个 3D 跑酷原型，用程序氛围包 + 主题材质打磨成正式雪山关卡，并给 L3 桥接血糖系统。

**Architecture:** 一份共享「氛围包」（渐变天空盒 + 雾 + 冷色光 + 飘雪 + 纯色材质库），由一个幂等 Editor 菜单一键套用到任意场景。L2 复用已有血糖系统只补美术；L3 在 Entitas ECS 跑酷外**挂接**（不改第三方源码）一个视图层血糖桥 + 复用 HUD。

**Tech Stack:** Unity 2022.3.62f3 LTS、C#、Built-in Render Pipeline、UGUI、Entitas ECS（仅 L3，外挂不改源）、Unity MCP（场景/材质/截图操作）。

## Global Constraints

- 引擎：Unity 2022.3.62f3 LTS，Built-in Render Pipeline（**非** URP/HDRP，天空盒/雾用 Built-in）。
- 命名：类 PascalCase、私有字段 `_camelCase`、文件名匹配类名。
- **提交需用户明确指令**（项目规矩，优先于技能默认）。计划中的 `git commit` 步骤执行前先征得用户同意。
- **不改操控**（保留只跳）；不生成新贴图（纯程序氛围 + 纯色材质）；不改 L1。
- 第三方 `Assets/ThirdParty/EndlessRunner/` **核心源码不改**；唯一例外见 Task 4（仅改配置常量行 + 加编译宏）。
- 每步改完 C# 后用 `read_console` 确认编译无 error 再继续。
- Unity 实例：`unity-project@8550dbe1e5473205`（如断开，重新 `set_active_instance`）。

---

## File Structure（文件职责）

| 文件 | 职责 | 任务 |
|------|------|------|
| `Assets/Materials/SnowWhite.mat` 等 5 个 | 共享纯色/天空盒材质 | T1 |
| `Assets/Editor/SnowAtmosphereSetup.cs` | 幂等菜单：对当前场景套用天空盒/雾/光/飘雪 | T2 |
| `L2_Fusion_Ours.unity` + Chunk/Player 预制体 | L2 套氛围 + 主题材质 | T3 |
| `GameController.cs`（第三方，仅 2 处常量 + 宏） | L3 平台冰雪色 + 难度 + 去调试层 | T4 |
| `Assets/Scripts/Gameplay/GlucoseEntitasBridge.cs` | L3 视图层血糖桥 | T5 |
| `L2_Fusion.unity`（L3 场景） | 挂血糖管理对象 + HUD | T5 |

---

### Task 1: 材质库 + 渐变天空盒（共享资产）

**Files:**
- Create: `Assets/Materials/SnowWhite.mat`、`IceBlue.mat`、`CandyPink.mat`、`HazardRed.mat`
- Create: `Assets/Materials/SnowSkybox.mat`（Shader: `Skybox/Gradient` 不存在则用 `Skybox/Procedural`）

**Interfaces:**
- Produces: 5 个材质资产路径，供 T2（天空盒）、T3（L2 材质）、T4（L3 平台色参考）复用。

- [ ] **Step 1: 创建 4 个纯色材质**

用 MCP `manage_material` 逐个创建（Built-in `Standard` shader）：
- `SnowWhite`：albedo `#F2F7FC`，Smoothness 0.15
- `IceBlue`：albedo `#9FD2E8`，Smoothness 0.6，Metallic 0.1
- `CandyPink`：albedo `#F2A8C8`，Smoothness 0.4
- `HazardRed`：albedo `#E0463C`，Smoothness 0.2

- [ ] **Step 2: 创建天空盒材质**

`SnowSkybox.mat`：优先 `Skybox/Procedural`（Sky Tint `#CFE4F5`、Ground `#E8F0F8`、Atmosphere Thickness 0.4、Exposure 1.1），得到冷白→淡蓝的高空感。

- [ ] **Step 3: 验证资产存在且无编译错误**

`manage_asset` search `Assets/Materials/`，确认 5 个 `.mat` 存在；`read_console` 无 error。
Expected: 5 个材质列出，控制台干净。

- [ ] **Step 4: Commit（征得用户同意后）**

```bash
git add unity-project/Assets/Materials
git commit -m "feat(art): add snow-theme material library + gradient skybox"
```

---

### Task 2: 雪山氛围一键套用 Editor 工具

**Files:**
- Create: `Assets/Editor/SnowAtmosphereSetup.cs`

**Interfaces:**
- Consumes: `Assets/Materials/SnowSkybox.mat`（T1）。
- Produces: 菜单 `SugarRush/Setup/Apply Snow Atmosphere`，幂等地对当前激活场景设置天空盒、雾、方向光、飘雪粒子。供 T3、T4 调用。

- [ ] **Step 1: 写 Editor 工具（完整代码）**

```csharp
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace SugarRush.EditorTools
{
    /// <summary>
    /// Idempotent one-click snow atmosphere for any open scene:
    /// gradient skybox + cool fog + winter directional light + camera-following snow.
    /// </summary>
    public static class SnowAtmosphereSetup
    {
        private const string SkyboxPath = "Assets/Materials/SnowSkybox.mat";
        private const string SnowRootName = "SnowAtmosphere";

        [MenuItem("SugarRush/Setup/Apply Snow Atmosphere")]
        public static void Apply()
        {
            // 1. Skybox + ambient
            var skybox = AssetDatabase.LoadAssetAtPath<Material>(SkyboxPath);
            if (skybox != null) RenderSettings.skybox = skybox;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.85f, 0.92f, 1f);
            RenderSettings.ambientEquatorColor = new Color(0.78f, 0.85f, 0.95f);
            RenderSettings.ambientGroundColor = new Color(0.7f, 0.75f, 0.82f);

            // 2. Fog (hides chunk pop-in + depth)
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.82f, 0.89f, 0.96f);
            RenderSettings.fogStartDistance = 25f;
            RenderSettings.fogEndDistance = 120f;

            // 3. Winter directional light
            var light = Object.FindObjectOfType<Light>();
            if (light == null)
            {
                var go = new GameObject("Directional Light");
                light = go.AddComponent<Light>();
                light.type = LightType.Directional;
            }
            light.color = new Color(0.95f, 0.97f, 1f);
            light.intensity = 1.1f;
            light.transform.rotation = Quaternion.Euler(35f, -40f, 0f);
            light.shadows = LightShadows.Soft;

            // 4. Camera-following snow particle system
            var cam = Camera.main;
            if (cam != null)
            {
                var existing = cam.transform.Find(SnowRootName);
                if (existing == null)
                {
                    var snow = new GameObject(SnowRootName);
                    snow.transform.SetParent(cam.transform, false);
                    snow.transform.localPosition = new Vector3(0f, 8f, 12f);
                    var ps = snow.AddComponent<ParticleSystem>();
                    ConfigureSnow(ps);
                }
            }

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[SnowAtmosphere] Applied to scene: " + EditorSceneManager.GetActiveScene().name);
        }

        private static void ConfigureSnow(ParticleSystem ps)
        {
            var main = ps.main;
            main.startSize = 0.12f;
            main.startSpeed = 2.5f;
            main.startLifetime = 5f;
            main.maxParticles = 600;
            main.startColor = new Color(1f, 1f, 1f, 0.85f);
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 80f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(40f, 1f, 30f);

            var vel = ps.velocityOverLifetime;
            vel.enabled = true;
            vel.space = ParticleSystemSimulationSpace.World;
            vel.y = new ParticleSystem.MinMaxCurve(-2.5f);
            vel.x = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        }
    }
}
```

- [ ] **Step 2: 等待编译并验证菜单存在**

`refresh_unity` 后轮询 `editor_state.isCompiling=false`；`read_console` 无 error。
Expected: 编译通过，无 error。

- [ ] **Step 3: Commit（征得用户同意后）**

```bash
git add unity-project/Assets/Editor/SnowAtmosphereSetup.cs
git commit -m "feat(tools): add Apply Snow Atmosphere editor menu"
```

---

### Task 3: L2 套用氛围 + 主题材质 + 修粒子

**Files:**
- Modify: `Assets/Scenes/L2_Fusion_Ours.unity`
- Modify: `Assets/Resources/Prefabs/Chunks/Chunk0.prefab`、`ChunkFlat.prefab`、`ChunkRotating.prefab`（renderer 材质）
- Modify: `Assets/Resources/Prefabs/Player.prefab`（renderer 材质）

**Interfaces:**
- Consumes: T1 材质、T2 菜单。

- [ ] **Step 1: 打开 L2 场景并套用氛围**

`manage_scene` load `Assets/Scenes/L2_Fusion_Ours.unity` → `execute_menu_item "SugarRush/Setup/Apply Snow Atmosphere"`。

- [ ] **Step 2: 给地形预制体赋雪地材质**

对 `Chunk0`、`ChunkFlat`、`ChunkRotating` 预制体内所有 `MeshRenderer` 赋 `SnowWhite`（用 `manage_prefabs` 打开 prefab stage → `manage_material` 赋值 → 保存）。

- [ ] **Step 3: 给 Player 预制体赋糖粉材质**

`Player.prefab` 的球体 `MeshRenderer` 赋 `CandyPink`（基础质感；运行时 `GlucoseSystem3DAdapter` 会按血糖区覆盖颜色——这是预期共存）。

- [ ] **Step 4: 修复飘雪事件粒子 sprite 警告**

场景内 `SimpleParticleSpawner` 组件的 `_particleSprite` 字段赋 `Assets/Art/Textures/Snowflake.png`（用 `manage_components` 设属性）。

- [ ] **Step 5: 保存并截图验证**

`manage_scene` save → `manage_camera` screenshot(game_view, include_image)。
Expected: 画面**不再纯白**，有天空盒/雾/雪地/飘雪；`read_console` 无 error 且**无** "Particle sprite not assigned" 警告。

- [ ] **Step 6: Play 模式验证血糖染色未回归**

`manage_editor` play → 等几秒 → screenshot → stop。
Expected: 玩家球随血糖区变色仍工作，HUD 显示血糖/速度/距离。

- [ ] **Step 7: Commit（征得用户同意后）**

```bash
git add unity-project/Assets/Scenes/L2_Fusion_Ours.unity unity-project/Assets/Resources/Prefabs
git commit -m "feat(L2): apply snow atmosphere + theme materials to L2_Fusion_Ours"
```

---

### Task 4: L3 去调试层 + 冰雪平台色 + 难度 + 氛围

**Files:**
- Modify: `ProjectSettings/ProjectSettings.asset`（scripting define symbols 加 `ENTITAS_DISABLE_VISUAL_DEBUGGING`）
- Modify: `Assets/ThirdParty/EndlessRunner/Sources/GameController.cs:68`（TypeColorTable）+ 难度常量
- Modify: `Assets/Scenes/L2_Fusion.unity`

**Interfaces:**
- Consumes: T2 菜单。

- [ ] **Step 1: 加编译宏移除 ECS 调试覆盖层**

Player Settings → Scripting Define Symbols 加 `ENTITAS_DISABLE_VISUAL_DEBUGGING`（用 `manage_editor` 或直接改 `ProjectSettings.asset`）。这会让 `GameController.OnGUI()`（`#if !ENTITAS_DISABLE_VISUAL_DEBUGGING` 守卫）整体编译排除。

- [ ] **Step 2: 改平台颜色为冰雪系**

`GameController.cs:68` 把 `SetTypeColorTable` 改为冰雪色：
```csharp
contexts.config.SetTypeColorTable(new List<Color> {new Color(0.62f, 0.82f, 0.91f), new Color(0.85f, 0.91f, 0.97f), new Color(0.95f, 0.66f, 0.78f)});
```
（冰蓝 / 雪白 / 糖粉，替换原蓝/橙/绿）

- [ ] **Step 3: 提升难度（L3 高潮）**

`GameController.cs` Configure() 内：`SetStandardMaxVelocity` 由 `(20, 70)` 提到 `(26, 70)`（更快）；`SetDeathHeight(-5f)` 收紧到 `SetDeathHeight(-3.5f)`（更早判死，断崖更致命）。

- [ ] **Step 4: 等待编译并验证调试层消失**

`read_console` 无 error → load `L2_Fusion.unity` → `execute_menu_item "SugarRush/Setup/Apply Snow Atmosphere"` → save → play → screenshot → stop。
Expected: 截图**不再有** Jumping/Velocity 等文字覆盖层；平台为冰雪色；有天空盒/雾/飘雪。

- [ ] **Step 5: Commit（征得用户同意后）**

```bash
git add unity-project/ProjectSettings/ProjectSettings.asset unity-project/Assets/ThirdParty/EndlessRunner/Sources/GameController.cs unity-project/Assets/Scenes/L2_Fusion.unity
git commit -m "feat(L3): snow atmosphere, ice palette, harder tuning, remove ECS debug HUD"
```

---

### Task 5: L3 血糖桥接（视图层，不改 Entitas 源码）

**Files:**
- Create: `Assets/Scripts/Gameplay/GlucoseEntitasBridge.cs`
- Modify: `Assets/Scenes/L2_Fusion.unity`（新增血糖管理对象 + 复用 HUD Canvas）
- Modify: Entitas 玩家视图（确保 tag = `Player`，仅改 tag 不改逻辑）

**Interfaces:**
- Consumes: 既有 `GlucoseSystem`、`GlucoseConfig`、`GlucoseSystem3DAdapter`、`GlucoseUI`、`HUD`、`SugarRush.Core.GameEvents.RaiseGameFinished(bool)`。
- Produces: `GlucoseEntitasBridge`（运行时按 tag 找玩家球、驱动血糖染色、crisis 触发失败）。

- [ ] **Step 1: 写桥接脚本（完整代码）**

```csharp
using SugarRush.Core;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// View-layer bridge that runs SugarRush glucose mechanics on top of the
    /// third-party Entitas endless runner WITHOUT touching its ECS source.
    /// Finds the runtime player sphere by tag, lets GlucoseSystem drift passively,
    /// feeds the sphere renderer to GlucoseSystem3DAdapter, and ends the run on
    /// sustained crisis.
    /// </summary>
    [RequireComponent(typeof(GlucoseSystem))]
    public class GlucoseEntitasBridge : MonoBehaviour
    {
        [SerializeField] private GlucoseSystem _glucoseSystem;
        [SerializeField] private GlucoseSystem3DAdapter _adapter;
        [SerializeField] private string _playerTag = "Player";
        [Tooltip("Seconds of continuous crisis (Low/High) before the run fails.")]
        [SerializeField] private float _crisisGraceSeconds = 3f;

        private Transform _player;
        private float _crisisTimer;
        private bool _finished;

        private void Awake()
        {
            if (_glucoseSystem == null) _glucoseSystem = GetComponent<GlucoseSystem>();
            if (_adapter == null) _adapter = GetComponent<GlucoseSystem3DAdapter>();
        }

        private void Update()
        {
            if (_finished) return;

            if (_player == null)
            {
                var go = GameObject.FindWithTag(_playerTag);
                if (go == null) return;
                _player = go.transform;
                // Late-bind the sphere renderer so the adapter can tint it.
                var rend = go.GetComponentInChildren<Renderer>();
                if (rend != null && _adapter != null) _adapter.SetBodyRenderer(rend);
            }

            var zone = _glucoseSystem.CurrentZone;
            bool inCrisis = zone == GlucoseZone.LowCrisis || zone == GlucoseZone.HighCrisis;
            _crisisTimer = inCrisis ? _crisisTimer + Time.deltaTime : 0f;

            if (_crisisTimer >= _crisisGraceSeconds)
            {
                _finished = true;
                GameEvents.RaiseGameFinished(false);
            }
        }
    }
}
```

- [ ] **Step 2: 给 `GlucoseSystem3DAdapter` 加 renderer 注入方法**

`GlucoseSystem3DAdapter.cs` 增加（桥在运行时注入玩家球 renderer）：
```csharp
public void SetBodyRenderer(Renderer renderer) => _bodyRenderer = renderer;
```

- [ ] **Step 3: 编译验证**

`read_console` 无 error。
Expected: 编译通过。

- [ ] **Step 4: L3 场景挂血糖管理对象**

在 `L2_Fusion.unity` 新建 GameObject `GlucoseManager`，加组件：`GlucoseSystem`（赋 `Assets/Data/GlucoseConfig.asset`）、`GlucoseSystem3DAdapter`、`GlucoseEntitasBridge`（用 `manage_gameobject` + `manage_components`）。

- [ ] **Step 5: 复用 HUD + EventSystem**

从 `L2_Fusion_Ours.unity` 复制 `HUD` Canvas（含 `GlucoseUI`/`GlucoseSlider`/`GlucoseValueText`）与 `EventSystem` 到 L3 场景，`GlucoseUI` 绑定到 `GlucoseManager` 的 `GlucoseSystem`。确保 Entitas 玩家视图 tag = `Player`（`find_gameobjects` 查；缺则在玩家预制体/视图加 tag）。

- [ ] **Step 6: Play 模式验证血糖闭环**

save → play → 观察 HUD 血糖条随时间漂移、玩家球随区变色；停留在 crisis 区超过 grace 秒后游戏失败（ResultPanel/GameFinished）。screenshot 取证 → stop。
Expected: L3 出现血糖 HUD 且漂移；球变色；crisis 致败；`read_console` 无 error。

- [ ] **Step 7: Commit（征得用户同意后）**

```bash
git add unity-project/Assets/Scripts/Gameplay/GlucoseEntitasBridge.cs unity-project/Assets/Scripts/Gameplay/GlucoseSystem3DAdapter.cs unity-project/Assets/Scenes/L2_Fusion.unity
git commit -m "feat(L3): bridge SugarRush glucose system into Entitas runner (view-layer)"
```

---

## Self-Review

**Spec coverage:**
- 模块 A 氛围包 → T1（材质/天空盒）+ T2（套用工具）✅
- 模块 B L2 → T3 ✅
- 模块 C L3 美术/去调试/难度 → T4；血糖桥接 → T5 ✅
- 验收标准 1（非白底）→ T3S5/T4S4；2（L2 染色+HUD）→ T3S6；3（去调试层）→ T4S4；
  4（L3 血糖 HUD+crisis 致败）→ T5S6；5（冰雪平台+更难）→ T4；6（无新 error/无粒子警告）→ 各任务验证步 ✅

**Placeholder scan:** 两个新 C# 文件给出完整代码；材质给出确切色值；难度给出确切数值；无 TODO/TBD。

**Type consistency:** `GlucoseSystem3DAdapter.SetBodyRenderer(Renderer)` 在 T5S2 定义、T5S1 桥内调用，签名一致；`GlucoseSystem.CurrentZone`/`GlucoseZone.{LowCrisis,HighCrisis}`/`GameEvents.RaiseGameFinished(bool)` 均为既有 API（见现状代码）。

**已知实现期风险（非占位符，需实现者现场确认）：**
- Entitas 玩家视图是否已 tag=`Player`：T5S5 已含检查/补 tag 步骤。
- `Skybox/Procedural` 色值需肉眼微调以达"冷白→淡蓝"——T1S2 给了起始值，截图后可再调。
