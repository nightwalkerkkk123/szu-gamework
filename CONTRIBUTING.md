# 贡献指南 (Contributing)

感谢您考虑为 **糖大冒险:雪山狂飙 / SugarRush** 贡献代码、设计、文档或试玩反馈!
本项目是 2-3 人课程项目(深圳大学),但欢迎外部贡献以完善健康教育游戏设计。

## 🤝 协作模式

- **工作流**:`/start` 已引导完成 Phase 1 (Concept) — 当前处于 W2
- **代码评审**:`lean` 模式,Phase Gate 时启动 4 总监审查;日常 PR 走轻量 1 人 review
- **决策协议**:任何超出 `game-concept.md` / `art-bible.md` 范围的改动,需在 PR 描述引用对应文档章节

## 🌿 Git Flow

### 分支策略
- `main` — 稳定可演示版,只接受通过 review 的 PR
- `feat/*` — 新功能(如 `feat/glucose-system`, `feat/l1-hospital`)
- `fix/*` — 修 Bug
- `docs/*` — 仅文档改动
- `chore/*` — 构建/CI/配置
- `spike/*` — 调研性分支(throwaway)

### 提交规范(Conventional Commits)

```
<type>(<scope>): <description>

[optional body]

[optional footer(s)]
```

**type 必选**:`feat` `fix` `refactor` `docs` `test` `chore` `perf` `ci`

**scope 推荐**:
- 系统名:`glucose` `skiing` `item` `level` `ui` `input` `scene`
- 区域名:`l1-hospital` `l2-red-storm` `l3-ice-cave`
- 文档:`gdd` `art-bible` `readme` `changelog`

**示例**:
```
feat(glucose): add bounded blood sugar state with 5-tier classification
fix(input): map Gamepad A button to Jump consistently across L1-L3
docs(gdd): update MVP scope to 1-3 levels per 4-week jam schedule
refactor(skiing): extract Collider2D detection into IRollingSurface
test(glucose): add acceptance test for high-glucose critical state
chore(unity): pin com.unity.inputsystem to 1.5.1
```

**注意**:`git commit` 末尾不附加 Co-Authored-By 归功信息(已在 `~/.claude/settings.json` 全局禁用)

## 🔄 Pull Request 流程

1. **Fork** 仓库到您账户
2. **创建 feature 分支**:`git checkout -b feat/your-feature main`
3. **开发** — 保持小颗粒、频繁提交
4. **本地验证**:
   - Unity 项目:`unity-project/`,打开 Unity Hub → SugarRush → 等导入 → Play Mode
   - 自动化测试:`Window > General > Test Runner`(NUnit 框架)
5. **更新文档**:
   - 改动 gameplay → 更新 `game-concept.md`
   - 改动视觉/资产 → 更新 `art-bible.md`
   - 任何 user-facing 改动 → 更新 `CHANGELOG.md` 的 Unreleased 段
6. **推送**:`git push origin feat/your-feature`
7. **创建 PR**:
   - 标题遵循 Conventional Commits 格式
   - 描述用 `.github/PULL_REQUEST_TEMPLATE.md` 模板
   - 关联相关 issue (`Closes #123`)
8. **响应评审** — 至少 1 人 approve
9. **Squash merge** — 保持 main 历史干净

### PR 描述必填
- 改了啥(why > what)
- 测试如何验证(截图 / 录像链接 / 复现步骤)
- 关联的设计文档 / issue
- 风险评估
- 截图(若涉及 UI/视觉)

## 🐛 Issue 流程

我们用 **GitHub Issues** 跟踪 bug、特性请求、文档问题。

| 模板 | 适用场景 |
|------|---------|
| `bug_report.md` | 游戏崩溃、行为错误、性能问题 |
| `feature_request.md` | 新道具、新关卡、新机制、UI 改进 |
| 普通 issue | 设计讨论、文档补充、问题咨询 |

### 标签约定
- `W1/W2/W3/W4` — 所属冲刺
- `bug` `enhancement` `docs` `question` `wontfix` — 类别
- `priority:high/medium/low` — 优先级
- `system:glucose/skiing/item/...` — 涉及系统
- `area:L1/L2/L3/menu/audio` — 关卡/区域

## ✅ Code Review 标准

详见 `.claude/docs/coding-standards.md` 摘要:

- 函数 < 50 行,文件 < 800 行
- 嵌套 ≤ 4 层(用 early return)
- 不可变优先(尤其数据/状态)
- 错误显式处理(不静默吞掉)
- 无硬编码密钥/凭据
- 无调试 `console.log` 残留
- 新 gameplay 系统必须有单元测试
- 测试覆盖率 ≥ 50% (核心系统)

## 🎮 Playtest 反馈

课程演示后会收到试玩反馈,欢迎:
- 在 Issues 提"Bug Report"或"Feature Request"
- 提 PR 修复发现的 bug
- 在 Discussions 分享试玩体验(若启用)

## ⚠️ 不在贡献范围

- **多人模式代码** — 移入扩展规划,MVP 不接受
- **课程/医学权威内容** — 需要 SZU 医学顾问或文献审核,普通贡献者勿自行添加
- **未授权的品牌/IP** — 道具视觉、UI 设计避免使用任何商业 IP
- **数据收集后端** — MVP 不接受

## 📜 行为准则

- 尊重糖尿病患者群体 — 这是个**教育游戏**,目标是让玩家**理解血糖管理**而非"教育"或"恐吓"玩家
- 设计改动必须保留题材尊严(game-concept.md 风险 3 已识别)
- 不接受强化刻板印象(体型 = 糖尿病)的视觉/文案

## 📞 联系方式

- 项目负责人:wangzihao (2481772498@qq.com)
- GitHub:https://github.com/nightwalkerkkk123/szu-gamework/issues
- 隶属:深圳大学课程项目

---

再次感谢您的贡献! 🎮⛷️
