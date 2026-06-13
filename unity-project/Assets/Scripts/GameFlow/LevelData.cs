using System.Collections.Generic;
using UnityEngine;

namespace SugarRush.GameFlow
{
    /// <summary>
    /// Static level configuration. Create via Assets > Create > SugarRush > Level Data.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelData", menuName = "SugarRush/Level Data")]
    public class LevelData : ScriptableObject
    {
        [field: SerializeField] public string LevelId { get; private set; } = "L1";
        [field: SerializeField] public string DisplayName { get; private set; } = "Hospital Chalet";
        [field: SerializeField, TextArea] public string Description { get; private set; } = "";

        [field: SerializeField] public float TargetDistanceMeters { get; private set; } = 100f;
        [field: SerializeField] public float ParTimeSeconds { get; private set; } = 75f;
        [field: SerializeField] public float StartGlucose { get; private set; } = 60f;
        [field: SerializeField] public int BaseScore { get; private set; } = 1000;
        [field: SerializeField] public float TimeBonusMultiplier { get; private set; } = 10f;

        [Tooltip("Segments that make up this level, placed sequentially from start to finish.")]
        [field: SerializeField] public List<LevelSegmentData> Segments { get; private set; } = new();

        /// <summary>
        /// Total length of all segments along the X axis (approximate, assumes shallow slopes).
        /// </summary>
        public float TotalSegmentLength
        {
            get
            {
                float total = 0f;
                if (Segments == null) return total;

                foreach (var segment in Segments)
                {
                    if (segment != null)
                    {
                        total += segment.Length * Mathf.Cos(segment.SlopeAngle * Mathf.Deg2Rad);
                    }
                }
                return total;
            }
        }
    }
}
