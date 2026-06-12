# Changelog

All notable changes to **糖大冒险:雪山狂飙 / SugarRush** are documented here.
This project adheres to [Semantic Versioning](https://semver.org/) and the
[Keep a Changelog](https://keepachangelog.com/) format.

> ⚠️ 项目处于早期 W1 阶段(W1 = 引擎配置 + 概念对齐 + 视觉规范)
> 版本号 0.x 表示**功能未完成**,Minor 版本号变化 = 重大设计决策,Patch = 文档/小修正

---

## [Unreleased] — W2 计划

- 创建 Epics + Stories + Sprint Plan (`/create-epics`, `/create-stories`, `/sprint-plan`)
- 启动核心循环原型 (`/prototype`)

## [0.1.0] — 2026-06-12 — W1 基础设施完成

### Added
- `design/gdd/game-concept.md` — 21 节项目北极星(品类、世界观、机制转译矩阵、MVP 范围、4 周计划、风险与缓解)
- `design/art/art-bible.md` — Section 1-4 jam-paced(Visual Identity、Mood & Atmosphere 10 状态、Shape Language、Color System 7 主色)
- `unity-project/` — Unity 2022.3.62f3c1,2D Core 模板,产品名 SugarRush
- `.claude/docs/technical-preferences.md` — 引擎/输入/性能/测试规范
- `docs/engine-reference/unity/VERSION.md` — 版本锚定与参考资料
- `CLAUDE.md` — Technology Stack + 引擎参考 import
- `.github/ISSUE_TEMPLATE/` — bug_report, feature_request
- `.github/PULL_REQUEST_TEMPLATE.md` — PR 规范
- `README.md` — 项目门户
- `CONTRIBUTING.md` — git 流程 + 提交规范 + PR/Issue 流程
- `SECURITY.md` — 安全策略
- `LICENSE` — MIT
- 顶层 `.gitignore`(141 行) + `unity-project/.gitignore`(60 行)

### Changed
- `game-concept.md`:3D → 2D 横版,移动端 → PC 改向(共 16 处同步)
- `unity-project/`:从 `MyGameUnity/My project (6)/` 移到 `unity-project/`,整理占位资产

### Confirmed
- 单人模式 MVP(多人移入扩展规划)
- Unity 2022.3 LTS + 2D Built-in Renderer + Box2D
- 性能目标:60/144 fps,< 500 draw call,< 2 GB 内存
- 测试覆盖率:50% 核心优先(jam 节奏现实)
- 审查模式:`lean`(仅阶段门 Director 审查)

### Risks Identified (with mitigation in concept doc)
1. 医学准确性 — 课程标准 + 关键文案内部审核
2. 多人模式复杂度 — 已移入扩展规划
3. 角色刻板化 — 糖糖 A 中性 mascot 方案已具体化
4. 教育效果验证 — 5 道试玩问卷草案
5. 范围超限 — MVP 砍至 1-3 关 + 3 道具 + 1-2 视觉区
6. 目标人群未定 — W1 团队会议必决

---

## 版本标签指南

- `v0.x.y` — 早期开发(任何 v0.x.y 都有 breaking change 风险)
- `v1.0.0` — MVP 答辩通过,功能完整
- `v1.x.y` — 课程演示 / 内测版
- `v2.0.0` — 产品化(医疗合作、多人模式、扩展规划实现)
