# 《糖大冒险：雪山狂飙》— 道具库扩展 + 文生图资产

> **文档版本**：v0.1
> **创建日期**：2026-06-20
> **依据**：`design/gdd/game-concept.md` §7 机制转译矩阵、`design/gdd/item-pickup-system.md`、`design/art/art-bible.md`
> **配套**：`design/art/t2i-prompt-cards.md`（已含 B1–B5 五个基础道具 prompt）
> **状态**：草案 — 待 game-designer / Art Director 审阅
> **范围**：5 个已规划道具 + 7 个新增**医学映射**道具，组成完整血糖管理光谱；含每个新道具的自包含文生图 prompt。

---

## 0. 设计原则（为什么不能随便加道具）

本项目是 Serious Game，最高得分项是**机制转译**——道具不是通用 buff/debuff，而是真实糖尿病管理概念的游戏化身（game-concept §6/§7）。因此**每个新道具必须满足三条**：

1. **有医学依据**：对应一个真实的控糖手段或风险因素（饮食、用药、运动、监测、生活方式、风险源）。
2. **有教学点**：玩家用过后能下意识理解一条控糖知识。
3. **视觉一眼可读**：形状/配色遵守 art-bible §3/§4——收集物圆润暖色、负面道具棱角冷色/警示。

> 医学准确性走"课程标准"（game-concept §15）：概念方向正确即可，**不显示具体 mmol/L 数值、不暗示真实剂量**。

---

## 1. 完整道具库总表（血糖管理光谱）

> 效果列对齐现有 `GlucoseSystem` API（`instantDelta` / `deltaPerSecond` / `duration` / `speedModifierBonus` / `controlModifierBonus` / `cooldown`）。**数值均为起点，需内部试玩校准。** `★` = 已实现，`◆` = 策划案已规划(W3)，`✚` = 本次新增。

### 升糖源（Glucose-Raising）— 风险/收益与陷阱

| 道具 | 状态 | 医学映射 | 游戏效果（起点值） | 教学点 | 视觉语言 |
|------|:--:|---------|------------------|--------|---------|
| 高糖雪花 Sugar Snowflake | ★ | 高糖食物 | `+25` 即时；3s `+0.15` 速度 | 高糖食物短爽长险 | 圆角六芒星·红橙核+黄晕 |
| 含糖汽水 Sugary Soda | ✚ | 含糖饮料（升糖最快） | `+35` 即时；3s `+0.25` 速度；冷却 6s | 含糖饮料比固体升糖更猛更快 | 红色易拉罐·气泡·暖色 |
| 压力闹钟 Stress Clock | ✚ | 压力/皮质醇升糖（负面陷阱，碰到即触发） | 进入区域内 4s 被动 `+3/s`，无任何收益 | 压力会悄悄推高血糖 | **棱角**紫色闹钟·乌云·警示 |

### 急救降糖（Emergency Lowering）

| 道具 | 状态 | 医学映射 | 游戏效果（起点值） | 教学点 | 视觉语言 |
|------|:--:|---------|------------------|--------|---------|
| 胰岛素喷雾 Insulin Spray | ★ | 紧急胰岛素 | `-30` 即时；冷却 3s | 高血糖即时急救 | 圆润喷笔·蓝光环 |
| 胰岛素泵 Insulin Pump | ✚ | 胰岛素泵持续给药 | `-6/s` 持续 6s；冷却 10s | 泵=平稳持续给药，不靠单次猛降 | 圆润小泵·蓝白·细管圈 |
| 降糖药瓶 Hypoglycemic Pill | ★ | 口服降糖药 | `-8/s` 持续 5s；冷却 8s | 长效防控 vs 短期急救 | 圆润药瓶·绿盖绿蒸汽 |

### 慢控稳定（Slow Control / Stabilize）

| 道具 | 状态 | 医学映射 | 游戏效果（起点值） | 教学点 | 视觉语言 |
|------|:--:|---------|------------------|--------|---------|
| 全谷低 GI 餐 Whole-Grain Meal | ✚ | 低 GI 食物（平稳升糖） | `+12` 即时（温和），无骤升 | 低 GI 食物升糖平缓更安全 | 圆润全麦面包·暖棕黄 |
| 膳食纤维蔬菜 Fiber Veggie | ✚ | 膳食纤维延缓糖吸收 | 8s 内血糖漂移幅度 `-50%`（需 GlucoseSystem 加 1 个漂移系数钩子） | 纤维延缓血糖波动 | 圆润西兰花/沙拉·健康绿 |
| 水壶补水 Water Bottle | ✚ | 补水温和助控糖 | `-5` 即时（温和）；5s `+0.05` 操控 | 多喝水有助血糖稳定 | 圆润水壶·胰岛素蓝·水滴 |

### 生活方式增益（Lifestyle Buffs）

| 道具 | 状态 | 医学映射 | 游戏效果（起点值） | 教学点 | 视觉语言 |
|------|:--:|---------|------------------|--------|---------|
| 运动靴 Sports Boots | ◆ | 运动干预 | 6s 跳跃距离/滞空 `+` ；轻微 `-2/s` | 运动正向控糖 | 圆润运动靴·小翅膀·绿黄 |
| 睡眠枕 Rest Pillow | ✚ | 睡眠改善胰岛素敏感性 | 清除警戒状态；6s `+0.2` 操控 | 充足睡眠帮助控糖 | 圆润月亮枕·柔紫蓝·Zzz |
| 血糖计 Glucose Meter | ◆ | 自我监测 | 显示精确状态 + 短暂冲刺 | 定期监测的价值 | 圆润仪器·小屏·蓝白黄 |

**完整光谱 = 12 个**：3 已实现 + 2 已规划 + **7 新增**。新增 7 个里 6 个正面（升糖/急救/慢控/增益）、1 个负面陷阱（压力），构成"升得太高有汽水的诱惑、降不下来有泵和药、想稳住有纤维和水、想更强有运动和睡眠、还有压力在偷袭"的完整决策空间。

> **道具栏建议**：配合 active.md 待办的"3 槽位 PlayerInventory 重构"，扩展库才有意义——单槽位装不下这条光谱。本文档只负责设计与美术；代码重构是另一项工作（你选了"生成美术资产"，故此处不动代码）。

---

## 2. 新增道具 — 文生图 Prompt 卡片（可单条复制）

> 格式同 `t2i-prompt-cards.md`：风格/配色/负向提示已全部内联，复制整块直接生成。**先用 t2i-prompt-cards.md 的 A1 糖糖锁定全局风格参考，再跑这些**，保证同一画风。
> 已有 5 个基础道具的 prompt 见 `t2i-prompt-cards.md` 的 **B1–B5**，此处不重复。

### N1. 含糖汽水 `Item_SugarySoda.png`（升糖源·更猛）
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute cartoon soda can with fizzy bubbles popping out the top, glossy red body (#E63946) with sunny-yellow label accent (#FFD166), tempting sugary drink, small sparkle, very rounded and appealing
NEGATIVE: photorealistic, 3D render, real brand logo, text, watermark, scary, busy background, muddy colors, low contrast
```

### N2. 压力闹钟 `Item_StressClock.png`（负面陷阱·棱角警示）
```
2D game art, cute cartoon style with slightly angular warning shapes, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single object, centered, transparent background, a grumpy angular alarm clock hazard with sharp bell tops, grape-purple body (#7B2CBF) with a small dark storm cloud and tiny lightning above it, stressed annoyed comic face, warning vibe but still cartoonish, NOT cute or collectible-looking
NEGATIVE: photorealistic, 3D render, friendly, rounded soft, gore, real horror, text, watermark, busy background, muddy colors, low contrast
```

### N3. 胰岛素泵 `Item_InsulinPump.png`（持续急救）
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute small rounded insulin pump device with a tiny coiled tube, soft blue and white body (#4CC9F0 #F5F0E8), a gentle steady blue glow, a small friendly indicator light, NOT medical-scary
NEGATIVE: photorealistic, 3D render, realistic medical device, needle, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### N4. 全谷低 GI 餐 `Item_WholeGrain.png`（安全升糖）
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute round loaf of whole-grain bread with little seeds, warm brown and golden tones (#F4A261 #FFD166), wholesome healthy comfortable look, small soft glow, very rounded and friendly
NEGATIVE: photorealistic, 3D render, gross, moldy, scary, text, watermark, busy background, muddy colors, low contrast
```

### N5. 膳食纤维蔬菜 `Item_FiberVeggie.png`（减缓漂移）
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute bundle of fresh green vegetables, a plump broccoli and leafy greens, health-green tones (#06D6A0 #00C49A) with tiny water-drop freshness sparkles, wholesome energetic, very rounded and friendly
NEGATIVE: photorealistic, 3D render, gross, wilted, scary, text, watermark, busy background, muddy colors, low contrast
```

### N6. 水壶补水 `Item_WaterBottle.png`（温和稳血糖）
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute rounded water bottle with a clear blue liquid and a small splash drop on top, insulin-blue and white (#4CC9F0 #F5F0E8), refreshing clean look, gentle cool glow, very rounded
NEGATIVE: photorealistic, 3D render, plastic waste, scary, text, watermark, busy background, muddy colors, low contrast
```

### N7. 睡眠枕 `Item_RestPillow.png`（恢复操控/清警戒）
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute fluffy pillow with a small crescent moon and tiny floating Zzz symbols, soft lavender and pale blue (#7B2CBF light #90E0EF), calm cozy restful glow, very rounded and soft
NEGATIVE: photorealistic, 3D render, scary, dark, gloomy, text, watermark, busy background, muddy colors, low contrast
```

---

## 3. 已规划 5 道具的 prompt 索引（避免重复）

| 道具 | Prompt 卡片位置 |
|------|----------------|
| 胰岛素喷雾 | `t2i-prompt-cards.md` → B1 |
| 降糖药瓶 | `t2i-prompt-cards.md` → B2 |
| 高糖雪花 | `t2i-prompt-cards.md` → B3 |
| 血糖计 | `t2i-prompt-cards.md` → B4 |
| 运动靴 | `t2i-prompt-cards.md` → B5 |

---

## 4. 拾取特效（粒子贴图，可选文生图）

> 每个道具拾取/使用时的粒子由 Unity Particle System 驱动，但**单颗粒子贴图**可文生图后抠成软边小图。颜色对齐道具语义。

| 特效 | 用途 | 单颗贴图 prompt（截断粘贴） |
|------|------|---------------------------|
| 蓝色光环 | 胰岛素/泵/水 使用 | `soft glowing blue ring particle (#4CC9F0), radial gradient, transparent background, single particle texture, no outline` |
| 绿色蒸汽 | 药瓶/蔬菜/全谷 | `soft green steam wisp particle (#06D6A0), wispy soft edges, transparent background, single particle, no outline` |
| 红色糖晶 | 雪花/汽水 | `small red-orange sugar crystal sparkle particle (#FF9F1C), glowing, transparent background, single particle, no outline` |
| 紫色压力波 | 压力闹钟 | `dark purple stress ripple particle (#7B2CBF), jagged soft edge, transparent background, single particle, no outline` |
| 黄色星点 | 运动/睡眠/胜利 | `tiny sunny-yellow star sparkle particle (#FFD166), glowing soft, transparent background, single particle, no outline` |

---

## 5. 生成 checklist

```
新增道具世界 sprite
[ ] N1 SugarySoda  [ ] N2 StressClock  [ ] N3 InsulinPump  [ ] N4 WholeGrain
[ ] N5 FiberVeggie [ ] N6 WaterBottle  [ ] N7 RestPillow
已有 5 道具（见 t2i-prompt-cards.md B1–B5）
[ ] B1 Insulin [ ] B2 Pills [ ] B3 Snowflake [ ] B4 Meter [ ] B5 Boots
拾取特效粒子（可选）
[ ] 蓝环 [ ] 绿蒸汽 [ ] 红糖晶 [ ] 紫压力波 [ ] 黄星点
```

---

## 6. 下一步建议

1. **若要让这条道具光谱真正可玩**：需做 active.md 已列的 **3 槽位 PlayerInventory 重构** + 给 `ItemData`/`ItemEffect` 补 `cooldown/duration/charges` 字段（GDD §ItemData Schema 已设计好，代码尚未跟上）。我可以单独做这块。
2. **膳食纤维(N5) 的"减缓漂移"**需要 `GlucoseSystem` 加一个 `driftMultiplier` 临时系数钩子（现有 `ResolveZoneDrift` 改 1 处即可）。
3. **新增道具应回写 GDD**：建议把本表的 7 个新道具合并进 `design/gdd/item-pickup-system.md` 的 Item Table，并在 `design/registry/entities.yaml` 的 items 段登记（目前为空模板）。要我做吗？
```
</content>
