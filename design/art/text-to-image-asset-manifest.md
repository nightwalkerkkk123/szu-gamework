# 《糖大冒险：雪山狂飙》— 文生图（Text-to-Image）资产生成清单

> **文档版本**：v0.1（Initial Draft）
> **创建日期**：2026-06-20
> **依据**：`design/art/art-bible.md`（Section 1-4）、`design/gdd/game-concept.md`、`docs/ASSET_RESOURCES.md`
> **目的**：把 Art Bible 的视觉规范翻译成**可直接粘贴**的文生图 prompt，列出 MVP 阶段可由 AI 文生图生成的全部 2D 资产，替换当前的白块占位符。
> **状态**：草案 — 待美术/Art Director 审阅后批量生成

---

## 0. 怎么用这份文档

1. 先读 **Section 2 统一风格前缀（STYLE PREFIX）** —— 它是所有 prompt 的灵魂，保证全项目风格一致。
2. 每个资产条目给出：用途 → 目标 Unity 路径 → 尺寸/格式 → 推荐工具 → **完整可粘贴 prompt**。
3. 每条 prompt **已内联风格前缀**，可直接粘贴到工具；如工具支持"风格参考图/Style Reference"，先生成 1 张糖糖做风格锚，后续都引用它。
4. 生成后按 **Section 8 导入工作流** 处理（去背景 → 切图 → Unity Sprite 设置）。

> ⚠️ **许可红线**：MVP 是课程项目但有产品化潜力。优先 **Adobe Firefly**（设计上即商用安全）或 **Leonardo.AI**（游戏向、明确商用条款）。Midjourney/SD 输出**商用前必须复核 license**。所有 AI 生成占位在代码里标注 `// ASSET: [描述]`。

---

## 1. 总览：什么该文生图，什么不该

文生图擅长**单帧静态 2D 图像**，对**逐帧一致性**和**非视觉资产**很弱。诚实划界：

| 资产类别 | 文生图适配度 | 说明 |
|---------|:---:|------|
| 角色立绘 / 单姿势 sprite | ✅ 强 | 圆润 Q 版卡通正是 T2I 最稳的画风 |
| 道具图标 / UI 图标 | ✅ 强 | 单体、居中、透明背景，T2I 最佳场景 |
| Parallax 背景分层（远/中/近） | ✅ 强 | 风景图、宽幅，T2I 强项 |
| 天空渐变 / 氛围图 | ✅ 强 | 纯渐变背景，可直接出图或当参考 |
| 障碍物 / 收集物单体 | ✅ 强 | 同道具图标 |
| UI 面板 / 按钮装饰 / LOGO | 🟡 中 | 能出概念图，但精确尺寸/九宫格切图需后处理 |
| 粒子贴图（糖晶/雪花/礼花单颗） | 🟡 中 | 能出，但需手工抠成单颗 + 软边 |
| 角色**逐帧动画**（滑行/跳跃序列） | ❌ 弱 | 帧间一致性差；改用骨骼动画(Unity Animation/Spine) 或 DOTween 程序化形变 |
| Tilemap / 可拼接地形块 | ❌ 弱 | 无缝拼接、碰撞体对齐 T2I 做不到；用 Asset Store tileset 或手作 |
| 音效 / 音乐 | ❌ 不适用 | 用 Suno/ElevenLabs，见 Section 9 |
| Shader / 后期特效（vignette/抖动/速度线运动） | ❌ 不适用 | 用 Shader/粒子系统/C# 脚本，见 Section 9 |

`核心判断`：T2I 负责"**长什么样的静态资产**"；动画、拼接、运动、声音交给引擎和专用工具。本清单只列 ✅ 和 🟡 两档。

---

## 2. 统一风格前缀（STYLE PREFIX）★ 最重要

> 提炼自 Art Bible Section 1（视觉总则）、Section 3（形状语言）、Section 4（调色板）。每条 prompt 都以它开头。

**中文意图**：卡通奇幻游乐场风、圆润 Q 版、可可黑粗描边、鲜艳但克制的医疗主题三色、2D 游戏美术、纯色或透明背景、扁平卡通无写实。

**英文 STYLE PREFIX（粘贴用）：**

```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical
"medical theme park" mood, thick cocoa-black outline (#2D2A32), flat cel-shaded
coloring, bright but not garish, vibrant kids-storybook palette, no realism,
no photographic detail, clean vector-like edges, centered composition,
game asset sprite, transparent background
```

**项目调色板（在 prompt 里点名 HEX 锁色）：**

| 名称 | HEX | 语义 |
|------|-----|------|
| 唐白 Tang White | `#F5F0E8` | 雪地/基底 |
| 血糖红 Glucose Red | `#E63946` | 危险/高血糖 |
| 胰岛素蓝 Insulin Blue | `#4CC9F0` | 冷静/干预/友方 |
| 健康绿 Health Green | `#06D6A0` | 安全/奖励 |
| 阳光黄 Sunny Yellow | `#FFD166` | 警戒/能量 |
| 葡萄紫 Grape Purple | `#7B2CBF` | 失败/Boss |
| 可可黑 Cocoa Black | `#2D2A32` | 描边/文字 |

**全局负向提示（Negative Prompt，所有图通用）：**

```
photorealistic, 3D render, realistic human anatomy, realistic medical equipment,
hospital horror, gore, scary, sharp aggressive shapes on friendly characters,
text, watermark, signature, busy background, drop shadow on transparent areas,
muddy colors, low contrast
```

---

## 3. 角色（Characters）— 优先级 P0

> 形状契约（Art Bible §3）：糖糖 = 汤圆形剪影、**2.5 头身**、oversized 红色毛线帽(尖顶+毛球)、简化胶囊臂、白底+红帽+深棕靴、可可黑描边。NPC = 单一几何原型 + 差异化配件。

### 3.1 糖糖 — 主角基础姿势（滑雪滑行）`P0`
- **用途**：玩家角色主 sprite（替换 Player.prefab 上的蓝方块）
- **路径**：`Assets/Art/Characters/Tang/Tang_Ski.png`
- **规格**：1024×1024 生成 → 切到 ~512px；透明背景；侧面 3/4 朝右（横版前进方向）
- **工具**：Firefly / Leonardo（先出这张当全局 Style Reference）
```
[STYLE PREFIX], a round soup-dumpling shaped chibi mascot character named
"Tangtang", 2.5 head-tall proportions, big oversized red knit beanie with pointed
top and pom-pom (#E63946), white body (#F5F0E8), dark brown ski boots, simple
capsule arms holding rounded ski poles, on small skis, side 3/4 view facing right,
cheerful happy expression, gliding skiing pose, gender-neutral, NOT chubby,
single character centered, transparent background
```

### 3.2 糖糖 — 动作姿势组（跳跃 / 翻滚 / 摔倒 / 胜利 / 失败）`P0`
- **用途**：状态切换静态帧（动画过渡靠 DOTween 形变，关键姿势用图）
- **路径**：`Assets/Art/Characters/Tang/Tang_<Pose>.png`（Jump / Roll / Stumble / Win / Fail）
- **规格**：同 3.1；**务必引用 3.1 作 Style Reference 保证同一只糖糖**
- 逐姿势 prompt 主体（前缀+负向同上，仅替换动作句）：
  - **Jump**：`...mid-air jumping pose, knees tucked, arms up, excited open-mouth expression...`
  - **Roll**：`...curled into a tight ball doing a forward roll, motion-friendly round silhouette...`
  - **Stumble**：`...tripping forward, arms flailing, dizzy surprised expression, swirl eyes...`
  - **Win**：`...victory pose both arms raised, star sparkles, big joyful smile...`
  - **Fail**：`...cartoon defeated pose, lying back with tiny "out of sugar" dizzy spiral eyes, still cute and humorous, not sad or scary...`

### 3.3 NPC：胰岛素精灵 `P1`
- **路径**：`Assets/Art/Characters/NPC/Sprite_Insulin.png`
```
[STYLE PREFIX], a friendly teardrop-shaped sprite creature, soft blue water-drop
robe (#4CC9F0), tiny cute wings, gentle helpful smiling face, glowing soft aura,
rounded harmless silhouette, single character centered, transparent background
```

### 3.4 NPC：血糖仪精灵 `P2`
- **路径**：`Assets/Art/Characters/NPC/Sprite_Meter.png`
```
[STYLE PREFIX], a rounded-rectangle robot-like device sprite, small screen face
with friendly pixel eyes, a few buttons, neutral helpful look, blue and white body
with small red accent (#E63946), machine-but-cute, single object centered,
transparent background
```

### 3.5 NPC：饮食辅导员 `P2`
- **路径**：`Assets/Art/Characters/NPC/Sprite_Coach.png`
```
[STYLE PREFIX], a pear-shaped chibi coach character, green sportswear (#06D6A0),
holding a stopwatch, energetic cheerful expression, whistle, lively motivating pose,
single character centered, transparent background
```

### 3.6 Boss：血糖巨魔 `P2`（扩展，L2 用）
- **路径**：`Assets/Art/Characters/Boss/Boss_GlucoseTroll.png`
- **形状例外**：障碍/Boss 允许棱角 + 锯齿王冠，比友方更复杂
```
[STYLE PREFIX] but with menacing edge: an irregular jagged-polygon sugar troll
boss, dark red body (#C1121F) wrapped in red mist, jagged crown, grumpy comic
villain face with thick black outline, glowing purple energy veins (#7B2CBF),
imposing but still cartoonish not gory, single character centered, transparent background
```

---

## 4. 道具图标（Item Icons）— 优先级 P0

> 一眼可识别（Art Bible §16）；圆润身，危险道具(注射器)可带尖锐针头。同时出 **世界 sprite**（关卡内）和 **UI 图标**（HUD 道具栏，圆角方形边框、饱和度略降）。MVP 三件套 = 胰岛素喷雾 + 降糖药瓶 + 高糖雪花。

### 4.1 胰岛素喷雾 `P0`（MVP 必做）
- **路径**：`Assets/Art/Items/Item_InsulinSpray.png` / UI：`Assets/Art/UI/Icons/Icon_Insulin.png`
```
[STYLE PREFIX], a cute cartoon insulin spray pen item, rounded body, soft blue glow
ring around it (#4CC9F0), white core highlight, friendly not medical-scary, small
sparkle, single item icon centered, transparent background
```

### 4.2 降糖药瓶 `P0`（MVP 必做）
- **路径**：`Assets/Art/Items/Item_Pills.png` / `Icon_Pills.png`
```
[STYLE PREFIX], a cute rounded pill bottle item, white body (#F5F0E8) with green
cap and green healing steam rising (#06D6A0), friendly cartoon, single item icon
centered, transparent background
```

### 4.3 高糖雪花 `P0`（MVP 必做，双刃剑视觉）
- **路径**：`Assets/Art/Items/Item_SugarSnowflake.png` / `Icon_SugarSnowflake.png`
- **形状要点**：保留六芒星认知结构但**尖端圆润化**（Art Bible §3 裁决案例）
```
[STYLE PREFIX], a hexagonal sugar-crystal snowflake item with ROUNDED tips (not
sharp), red-orange glowing core (#FF9F1C), sunny yellow outer glow (#FFD166),
sparkly tempting candy look, single item icon centered, transparent background
```

### 4.4 血糖计 `P1`（W3）
- **路径**：`Assets/Art/Items/Item_GlucoseMeter.png`
```
[STYLE PREFIX], a cute handheld glucose meter device, rounded body, small glowing
screen with friendly digits, blue-white with yellow highlight, single item icon
centered, transparent background
```

### 4.5 运动靴 `P1`（W3）
- **路径**：`Assets/Art/Items/Item_SportsBoots.png`
```
[STYLE PREFIX], a pair of cute energetic sport boots with small motion sparkles
and tiny wings, green and yellow accents (#06D6A0 #FFD166), bouncy lively look,
single item icon centered, transparent background
```

---

## 5. 障碍物 & 收集物（Obstacles & Pickups）— 优先级 P0

> 形状契约：障碍物剪影**比友方更复杂/更尖角**（棱角=威胁）；收集物圆润+暖色>70%；收集物与障碍物形状差异度 > 60%。代码已有 5 类障碍（Stumble/Crash/Jumpable/Rollable/Avoidable）。

### 5.1 雪块 / 可跳障碍（Jumpable）`P0`
- **路径**：`Assets/Art/Obstacles/Obs_SnowMound.png`
```
[STYLE PREFIX], a small rounded snow mound obstacle, white and ice-blue (#90E0EF),
low cartoon bump shape you could jump over, thick black outline, single object
centered, transparent background
```

### 5.2 炸糖块 / 可翻滚障碍（Rollable）`P0`
- **路径**：`Assets/Art/Obstacles/Obs_SugarBlock.png`
```
[STYLE PREFIX], a stack of cartoon sugar cubes obstacle, amber accents (#F4A261),
slightly angular threatening but cute, thick black outline, single object centered,
transparent background
```

### 5.3 低血糖冰刺 / 不可豁免障碍（Avoidable）`P0`
- **路径**：`Assets/Art/Obstacles/Obs_IceSpike.png`
```
[STYLE PREFIX], a triangular jagged ice spike obstacle, deep blue and white
(#023E8A #90E0EF), sharp angular warning silhouette but stylized cartoon, dark
edge, single object centered, transparent background
```

### 5.4 高血糖漩涡（L2 障碍）`P1`
- **路径**：`Assets/Art/Obstacles/Obs_HyperVortex.png`
```
[STYLE PREFIX], a swirling spiral hazard, dominant red (#E63946) over 50%, red mist,
cartoon comic energy swirl, menacing but playful, single object centered,
transparent background
```

### 5.5 通用收集物 / 能量星（健康绿）`P0`
- **路径**：`Assets/Art/Pickups/Pickup_Star.png`
```
[STYLE PREFIX], a plump round collectible energy star, health-green body (#06D6A0)
with warm yellow glow (#FFD166), gentle floating sparkle, very rounded friendly,
single item centered, transparent background
```

### 5.6 检查点旗 / 终点：医院降糖站 `P0`
- **路径**：`Assets/Art/Environment/HospitalStation.png`
- **规格**：宽幅 ~1024×768，可半透明（地面接触处）
```
[STYLE PREFIX], a cute cartoon snow clinic / hospital station building at the run's
end, white walls (#F5F0E8) with friendly green cross (#06D6A0), rounded roof, warm
glowing windows, gold rim-light highlight, welcoming safe haven, single building
centered, transparent background
```

---

## 6. 环境与 Parallax 背景（Environment）— 优先级 P0/P1

> Art Bible §3 子主题 2：远景几何复杂度低、近景高；§2 三区域调色。**每关 = 远/中/近 三层 + 天空**。建议宽幅横向无缝倾向（实际无缝靠美术后处理，T2I 出基底）。

### 6.1 L1 医院雪屋 — 三层背景 `P0`（主玩法关）
- **路径**：`Assets/Art/Environment/L1/L1_<Far|Mid|Near>.png`、`L1_Sky.png`
- **调色**：蓝绿白主导 + 暖黄高光，对比弱、干净
  - **Sky**：`[STYLE PREFIX], wide gradient sky background, light blue daytime, soft cool-to-warm gradient, clean calm, gentle drifting cartoon clouds, no characters, seamless wide background`
  - **Far（远景，低复杂度）**：`[STYLE PREFIX], distant simplified rounded snow mountains silhouette row, single soft blue gradient (#48CAE4 #023E8A), no texture, minimal, wide parallax background layer, no characters`
  - **Mid（中景，中复杂度）**：`[STYLE PREFIX], midground geometric triangle pine trees with rounded trunks, two-tone dark green with white edges, a small cozy snow chalet with chimney puffing teal herbal steam, wide parallax layer, transparent background`
  - **Near（近景，高复杂度）**：`[STYLE PREFIX], foreground snowy slope ground, hand-drawn snow curves and texture, white and ice-white (#F5F0E8 #90E0EF), detailed snow sparkle, wide tileable-leaning ground strip, transparent top`

### 6.2 L2 高血糖狂暴雪区 — 三层背景 `P1`
- **路径**：`Assets/Art/Environment/L2/L2_<Far|Mid|Near>.png`、`L2_Sky.png`
- **调色**：红橙黑高饱和、对比极强、锐角山脊+锯齿云
  - **Sky**：`[STYLE PREFIX], wide dramatic sky, deep red and orange-red gradient (#E63946 #C1121F), rolling thunderclouds, high contrast oppressive but cartoon, no characters, seamless wide background`
  - **Far**：`[STYLE PREFIX], distant sharp angular jagged mountain ridges silhouette, dark red gradient, jagged comic clouds, wide parallax layer, no characters`
  - **Mid**：`[STYLE PREFIX], midground angular red rock formations and giant sugar blocks, amber rim highlights (#FF9F1C), carnival comic energy, wide parallax layer, transparent background`
  - **Near**：`[STYLE PREFIX], foreground red-tinted slope ground with cracks, blood-sugar red and cocoa black (#E63946 #2D2A32), aggressive texture, wide ground strip, transparent top`

### 6.3 L3 低血糖冰窟 — 三层背景 `P2`（弹性）
- **路径**：`Assets/Art/Environment/L3/L3_<Far|Mid|Near>.png`、`L3_Sky.png`
- **调色**：深蓝白低饱和、不规则冰晶+锥刺、雾化沉静
  - **Sky/Cave**：`[STYLE PREFIX], deep blue ice cave interior background, frozen stalactite icicles with cold white glow, very low brightness, low contrast, misty, calm but eerie cartoon, no characters, seamless wide background`
  - **Far**：`[STYLE PREFIX], distant irregular ice crystal formations silhouette, deep blue (#023E8A), faint, slow misty depth, wide parallax layer, no characters`
  - **Mid**：`[STYLE PREFIX], midground irregular ice crystals and cones, deep blue with insulin-blue edges (#4CC9F0), thick fog layer, wide parallax layer, transparent background`
  - **Near**：`[STYLE PREFIX], foreground icy slope ground, ice-white and deep blue (#90E0EF #023E8A), dense floating large ice particles feel, wide ground strip, transparent top`

### 6.4 减肥营地（扩展，绿区）`P2`
- **路径**：`Assets/Art/Environment/Camp/Camp_<layer>.png`
```
[STYLE PREFIX], bright cheerful green outdoor camp scene, lush green hills,
clear teal-blue sky, colorful tents picnic blankets and balloons, warm sunlight,
geometric blocks and rings shapes, energetic healthy mood, wide parallax background
```

---

## 7. UI 资产（UI / HUD）— 优先级 P0/P1

> Art Bible §3 子主题 3 + §4 子主题 4：UI 圆角、胶囊形、弧形弹药计血糖条；可可黑 80% 透明卡片背景保可读；血糖数字用深色保对比。**UI 多为 🟡（出概念图，精确切图/九宫格需后处理）**。

### 7.1 弧形血糖条（Arc Glucose Bar）`P0`
- **路径**：`Assets/Art/UI/HUD/GlucoseBar_Frame.png` + `GlucoseBar_Fill_<Low|Normal|High>.png`
- **注意**：分**外框**和**填充**两张出图，便于代码按血糖值改填充
```
[STYLE PREFIX], a horizontal capsule-shaped arc ammo-style meter UI frame, rounded
end-caps, one end a tiny Tangtang head icon other end a pill icon, clean cartoon
game HUD, flat, centered on transparent background
```
- 填充三档分别加色：低=`deep blue gradient (#023E8A #4CC9F0)`、正常=`health-green gradient (#06D6A0)`、高=`red-purple gradient (#E63946 #7B2CBF)`

### 7.2 通用按钮（Default / Hover / Pressed）`P1`
- **路径**：`Assets/Art/UI/Buttons/Btn_<state>.png`
```
[STYLE PREFIX], a rounded-rectangle cartoon game UI button, insulin-blue fill
(#4CC9F0), soft bevel, friendly, empty (no text), centered on transparent background
```
（hover 版加 `with sunny-yellow 2px outline (#FFD166), slightly bouncy`）

### 7.3 圆角面板 / 菜单气泡 `P1`
- **路径**：`Assets/Art/UI/Panels/Panel_Bubble.png`（建议出图后切九宫格 9-slice）
```
[STYLE PREFIX], a rounded-rectangle speech-bubble style UI panel, cocoa-black
semi-transparent fill (#2D2A32), soft rounded corners, clean game menu card, empty,
centered on transparent background
```

### 7.4 道具栏图标边框（圆角方形）`P1`
- **路径**：`Assets/Art/UI/Frames/ItemSlot.png`
```
[STYLE PREFIX], a rounded-square item slot frame for a game inventory bar, 2px
clean outline, subtle inner shadow, empty center, centered on transparent background
```

### 7.5 游戏 LOGO / 标题 `P2`
- **路径**：`Assets/Art/UI/Logo/Logo_SugarRush.png`
- **注意**：T2I 出**图形/装饰**好，**文字易出错**——文字部分建议用字体在 Unity/Figma 排，T2I 只出周边糖晶/雪山装饰
```
[STYLE PREFIX], a game title logo decoration (NO text), floating sugar crystals,
warm pink-orange dusk snow mountain motif, festive whimsical, centered on
transparent background
```

### 7.6 结算面板装饰（胜利礼花 / 失败谢幕）`P2`
- 胜利：`...golden confetti and sugar-crystal fireworks burst, warm gold-orange, celebratory...`
- 失败：`...giant cartoon "out of sugar" candy-apple mascot, grape-purple comic curtain mood, humorous not depressing...`

---

## 8. 生成后导入工作流（Unity）

1. **去背景**：T2I 出的"透明背景"常带残底 → 用 Photopea / remove.bg / Firefly 自带去背景清理 alpha。
2. **切图**：多姿势/图集用 Unity **Sprite Editor**（Sprite Mode: Multiple）切分；单体直接 Single。
3. **导入设置**（Inspector）：
   - Texture Type = `Sprite (2D and UI)`
   - Pixels Per Unit：角色/障碍统一一个基准（建议 100，糖糖目标世界高度 ~1.6 → 出图 ~160px 有效区）
   - Filter Mode = `Bilinear`（卡通描边）；Compression 视包体调
   - Pivot：角色 `Bottom`（贴地），道具 `Center`
4. **命名规范**（呼应项目 PascalCase）：`Tang_Ski.png`、`Item_InsulinSpray.png`、`L1_Far.png`、`Icon_Pills.png`。
5. **代码挂载**：当前白块由程序生成（`L1SceneBuilder` 用 `WhiteSprite.png`）。导入真图后，把 SpriteRenderer 的 sprite 引用换成新资源；建议后续把"类型→sprite"映射做成 ScriptableObject 表，避免硬编码。
6. **占位标注**：仍是 AI 占位的资产，在引用处留 `// ASSET: 待美术终稿` 注释。
7. **许可归档**：每张图记录来源工具 + license，存 `design/art/asset-license-log.md`（建议新建）。

---

## 9. 不走文生图的资产（改用其他方案）

| 资产 | 为什么不文生图 | 推荐方案 |
|------|--------------|---------|
| 角色滑行/跳跃/翻滚**动画** | 帧间一致性差 | 单姿势 sprite（本清单 §3.2）+ Unity Animation / DOTween 形变；或 Spine/骨骼 |
| 雪坡 tilemap / 无缝地形 | 无缝拼接 + 碰撞对齐做不到 | Asset Store 卡通雪地 tileset，或近景背景图(§6)当视觉、碰撞用纯 Collider |
| 粒子运动（暴风雪/礼花/速度线） | T2I 出静态单颗可以，运动不行 | Unity Particle System（单颗贴图来自 §7 概念）+ Shader |
| vignette / 画面抖动 / 高斯模糊 / 去饱和 | 是后期效果不是图片 | Built-in Post-processing + C# 脚本（Art Bible §2 已列方案） |
| 音效（咻/叮/嗖/心跳） | 非视觉 | ElevenLabs Sound Effects / Freesound（见 ASSET_RESOURCES.md） |
| BGM（轻快电子） | 非视觉 | Suno / Mubert（商用复核） |
| 字体 / LOGO 文字 | T2I 文字易畸变 | Google Fonts 圆润无衬线 + 在 Unity/Figma 排版 |

---

## 10. 生成优先级排期建议

| 批次 | 内容 | 理由 |
|------|------|------|
| **Batch 1（P0，先做）** | 糖糖 6 姿势 + MVP 三道具 + 5 障碍/收集物 + 医院降糖站 + L1 三层背景 + 弧形血糖条 | 直接替换 L1 全部白块，立刻让主玩法"有成品感" |
| **Batch 2（P1）** | L2 三层背景 + 高血糖障碍 + 血糖计/运动靴 + 按钮/面板/道具栏框 | 支撑 W3 L2 与 UI 完整化 |
| **Batch 3（P2，弹性）** | NPC 三种 + Boss + L3 冰窟 + 减肥营地 + LOGO/结算装饰 | 扩展关卡与打磨 |

> **建议流程**：先只生成 **§3.1 糖糖滑行** 一张，由 Art Director 确认风格 → 锁定为全局 Style Reference → 再批量跑 Batch 1。一致性的成败在第一张。

---

## 附录：资产清单速查（生成 checklist）

```
P0 角色   [ ] Tang_Ski [ ] Tang_Jump [ ] Tang_Roll [ ] Tang_Stumble [ ] Tang_Win [ ] Tang_Fail
P0 道具   [ ] InsulinSpray [ ] Pills [ ] SugarSnowflake (+ 3 个 UI Icon)
P0 障碍   [ ] SnowMound [ ] SugarBlock [ ] IceSpike  收集 [ ] Star  终点 [ ] HospitalStation
P0 背景   [ ] L1_Sky [ ] L1_Far [ ] L1_Mid [ ] L1_Near
P0 UI     [ ] GlucoseBar_Frame [ ] Fill_Low [ ] Fill_Normal [ ] Fill_High
P1 道具   [ ] GlucoseMeter [ ] SportsBoots
P1 障碍   [ ] HyperVortex
P1 背景   [ ] L2_Sky/Far/Mid/Near
P1 UI     [ ] Btn(3态) [ ] Panel_Bubble [ ] ItemSlot
P2 NPC    [ ] Insulin [ ] Meter [ ] Coach   Boss [ ] GlucoseTroll
P2 背景   [ ] L3(4) [ ] Camp(4)
P2 UI     [ ] Logo [ ] 结算装饰(胜/负)
```
</content>
</invoke>
