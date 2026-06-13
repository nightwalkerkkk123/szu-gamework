using SugarRush.Gameplay;
using SugarRush.Gameplay.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SugarRush.GameFlow
{
    /// <summary>
    /// Data-driven definition of a single level segment.
    /// Used by the scene builder to generate ground, obstacles, pickups and hazards.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelSegment", menuName = "SugarRush/Level Segment")]
    public class LevelSegmentData : ScriptableObject
    {
        [field: SerializeField] public string SegmentId { get; private set; } = "Segment";

        [Tooltip("Horizontal length of this segment in world units.")]
        [field: SerializeField] public float Length { get; private set; } = 60f;

        [Tooltip("Ground slope angle in degrees (0 = flat, positive = downhill to the right).")]
        [field: SerializeField, Range(0f, 45f)] public float SlopeAngle { get; private set; } = 12f;

        [Tooltip("Ground segment piece length. Smaller values = more platform pieces.")]
        [field: SerializeField] public float GroundPieceLength { get; private set; } = 12f;

        [field: SerializeField] public List<SegmentObstacle> Obstacles { get; private set; } = new();
        [field: SerializeField] public List<SegmentPickup> Pickups { get; private set; } = new();
        [field: SerializeField] public List<SegmentHazard> Hazards { get; private set; } = new();
    }

    [Serializable]
    public class SegmentObstacle
    {
        [Tooltip("Distance from the start of the segment along the slope.")]
        public float DistanceAlongSegment;

        [Tooltip("Height above the ground at this distance.")]
        public float HeightOffset = 0.4f;

        public Obstacle.ObstacleType Type = Obstacle.ObstacleType.Stumble;
        public Vector2 Size = new Vector2(0.8f, 0.8f);
        public Color Color = new Color(0.55f, 0.4f, 0.35f);
    }

    [Serializable]
    public class SegmentPickup
    {
        [Tooltip("Distance from the start of the segment along the slope.")]
        public float DistanceAlongSegment;

        [Tooltip("Height above the ground at this distance.")]
        public float HeightOffset = 1.5f;

        public ItemEffect ItemEffect;
        public Color Color = Color.yellow;
    }

    [Serializable]
    public class SegmentHazard
    {
        [Tooltip("Distance from the start of the segment along the slope.")]
        public float DistanceAlongSegment;

        [Tooltip("Vertical center offset from the ground at this distance.")]
        public float CenterHeightOffset = 4f;

        public Vector2 Size = new Vector2(20f, 8f);
        public float DeltaPerSecond = -5f;
    }
}
