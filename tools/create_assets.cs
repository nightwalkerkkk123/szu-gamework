System.IO.Directory.CreateDirectory("Assets/Data/Configs");
System.IO.Directory.CreateDirectory("Assets/Data/Items");

var glucose = UnityEngine.ScriptableObject.CreateInstance<SugarRush.Foundation.GlucoseConfig>();
glucose.name = "GlucoseConfig";
UnityEditor.AssetDatabase.CreateAsset(glucose, "Assets/Data/Configs/GlucoseConfig.asset");

var skiing = UnityEngine.ScriptableObject.CreateInstance<SugarRush.Gameplay.SkiingConfig>();
skiing.name = "SkiingConfig";
UnityEditor.AssetDatabase.CreateAsset(skiing, "Assets/Data/Configs/SkiingConfig.asset");

var level = UnityEngine.ScriptableObject.CreateInstance<SugarRush.GameFlow.LevelData>();
level.name = "L1_HospitalChalet";
UnityEditor.AssetDatabase.CreateAsset(level, "Assets/Data/Configs/L1_HospitalChalet.asset");

var insulin = UnityEngine.ScriptableObject.CreateInstance<SugarRush.Gameplay.Items.InsulinSprayEffect>();
insulin.name = "InsulinSpray";
UnityEditor.AssetDatabase.CreateAsset(insulin, "Assets/Data/Items/InsulinSpray.asset");

var pills = UnityEngine.ScriptableObject.CreateInstance<SugarRush.Gameplay.Items.HypoglycemicPillsEffect>();
pills.name = "HypoglycemicPills";
UnityEditor.AssetDatabase.CreateAsset(pills, "Assets/Data/Items/HypoglycemicPills.asset");

var snowflake = UnityEngine.ScriptableObject.CreateInstance<SugarRush.Gameplay.Items.HighSugarSnowflakeEffect>();
snowflake.name = "HighSugarSnowflake";
UnityEditor.AssetDatabase.CreateAsset(snowflake, "Assets/Data/Items/HighSugarSnowflake.asset");

UnityEditor.AssetDatabase.SaveAssets();
UnityEngine.Debug.Log("Core assets created.");
return "done";
