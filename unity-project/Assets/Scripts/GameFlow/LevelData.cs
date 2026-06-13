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

        [Header("Objectives")]
        [field: SerializeField] public float TargetDistanceMeters { get; private set; } = 100f;
        [field: SerializeField] public float ParTimeSeconds { get; private set; } = 75f;

        [Header("Glucose")]
        [field: SerializeField] public float StartGlucose { get; private set; } = 60f;

        [Header("Score")]
        [field: SerializeField] public int BaseScore { get; private set; } = 1000;
        [field: SerializeField] public float TimeBonusMultiplier { get; private set; } = 10f;
    }
}
