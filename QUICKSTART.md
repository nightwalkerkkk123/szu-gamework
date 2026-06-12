# MyGame — 启动指引

> Claude Unity Game Studio · 完整工作室模式（Mode A）
> 安装时间：2026-06-12 · 工具集路径：`E:\CODE\claude-unity-game-studio`

---

## 已安装内容核对

| 组件 | 数量 | 路径 |
|------|------|------|
| 工作室 agents | **49** | `.claude/agents/` |
| 工作室 skills | **72** | `.claude/skills/` |
| 工作流 skills | **13**（`uw-*`） | `.claude/skills/`（已合并） |
| Hooks | **12** | `.claude/hooks/` |
| 编码规则 | **11** | `.claude/rules/` |
| 工作流命令 | **9**（`/uw-cmd-*`） | `.claude/commands/` |
| 文档模板 | **7** | `templates/` |
| 文档/标准 | 9 个 .md | `.claude/docs/` |

合计 **85 skills + 9 commands + 12 hooks + 11 rules + 7 templates**。对照 README 声称的 120+ skills，差额来自 `unity-knowledge-skills` 和 `unity-middleware-skills` 插件 — 这两个需要通过 `--plugin-dir` 在 Claude 启动时加载（见下文），不复制到工程内。

> **注**：setup.sh 在复制 `unity-ai-workflow` 的 commands 时漏了 `mkdir -p .claude/commands`，9 个 `/uw-cmd-*` 命令会丢失。已在初始化时手工补齐，工具集上游可考虑修一下脚本。

---

## 第一次启动：3 步走

### ① 在 Unity Hub 创建 Unity 项目（✅ 目录已预创建）

`E:\CODE\MyGame\MyGameUnity\` 目录骨架已经预创建好了：

- 标准 3 目录：`Assets/`、`Packages/`、`ProjectSettings/`
- `Assets/` 下的 13 个子目录（`Scenes/`、`Scripts/`、`Prefabs/`、`Materials/`、`Shaders/`、`Art/Textures|Models|Animations/`、`Audio/Music|SFX/`、`VFX/`、`Data/`、`Editor/`）
- Unity 6 标准 `.gitignore`（排除 `Library/`、`Temp/`、`Logs/`、`UserSettings/` 等）
- 占位 `README.md`

你**只需要在 Unity Hub 里覆盖这个目录**：

1. 打开 **Unity Hub**
2. 如果还没装 Unity：**Installs → Install Editor → 选 Unity 6.3 LTS**（推荐）
3. **Projects → New Project**：
   - 模板：**3D Core**（或 2D Core）
   - **Project name**：`MyGameUnity`
   - **Location**：浏览到 `E:\CODE\MyGame\MyGameUnity`
   - 取消勾选 "Initialize repositories for ..."（让上层工作区管 git）
4. 点 **Create Project** — Unity 会用同名模板填充 `Assets/`、`Packages/manifest.json`、`ProjectSettings/`
5. 第一次打开会下载一堆资源包（几分钟），让它跑完

> 💡 已经预创建的子目录会被 Unity 保留，新场景/默认设置会合并进去。

### ② 在工作区根目录启动 Claude Code + 加载 Unity 插件

```bash
cd /e/CODE/MyGame
claude \
  --plugin-dir /e/CODE/claude-unity-game-studio/unity-knowledge-skills \
  --plugin-dir /e/CODE/claude-unity-game-studio/unity-middleware-skills
```

加载完会看到 35 个 Unity API 技能 + 21 个中间件技能自动可用。

> **提示**：插件路径已写入 `.claude-plugins` 注释文件。如果你想用绝对路径简化每次启动，可以写个 `start-claude.sh` 脚本（tools/ 目录是空的，可以放这种小工具）。

### ③ 在 Claude Code 中跑 `/start`

启动后直接输入：

```
/start
```

这是**引导式 onboarding**，会问你是哪种起点（无概念 / 模糊想法 / 明确概念 / 现有项目），并把 `CLAUDE.md` 里未填的 `[CHOOSE: ...]` 占位符一步步替换掉。

---

## 接下来会发生什么（按 Phase 顺序）

```
Phase 1  概念      →  /start, /brainstorm
Phase 2  系统设计  →  /map-systems, /design-review
Phase 3  架构      →  /create-architecture, /gate-check concept
Phase 4  预生产    →  /setup-engine, /test-setup, /gate-check pre-prod
Phase 5  生产      →  /create-epics, /create-stories, /dev-story, /story-done
Phase 6  打磨      →  /team-polish
Phase 7  发布      →  /gate-check release, /launch-checklist
```

每个 Phase 出口都有 **gate-check**：4 位总监（创意/技术/制作/艺术）并行 review，最严 verdict 胜出。

---

## 三种使用模式（按需切换）

- **Mode A 完整工作室**（默认）— 49 agents + 全部 gates + 全部 72 skills。**本工作区已就绪**。
- **Mode B 简化工作流** — 9 个 `/uw-cmd-*` 高级命令。已安装，独游/Game Jam 友好。
- **Mode C 混合（推荐）** — 规划用全工作室（`/map-systems`、`/create-architecture`），实现用工作流命令（`/uw-cmd-implement-feature "..."`）。

AI 协作风格（引导 vs 自主）写在 `docs/ProjectConfig.yaml` 的 `ai_mode` 字段。

---

## 可选：Unity MCP 实时桥接

如果你想让 Claude 直接操控 Unity Editor（读写场景、调 Console、查对象）：

```bash
# 见 unity-mcp 目录的安装说明
cat /e/CODE/claude-unity-game-studio/unity-mcp/README.md
```

MCP 是独立组件，需要在 Unity 内安装 C# 插件并启动本地 server。**非必需**，先用 `/start` 跑起来再说。

---

## 常用目录速查

```
MyGame/                          ← 你现在在这
├── CLAUDE.md                    ← Studio 主指令（agent 自动读取）
├── QUICKSTART.md                ← 本文件
├── .claude/                     ← 49 agents / 85 skills / 12 hooks / 11 rules
│   ├── settings.json            ← 会话配置
│   └── docs/                    ← 协调规则、标准、模板索引
├── .claude-plugins              ← 插件加载提示（注释文件）
├── design/
│   ├── gdd/                     ← 游戏设计文档（每个系统一份）
│   ├── narrative/, levels/, balance/
│   └── registry/entities.yaml   ← 实体注册表
├── src/                         ← 源代码（core/ gameplay/ ai/ networking/ ui/ tools/）
├── assets/                      ← 美术/音频/VFX/着色器/数据
├── tests/                       ← 单元/集成/性能
├── docs/                        ← 架构、引擎参考、各 Phase 文档
├── production/                  ← 冲刺计划、里程碑、QA、session 状态
├── prototypes/                  ← 一次性原型（与 src/ 隔离）
├── templates/                   ← 7 个文档模板（GDD / PRD / Sprint Plan …）
├── tools/                       ← 你的小工具脚本
└── MyGameUnity/                 ← Unity 项目（✅ 目录骨架已预创建，等 Unity Hub 填充）
    ├── Assets/   Packages/   ProjectSettings/   .gitignore   README.md
```

---

## 第一次跟 AI 协作会问什么？

`/start` 会先检测工程状态，看到 `CLAUDE.md` 里还有这些占位符没填：

- **Engine**：`[CHOOSE: Godot 4 / Unity / Unreal Engine 5]` ← 你要选 **Unity**
- **Language**：`[CHOOSE: GDScript / C# / C++ / Blueprint]` ← Unity → **C#**
- **Build System**：`[SPECIFY after choosing engine]` ← 让 AI 帮你决定（URP 还是 Built-in 等）

`/start` 会引导你把这些填好，然后开始 Phase 1 概念 brainstorm。

---

## 遇到问题？

- `setup.sh` 是只读的？看 `E:\CODE\claude-unity-game-studio\SETUP.md` 里有完整手动安装说明
- 工具集如何升级？看 `E:\CODE\claude-unity-game-studio\game-studios-template\UPGRADING.md`
- hooks/agents 怎么改？编辑 `.claude/` 下对应文件即可
- 想知道 agents 各自干啥？看 `.claude/docs/agent-roster.md` 和 `.claude/docs/agent-coordination-map.md`

祝你做出好玩的游戏 🎮
