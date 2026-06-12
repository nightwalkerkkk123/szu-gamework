# SugarRush — 快速启动

> **项目状态**:W1 完成(基础设施就绪)
> **最后更新**:2026-06-12
> **隶属**:深圳大学课程项目 / MIT 协议

---

## 5 分钟跑起来

### ① 准备 Unity Editor

- 安装 **Unity 2022.3.62f3c1**(中国版,等同 .62f1)
- 若未装,Unity Hub → **Installs → Install Editor → 选 2022.3.62f3c1**
- 若已装其他 Unity 版本,继续 — Unity Hub 可多版本并存

### ② 在 Unity Hub 中打开项目

1. 启动 **Unity Hub**
2. **Projects → Add → 打开本地**
3. 浏览到仓库的 `unity-project/` 子目录
4. 选中,点击 **Open**
5. 首次打开会下载资源包 + 编译,等几分钟

> ✅ 验证:Unity Editor 标题栏应显示 `unity-project - SugarRush - Unity 2022.3.62f3c1`
> (SugarRush 来自 Project Settings > Player > Product Name,需在 Editor 内手动设置)

### ③ 首次设置(必做)

在 Unity Editor 中:
1. **Edit → Project Settings → Player**
   - **Product Name**:`SugarRush`(默认是 `unity-project`)
   - **Company Name**:`SZU Gamework Team`(或你的学校/团队)
2. (可选)**Window → Package Manager**
   - 安装 `Input System` 1.5.1+ — 键鼠/手柄统一支持
   - 安装 `Cinemachine` 2.9+ — 2D 横版相机跟随

### ④ 试运行

- 打开 `unity-project/Assets/Scenes/SampleScene.unity`(默认空场景)
- 点 **▶ Play** 按钮(中心顶部)
- 空场景运行无障碍,SampleScene 仅有默认光照和相机

## 📂 项目文档导航

| 想看什么 | 文档 |
|---------|------|
| 游戏是什么 | [`README.md`](README.md) |
| 怎么设计、机制转译矩阵 | [`design/gdd/game-concept.md`](design/gdd/game-concept.md) |
| 视觉/色彩/形状规范 | [`design/art/art-bible.md`](design/art/art-bible.md) |
| 怎么写提交/PR | [`CONTRIBUTING.md`](CONTRIBUTING.md) |
| 版本历史 | [`CHANGELOG.md`](CHANGELOG.md) |
| 安全漏洞报告 | [`SECURITY.md`](SECURITY.md) |

## 🤖 可选:Claude Code AI 协作

本项目配套 **Claude Code Game Studios 工具集**(49 agents / 85 skills)已安装到 `.claude/`。

启动 Claude Code 时加载 Unity 插件:

```bash
cd /e/CODE/MyGame
claude \
  --plugin-dir /e/CODE/claude-unity-game-studio/unity-knowledge-skills \
  --plugin-dir /e/CODE/claude-unity-game-studio/unity-middleware-skills
```

> 路径见 `.claude-plugins` 注释文件。

启动后尝试:
- `/start` — 项目状态检查
- `/map-systems` — 拆解 MVP 系统(W1 下一个任务)
- `/prototype` — 快速核心循环原型

## 🗺️ 接下来的开发路径

```
W1 ✅ 引擎配置 + 概念 + Art Bible + git 基线        [DONE]
W2 ⏭️ 核心循环:滑行控制器 + 血糖系统 + 3 道具     [NEXT]
W3   关卡 L1-L2 + UI 完整化 + 音效
W4   打磨 + Bug + Demo 录制 + 课程答辩
```

## 🐛 遇到问题?

| 症状 | 排查 |
|------|------|
| Unity Hub 找不到 2022.3.62f3c1 | 装最新 .62f1(中国版绑定),功能完全等同 |
| 打开项目报错 "missing package" | `Window → Package Manager → 确认 manifest.json 加载完整` |
| 场景空 / 找不到糖糖 sprite | W2 才会创建主角;现在 SampleScene 是空场景 |
| Claude 不识别 Unity 上下文 | 启动时需 `--plugin-dir`(见上) |
| git push 失败 | 仓库 `nightwalkerkkk123/szu-gamework` 需要 `gh auth login` |

## 📞 求助

- **GitHub Issues**:https://github.com/nightwalkerkkk123/szu-gamework/issues
- **Email**:2481772498@qq.com
- 紧急问题(Bug/崩溃):直接联系项目负责人
