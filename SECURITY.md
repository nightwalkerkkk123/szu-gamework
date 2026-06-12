# 安全策略 (Security Policy)

## 报告漏洞

如果您发现 **糖大冒险:雪山狂飙 / SugarRush** 项目中的安全漏洞,
请通过以下方式**私下**报告,不要在公开 Issue 公开:

- **Email**:2481772498@qq.com
- **GitHub**:通过 https://github.com/nightwalkerkkk123/szu-gamework/security/advisories/new 提安全建议
- 主题行请以 `[SECURITY]` 开头

## 我们关注的范围

由于这是**客户端单人 Unity 2D 游戏**(MVP),不涉及:

- ❌ 服务器端漏洞
- ❌ 联网功能(MVP 移除了多人模式)
- ❌ 用户认证 / 账户系统
- ❌ 支付 / 金融逻辑
- ❌ 玩家数据收集(无后端)
- ❌ API 端点

## 仍可能的安全关注

虽然范围有限,我们仍重视:

1. **依赖项漏洞** — Unity 包、第三方 .NET 库的已知 CVE
2. **构建产物安全** — release .exe / .dll 的代码签名(若发布)
3. **本地存档篡改** — 玩家本地 Save Data 完整性(若有)
4. **健康信息误导** — 道具说明 / 文案中的医学错误可能造成实际伤害
5. **隐私** — 若未来接入遥测或云存档

## 响应承诺

- **初次响应**:**72 小时内**确认收到
- **评估**:**7 天内**给初步严重度评估
- **修复**:依严重度分级处理:
  - 🔴 关键(健康误导 / 隐私泄露)— 立即修复
  - 🟡 高(构建产物问题)— 下一个 sprint
  - 🟢 中(依赖项 CVE)— 列入 backlog
  - ⚪ 低(最佳实践)— 内部讨论

## 安全开发实践

项目遵循 `.claude/rules/ecc/common/security.md` 全局规则:

- [ ] 无硬编码密钥(API keys、密码、tokens)
- [ ] 所有用户输入验证(若未来加入)
- [ ] Unity Asset Store 资产来源可追溯
- [ ] `.gitignore` 已屏蔽常见敏感文件
- [ ] 第三方包来源审查(Packages/manifest.json)
- [ ] 关键医学文案内部审核(规避健康误导)

## 安全相关链接

- [GitHub Security Advisories](https://docs.github.com/en/code-security/security-advisories/guidance-on-reporting-and-writing-information-about-vulnerabilities/privately-reporting-a-security-vulnerability)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/) — 适用未来的 web/在线功能
- [Unity 安全最佳实践](https://docs.unity3d.com/Manual/Security.html)

---

*本项目仍处于早期开发阶段,随着多人/在线功能进入扩展规划,本安全策略会持续更新。*
