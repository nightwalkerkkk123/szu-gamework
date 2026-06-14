namespace SugarRush.Core
{
    /// <summary>
    /// Backend-style repository for game configuration data.
    /// Abstracts where configs come from (ScriptableObject, JSON, remote server, etc.).
    /// </summary>
    public interface IConfigRepository<T> where T : class
    {
        /// <summary>
        /// Load the configuration. Returns null if not found.
        /// </summary>
        T Load();

        /// <summary>
        /// Save configuration back to the repository (if supported).
        /// </summary>
        void Save(T config);

        /// <summary>
        /// Unique identifier for this config, used for caching and diagnostics.
        /// </summary>
        string ConfigId { get; }
    }
}
