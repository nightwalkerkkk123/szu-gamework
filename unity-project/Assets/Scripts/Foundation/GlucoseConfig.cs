using SugarRush.Core;
using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Configuration for the Glucose system. Create via Assets > Create > SugarRush > Glucose Config.
    /// </summary>
    [CreateAssetMenu(fileName = "GlucoseConfig", menuName = "SugarRush/Glucose Config")]
    public class GlucoseConfig : ScriptableObject
    {
        [Header("Base")]
        [Tooltip("Starting glucose value (0-100).")]
        [Range(0f, 100f)] public float startValue = 60f;

        [Header("Passive Drift (per zone, GDD §4.4)")]
        [Tooltip("Drift per second while in Low Crisis (0..lowCrisisThreshold).")]
        public float driftLowCrisis = -0.2f;

        [Tooltip("Drift per second while in Low Warning (lowCrisisThreshold..lowWarningThreshold).")]
        public float driftLowWarning = -0.3f;

        [Tooltip("Drift per second while in Safe zone (lowWarningThreshold..highWarningThreshold).")]
        public float driftNormal = -0.5f;

        [Tooltip("Drift per second while in High Warning (highWarningThreshold..highCrisisThreshold).")]
        public float driftHighWarning = 0.8f;

        [Tooltip("Drift per second while in High Crisis (highCrisisThreshold..100).")]
        public float driftHighCrisis = 1.2f;

        [Tooltip("Legacy single-value decay kept temporarily for asset compat. Use the per-zone drift fields above instead.")]
        [System.Obsolete("Use driftLowCrisis / driftLowWarning / driftNormal / driftHighWarning / driftHighCrisis instead.")]
        public float passiveDecayPerSecond = 0.5f;

        [Header("Zones")]
        [Tooltip("Upper bound of Low Crisis zone (inclusive).")]
        public float lowCrisisThreshold = 10f;

        [Tooltip("Upper bound of Low Warning zone.")]
        public float lowWarningThreshold = 30f;

        [Tooltip("Lower bound of High Warning zone.")]
        public float highWarningThreshold = 75f;

        [Tooltip("Lower bound of High Crisis zone.")]
        public float highCrisisThreshold = 95f;

        [Header("Crisis Failure")]
        [Tooltip("Seconds in Low Crisis before failure.")]
        public float lowCrisisFailTime = 3f;

        [Tooltip("Seconds in High Crisis before failure.")]
        public float highCrisisFailTime = 3f;

        [Header("Modifiers")]
        [Tooltip("Speed multiplier applied at the center of each zone. Safe=1.0. Linearly interpolated between zones.")]
        public AnimationCurve speedModifierCurve = AnimationCurve.Linear(0f, 0.6f, 1f, 1.4f);

        [Tooltip("Control multiplier (input responsiveness) at zone extremes.")]
        public AnimationCurve controlModifierCurve = AnimationCurve.Linear(0f, 0.5f, 1f, 1.2f);

        [Tooltip("Vision multiplier (post-processing/vignette intensity placeholder).")]
        public AnimationCurve visionModifierCurve = AnimationCurve.Linear(0f, 1f, 1f, 0.4f);

        [Header("Debug")]
        public bool logZoneChanges = false;

        public GlucoseZone GetZone(float value)
        {
            if (value <= lowCrisisThreshold) return GlucoseZone.LowCrisis;
            if (value <= lowWarningThreshold) return GlucoseZone.LowWarning;
            if (value >= highCrisisThreshold) return GlucoseZone.HighCrisis;
            if (value >= highWarningThreshold) return GlucoseZone.HighWarning;
            return GlucoseZone.Safe;
        }

        /// <summary>
        /// Normalizes glucose value to 0-1 range for curve evaluation.
        /// 0 = lowest possible (0), 1 = highest possible (100).
        /// </summary>
        public float Normalize(float value) => Mathf.Clamp01(value / 100f);
    }
}
