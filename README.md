# 糖大冒险:雪山狂飙 / SugarRush

> **2D 横版滑雪跑酷 + 糖尿病健康教育 Serious Game**
> 🎮 把血糖管理变成游戏机制 — 玩着玩着懂了控糖

![Status](https://img.shields.io/badge/status-W1%20%E5%9F%BA%E7%A1%80%E8%AE%BE%E6%96%BD-blue)
![Engine](https://img.shields.io/badge/Unity-2022.3%20LTS-black)
![Platform](https://img.shields.io/badge/platform-PC-lightgrey)
![License](https://img.shields.io/badge/license-MIT-green)

> 📸 **截图占位** — W2 完成核心循环原型后补
> 🎬 **Demo 录像** — W4 准备课程演示

---

## 一句话定位

一款把血糖管理变成游戏机制的 **2D 横版滑雪跑酷** 游戏 —— 玩家在滑、跳、翻滚、躲、收集的过程中,自然理解糖尿病控糖的关键概念。

## 🎮 核心特色

### 机制转译 (Mechanic Translation) — 设计的灵魂

医学知识 **不是** 贴在游戏外面,而是 **变成** 玩家必须实时处理的游戏变量:

| 医学概念 | 游戏化身 |
|---------|---------|
| 血糖水平 | 实时状态条(核心机制) |
| 高糖饮食 | 高糖雪花(双刃剑道具) |
| 胰岛素治疗 | 胰岛素喷雾(救命道具) |
| 口服降糖药 | 降糖药瓶(持续型) |
| 运动干预 | 运动靴(机动性增益) |
| 自我监测 | 血糖计(信息+冲刺奖励) |
| 高血糖危机 | 红色狂暴雪区(环境压力) |
| 低血糖危机 | 冰蓝冰窟(操控阻力) |
| 健康干预 | 减肥营地(绿色正反馈) |

### 三大支柱
- **幽默** — 卡通角色、夸张医疗道具、奇幻医疗雪山
- **教育** — 机制转译:医学知识就是游戏变量
- **爽感** — 跑酷反馈感(流畅滑行、跳跃、翻滚、躲避)

---

## 📚 项目文档

| 文档 | 用途 | 状态 |
|------|------|------|
| [`design/gdd/game-concept.md`](design/gdd/game-concept.md) | 项目北极星(21 节,品类/世界观/机制矩阵/MVP 范围/4 周计划/风险) | ✅ v0.1 |
| [`design/art/art-bible.md`](design/art/art-bible.md) | 视觉权威(Identity/Mood/Shape/Color 4 节 jam-paced) | ✅ W1 |
| [`.claude/docs/technical-preferences.md`](.claude/docs/technical-preferences.md) | 引擎/输入/性能/测试规范 | ✅ |
| [`docs/engine-reference/unity/VERSION.md`](docs/engine-reference/unity/VERSION.md) | Unity 版本锚定 2022.3.62f3c1 | ✅ |
| [`CLAUDE.md`](CLAUDE.md) | Studio 主指令(Claude 自动读取) | ✅ |
| [`CHANGELOG.md`](CHANGELOG.md) | 版本历史 | ✅ |
| [`CONTRIBUTING.md`](CONTRIBUTING.md) | 贡献流程 | ✅ |
| [`SECURITY.md`](SECURITY.md) | 安全策略 | ✅ |
| [`QUICKSTART.md`](QUICKSTART.md) | 快速启动(项目就绪版) | ✅ |
| [`.github/ISSUE_TEMPLATE/`](.github/ISSUE_TEMPLATE/) | Bug 报告 + Feature 请求模板 | ✅ |
| [`.github/PULL_REQUEST_TEMPLATE.md`](.github/PULL_REQUEST_TEMPLATE.md) | PR 模板 | ✅ |
| [`LICENSE`](LICENSE) | MIT | ✅ |

## 🎯 MVP 范围(4 周)

| 周 | 阶段 | 关键产出 |
|----|------|---------|
| **W1** | 引擎 + 设计 | ✅ 引擎配置 + 概念文档 + Art Bible + git 基线 |
| **W2** | 核心机制 | 血糖系统完整、3 个核心道具、L1 医院雪屋可玩 |
| **W3** | 内容扩展 | L2 高血糖区、UI 完整化、音效集成(L3 弹性) |
| **W4** | 打磨 | Bug 修复、试玩调整、Demo 录制、课程答辩材料 |

**MVP 必含**:1-3 关(从 5-8 关砍)、3 个核心道具(从 5 砍)、2 个视觉区(从 3 砍)、单人模式。
**移入扩展规划**:多人模式、新手引导模块、控糖成就系统、医疗百科图鉴、排行榜、每日挑战。

---

## 🛠️ 技术栈

- **引擎**:Unity 2022.3 LTS(2D Core 模板)
- **语言**:C#
- **渲染**:Built-in Render Pipeline(Unity 2D Renderer)
- **物理**:Box2D(Unity 2D Physics)
- **目标平台**:PC(Windows 主 / Mac+Linux 可选)
- **输入**:键盘/鼠标 + 手柄(Partial 支持)
- **性能目标**:60 fps(基础)/ 144 fps(自适应)
- **包体目标**:< 200 MB

## 🚀 快速开始

```bash
# 1. 克隆仓库
git clone https://github.com/nightwalkerkkk123/szu-gamework.git
cd szu-gamework

# 2. 在 Unity Hub 中添加 unity-project/
#    Hub → Add → 浏览到本地 unity-project/ → 添加

# 3. 用 Unity 2022.3.62f3c1 打开 SugarRush
#    Hub → 选中 → Open
#    等待首次导入(几分钟)

# 4. (可选)Claude Code AI 协作
#    cd 到仓库根目录,启动 Claude Code
#    加载 .claude-plugins 中提示的 Unity 插件(见 QUICKSTART.md)
```

详细启动指引见 [`QUICKSTART.md`](QUICKSTART.md)。

## 👥 团队

| 角色 | 姓名 | 邮箱 |
|------|------|------|
| 项目负责人 / 主程 | wangzihao | 2481772498@qq.com |
| 美术 / 设计 | _TBD_ | _TBD_ |
| 策划 / 测试 | _TBD_ | _TBD_ |

> 团队 2-3 人课程项目,隶属深圳大学(Shenzhen University)。
> 招募中:美术 / 策划角色。

## 🏗️ 项目结构

```
szu-gamework/
├── CLAUDE.md                    # Studio 主指令
├── README.md                    # 项目门户 (本文件)
├── QUICKSTART.md                # 快速启动
├── CHANGELOG.md                 # 版本历史
├── CONTRIBUTING.md              # 贡献流程
├── SECURITY.md                  # 安全策略
├── LICENSE                      # MIT
├── .gitignore                   # 顶层忽略规则
├── .claude-plugins              # Unity 插件加载提示
├── .claude/                     # Studio agents / skills / hooks / rules
│   ├── docs/                    # 协调规则、模板、ADR
│   ├── agents/                  # 49 个专业 agent 定义
│   ├── skills/                  # 85 个 skill
│   └── rules/                   # 11 个编码/开发规则
├── .github/
│   ├── ISSUE_TEMPLATE/          # Bug / Feature 模板
│   └── PULL_REQUEST_TEMPLATE.md # PR 模板
├── design/
│   ├── gdd/game-concept.md      # 游戏设计文档(项目北极星)
│   └── art/art-bible.md         # 视觉规范
├── docs/
│   └── engine-reference/unity/  # Unity 版本参考
├── production/
│   ├── review-mode.txt          # 当前: lean
│   └── session-state/           # 临时会话状态
├── templates/                   # 7 个文档模板
├── tools/                       # 工具脚本
├── tests/                       # 单元/集成/性能测试
├── src/                         # (W2+ 使用)源代码
├── assets/                      # (W2+ 使用)美术/音频资产
├── prototypes/                  # 一次性原型
└── unity-project/               # ⭐ Unity 2D 工程
    ├── Assets/
    ├── Packages/
    ├── ProjectSettings/
    └── .gitignore
```

## 🤝 贡献

欢迎贡献!请先阅读 [`CONTRIBUTING.md`](CONTRIBUTING.md),包括:

- Git Flow(主分支 + feat/fix/docs 分支)
- Conventional Commits 格式
- PR 流程(轻量 review,1 人 approve)
- Issue 模板(bug report / feature request)
- 行为准则(题材尊严 + 避免"肥胖=糖尿病"刻板)

## 📜 许可证

本项目代码采用 **MIT License** — 详见 [`LICENSE`](LICENSE)。

> ⚠️ **关于内容的特别声明**:本项目包含的医学相关内容(道具说明、关卡介绍、文案)**不构成医学建议**。
> 详见 [SECURITY.md](SECURITY.md) §"仍可能的安全关注"。

## 📞 联系方式

- **GitHub Issues**:https://github.com/nightwalkerkkk123/szu-gamework/issues
- **GitHub Security**:https://github.com/nightwalkerkkk123/szu-gamework/security/advisories/new
- **Email**:2481772498@qq.com
- **隶属**:深圳大学(SZU)课程项目

---

## 🎓 学术归属

This is a course project for Shenzhen University.
本项目为深圳大学课程项目,旨在探索"机制转译"方法在健康教育游戏中的实践。

⭐ 如果这个项目给您带来灵感,请 Star 仓库!
