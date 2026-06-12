## 关联 Issue / 设计文档

<!-- 引用相关 GDD / Art Bible 章节 / Issue 编号 -->
<!-- 例:Closes #123, Refs game-concept.md §7, Refs art-bible.md §3 -->

## 改动简述

<!-- 1-3 句话说明 PR 改了什么,为什么 -->

## 改动类型

<!-- 用 [x] 选择 -->

- [ ] 新功能(feat)
- [ ] Bug 修复(fix)
- [ ] 重构(refactor)
- [ ] 文档(docs)
- [ ] 测试(test)
- [ ] 性能优化(perf)
- [ ] 构建/CI(chore)

## 涉及系统

- [ ] 滑行控制器
- [ ] 血糖状态系统
- [ ] 道具系统
- [ ] 障碍物生成
- [ ] 关卡流程
- [ ] UI / HUD
- [ ] 输入映射
- [ ] 音频
- [ ] Art / Animation
- [ ] Build / 工程配置
- [ ] 文档(README/GDD/Art Bible/CHANGELOG)
- [ ] 其他:___

## 涉及关卡

- [ ] 主菜单
- [ ] L1 医院雪屋
- [ ] L2 高血糖狂暴雪区
- [ ] L3 低血糖冰窟
- [ ] 减肥营地
- [ ] 多关卡 / 跨关

## 测试

<!-- 描述如何验证: -->

- [ ] 单元测试已添加/更新(`Window > General > Test Runner`)
- [ ] 集成测试已添加/更新
- [ ] 已手动 Play Mode 验证
- [ ] 视觉改动已截图对比

**复现/测试步骤**:

```
1. 打开 Unity Hub → SugarRush 项目
2. ...
3. 观察:...
```

## 截图 / 录像

<!-- UI/视觉改动必附,文字/代码改动可选 -->

## 风险与权衡

<!-- 该改动的潜在风险、与现有系统的冲突、性能影响、医学/伦理风险 -->

## 文档同步

- [ ] `game-concept.md` 已更新(若影响游戏机制)
- [ ] `art-bible.md` 已更新(若影响视觉/色彩)
- [ ] `CHANGELOG.md` 的 Unreleased 段已更新
- [ ] 无需文档更新(纯内部实现)

## 自查清单

- [ ] 代码遵循 `coding-standards.md`(函数 < 50 行,嵌套 ≤ 4 层)
- [ ] 无 `Debug.Log` 残留
- [ ] 无硬编码密钥 / 凭据
- [ ] 无 `Library/` / `Temp/` 等 gitignore 应屏蔽文件
- [ ] 提交信息遵循 Conventional Commits 格式
- [ ] 分支名称遵循 `feat/*` / `fix/*` / `docs/*` 格式

## Merge 后计划

<!-- 例如:运行 smoke test / 更新演示录像 / 在 Issue 关闭后通知 / 等 -->
