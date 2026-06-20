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

        [Header("Downhill Terrain")]
        [Tooltip("Minimum vertical drop (units) between consecutive chunks. Track always descends — never rises — so the auto-running player never faces an un-climbable wall.")]
        [SerializeField] private float _minDropPerChunk = 1f;
        [Tooltip("Maximum vertical drop (units) between consecutive chunks. Larger spread = bumpier slope.")]
        [SerializeField] private float _maxDropPerChunk = 6f;

        private float _nextChunkX;
        private float _nextChunkY;
        private readonly List<GameObject> _activeChunks = new();

        public float NextChunkX => _nextChunkX;
        public float NextChunkY => _nextChunkY;

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

                Bounds bounds = CalculateBounds(chunk);
                float chunkEndX = bounds.max.x;

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

            // Instantiate at origin first; prefabs may have been saved at arbitrary world positions.
            var chunk = Instantiate(definition.Prefab, Vector3.zero, Quaternion.identity, transform);
            chunk.name = $"Chunk_{definition.Prefab.name}_{_activeChunks.Count}";

            // These chunks were authored for a 2D side view: "Background" pieces are
            // decoration that, in 3D forward view, become ugly floating blocks AND carry
            // solid colliders intruding into the player's z=0 plane. Remove them entirely.
            StripDecor(chunk);

            Bounds bounds = CalculateBounds(chunk);

            // Align the chunk's start (left edge) to _nextChunkX and its bottom to the
            // running descent height _nextChunkY. Chunks stay contiguous in X (always a
            // landing ahead) while stepping DOWN in Y to form a rolling ski slope.
            Vector3 position = new Vector3(_nextChunkX - bounds.min.x, _nextChunkY - bounds.min.y, 0f);
            chunk.transform.position = position;

            _activeChunks.Add(chunk);
            _nextChunkX += bounds.size.x;
            _nextChunkY -= Random.Range(_minDropPerChunk, _maxDropPerChunk);
        }

        private static void StripDecor(GameObject root)
        {
            foreach (var t in root.GetComponentsInChildren<Transform>(true))
            {
                if (t.name.StartsWith("Background"))
                {
                    t.gameObject.SetActive(false);
                }
            }
        }

        private static Bounds CalculateBounds(GameObject root)
        {
            var colliders = root.GetComponentsInChildren<Collider>();
            if (colliders.Length == 0)
            {
                var renderers = root.GetComponentsInChildren<Renderer>();
                if (renderers.Length == 0) return new Bounds(root.transform.position, Vector3.zero);

                Bounds renderBounds = renderers[0].bounds;
                for (int i = 1; i < renderers.Length; i++)
                {
                    renderBounds.Encapsulate(renderers[i].bounds);
                }
                return renderBounds;
            }

            Bounds bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
            {
                if (colliders[i].isTrigger) continue;
                bounds.Encapsulate(colliders[i].bounds);
            }
            return bounds;
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
