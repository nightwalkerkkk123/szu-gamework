# 《糖大冒险：雪山狂飙》— 文生图 Prompt 卡片（可单条复制）

> **文档版本**：v0.1
> **创建日期**：2026-06-20
> **配套**：`design/art/text-to-image-asset-manifest.md`（含路径/尺寸/工作流）；本文件只放**提示词**。
> **用法**：每个 `###` 标题 = 一个资产。下面的代码块是**完全自包含**的提示词——风格、配色、负向提示已全部内联，**复制一整块直接粘贴**即可生成，无需再拼前缀。
> **建议**：先生成 **A1 糖糖滑行**，确认风格后锁为 Style Reference，再按顺序逐条生成其余。

---

## A. 角色 Characters（P0–P2）

### A1. 糖糖 — 滑行 `Tang_Ski.png` ⭐先生成
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright but not garish, vibrant kids-storybook palette, clean vector-like edges, game asset sprite, centered, transparent background, a round soup-dumpling shaped chibi mascot named Tangtang, 2.5 head-tall proportions, big oversized red knit beanie with pointed top and pom-pom (#E63946), white body (#F5F0E8), dark brown ski boots, simple capsule arms holding rounded ski poles, standing on small skis, side three-quarter view facing right, cheerful happy expression, smooth gliding skiing pose, gender-neutral, NOT chubby, single character
NEGATIVE: photorealistic, 3D render, realistic human anatomy, realistic medical equipment, hospital horror, gore, scary, text, watermark, signature, busy background, muddy colors, low contrast
```

### A2. 糖糖 — 跳跃 `Tang_Jump.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant kids-storybook palette, clean vector-like edges, game asset sprite, centered, transparent background, the same round soup-dumpling chibi mascot Tangtang with oversized red pom-pom beanie (#E63946), white body (#F5F0E8), brown ski boots, 2.5 head-tall, mid-air jumping pose with knees tucked and arms up, excited open-mouth joyful expression, side three-quarter view facing right, single character
NEGATIVE: photorealistic, 3D render, realistic anatomy, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### A3. 糖糖 — 翻滚 `Tang_Roll.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant kids-storybook palette, game asset sprite, centered, transparent background, the same chibi mascot Tangtang with red pom-pom beanie (#E63946) and white body (#F5F0E8), 2.5 head-tall, curled into a tight ball doing a forward roll, motion-friendly round silhouette, determined cute expression, single character
NEGATIVE: photorealistic, 3D render, realistic anatomy, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### A4. 糖糖 — 摔倒/绊倒 `Tang_Stumble.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant kids-storybook palette, game asset sprite, centered, transparent background, the same chibi mascot Tangtang with red pom-pom beanie (#E63946) and white body (#F5F0E8), 2.5 head-tall, tripping forward with arms flailing, dizzy surprised expression with swirl spiral eyes, humorous not painful, single character
NEGATIVE: photorealistic, 3D render, realistic anatomy, gore, scary, blood, text, watermark, busy background, muddy colors, low contrast
```

### A5. 糖糖 — 胜利 `Tang_Win.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant kids-storybook palette, game asset sprite, centered, transparent background, the same chibi mascot Tangtang with red pom-pom beanie (#E63946) and white body (#F5F0E8), 2.5 head-tall, victory pose with both arms raised high, big joyful smile, small yellow star sparkles around (#FFD166), single character
NEGATIVE: photorealistic, 3D render, realistic anatomy, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### A6. 糖糖 — 失败/谢幕 `Tang_Fail.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant kids-storybook palette, game asset sprite, centered, transparent background, the same chibi mascot Tangtang with red pom-pom beanie (#E63946) and white body (#F5F0E8), 2.5 head-tall, gentle cartoon defeated pose lying back with tiny dizzy spiral eyes and a small sweat drop, still cute and humorous, NOT sad or scary, light grape-purple ambient tint (#7B2CBF), single character
NEGATIVE: photorealistic, 3D render, realistic anatomy, gore, scary, depressing, medical distress, text, watermark, busy background, muddy colors, low contrast
```

### A7. NPC — 胰岛素精灵 `Sprite_Insulin.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, game asset sprite, centered, transparent background, a friendly teardrop-shaped sprite creature wearing a soft blue water-drop robe (#4CC9F0), tiny cute wings, gentle helpful smiling face, soft glowing aura, rounded harmless silhouette, single character
NEGATIVE: photorealistic, 3D render, realistic anatomy, syringe needle, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### A8. NPC — 血糖仪精灵 `Sprite_Meter.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, game asset sprite, centered, transparent background, a rounded-rectangle robot-like device sprite with a small screen face showing friendly pixel eyes, a few buttons, neutral helpful look, blue and white body with a small red accent (#E63946), machine-but-cute, single object
NEGATIVE: photorealistic, 3D render, realistic medical device, scary, text, watermark, busy background, muddy colors, low contrast
```

### A9. NPC — 饮食辅导员 `Sprite_Coach.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, game asset sprite, centered, transparent background, a pear-shaped chibi coach character in green sportswear (#06D6A0) holding a stopwatch, a whistle, energetic cheerful motivating expression, lively pose, single character
NEGATIVE: photorealistic, 3D render, realistic anatomy, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### A10. Boss — 血糖巨魔 `Boss_GlucoseTroll.png`
```
2D game art, cartoon villain style with menacing edge, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, game asset sprite, centered, transparent background, an irregular jagged-polygon sugar troll boss, dark red body (#C1121F) wrapped in red mist, jagged spiky crown, grumpy comic villain face, glowing purple energy veins (#7B2CBF), imposing intimidating but still cartoonish, single character
NEGATIVE: photorealistic, 3D render, gore, blood, real horror, text, watermark, busy background, muddy colors, low contrast
```

---

## B. 道具 Items（P0–P1）

### B1. 胰岛素喷雾 `Item_InsulinSpray.png` ⭐MVP
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute cartoon insulin spray pen, rounded friendly body, soft blue glow ring around it (#4CC9F0), white core highlight, small sparkle, NOT medical-scary
NEGATIVE: photorealistic, 3D render, realistic syringe, sharp needle, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### B2. 降糖药瓶 `Item_Pills.png` ⭐MVP
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute rounded pill bottle, white body (#F5F0E8) with a green cap, gentle green healing steam rising from the top (#06D6A0), friendly cartoon look
NEGATIVE: photorealistic, 3D render, realistic pills, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### B3. 高糖雪花 `Item_SugarSnowflake.png` ⭐MVP
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a hexagonal sugar-crystal snowflake with ROUNDED soft tips (not sharp), glowing red-orange core (#FF9F1C), sunny-yellow outer glow (#FFD166), sparkly tempting candy look
NEGATIVE: photorealistic, 3D render, sharp pointed spikes, realistic snowflake, scary, text, watermark, busy background, muddy colors, low contrast
```

### B4. 血糖计 `Item_GlucoseMeter.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a cute handheld glucose meter device, rounded body, small glowing screen with friendly digits, blue and white with a yellow highlight (#FFD166)
NEGATIVE: photorealistic, 3D render, realistic medical device, scary, text, watermark, busy background, muddy colors, low contrast
```

### B5. 运动靴 `Item_SportsBoots.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item icon, centered, transparent background, a pair of cute energetic sport boots with tiny wings and small motion sparkles, green and yellow accents (#06D6A0 #FFD166), bouncy lively look
NEGATIVE: photorealistic, 3D render, realistic shoes, scary, text, watermark, busy background, muddy colors, low contrast
```

---

## C. 障碍物 & 收集物 Obstacles & Pickups（P0–P1）

### C1. 雪堆 / 可跳障碍 `Obs_SnowMound.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single object, centered, transparent background, a small rounded snow mound obstacle, white and ice-blue (#90E0EF), low cartoon bump shape easy to jump over
NEGATIVE: photorealistic, 3D render, scary, text, watermark, busy background, muddy colors, low contrast
```

### C2. 炸糖块 / 可翻滚障碍 `Obs_SugarBlock.png`
```
2D game art, cute chibi cartoon style, semi-angular shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single object, centered, transparent background, a stack of cartoon sugar cubes obstacle, amber accents (#F4A261), slightly angular and threatening but still cute
NEGATIVE: photorealistic, 3D render, gore, scary, text, watermark, busy background, muddy colors, low contrast
```

### C3. 低血糖冰刺 / 不可豁免障碍 `Obs_IceSpike.png`
```
2D game art, cute cartoon style with angular warning shapes, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single object, centered, transparent background, a triangular jagged ice spike obstacle, deep blue and white (#023E8A #90E0EF), sharp angular warning silhouette but stylized and cartoonish, dark edge
NEGATIVE: photorealistic, 3D render, gore, real horror, text, watermark, busy background, muddy colors, low contrast
```

### C4. 高血糖漩涡（L2）`Obs_HyperVortex.png`
```
2D game art, cute cartoon comic style, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single object, centered, transparent background, a swirling spiral hazard dominated by red over 50% (#E63946), red mist, cartoon comic energy swirl, menacing but playful
NEGATIVE: photorealistic, 3D render, gore, real horror, text, watermark, busy background, muddy colors, low contrast
```

### C5. 能量星 / 通用收集物 `Pickup_Star.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single item, centered, transparent background, a plump round collectible energy star, health-green body (#06D6A0) with a warm yellow glow (#FFD166), gentle sparkle, very rounded and friendly
NEGATIVE: photorealistic, 3D render, sharp spikes, scary, text, watermark, busy background, muddy colors, low contrast
```

### C6. 终点：医院降糖站 `HospitalStation.png`
```
2D game art, cute chibi cartoon style, rounded soft shapes, playful whimsical medical theme park mood, thick cocoa-black outline, flat cel-shaded coloring, bright vibrant palette, single building, centered, transparent background, a cute cartoon snow clinic hospital station building, white walls (#F5F0E8) with a friendly green cross sign (#06D6A0), rounded roof, warm glowing windows, gold rim-light highlight, welcoming safe-haven feeling
NEGATIVE: photorealistic, 3D render, hospital horror, scary, text, watermark, busy background, muddy colors, low contrast
```

---

## D. 环境 / Parallax 背景 Environment（P0–P2）

> 每关 4 张：天空 + 远景 + 中景 + 近景。远景几何简单、近景细节丰富。建议宽幅出图（如 1536×640）。

### D1. L1 医院雪屋 — 天空 `L1_Sky.png`
```
2D game art, cartoon style, soft flat gradient, whimsical clean calm mood, no outline needed, wide panoramic background, no characters, a light blue daytime sky with a soft cool-to-warm gradient, a few gentle puffy rounded cartoon clouds, peaceful and friendly, seamless wide background
NEGATIVE: photorealistic, 3D render, characters, text, watermark, dark, gloomy, muddy colors, low contrast
```

### D2. L1 — 远景 `L1_Far.png`
```
2D game art, cartoon style, flat simplified shapes, minimal, wide panoramic parallax layer, no characters, transparent background, a row of distant simplified rounded snow mountains as a silhouette, single soft blue gradient (#48CAE4 to #023E8A), no texture, calm depth
NEGATIVE: photorealistic, 3D render, detailed texture, characters, text, watermark, busy, muddy colors
```

### D3. L1 — 中景 `L1_Mid.png`
```
2D game art, cute cartoon style, thick cocoa-black outline, flat cel-shaded coloring, wide panoramic parallax layer, no characters, transparent background, midground geometric triangle pine trees with rounded trunks in two-tone dark green with white edges, a small cozy snow chalet with a chimney puffing gentle teal herbal steam, clean friendly
NEGATIVE: photorealistic, 3D render, characters, text, watermark, busy, muddy colors, low contrast
```

### D4. L1 — 近景地面 `L1_Near.png`
```
2D game art, cute cartoon style, thick cocoa-black outline, flat cel-shaded coloring with detail, wide ground strip, no characters, transparent top, a foreground snowy ski slope ground with hand-drawn snow curves and texture, white and ice-white (#F5F0E8 #90E0EF), small snow sparkles, detailed near layer
NEGATIVE: photorealistic, 3D render, characters, text, watermark, muddy colors, low contrast
```

### D5. L2 高血糖狂暴 — 天空 `L2_Sky.png`
```
2D game art, cartoon style, dramatic flat gradient, high contrast oppressive but cartoonish mood, wide panoramic background, no characters, a deep red and orange-red gradient sky (#E63946 #C1121F) with rolling jagged thunderclouds, intense energetic
NEGATIVE: photorealistic, 3D render, real horror, characters, text, watermark, muddy colors, low contrast
```

### D6. L2 — 远景 `L2_Far.png`
```
2D game art, cartoon style, flat simplified angular shapes, wide panoramic parallax layer, no characters, transparent background, distant sharp angular jagged mountain ridges silhouette in a dark red gradient, jagged comic clouds, tense depth
NEGATIVE: photorealistic, 3D render, detailed texture, characters, text, watermark, busy, muddy colors
```

### D7. L2 — 中景 `L2_Mid.png`
```
2D game art, cute cartoon comic style, thick cocoa-black outline, flat cel-shaded coloring, wide panoramic parallax layer, no characters, transparent background, midground angular red rock formations and giant sugar blocks with amber rim highlights (#FF9F1C), carnival comic energy
NEGATIVE: photorealistic, 3D render, gore, characters, text, watermark, busy, muddy colors, low contrast
```

### D8. L2 — 近景地面 `L2_Near.png`
```
2D game art, cute cartoon style, thick cocoa-black outline, flat cel-shaded coloring, wide ground strip, no characters, transparent top, a foreground red-tinted slope ground with cracks, blood-sugar red and cocoa black (#E63946 #2D2A32), aggressive comic texture
NEGATIVE: photorealistic, 3D render, gore, characters, text, watermark, muddy colors, low contrast
```

### D9. L3 低血糖冰窟 — 背景 `L3_Sky.png`
```
2D game art, cartoon style, flat low-saturation coloring, calm but eerie mood, wide panoramic background, no characters, a deep blue ice cave interior, frozen stalactite icicles with cold white glow, very low brightness, low contrast, misty
NEGATIVE: photorealistic, 3D render, real horror, characters, text, watermark, oversaturated, low detail
```

### D10. L3 — 远景 `L3_Far.png`
```
2D game art, cartoon style, flat simplified irregular shapes, wide panoramic parallax layer, no characters, transparent background, distant irregular ice crystal formations silhouette in deep blue (#023E8A), faint, slow misty depth
NEGATIVE: photorealistic, 3D render, characters, text, watermark, busy, oversaturated
```

### D11. L3 — 中景 `L3_Mid.png`
```
2D game art, cute cartoon style, thick cocoa-black outline, flat cel-shaded coloring, wide panoramic parallax layer, no characters, transparent background, midground irregular ice crystals and cones in deep blue with insulin-blue edges (#4CC9F0), a thick fog layer
NEGATIVE: photorealistic, 3D render, characters, text, watermark, busy, low contrast
```

### D12. L3 — 近景地面 `L3_Near.png`
```
2D game art, cute cartoon style, thick cocoa-black outline, flat cel-shaded coloring, wide ground strip, no characters, transparent top, a foreground icy slope ground in ice-white and deep blue (#90E0EF #023E8A), large floating ice particles feel
NEGATIVE: photorealistic, 3D render, characters, text, watermark, oversaturated, low contrast
```

### D13. 减肥营地（扩展，绿区）`Camp_Bg.png`
```
2D game art, cute cartoon style, thick cocoa-black outline, flat cel-shaded coloring, bright cheerful mood, wide panoramic background, no characters, a bright green outdoor camp scene with lush green hills, clear teal-blue sky, colorful tents picnic blankets and balloons, warm sunlight, geometric blocks and ring shapes, energetic healthy vibe
NEGATIVE: photorealistic, 3D render, characters, text, watermark, dark, muddy colors, low contrast
```

---

## E. UI / HUD（P0–P2）

> UI 多为概念图，精确切图/九宫格需后处理。文字部分用字体排版，不靠文生图。

### E1. 弧形血糖条 — 外框 `GlucoseBar_Frame.png`
```
2D game art, cute cartoon game UI, thick cocoa-black outline, flat clean coloring, single UI element, centered, transparent background, a horizontal capsule-shaped arc ammo-style meter frame with rounded end-caps, one end has a tiny round Tangtang head icon and the other end a small pill icon, clean friendly HUD bar, empty interior
NEGATIVE: photorealistic, 3D render, text, numbers, watermark, busy background, muddy colors, low contrast
```

### E2. 血糖条填充 — 低血糖 `GlucoseBar_Fill_Low.png`
```
2D game art, cute cartoon game UI, flat clean coloring, single UI element, centered, transparent background, a horizontal capsule-shaped meter fill bar with a deep-blue gradient (#023E8A to #4CC9F0), rounded end-caps, smooth glossy
NEGATIVE: photorealistic, 3D render, text, numbers, watermark, busy background, muddy colors, low contrast
```

### E3. 血糖条填充 — 正常 `GlucoseBar_Fill_Normal.png`
```
2D game art, cute cartoon game UI, flat clean coloring, single UI element, centered, transparent background, a horizontal capsule-shaped meter fill bar with a health-green gradient (#06D6A0), rounded end-caps, smooth glossy
NEGATIVE: photorealistic, 3D render, text, numbers, watermark, busy background, muddy colors, low contrast
```

### E4. 血糖条填充 — 高血糖 `GlucoseBar_Fill_High.png`
```
2D game art, cute cartoon game UI, flat clean coloring, single UI element, centered, transparent background, a horizontal capsule-shaped meter fill bar with a red-to-purple gradient (#E63946 to #7B2CBF), rounded end-caps, smooth glossy
NEGATIVE: photorealistic, 3D render, text, numbers, watermark, busy background, muddy colors, low contrast
```

### E5. 通用按钮 — 默认态 `Btn_Default.png`
```
2D game art, cute cartoon game UI, thick cocoa-black outline, flat clean coloring, single UI element, centered, transparent background, a rounded-rectangle game button with an insulin-blue fill (#4CC9F0), soft bevel, friendly, completely empty with no text
NEGATIVE: photorealistic, 3D render, text, letters, watermark, busy background, muddy colors, low contrast
```

### E6. 通用按钮 — 悬停态 `Btn_Hover.png`
```
2D game art, cute cartoon game UI, thick cocoa-black outline, flat clean coloring, single UI element, centered, transparent background, a rounded-rectangle game button with a light-blue fill (#48CAE4) and a 2px sunny-yellow outline (#FFD166), slightly bouncy lifted look, completely empty with no text
NEGATIVE: photorealistic, 3D render, text, letters, watermark, busy background, muddy colors, low contrast
```

### E7. 圆角面板 / 菜单气泡 `Panel_Bubble.png`
```
2D game art, cute cartoon game UI, soft rounded corners, flat clean coloring, single UI element, centered, transparent background, a rounded-rectangle speech-bubble style menu panel with a cocoa-black semi-transparent fill (#2D2A32), clean simple card, completely empty
NEGATIVE: photorealistic, 3D render, text, watermark, busy background, muddy colors, low contrast
```

### E8. 道具栏图标边框 `ItemSlot.png`
```
2D game art, cute cartoon game UI, flat clean coloring, single UI element, centered, transparent background, a rounded-square item slot frame for an inventory bar, clean 2px outline, subtle inner shadow, empty center
NEGATIVE: photorealistic, 3D render, text, item content, watermark, busy background, muddy colors, low contrast
```

### E9. 游戏 LOGO 装饰（无文字）`Logo_Deco.png`
```
2D game art, cute cartoon style, thick cocoa-black outline, flat cel-shaded coloring, bright festive palette, centered, transparent background, a game title logo decoration with NO text, floating sugar crystals and a warm pink-orange dusk snow mountain motif, whimsical celebratory ornament
NEGATIVE: photorealistic, 3D render, text, letters, words, watermark, busy background, muddy colors, low contrast
```

### E10. 结算装饰 — 胜利礼花 `Result_Win_Deco.png`
```
2D game art, cute cartoon style, bright festive palette, centered, transparent background, a burst of golden confetti and sugar-crystal fireworks, warm gold and orange (#FFD166 #F4A261), celebratory joyful explosion ornament, no text
NEGATIVE: photorealistic, 3D render, text, watermark, busy realistic background, muddy colors, low contrast
```

### E11. 结算装饰 — 失败谢幕 `Result_Fail_Deco.png`
```
2D game art, cute cartoon style, thick cocoa-black outline, flat coloring, centered, transparent background, a giant humorous cartoon candy-apple mascot that looks out of sugar with a small dizzy face, grape-purple comic curtain mood (#7B2CBF), gentle and funny NOT depressing, no text
NEGATIVE: photorealistic, 3D render, gore, scary, sad, depressing, text, watermark, busy background, muddy colors, low contrast
```

---

## 生成顺序速记

```
先做 → A1（锁风格）
P0  → A1 A2 A3 A4 A5 A6 / B1 B2 B3 / C1 C2 C3 C5 C6 / D1 D2 D3 D4 / E1 E2 E3 E4
P1  → B4 B5 / C4 / D5 D6 D7 D8 / E5 E6 E7 E8
P2  → A7 A8 A9 A10 / D9 D10 D11 D12 D13 / E9 E10 E11
```
</content>
