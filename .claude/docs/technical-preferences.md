# Technical Preferences

<!-- Populated by /setup-engine. Updated as the user makes decisions throughout development. -->
<!-- All agents reference this file for project-specific standards and conventions. -->

## Engine & Language

- **Engine**: Unity 2022.3 LTS
- **Language**: C#
- **Rendering**: Built-in Render Pipeline (Unity 2D Renderer)
- **Physics**: Unity 2D Physics (Box2D)

## Input & Platform

<!-- Written by /setup-engine. Read by /ux-design, /ux-review, /test-setup, /team-ui, and /dev-story -->
<!-- to scope interaction specs, test helpers, and implementation to the correct input methods. -->

- **Target Platforms**: PC (Windows 主要 / Mac/Linux 可选)
- **Input Methods**: Keyboard/Mouse, Gamepad (推荐支持)
- **Primary Input**: Keyboard/Mouse
- **Gamepad Support**: Partial (推荐 — Xbox/PS 标准按键映射)
- **Touch Support**: None
- **Platform Notes**: 仅 PC 开发,无移动端。所有 UI 必须支持键盘导航,无依赖 hover 的交互。Steam 发布不在 MVP 范围。

## Naming Conventions

- **Classes**: PascalCase (e.g., `PlayerController`)
- **Public Fields/Properties**: PascalCase (e.g., `MoveSpeed`)
- **Private Fields**: _camelCase (e.g., `_moveSpeed`)
- **Methods**: PascalCase (e.g., `TakeDamage()`)
- **Files**: PascalCase 匹配类名 (e.g., `PlayerController.cs`)
- **Constants**: PascalCase 或 UPPER_SNAKE_CASE
- **Events** (C# events / UnityEvent): PascalCase + 过去时 (e.g., `HealthChanged`, `OnPlayerDied`)

## Performance Budgets

- **Target Framerate**: 60 fps (基础) / 144 fps (高刷新率自适应)
- **Frame Budget**: 16.6 ms (60fps) / 6.9 ms (144fps)
- **Draw Calls**: < 500 (低-中端 PC GPU 目标)
- **Memory Ceiling**: < 2 GB (4GB 显存机型友好)
- **Build Size**: < 200 MB

## Testing

- **Framework**: Unity Test Framework (内置 NUnit)
- **Minimum Coverage**: 50% (核心系统优先,jam 节奏现实)
- **Required Tests**: 血糖系统逻辑、滑行控制器、关卡完成判定、道具效果。非核心(UI 反馈、动画播放)按需

## Forbidden Patterns

<!-- Add patterns that should never appear in this project's codebase -->
- [None configured yet — add as architectural decisions are made]

## Allowed Libraries / Addons

<!-- Add approved third-party dependencies here -->
- [None configured yet — add as dependencies are approved]

## Architecture Decisions Log

<!-- Quick reference linking to full ADRs in docs/architecture/ -->
- [No ADRs yet — use /architecture-decision to create one]

## Engine Specialists

<!-- Written by /setup-engine when engine is configured. -->
<!-- Read by /code-review, /architecture-decision, /architecture-review, and team skills -->
<!-- to know which specialist to spawn for engine-specific validation. -->

- **Primary**: unity-specialist
- **Language/Code Specialist**: unity-specialist (C# review — primary 兼任)
- **Shader Specialist**: unity-shader-specialist (Shader Graph 2D Renderer, sprite shaders — Built-in pipeline)
- **UI Specialist**: unity-ui-specialist (UI Toolkit UXML/USS, UGUI Canvas, runtime UI)
- **Additional Specialists**:
  - unity-dots-specialist (ECS / Jobs / Burst — **本项目不使用**)
  - unity-addressables-specialist (asset loading, memory management — **MVP 不强制**)
- **Routing Notes**: Invoke primary for architecture and general C# code review. Invoke shader specialist for 2D sprite shader / lighting work (Built-in 2D renderer, no URP). Invoke UI specialist for all menu/HUD implementation. DOTS/ECS is over-engineering for this scope; Addressables deferred to post-MVP.

### File Extension Routing

<!-- Skills use this table to select the right specialist per file type. -->
<!-- If a row says [TO BE CONFIGURED], fall back to Primary for that file type. -->

| File Extension / Type | Specialist to Spawn |
|-----------------------|---------------------|
| Game code (.cs files) | unity-specialist |
| Shader / material files (.shader, .shadergraph, .mat) | unity-shader-specialist |
| UI / screen files (.uxml, .uss, Canvas prefabs) | unity-ui-specialist |
| Scene / prefab / level files (.unity, .prefab) | unity-specialist |
| Native extension / plugin files (.dll, native plugins) | unity-specialist |
| General architecture review | unity-specialist |
