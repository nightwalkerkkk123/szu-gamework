using UnityEngine;

namespace SugarRush.GameFlow
{
    /// <summary>
    /// Loads level data and tracks runtime level metrics (distance, time).
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private LevelData _levelData;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _startPoint;

        public LevelData Data => _levelData;
        public float ElapsedTime { get; private set; }
        public float DistanceTraveled { get; private set; }

        private Vector3 _lastPosition;

        private void Start()
        {
            if (_levelData == null)
            {
                Debug.LogWarning("[LevelManager] No LevelData assigned.", this);
                return;
            }

            if (_startPoint != null && _player != null)
            {
                _player.position = _startPoint.position;
            }

            if (_player != null)
            {
                _lastPosition = _player.position;
            }

            Debug.Log($"[LevelManager] Level {_levelData.LevelId} started: {_levelData.DisplayName}", this);
        }

        private void Update()
        {
            if (Time.timeScale <= 0f) return;

            ElapsedTime += Time.deltaTime;

            if (_player != null)
            {
                float delta = _player.position.x - _lastPosition.x;
                if (delta > 0f)
                {
                    DistanceTraveled += delta;
                }
                _lastPosition = _player.position;
            }
        }

        public float NormalizedProgress => _levelData != null && _levelData.TargetDistanceMeters > 0f
            ? Mathf.Clamp01(DistanceTraveled / _levelData.TargetDistanceMeters)
            : 0f;
    }
}
