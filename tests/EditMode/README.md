# Edit Mode Tests

Unit tests that run without entering Play Mode.
Use for pure logic: formulas, state machines, data validation, and config parsing.

## Assembly Definition

Create `tests/EditMode/EditModeTests.asmdef` with references to your runtime
assemblies (e.g., `Assembly-CSharp`). Example:

```json
{
    "name": "EditModeTests",
    "rootNamespace": "SugarRush.Tests",
    "references": [
        "UnityEngine.TestRunner",
        "UnityEditor.TestRunner",
        "Assembly-CSharp"
    ],
    "includePlatforms": [
        "Editor"
    ],
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
using NUnit.Framework;
using SugarRush.Foundation;
using UnityEngine;

public class GlucoseSystemEditTests
{
    [Test]
    public void GlucoseConfig_GetZone_ValueBelowLowCrisisThreshold_ReturnsLowCrisis()
    {
        var config = ScriptableObject.CreateInstance<GlucoseConfig>();
        config.lowCrisisThreshold = 10f;

        Assert.AreEqual(GlucoseZone.LowCrisis, config.GetZone(5f));
    }
}
```
