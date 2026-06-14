using SugarRush.Foundation;
using SugarRush.GameFlow;
using SugarRush.Gameplay;
using UnityEngine;

namespace SugarRush.Core
{
    /// <summary>
    /// Centralized, backend-style config loader for SugarRush.
    /// Registers all repositories at game start and exposes typed config accessors.
    /// </summary>
    public static class GameConfig
    {
        public const string GlucoseConfigId = "Glucose";
        public const string SkiingConfigId = "Skiing";
        public const string LevelDataConfigId = "LevelData";

        /// <summary>
        /// Initialize config service with ScriptableObject-backed repositories.
        /// </summary>
        public static void Initialize(
            GlucoseConfig glucoseConfig,
            SkiingConfig skiingConfig,
            LevelData levelData)
        {
            var service = ConfigService.Instance;
            service.ClearCache();

            service.Register(new ScriptableObjectConfigRepository<GlucoseConfig>(glucoseConfig, GlucoseConfigId));
            service.Register(new ScriptableObjectConfigRepository<SkiingConfig>(skiingConfig, SkiingConfigId));
            service.Register(new ScriptableObjectConfigRepository<LevelData>(levelData, LevelDataConfigId));
        }

        public static GlucoseConfig Glucose => ConfigService.Instance.Get<GlucoseConfig>(GlucoseConfigId);
        public static SkiingConfig Skiing => ConfigService.Instance.Get<SkiingConfig>(SkiingConfigId);
        public static LevelData Level => ConfigService.Instance.Get<LevelData>(LevelDataConfigId);
    }
}
