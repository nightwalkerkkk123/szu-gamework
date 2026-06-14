# Play Mode Tests

Integration tests that run in a real game scene.
Use for cross-system interactions, physics, coroutines, and level flow.

## Assembly Definition

Create `tests/PlayMode/PlayModeTests.asmdef` with references to your runtime
assemblies. Example:

```json
{
    "name": "PlayModeTests",
    "rootNamespace": "SugarRush.Tests",
    "references": [
        "UnityEngine.TestRunner",
        "UnityEditor.TestRunner",
        "Assembly-CSharp"
    ],
    "includePlatforms": [],
    "excludePlatforms": [],
    "allowUnsafeCode": false,
    "overrideReferences": true,
    "precompiledReferences": [
        "nunit.framework.dll"
    ],
    "autoReferenced": false,
    "defineConstraints": [
        "UNITY_INCLUDE_TESTS"
    ],
    "versionDefines": [],
    "noEngineReferences": false
}
```

## Example Test

```csharp
using System.Collections;
using NUnit.Framework;
using SugarRush.GameFlow;
using UnityEngine;
using UnityEngine.TestTools;

public class LevelFlowPlayModeTests
{
    [UnityTest]
    public IEnumerator Level_StartsInIntroState()
    {
        var go = new GameObject("Flow");
        var flow = go.AddComponent<GameFlowManager>();

        yield return null;

        Assert.AreEqual(GameState.Intro, flow.CurrentState);

        Object.Destroy(go);
    }
}
```
