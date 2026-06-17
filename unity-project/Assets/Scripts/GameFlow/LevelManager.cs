using SugarRush.Core;
using SugarRush.Gameplay;
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

        public LevelData Data => _levelData ?? GameConfig.Level;
        public float ElapsedTime { get; private set; }
        public float DistanceTraveled { get; private set; }

        private Vector3 _lastPosition;

        private void Start()
        {
            if (_levelData == null)
            {
                _levelData = GameConfig.Level;
            }

            if (_levelData == null)
            {
                Debug.LogWarning("[LevelManager] No LevelData assigned and none registered in GameConfig.", this);
                return;
            }

            ResolvePlayerReferences();

            if (_startPoint != null && _player != null)
            {
                _player.position = _startPoint.position;
            }

            if (_player != null)
            {
                _lastPosition = _player.position;
            }

            SaveService.Instance.Data.LastPlayedLevelId = _levelData.LevelId;
            SaveService.Instance.MarkDirty();

            Debug.Log($"[LevelManager] Level {_levelData.LevelId} started: {_levelData.DisplayName}", this);
        }

        private void ResolvePlayerReferences()
        {
            if (_player == null)
            {
                var playerGo = GameObject.FindWithTag("Player");
                if (playerGo != null) _player = playerGo.transform;
            }

            if (_player == null)
            {
                var bridge = FindObjectOfType<EntitasPlayerBridge>();
                if (bridge != null && bridge.PlayerTransform != null)
                    _player = bridge.PlayerTransform;
            }

            if (_startPoint == null)
            {
                var startGo = GameObject.Find("StartPoint");
                if (startGo != null) _startPoint = startGo.transform;
            }
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

        /// <summary>
        /// Resets run-level counters and snaps the player back to the start point.
        /// Does not reload the level data asset or rebuild the scene.
        /// </summary>
        public void Reset()
        {
            ElapsedTime = 0f;
            DistanceTraveled = 0f;

            ResolvePlayerReferences();

            if (_startPoint != null && _player != null)
            {
                _player.position = _startPoint.position;
            }

            if (_player != null)
            {
                _lastPosition = _player.position;
            }
        }
    }
}
