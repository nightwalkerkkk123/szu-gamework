# StoryCard + UI 背景 — Play 模式验收记录

> 日期：2026-06-21
> 分支：feature/illustration-storycard
> 计划：docs/superpowers/plans/2026-06-21-illustration-storycard.md Task 8

## 自动化逻辑验收（BLOCKING）

`SugarRush.StoryCard.Tests` EditMode：**6/6 通过**（StoryProgress.ResolvePlayable / FlattenPages 全覆盖）。

## Play 模式运行时验收（ADVISORY）

环境：Unity 2022.3.62f3，L1_HospitalChalet 场景，PlayerPrefs 已重置。

| 检查项 | 期望 | 实测 | 结论 |
|--------|------|------|------|
| 首进 L1 弹故事卡 | 显示序章第①页 | sprite=picturebook_01_imbalance_day | ✅ |
| 拼页数 | 5（序章①②④⑥ + L1③）| pageCount=5 | ✅ |
| 页顺序 | ① → … → ③ | first=imbalance_day, last=snow_house_start | ✅ |
| gameplay 冻结 | timeScale=0 | timeScale=0，gameState 卡在 Intro | ✅ |
| 完成后解冻 | timeScale=1 | Complete() 后 timeScale=1 | ✅ |
| 标记已看 | prologue+l1 置 1 | prologueSeen=1, l1Seen=1 | ✅ |
| 视觉 | 16:9 不变形 + 翻页提示 | 见截图，preserveAspect 正确，黑边为视口比例 | ✅ |

截图：`2026-06-21-storycard-prologue-page1.png`（序章第①页 + "空格/点击 继续 ESC 跳过" 提示）。

## 验收中发现并修复的缺陷

**冻结被覆盖**：`GameFlowManager.Start() → EnterIntro()` 设 `Time.timeScale = 1f`，与
`StoryCardPlayer.Start()` 的 `0f` 在同帧竞争，导致 gameplay 在故事卡背后偷跑。
修复：`StoryCardPlayer.Update()` 在播放期间每帧重断言 `Time.timeScale = 0f`（其它 Start
只设一次，每帧重断言必胜；GameFlow intro 计时在 deltaTime=0 时自然卡住，不误触发 StartPlaying）。
修复后 timeScale=0 稳定保持，gameState 停在 Intro，已验证。

## 未在本轮人工逐帧验证（信任既有覆盖 / 留待真机）

- L1→L2、L2→L3 过关后各关故事卡首触发：逻辑由 ResolvePlayable 单测保证 + L1 已实证，未逐场景跑 Play。
- StartScene/LevelSelect 背景、ResultPanel 胜负背景：已在编辑期连线确认（sprite 非空），未单独跑 Play 截图。
