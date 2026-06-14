using System;

namespace SugarRush.Core
{
    /// <summary>
    /// Serializable game save data model.
    /// Designed to be versioned for migration support.
    /// </summary>
    [Serializable]
    public class GameSaveData
    {
        public int Version = 1;

        // Progress
        public string LastPlayedLevelId = "L1";
        public int HighestUnlockedLevelIndex = 0;

        // Stats
        public int TotalRuns = 0;
        public int SuccessfulRuns = 0;
        public float BestTimeSeconds = float.MaxValue;
        public float TotalDistanceMeters = 0f;
        public int BestScore = 0;

        // Per-level bests
        public System.Collections.Generic.List<LevelBest> LevelBests = new();

        [Serializable]
        public class LevelBest
        {
            public string LevelId;
            public float BestTimeSeconds = float.MaxValue;
            public int BestScore;
            public bool Completed;
        }

        public LevelBest GetOrCreateLevelBest(string levelId)
        {
            var best = LevelBests.Find(b => b.LevelId == levelId);
            if (best == null)
            {
                best = new LevelBest { LevelId = levelId };
                LevelBests.Add(best);
            }
            return best;
        }
    }
}
