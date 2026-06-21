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

        [Header("Finite Track")]
        [Tooltip("0 = infinite endless run. >0 = finite: stop spawning chunks past this X distance, then place the finish line.")]
        [SerializeField] private float _totalDistance = 0f;
        [Tooltip("Finish line prefab (FinishLine3D + trigger collider). Instantiated at the end of a finite track.")]
        [SerializeField] private GameObject _finishLinePrefab;

        private float _nextChunkX;
        private float _nextChunkY;
        private bool _finishPlaced;

        // Self-managed chunk record: the spawner owns each chunk's authored end-X (computed
        // once at spawn). Destroy decisions use this stored value — never a fragile re-derivation
        // from live colliders, which previously mis-sized chunks and culled the ground under the player.
        private struct ActiveChunk
        {
            public GameObject Go;
            public float EndX;
        }
        private readonly List<ActiveChunk> _activeChunks = new();

        // [DIAG] Temporary instrumentation — remove once the "track runs out" bug is root-caused.
        private float _diagTimer;
        private int _diagSpawnCount;

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
            if (_player == null)
            {
                Debug.LogWarning("[ChunkSpawner3D-DIAG] _player is NULL — generation halted, only initial chunks exist.");
                return;
            }

            float playerX = _player.position.x;

            // [DIAG] Heartbeat: shows whether playerX advances and the spawn frontier keeps up.
            _diagTimer += Time.deltaTime;
            if (_diagTimer >= 1f)
            {
                _diagTimer = 0f;
                Debug.Log($"[ChunkSpawner3D-DIAG] heartbeat playerX={playerX:F1} playerY={_player.position.y:F1} nextChunkX={_nextChunkX:F1} frontierGap={_nextChunkX - playerX:F1} active={_activeChunks.Count} totalSpawned={_diagSpawnCount}");
            }

            bool finite = _totalDistance > 0f;
            while (playerX + _spawnDistance > _nextChunkX && (!finite || _nextChunkX < _totalDistance))
            {
                SpawnNextChunk();
            }

            if (finite && !_finishPlaced && _nextChunkX >= _totalDistance)
            {
                PlaceFinishLine();
            }

            // Cull only chunks whose recorded end is safely behind the player. Uses the
            // spawn-time end-X, so it can never mistake a chunk under the player for a stale one.
            for (int i = _activeChunks.Count - 1; i >= 0; i--)
            {
                if (_activeChunks[i].Go == null)
                {
                    _activeChunks.RemoveAt(i);
                    continue;
                }

                if (playerX - _destroyDistance > _activeChunks[i].EndX)
                {
                    Destroy(_activeChunks[i].Go);
                    _activeChunks.RemoveAt(i);
                }
            }
        }

        private void PlaceFinishLine()
        {
            _finishPlaced = true;
            if (_finishLinePrefab == null)
            {
                Debug.LogWarning("[ChunkSpawner3D] Finite track reached its end but no finish line prefab is assigned.", this);
                return;
            }

            // Sit the gate at the frontier, on the running descent surface. The trigger box
            // is authored tall enough to catch the sphere regardless of the exact drop step.
            var position = new Vector3(_nextChunkX, _nextChunkY, 0f);
            var finish = Instantiate(_finishLinePrefab, position, Quaternion.identity, transform);
            finish.name = "FinishLine3D";
            Debug.Log($"[ChunkSpawner3D] Finish line placed at x={_nextChunkX:F1} y={_nextChunkY:F1}.", this);
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

            float chunkEndX = _nextChunkX + bounds.size.x;
            _activeChunks.Add(new ActiveChunk { Go = chunk, EndX = chunkEndX });

            // [DIAG] Log placement + scan for holes in the walkable ground.
            _diagSpawnCount++;
            float startX = _nextChunkX;
            float biggestGap = LargestGroundGapX(chunk);
            Debug.Log($"[ChunkSpawner3D-DIAG] spawn #{_diagSpawnCount} {chunk.name} X[{startX:F1}..{chunkEndX:F1}] boundsW={bounds.size.x:F1} defWidth={definition.Width} topY={bounds.max.y:F1} biggestFloorGapX={biggestGap:F1}");
            if (biggestGap > 1.5f)
            {
                Debug.LogWarning($"[ChunkSpawner3D-DIAG] HOLE in {chunk.name}: {biggestGap:F1}-unit gap in the floor — the rolling sphere will fall through here.");
            }

            _nextChunkX += bounds.size.x;
            _nextChunkY -= Random.Range(_minDropPerChunk, _maxDropPerChunk);
        }

        // [DIAG] Largest horizontal gap between the active, non-trigger floor colliders of a
        // chunk. A large value means the walkable surface has a hole the sphere will fall into.
        private static float LargestGroundGapX(GameObject root)
        {
            var intervals = new List<Vector2>(); // (minX, maxX) per solid collider
            foreach (var c in root.GetComponentsInChildren<Collider>())
            {
                if (c.isTrigger || !c.gameObject.activeInHierarchy) continue;
                intervals.Add(new Vector2(c.bounds.min.x, c.bounds.max.x));
            }
            if (intervals.Count <= 1) return 0f;

            intervals.Sort((a, b) => a.x.CompareTo(b.x));
            float biggest = 0f;
            float coveredTo = intervals[0].y;
            for (int i = 1; i < intervals.Count; i++)
            {
                float gap = intervals[i].x - coveredTo;
                if (gap > biggest) biggest = gap;
                if (intervals[i].y > coveredTo) coveredTo = intervals[i].y;
            }
            return biggest;
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
