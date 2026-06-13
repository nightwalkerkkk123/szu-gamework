using System;
using UnityEngine;

namespace SugarRush.Core
{
    /// <summary>
    /// Project-wide static event bus for decoupled systems.
    /// Keep payloads small and well-defined; prefer direct references for high-frequency data.
    /// </summary>
    public static class GameEvents
    {
        // Game flow
        public static event Action OnGameStarted;
        public static event Action OnGamePaused;
        public static event Action OnGameResumed;
        public static event Action<bool> OnGameFinished; // true = win, false = lose

        // Glucose
        public static event Action<float> OnGlucoseChanged; // 0-100 normalized or absolute? Use absolute.
        public static event Action<GlucoseZone> OnGlucoseZoneChanged;

        public static void RaiseGameStarted() => OnGameStarted?.Invoke();
        public static void RaiseGamePaused() => OnGamePaused?.Invoke();
        public static void RaiseGameResumed() => OnGameResumed?.Invoke();
        public static void RaiseGameFinished(bool win) => OnGameFinished?.Invoke(win);

        public static void RaiseGlucoseChanged(float value) => OnGlucoseChanged?.Invoke(value);
        public static void RaiseGlucoseZoneChanged(GlucoseZone zone) => OnGlucoseZoneChanged?.Invoke(zone);
    }

    public enum GlucoseZone
    {
        LowCrisis,
        LowWarning,
        Safe,
        HighWarning,
        HighCrisis
    }
}
