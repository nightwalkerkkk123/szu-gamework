namespace SugarRush.Core
{
    /// <summary>
    /// Backend-style repository for game save data.
    /// Abstracts persistence mechanism (local file, cloud, PlayerPrefs, etc.).
    /// </summary>
    public interface ISaveRepository
    {
        /// <summary>
        /// Load save data. Returns a fresh object if none exists.
        /// </summary>
        GameSaveData Load();

        /// <summary>
        /// Persist save data.
        /// </summary>
        void Save(GameSaveData data);

        /// <summary>
        /// Delete all save data.
        /// </summary>
        void Delete();

        /// <summary>
        /// True if save data exists.
        /// </summary>
        bool Exists();
    }
}
