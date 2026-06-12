# Unity — Version Reference

| Field | Value |
|-------|-------|
| **Engine Version** | Unity 2022.3.62f3c1 LTS（中国版,等同 .62f1） |
| **Language** | C# |
| **Rendering Pipeline** | Built-in Render Pipeline (Unity 2D Renderer) |
| **Project Pinned** | 2026-06-12 |
| **LLM Knowledge Cutoff** | May 2025 |
| **Risk Level** | LOW — Unity 2022.3 LTS API surface 完全在 LLM 训练数据内 |

## Note

This engine version is within the LLM's training data. Engine reference docs
are optional but can be added later if agents suggest incorrect APIs.

Unity 2022.3 LTS 于 2023 年 6 月发布,持续 LTS 补丁至 2026 年。所有核心 API
(MonoBehaviour, ScriptableObject, Input System, 2D Renderer, UGUI, UI Toolkit) 都在
LLM 训练数据范围内,Unity 专家代理可以直接基于训练知识工作。

## When to Upgrade This Doc

- 团队升级到具体 patch 版本(如 2022.3.50f1)时,在表格中标注具体 patch
- 切换到 Unity 6 / Unity 7 时,运行 `/setup-engine upgrade 2022.3 [新版本]`
- 出现 LLM 给出过期 API 建议时,运行 `/setup-engine refresh` 升级为完整 reference

## References

- [Unity 2022 LTS Release Notes](https://unity.com/releases/editor/qa/lts-releases)
- [Unity 2D Documentation](https://docs.unity3d.com/Manual/Unity2D.html)
- [Unity Input System](https://docs.unity3d.com/Packages/com.unity.inputsystem@1.5/manual/index.html)
- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@1.3/manual/index.html)
