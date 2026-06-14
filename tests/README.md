# Test Infrastructure

**Engine**: Unity 2022.3 LTS (Built-in Render Pipeline, Unity 2D Physics)  
**Test Framework**: Unity Test Framework (NUnit, included in Unity 2019+)  
**CI**: `.github/workflows/tests.yml`  
**Setup date**: 2026-06-13  

## Directory Layout

```
tests/
  unit/              # Isolated unit tests (formulas, state machines, logic)
  integration/       # Cross-system and save/load tests
  performance/       # Frame-time and memory benchmarks
  EditMode/          # Unity Edit Mode tests — pure logic, no Play Mode
  PlayMode/          # Unity Play Mode tests — physics, coroutines, scene flow
  smoke/             # Critical path test list for /smoke-check gate
  evidence/          # Screenshot logs and manual test sign-off records
```

## Running Tests

1. Open **Window → General → Test Runner** in the Unity Editor.
2. Switch to the **EditMode** or **PlayMode** tab.
3. Select tests and click **Run**.

For automated runs, the CI workflow uses `game-ci/unity-test-runner@v4`.

## Test Naming

- **Files**: `[System]_[Feature]Tests.cs`
- **Methods**: `[System]_[Scenario]_[ExpectedResult]`
- **Example**: `GlucoseSystemTests.cs` → `Glucose_ApplyDelta_EntersLowCrisis()`

## Story Type → Test Evidence

| Story Type | Required Evidence | Location |
|---|---|---|
| Logic | Automated unit test — must pass | `tests/unit/[System]/` or `tests/EditMode/` |
| Integration | Integration test OR playtest doc | `tests/integration/` or `tests/PlayMode/` |
| Visual/Feel | Screenshot + lead sign-off | `tests/evidence/` |
| UI | Manual walkthrough OR interaction test | `tests/evidence/` |
| Config/Data | Smoke check pass | `tests/smoke/critical-paths.md` |
| Performance | Benchmark log | `tests/performance/` |

## Required Core Tests

Per `docs/technical-preferences.md`:

- 血糖系统逻辑 (Glucose system)
- 滑行控制器 (Skiing controller)
- 关卡完成判定 (Level completion)
- 道具效果 (Item effects)

## CI

Tests run automatically on every push to `main` and on every pull request.
A failed test suite blocks merging.

**Note**: Unity CI requires a `UNITY_LICENSE` secret in the GitHub repository
settings before the first CI run.
