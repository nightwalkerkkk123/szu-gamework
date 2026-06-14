using System;

namespace SugarRush.Core
{
    /// <summary>
    /// Service layer for game save data.
    /// Provides business-level operations and hides persistence details.
    /// </summary>
    public class SaveService
    {
        private static SaveService _instance;
        public static SaveService Instance => _instance ??= new SaveService(new JsonFileSaveRepository());

        private readonly ISaveRepository _repository;
        private GameSaveData _cached;
        private bool _isDirty;

        public SaveService(ISaveRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Current in-memory save data. Loads lazily on first access.
        /// </summary>
        public GameSaveData Data
        {
            get
            {
                if (_cached == null)
                {
                    _cached = _repository.Load();
                }
                return _cached;
            }
        }

        /// <summary>
        /// Force a reload from storage.
        /// </summary>
        public void Reload()
        {
            _cached = _repository.Load();
            _isDirty = false;
        }

        /// <summary>
        /// Persist current in-memory data to storage.
        /// </summary>
        public void Save()
        {
            if (_cached == null) return;
            _repository.Save(_cached);
            _isDirty = false;
        }

        /// <summary>
        /// Mark data dirty so it will be saved automatically or on next Save call.
        /// </summary>
        public void MarkDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        /// Save only if dirty.
        /// </summary>
        public void SaveIfDirty()
        {
            if (_isDirty)
            {
                Save();
            }
        }

        /// <summary>
        /// Record a completed run. Updates totals and per-level bests.
        /// </summary>
        public void RecordRun(string levelId, bool completed, float timeSeconds, int score, float distanceMeters)
        {
            var data = Data;
            data.TotalRuns++;
            data.TotalDistanceMeters += distanceMeters;

            if (completed)
            {
                data.SuccessfulRuns++;
                if (timeSeconds < data.BestTimeSeconds)
                {
                    data.BestTimeSeconds = timeSeconds;
                }
                if (score > data.BestScore)
                {
                    data.BestScore = score;
                }

                var levelBest = data.GetOrCreateLevelBest(levelId);
                levelBest.Completed = true;
                if (timeSeconds < levelBest.BestTimeSeconds)
                {
                    levelBest.BestTimeSeconds = timeSeconds;
                }
                if (score > levelBest.BestScore)
                {
                    levelBest.BestScore = score;
                }
            }

            _isDirty = true;
        }

        /// <summary>
        /// Delete all save data.
        /// </summary>
        public void DeleteAll()
        {
            _repository.Delete();
            _cached = new GameSaveData();
            _isDirty = false;
        }
    }
}
