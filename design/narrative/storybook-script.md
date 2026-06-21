# 《糖大冒险：雪山狂飙》— 背景故事 + 玩法绘本脚本

> **文档版本**：v0.1（撰写中）
> **创建日期**：2026-06-21
> **性质**：美术交接稿（narrative brief + 分页插画脚本）。本文件用文字钉死"每页画什么、说什么、为什么"，供美术据此出图。**不是给玩家的成品**。
> **世界观框架**：A — 失衡的甜糖雪山
> **覆盖范围**：MVP 三关（L1 医院雪屋 / L2 高血糖狂暴雪区 / L3 低血糖冰窟）+ 减肥营地正向收尾
> **结构**：双线交替 —— 每章 1 故事页（推进剧情）+ 1 玩法页（拆解教学），共约 10 页
> **上游依据**：
> - `design/gdd/game-concept.md` 第 7 节「机制转译矩阵」、第 8 节「世界观与场景」
> - `design/art/art-bible.md` Section 2 氛围表、Section 4 调色板
> - `design/art/t2i-prompt-cards.md`（可复用提示词，如 A1 糖糖滑行）
> - 根目录已生成概念图：`img_*.png`、`ChatGPT Image*.png`（复用候选）

---

## 0. 背景故事（Lore）

### 0.1 一句话

> 甜糖雪山的「血糖天平」被搅乱了，最会滑雪的小糖人糖糖，要一路滑到山脚的医院降糖站，沿途亲手把失衡的山坡重新调平。

### 0.2 世界设定

很久以前，世界上有一座**甜糖雪山**——一座完全由糖、雪和奇思妙想堆成的山。山顶立着一座古老的装置：**血糖天平**。它不停地称量整座山的"甜度"，让雪山不太甜、也不太冷，恰到好处。住在山里的小糖人们因此过着平衡又快活的日子：滑雪、翻滚、晒太阳。

平衡，是这座山唯一的法则。甜一点会狂躁，冷一点会迟钝；只有不偏不倚，山才安稳。

### 0.3 失衡之日

谁也说不清那天到底发生了什么。有人说是一只贪吃的怪物偷舔了天平的糖砝码——那就是后来盘踞在半山腰的**血糖巨魔**；有人说只是天平年久失修。总之，天平歪了。

天平一歪，整座山跟着发起脾气：
- **山顶**喷出滚烫的**红色高糖暴风雪**，棱角的糖块横飞，一切都快得失控——这是"太甜"的那一端。
- **山脚**冻结成幽蓝的**低血糖冰窟**，雾气浓得睁不开眼，身子重得迈不动腿——这是"太冷"的那一端。
- 唯有山脚最深处的**医院降糖站**还亮着绿色的十字灯，像一座等人回家的灯塔。

### 0.4 糖糖出发

糖糖是雪山里最会滑雪的小糖人——一颗顶着红毛球帽的圆滚滚汤圆，脚踩雪板，天不怕地不怕。当别的小糖人都躲进屋里，糖糖却系紧帽子站到了坡顶：

> "天平歪了，那就一路滑下去，把它一段一段地扳回来。"

糖糖要做的，不是"治病"，而是一场**冒险**：从平静的医院雪屋出发，冲过狂暴的高糖雪区、熬过迟缓的低血糖冰窟，最后抵达医院降糖站，让天平重新归零。每滑平一段山坡，世界就回到平衡一分。

而糖糖很快发现一件事——**这座山的脾气，其实就是自己身体里那杆秤**。山太甜，糖糖就发飘失控；山太冷，糖糖就迟钝模糊。想扳平整座山，得先学会扳平自己。

### 0.5 主题落点（设计自检，不进绘本正文）

- "找回平衡" = 控糖的精确隐喻：不是消灭糖，而是让它**不高不低**。
- 糖糖是**英雄**不是**病人**——符合 art-bible「会翻跟头的快乐小糖人，不是病房里的患者」与反刻板原则。
- 血糖巨魔给了 L2 一个具体对手，让"高血糖危机"有了脸。
- 终点不是"康复"，而是"天平归零、可以再来一局"——呼应游戏的可重玩性与尊严守则。

---

## 1. 角色与世界速查（给美术的锚点，引自 art-bible）

> 此节为现成设定的浓缩索引，方便美术开页时随手对齐；完整规格见 `design/art/art-bible.md`。

| 角色 | 造型锚点 | 主色 | 出场页 |
|------|----------|------|--------|
| **糖糖**（主角） | 汤圆身、2.5 头身、超大红毛球帽、白身棕靴 | 白 `#F5F0E8` + 帽红 `#E63946` | 全程 |
| **胰岛素精灵** | 泪滴形、蓝水滴袍、小翅膀、友善 | 胰岛素蓝 `#4CC9F0` | 第2章 L2 |
| **血糖仪精灵** | 圆角矩形机器、屏幕脸、中性指示 | 蓝白 + 红点 `#E63946` | 序章/玩法页 |
| **饮食辅导员** | 梨形、绿运动服、秒表哨子、活力 | 健康绿 `#06D6A0` | 终章营地 |
| **血糖巨魔**（Boss） | 不规则尖角、红雾缠绕、紫色能量纹、锯齿王冠 | 深红 `#C1121F` + 紫 `#7B2CBF` | 第2章 L2 |

| 区域 | 主色调 | 氛围一句话 | 形状语言 |
|------|--------|------------|----------|
| **L1 医院雪屋** | 蓝绿白（弱对比） | 安全的卡通诊所导览 | 圆角丘陵 + 蓬蓬云 |
| **L2 高血糖狂暴雪区** | 红橙黑（极强对比） | 失控赛道的过山车癫狂 | 锐角山脊 + 锯齿云 |
| **L3 低血糖冰窟** | 深蓝白（弱对比） | 黏稠窒息的慢动作困境 | 不规则冰晶 + 锥刺 |
| **减肥营地 / 医院降糖站** | 翠绿青蓝 + 暖黄 | 汗水与欢笑的户外派对 / 英雄凯旋 | 几何错落方块 + 圆环 |

---

## 2. 绘本分页脚本（双线交替，约 10 页）

> 每页统一模板：**画面描述 / 正文文案 / 视觉提示 / 机制锚点 / 复用资产 / 可选 T2I 提示词**。
> 〔故事页〕推进剧情，〔玩法页〕拆解教学。

---

### 第 1 页 · 失衡之日 〔故事页 — 序章〕

- **画面描述**：大跨页全景。画面被一条对角线劈成两半——上半截是山顶，喷涌着翻卷的**红色高糖暴风雪**，棱角糖块横飞；下半截是山脚，冻成幽蓝的**冰窟**，雾气弥漫。山顶歪斜的**血糖天平**（一杆卡通大秤，两端砝码一高一低）是视觉焦点。山脚最深处一点绿色十字灯（医院降糖站）微微发亮。糖糖此刻还很小、站在画面正中的坡顶，背对镜头眺望这一切，红毛球帽是全图唯一的暖色钉子。
- **正文文案**：
  > 很久以前，有一座甜糖雪山。山顶的"血糖天平"称量着整座山的甜度，让它不太甜、也不太冷——平衡，是这里唯一的法则。
  > 直到那天，天平歪了。山顶喷出滚烫的红色暴风雪，山脚冻成幽蓝的冰窟。
  > 只有山脚的医院降糖站，还亮着一盏绿灯。
- **视觉提示**：一图之内同时呈现三区配色对撞——红橙黑（上）/ 深蓝白（下）/ 中间过渡。对比拉满，制造"失衡"的不安。天平歪斜角度要夸张但卡通。禁止写实医疗、禁止恐怖。形状语言：上半锐角、下半冰晶锥刺、糖糖保持圆润。
- **机制锚点**：建立"平衡 = 生存"的母题（对应 `game-concept.md` §7 行1 血糖水平、§8 三大区域）。高=红、低=蓝的颜色语义在此第一次亮相，后续所有玩法页复用。
- **复用资产**：根目录 `ChatGPT Image*.png` / `img_*.png` 中若有全景雪山概念图可作背景底；糖糖站姿可裁切 `Tang_Ski.png`（T2I 卡 A1）。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon storybook illustration, wide cinematic landscape, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant kids-storybook palette, a fantasy candy snow mountain split diagonally: upper half erupting with a swirling RED high-sugar blizzard and sharp flying sugar cubes (#E63946, #C1121F, #2D2A32), lower half frozen into a deep-blue icy cavern with mist (#023E8A, #90E0EF), a giant tilted cartoon balance scale at the summit, a tiny glowing green cross beacon at the mountain base (#06D6A0), a small round chibi mascot with a red pom-pom beanie standing on a ridge seen from behind, sense of imbalance and adventure
  NEGATIVE: photorealistic, 3D render, realistic medical equipment, hospital horror, gore, scary, text, watermark, muddy colors, low contrast
  ```

### 第 2 页 · 你就是那杆秤 〔玩法页 — 操作与血糖条总览〕

- **画面描述**：信息图式构图。中央是糖糖的滑行姿势，四周用卡通气泡框标注操作：**Space=跳**（糖糖上方画跳跃弧线）、**Shift=翻滚**（画蜷成球的残影）、**A/D=左右调向**、**Q/E/1-3=用道具**（画三个道具槽）。画面顶部横贯一条**弧形血糖条**（art-bible「弧形弹药计」），从左到右标五档色块：深蓝(低危)→浅蓝(偏低)→翠绿(正常)→阳光黄(偏高)→血糖红(高危)，绿色档位打一个高亮"安全区"标记。一行小字把"山的脾气=身体的秤"画成糖糖站在天平上的小图标。
- **正文文案**：
  > 糖糖很快发现：这座山的脾气，就是自己身体里那杆秤。
  > **太甜会发飘失控，太冷会迟钝模糊——只有待在中间的绿色，才滑得又快又稳。**
  > 滑行中盯紧顶上的血糖条，用动作和道具，让指针别冲出绿色。
- **视觉提示**：血糖条必须是全页最显眼元素（art-bible 原则3：UI 必须赢）。五档配色严格用调色板 HEX。操作图标用统一圆角卡片底。色盲冗余：每档配一个形状/图标（绿=✓、黄=⚠、红=八边形），不靠纯颜色。
- **机制锚点**：直接图解 `GlucoseSystem` —— 五档 `GlucoseZone`（LowCrisis/LowWarning/Normal/HighWarning/HighCrisis）、血糖值 0–100、`SpeedModifier`/`ControlModifier`/`VisionModifier` 随血糖曲线变化（"太甜发飘=高血糖降操控"）。操作对应 `SugarRushInput`（跳/滚/调向/用道具）。
- **复用资产**：糖糖滑行 `Tang_Ski.png`（A1）；血糖条样式参考 `GlucoseUI` 现有实现截图。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon infographic style, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, a round red-beanie chibi mascot skiing in the center, surrounded by clean rounded speech-bubble control labels (jump arc, tucked roll, left-right arrows, three item slots), a large arc-shaped glucose meter across the top with five color segments deep-blue / light-blue / green / yellow / red (#023E8A #4CC9F0 #06D6A0 #FFD166 #E63946), green segment highlighted as a safe zone, friendly clean UI mood
  NEGATIVE: photorealistic, 3D render, realistic UI, dark, gore, text paragraphs, watermark, busy background, muddy colors, low contrast
  ```

### 第 3 页 · 从雪屋出发 〔故事页 — L1〕

- **画面描述**：温柔的入门关全景。浅蓝日间天空、柔和远山，中景是几座圆顶**医院雪屋**，烟囱冒出蓝绿色草药蒸汽（暗示"治疗"而非"恐惧"）。糖糖刚从一座雪屋门口出发，雪板压出一道轻快的雪痕，表情兴奋。一只**血糖仪精灵**（圆角矩形小机器，屏幕上一对友善像素眼）飘在糖糖身旁，像个领路向导。整体平静、干净、零压力。
- **正文文案**：
  > 第一段山坡最是平缓。这里是医院雪屋，连风都是软的。
  > 血糖仪精灵飘过来，眨了眨屏幕："跟我来，先把滑雪练熟——别急，这一段，怎么滑都摔不疼。"
  > 糖糖深吸一口气，蹬下了第一道坡。
- **视觉提示**：L1 主色蓝绿白、弱对比、无威胁感（art-bible 状态表 #2）。形状全圆角（圆顶丘陵 + 蓬蓬云）。血糖条在画面角落以浅绿脉冲，暗示"现在很安全"。基调：友好引导。
- **机制锚点**：L1 = 教学关，验证核心循环（`game-concept.md` §18 W2 目标 / `level-data-progression.md` L1 医院雪屋）。引入血糖仪精灵作为"自我监测"的化身（§7 行6），但 MVP 仅作叙事向导，不强加监测玩法。
- **复用资产**：糖糖滑行 `Tang_Ski.png`（A1）；血糖仪精灵 `Sprite_Meter.png`（A8）；L1 背景可复用根目录雪屋概念图。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon storybook illustration, thick cocoa-black outline, flat cel-shaded coloring, soft bright palette, gentle calm mood, a side-scrolling snowy slope with rounded dome cottages (a friendly cartoon snow clinic), chimneys puffing soft blue-green herbal steam, light-blue daytime sky, soft distant mountains, a round red-beanie chibi mascot starting to ski down leaving a light snow trail with a cheerful expression, a small rounded-rectangle meter-robot sprite with friendly pixel eyes floating beside as a guide, low contrast clean and safe
  NEGATIVE: photorealistic, 3D render, realistic hospital, scary, sharp edges, high contrast, gore, text, watermark, muddy colors
  ```

### 第 4 页 · 滑、跳、滚 〔玩法页 — L1 核心循环〕

- **画面描述**：横向三连格分镜（像连环画），演示三种基本应对：
  1. **跳（Jumpable）**：糖糖按 Space 腾空，跃过一道圆木栏；旁注小图标"够高才能过"。
  2. **滚（Rollable）**：糖糖蜷成球用 Shift 翻滚，从一道低矮拦杆下钻过；旁注"压低身子"。
  3. **稳住（被动漂移）**：糖糖平稳滑行，血糖条指针缓缓向中间漂——画一个向绿色回落的小箭头。
  画面底部一条血糖条贯穿，三格里指针都安稳停在绿色。
- **正文文案**：
  > **跳**过挡路的栏杆，**翻滚**钻过低矮的横木——看清障碍的形状，就知道该跳还是该滚。
  > 不撞上东西时，血糖会自己慢慢回到舒服的中间。在雪屋这一段，平衡几乎是免费的。
  > 把这套手感练进身体里，因为接下来的山，可不会这么客气。
- **视觉提示**：三格等宽，箭头/图标引导阅读顺序。障碍物比糖糖更尖角（art-bible：棱角=威胁信号），但 L1 障碍仅"挡路"不致命。跳跃弧线、翻滚残影用卡通动势线。
- **机制锚点**：图解 `Obstacle.ObstacleType` 五型中的 **Jumpable**（需离地，高度判定 `h = v²/(2g)` 对比 `SkiingConfig.jumpForce`）与 **Rollable**（需翻滚）；以及 `GlucoseSystem.ResolveZoneDrift()` 在 Normal 区的被动漂移 `driftNormal = -0.5/s`（"不撞就慢慢回中"）。对应 §7 行1。
- **复用资产**：糖糖跳跃 `Tang_Jump.png`（A2）、翻滚 `Tang_Roll.png`（A3）、滑行 `Tang_Ski.png`（A1）。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon comic strip, three equal panels, thick cocoa-black outline, flat cel-shaded coloring, bright soft palette, panel 1: a round red-beanie chibi mascot jumping over a rounded log fence with a motion arc; panel 2: the same mascot curled into a ball rolling under a low bar; panel 3: the mascot skiing smoothly with a small arrow showing a glucose pointer drifting back to a calm center; a colorful arc glucose meter along the bottom, clean instructional mood
  NEGATIVE: photorealistic, 3D render, scary, gore, dense text, watermark, busy background, muddy colors, low contrast
  ```

### 第 5 页 · 红色暴风雪 〔故事页 — L2〕

- **画面描述**：高能量大跨页。深红/橙红翻涌的天空、夹着卡通闪电，红色糖晶横向暴风雪。山坡变成锐角山脊，糖糖被速度甩出夸张的红色残影，表情又紧张又兴奋。半山腰，**血糖巨魔**第一次现身——一团不规则尖角、红雾缠绕、紫色能量纹、戴着锯齿王冠的卡通反派，正咧嘴搅动着风暴。血糖条指针被推向右侧的黄/红档，画面边缘泛起红色脉动。
- **正文文案**：
  > 第二段山坡，是"太甜"的那一端。
  > 红色暴风雪把一切都吹得飞快，糖块像炮弹一样砸下来。糖糖一眼就看见了搅乱天平的家伙——**血糖巨魔**，正坐在半山腰哈哈大笑。
  > "甜过头了！"糖糖感觉自己越滑越飘，血糖条的指针，正一格一格往红色冲。
- **视觉提示**：L2 主色红橙黑、对比极强、轻微画面抖动感（art-bible 状态表 #3）。形状锐角主导、锯齿云。血糖巨魔造型严格按 art-bible NPC 矩阵 + T2I 卡 A10（不规则多边形、红雾、紫能量纹、卡通而非恐怖）。残影/速度线用漫画语言。
- **机制锚点**：L2 = 展示"机制转译爽点"（§18 W3）。高血糖区被动漂移为正：`driftHighWarning = +0.8/s`、`driftHighCrisis = +1.2/s`（"越待越甜"），对应 §7 行7 高血糖危机。血糖巨魔把"高血糖危机"具象成有脸的对手。速度感来自 `SpeedModifier` 在高血糖端升高（爽但失控）。
- **复用资产**：糖糖滑行/紧张姿态 `Tang_Ski.png`（A1）；血糖巨魔 `Boss_GlucoseTroll.png`（A10）；L2 红色暴风雪背景可复用根目录红色场景概念图。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon storybook illustration, high energy chaotic mood, thick cocoa-black outline, flat cel-shaded coloring, saturated palette, a wild downhill ski run in a RED high-sugar blizzard with deep-red/orange churning sky and cartoon lightning (#E63946 #C1121F #FF9F1C #2D2A32), sharp jagged ridges and flying sugar cubes, a round red-beanie chibi mascot skiing fast with exaggerated red speed trails and a thrilled-nervous face, an irregular jagged sugar-troll boss with a spiky crown wrapped in red mist and glowing purple energy veins laughing on the slope, comic speed lines
  NEGATIVE: photorealistic, 3D render, real horror, gore, blood, depressing, text, watermark, muddy colors, low contrast, calm
  ```

### 第 6 页 · 甜的诱惑与救命的喷雾 〔玩法页 — L2 高糖雪花 + 胰岛素〕

- **画面描述**：左右对照两栏，讲两个关键道具的"选择压力"。
  - **左栏 · 高糖雪花（双刃剑）**：一片红橙色六边形糖晶悬在空中诱人地发光。糖糖伸手要拿，上方画两个后果气泡——绿色"⚡速度↑"和红色"血糖↑"，中间一个天平图标在摇摆。标题：**甜，是有代价的。**
  - **右栏 · 胰岛素喷雾（救命）**：血糖条指针已冲进红色高危区，屏幕边缘红色脉动；糖糖按下 E，一只**胰岛素精灵**（蓝水滴+小翅膀）洒出蓝白光环，指针被猛地拉回绿色。标题：**冲过头了？一下压回来。**
- **正文文案**：
  > **高糖雪花**会让你瞬间提速——但也会把血糖往红色推。要爽，还是要稳？这一口，你自己选。
  > 万一真的冲进了红色高危区，别慌：**胰岛素喷雾**是你的安全网，一下就把血糖压回安全线。
  > 但安全网不是无限的。聪明的滑手，是尽量不让自己走到要靠它的那一步。
- **视觉提示**：高糖雪花=六边形糖晶（区别于圆形收集物，色盲冗余），红橙核心 `#FF9F1C`+阳光黄外层 `#FFD166`。胰岛素=圆润友善、蓝光环 `#4CC9F0`，绝不画成冷冰冰真实针头（art-bible 原则1）。两栏用天平/箭头强调"权衡"。
- **机制锚点**：
  - 高糖雪花 → `HighSugarSnowflakeEffect`：短暂加速 + 提升血糖（§7 行2 高糖饮食"双刃剑"）。〔注：现有实现直接改 `Rigidbody2D.velocity`，是临时 hack，已挂 TODO(speed-buff) 待重构为 `ApplySpeedMultiplierBonus`——不影响绘本表达。〕
  - 胰岛素喷雾 → `InsulinSprayEffect`：`GlucoseSystem.ApplyDelta` 瞬间大幅降糖 + 清除警戒（§7 行3 胰岛素治疗）。
  - 红色高危 = `GlucoseZone.HighCrisis`，停留超过 `highCrisisFailTime` 触发 `OnCrisisFailed`（呼应"安全网不是无限的"）。
- **复用资产**：高糖雪花 `Item_*Snowflake.png`、胰岛素喷雾 `Item_InsulinSpray.png`（B1）、胰岛素精灵 `Sprite_Insulin.png`（A7）。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon split illustration two columns, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette; LEFT: a glowing red-orange hexagonal sugar crystal candy (#FF9F1C core, #FFD166 glow) tempting a round red-beanie chibi mascot, two outcome bubbles above (green speed-up and red glucose-up) with a wobbling balance icon between them; RIGHT: a red high-danger glucose meter and red screen pulse, the mascot pressing a button while a friendly teardrop-shaped blue insulin sprite with tiny wings releases a soft blue-white glowing ring (#4CC9F0) pulling the meter pointer back to green
  NEGATIVE: photorealistic, 3D render, realistic syringe, sharp needle, scary, gore, dense text, watermark, muddy colors, low contrast
  ```

### 第 7 页 · 滑进冰窟 〔故事页 — L3〕

- **画面描述**：压抑、安静的大跨页。深蓝冰晶洞窟，顶部钟乳冰棱透出冷白微光，浓密冰雾笼罩。**视野是收窄的**——近景的糖糖清晰，中景渐渐模糊，远景几乎被白雾吞没（卡通积雪雾化，不是真实眼前发黑）。糖糖滑得很慢，呵出一团白气，身上有蓝色抖颤的冷粒子，表情凝重。血糖条指针这次掉到了**左侧的蓝色低区**。
- **正文文案**：
  > 冲下高糖雪区，糖糖一头扎进了"太冷"的那一端——低血糖冰窟。
  > 这里和上面完全相反：雾让人看不清前路，身子像灌了铅，越来越慢。指针不再往红色冲，而是往蓝色掉。
  > "原来太低，和太高一样危险。"糖糖打了个寒颤，放慢了呼吸。
- **视觉提示**：L3 主色深蓝白、弱对比、轻微高斯模糊（art-bible 状态表 #4）。形状不规则冰晶+锥刺。视野渐隐是本页核心视觉语言（近清晰→远消失）。沉静而非恐怖——糖糖仍圆润可爱，呵气和冷粒子是卡通的。题材尊严：绝不模拟真实低血糖昏厥/发黑。
- **机制锚点**：L3 = 完整双向血糖体验（§18 / §7 行8 低血糖危机）。低血糖区被动漂移继续下探：`driftLowWarning = -0.3/s`、`driftLowCrisis = -0.2/s`；`VisionModifier`/`ControlModifier` 在低血糖端下降（视野收窄、操控迟缓）。冰窟的环境寒风用 `HazardZone.SetPassiveDelta(负值)` 持续抽血糖。
- **复用资产**：糖糖滑行 `Tang_Ski.png`（A1，可加冷色叠加）；L3 冰窟背景复用根目录蓝色冰窟概念图。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon storybook illustration, quiet oppressive mood, thick cocoa-black outline, flat cel-shaded coloring, low-contrast deep-blue palette (#023E8A #4CC9F0 #90E0EF), a side-scrolling deep-blue ice cavern with hanging icicles glowing cold white, dense fog, foreground sharp but midground blurred and background swallowed by white mist (cartoon snow haze, NOT darkness), a round red-beanie chibi mascot skiing slowly, breathing a puff of white air, small blue shivering cold particles, serious focused expression, still cute
  NEGATIVE: photorealistic, 3D render, real fainting, black-out vision, medical distress, scary, gore, text, watermark, high contrast, warm colors
  ```

### 第 8 页 · 慢下来的世界 〔玩法页 — L3 降糖药 / 用糖自救 / 危机计时〕

- **画面描述**：三个小格 + 一条贯穿的"危机倒计时"视觉。
  1. **用糖自救（反转！）**：糖糖血糖条掉进蓝色低危区，这次主动吃下一片**高糖雪花**——上一关的"坏东西"在这里救了命，指针被抬回绿色。气泡："低了？这时候，甜就是解药。"
  2. **降糖药瓶（长效）**：糖糖喝下绿色药瓶，画一个持续数秒的绿色缓降箭头。气泡："慢慢压、压得久——这是长效防控。"
  3. **危机计时**：当指针卡在最左(低危)或最右(高危)，糖糖头顶弹出漫画 ❗❗❗ 和一圈倒计时环。气泡："别在红色或蓝色里赖太久——会出局。"
- **正文文案**：
  > 在冰窟里，规则反过来了：刚才让你冲过头的**高糖雪花**，现在成了救命的解药——血糖太低时，吃一口，把它抬回中间。
  > **降糖药瓶**则相反，它慢慢地、持续地往下压，适合提前防着血糖走高。
  > 记住：无论太高还是太低，只要在危险档里待太久，冒险就结束了。平衡，永远是唯一的活路。
- **视觉提示**：第①格刻意和第6页"高糖雪花=诱惑"形成镜像对照——同一道具，两种语境，教"剂量与时机"。危机倒计时用漫画 ❗❗❗ + 倒计时环（art-bible 危机=漫画化，非真实医疗警报）。降糖药=绿色蒸汽 `#06D6A0`。
- **机制锚点**：
  - 用糖自救 → `HighSugarSnowflakeEffect` 在低血糖语境的正向用法（§7 行2 的另一面）。
  - 降糖药瓶 → `HypoglycemicPillsEffect`：`GlucoseSystem.ApplyBuffOverTime(负值, 持续时长)` 持续降糖（§7 行4 口服降糖药 / "长效 vs 急救"对比胰岛素）。
  - 危机计时 → `GlucoseSystem.UpdateCrisisTimers`：`lowCrisisFailTime` / `highCrisisFailTime` 到点触发 `Fail()`。
- **复用资产**：高糖雪花 `Item_*Snowflake.png`、降糖药瓶 `Item_*Pills.png`、糖糖滑行 `Tang_Ski.png`（A1）。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon comic strip three panels, thick cocoa-black outline, flat cel-shaded coloring, deep-blue cavern palette with green accents; panel 1: a round red-beanie chibi mascot in a low-blue glucose state eating a red hexagonal sugar crystal that lifts the meter pointer back to green; panel 2: the mascot drinking a green potion bottle with a slow descending green arrow (#06D6A0 steam); panel 3: the mascot stuck at a danger zone with comic ❗❗❗ above its head and a countdown ring; clear instructional contrast mood
  NEGATIVE: photorealistic, 3D render, real medical alarm, scary, gore, dense text, watermark, muddy colors, low contrast
  ```

### 第 9 页 · 天平归零 〔故事页 — 终章 减肥营地 + 医院降糖站〕

- **画面描述**：明亮欢快的胜利大跨页。冰窟尽头豁然开朗——翠绿山野、青蓝晴空、彩色帐篷和野餐布的**减肥营地**，蒲公英绒毛和阳光金尘飘浮。**饮食辅导员**（梨形、绿运动服、吹哨子）在路边给糖糖加油。山坡尽头，**医院降糖站**以金色描边登场，绿色十字灯大放光明，大门敞开。糖糖摆出胜利 pose 冲进去，身后星星粒子炸开；远处山顶那杆**血糖天平，终于回正了**。血糖条稳稳停在正中绿色。
- **正文文案**：
  > 穿过冰窟，眼前忽然亮了起来——这是减肥营地，阳光、草地、欢笑，连呼吸都变轻了。饮食辅导员吹响哨子："最后一段，跑起来！"
  > 山脚的医院降糖站亮起绿灯，大门为糖糖敞开。糖糖一个漂亮的冲刺，滑了进去。
  > 山顶的天平，慢慢、慢慢地，回到了正中。整座甜糖雪山，又平衡了。
- **视觉提示**：减肥营地=翠绿青蓝+暖黄、明度高、HDR 阳光感（art-bible 状态表 #5）；胜利时刻=金橙渐变、礼花粒子、慢动作（状态表 #9）。形状回归圆润+几何错落方块。医院降糖站金色描边高光。天平回正是闭环的关键视觉，务必画出与第1页歪斜天平的呼应。
- **机制锚点**：终点医院降糖站 = `FinishLine` 触发 `Victory`（`level-data-progression.md`：进入终点触发器→胜利）。减肥营地呼应 §7 行5 运动干预 / 行9 健康干预正反馈。天平归零 = 背景故事母题闭环（平衡=生存）。
- **复用资产**：糖糖胜利 `Tang_Win.png`（A5）；饮食辅导员 `Sprite_Coach.png`（A9）；营地/医院背景复用根目录绿色营地或医院概念图。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon storybook illustration, bright joyful victory mood, thick cocoa-black outline, flat cel-shaded coloring, vibrant warm palette, a lush green fitness camp opening up after an ice cavern (#06D6A0 #00C49A #FFD166), colorful tents and picnic blankets, floating dandelion fluff and golden sun dust, a pear-shaped green-sportswear coach blowing a whistle cheering, a round red-beanie chibi mascot striking a victory pose dashing into a friendly snow clinic with a glowing green cross and gold-outlined open doors, star sparkles bursting, a giant cartoon balance scale at the far summit returning to level, golden confetti
  NEGATIVE: photorealistic, 3D render, scary, gore, dark, medical horror, text, watermark, muddy colors, low contrast
  ```

### 第 10 页 · 你扳平了整座山 〔玩法页 — 运动靴 + S/A/B/C 评级〕

- **画面描述**：结算页式构图。上半：糖糖穿着发光的**运动靴**跃起，跳得又高又远，拖一道绿色活力轨迹——展示"运动让我更敏捷"。下半：一块卡通结算板，列出 **S / A / B / C** 四档奖章，S 档高亮，旁边三行小字打勾的"完美通关条件"。四周金色礼花。整体是"复盘+鼓励再来一局"的基调。
- **正文文案**：
  > **运动靴**让糖糖跳得更高、飘得更远——运动，是控糖路上最靠谱的伙伴。
  > 这一趟你滑得怎么样？看看奖章：
  > **★ 全程血糖不进警戒　★ 一次没摔　★ 完成数个翻滚特技** —— 三项全中，就是 S 档滑手。
  > 天平已经归零，但雪山永远欢迎你再来一局。
- **视觉提示**：运动靴=机动性增益、绿色活力轨迹 `#06D6A0`。结算板用胜利配色（金黄+绿，art-bible 胜利状态色）。奖章 S/A/B/C 用统一圆角徽章。语气轻松、鼓励、非评判（呼应受众尊严）。
- **机制锚点**：
  - 运动靴 → `SpeedBoostEffect` / 跳跃增益（§7 行5 运动干预；W3 弹性道具）。
  - 评级 → `game-concept.md` §13 完美通关条件（血糖始终正常、不摔倒、完成 N 个翻滚），`ResultPanel` 结算 S/A/B/C 四档。
  - "再来一局" → 呼应 art-bible 失败/胜利状态均强调可重玩与尊严守则。
- **复用资产**：糖糖跳跃 `Tang_Jump.png`（A2）、胜利 `Tang_Win.png`（A5）；结算板参考 `ResultPanel` 现有 UI。
- **可选 T2I 提示词**：
  ```
  2D game art, cute chibi cartoon results screen illustration, celebratory encouraging mood, thick cocoa-black outline, flat cel-shaded coloring, gold-and-green victory palette (#FFD166 #06D6A0); top: a round red-beanie chibi mascot wearing glowing green sport boots leaping high and far with a green energy trail; bottom: a cartoon results board showing four rounded medals S / A / B / C with the S medal highlighted, three small checkmarked condition lines, golden confetti around
  NEGATIVE: photorealistic, 3D render, scary, gore, dark, judgmental, dense text, watermark, muddy colors, low contrast
  ```

---

## 3. 给美术的整体注记

### 3.1 全书统一风格（每页都要守）

- **总则**：「卡通奇幻游乐场」（art-bible Section 1）。任何歧义都回到这一句裁决。
- **糖糖一致性**：每页同一只糖糖——汤圆身、2.5 头身、超大红毛球帽、白身棕靴。先锁 `Tang_Ski.png`（T2I 卡 A1）为 Style Reference，再画其余姿态。
- **描边与上色**：统一可可黑 `#2D2A32` 粗描边（2–4px）+ 平涂 cel-shading，禁止写实渐变/3D 质感。
- **配色即语义**：红=高血糖危险、蓝=低血糖/胰岛素、绿=安全奖励、黄=警戒、紫=失败/Boss。区域主色严格用 Section 4 调色板 HEX，跨页不得漂移。
- **题材尊严守则**：糖糖是英雄不是病人；危机用漫画语言（❗❗❗/速度线/螺旋眼）不模拟真实症状；不出现真实针头、真实血糖数值、"立即送医"类医疗用语。
- **色盲冗余**：凡靠颜色区分状态/道具处，必须叠加形状或图标（绿=✓、黄=⚠、红=八边形；高糖=六边形、收集物=圆形）。

### 3.2 双线节奏检查

| 页 | 类型 | 区域 | 情绪能量（art-bible 曲线） |
|----|------|------|--------------------------|
| 1 失衡之日 | 故事 | 全景 | 不安/预告 |
| 2 你就是那杆秤 | 玩法 | UI | measured |
| 3 从雪屋出发 | 故事 | L1 | measured 友好 |
| 4 滑、跳、滚 | 玩法 | L1 | 轻快 |
| 5 红色暴风雪 | 故事 | L2 | frenetic 狂热 |
| 6 甜的诱惑/喷雾 | 玩法 | L2 | 紧张+决策 |
| 7 滑进冰窟 | 故事 | L3 | 沉静（负面） |
| 8 慢下来的世界 | 玩法 | L3 | 紧张+反转 |
| 9 天平归零 | 故事 | 营地/终点 | 宣泄/喜悦 |
| 10 你扳平了整座山 | 玩法 | 结算 | measured 鼓励 |

> 节奏意图：平静(1-4) → 高潮(5-6) → 低谷压抑(7-8) → 释放(9-10)，与游戏内三区情绪曲线同构。

### 3.3 复用资产清单（已有，优先裁切/重上色）

- **T2I 提示卡现成角色/道具**（`design/art/t2i-prompt-cards.md`）：A1 糖糖滑行、A2 跳跃、A3 翻滚、A5 胜利、A7 胰岛素精灵、A8 血糖仪精灵、A9 饮食辅导员、A10 血糖巨魔、B1 胰岛素喷雾等。
- **根目录已生成概念图**：`img_1.png`…`img_13.png`、`ChatGPT Image*.png`——开工前由美术过一遍，匹配到对应页作背景底或参考（雪山全景→第1页；雪屋→第3页；红色场景→第5页；冰窟→第7页；营地/医院→第9页）。

### 3.4 待生成图清单（本绘本新增，约 10 张跨页）

每页一张主图（见各页"可选 T2I 提示词"）。建议生产顺序：先出第 1、5、9 页三张"区域定调跨页"确认风格基调，再补其余玩法页分镜。玩法页（2/4/6/8/10）信息密度高，建议美术在主图基础上叠加 UI 标注层，文字最终以中文重排。

### 3.5 与代码/设计的对齐声明

本脚本所有"机制锚点"均回链至现有系统，确保美术表达不与实现脱节：
`GlucoseSystem`（五档区域 + per-zone 漂移 + 危机计时）、`Obstacle`（Jumpable/Rollable 等五型）、`HighSugarSnowflakeEffect` / `InsulinSprayEffect` / `HypoglycemicPillsEffect` / `SpeedBoostEffect`、`FinishLine`(Victory)、`ResultPanel`(S/A/B/C)。机制如有重构（见 active.md 的 speed-buff / inventory TODO），仅影响实现细节，不改变绘本叙事表达。
