using SugarRush.Gameplay;
using System.Collections.Generic;
using UnityEngine;

namespace SugarRush.GameFlow
{
    /// <summary>
    /// Infinite chunk spawner for the 3D fused endless-runner mode.
    /// Inspired by the Endless-Runner-Entitas-ECS chunk generation pattern.
    /// </summary>
    public class ChunkSpawner3D : MonoBehaviour
    {
        [System.Serializable]
        public class ChunkDefinition
        {
            [Tooltip("Prefab to instantiate. Should face +X and have its pivot at the left/start edge.")]
            public GameObject Prefab;

            [Tooltip("World width of this chunk along the X axis.")]
            public float Width = 100f;

            [Tooltip("Weight used when randomly selecting the next chunk. Higher = more frequent.")]
            public float Weight = 1f;
        }

        [SerializeField] private Transform _player;
        [SerializeField] private List<ChunkDefinition> _chunks = new();

        [Header("Spawning")]
        [SerializeField] private float _spawnDistance = 60f;
        [SerializeField] private float _destroyDistance = 40f;
        [SerializeField] private int _initialChunkCount = 2;

        private float _nextChunkX;
        private readonly List<GameObject> _activeChunks = new();

        public float NextChunkX => _nextChunkX;

        private void Start()
        {
            if (_player == null)
            {
                var playerGo = GameObject.FindWithTag("Player");
                if (playerGo != null) _player = playerGo.transform;
            }

            if (_chunks == null || _chunks.Count == 0)
            {
                Debug.LogError("[ChunkSpawner3D] No chunk definitions assigned.", this);
                enabled = false;
                return;
            }

            _nextChunkX = 0f;

            for (int i = 0; i < _initialChunkCount; i++)
            {
                SpawnNextChunk();
            }
        }

        private void Update()
        {
            if (_player == null) return;

            float playerX = _player.position.x;

            while (playerX + _spawnDistance > _nextChunkX)
            {
                SpawnNextChunk();
            }

            for (int i = _activeChunks.Count - 1; i >= 0; i--)
            {
                var chunk = _activeChunks[i];
                if (chunk == null)
                {
                    _activeChunks.RemoveAt(i);
                    continue;
                }

                float chunkEndX = chunk.transform.position.x + chunk.transform.localScale.x;
                // Approximate destruction check using the chunk's bounds if available.
                var renderer = chunk.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    chunkEndX = renderer.bounds.max.x;
                }

                if (playerX - _destroyDistance > chunkEndX)
                {
                    Destroy(chunk);
                    _activeChunks.RemoveAt(i);
                }
            }
        }

        private void SpawnNextChunk()
        {
            var definition = PickChunk();
            if (definition.Prefab == null) return;

            var chunk = Instantiate(definition.Prefab, new Vector3(_nextChunkX, 0f, 0f), Quaternion.identity, transform);
            chunk.name = $"Chunk_{definition.Prefab.name}_{_activeChunks.Count}";
            _activeChunks.Add(chunk);

            _nextChunkX += definition.Width;
        }

        private ChunkDefinition PickChunk()
        {
            float totalWeight = 0f;
            foreach (var chunk in _chunks)
            {
                if (chunk.Prefab != null) totalWeight += chunk.Weight;
            }

            float roll = Random.Range(0f, totalWeight);
            float cumulative = 0f;
            foreach (var chunk in _chunks)
            {
                if (chunk.Prefab == null) continue;
                cumulative += chunk.Weight;
                if (roll <= cumulative) return chunk;
            }

            return _chunks.Find(c => c.Prefab != null) ?? _chunks[0];
        }
    }
}
