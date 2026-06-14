using System;
using System.Collections.Generic;
using UnityEngine;

namespace SugarRush.Core
{
    /// <summary>
    /// Service layer for game configuration.
    /// Caches loaded configs and provides a single place to swap data sources (SO, JSON, remote).
    /// </summary>
    public class ConfigService
    {
        private static ConfigService _instance;
        public static ConfigService Instance => _instance ??= new ConfigService();

        private readonly System.Collections.Generic.Dictionary<string, object> _cache = new();

        /// <summary>
        /// Register a repository for a config type. Call before any Load.
        /// </summary>
        public void Register<T>(IConfigRepository<T> repository) where T : class
        {
            _cache[repository.ConfigId] = repository.Load();
        }

        /// <summary>
        /// Get a config. Throws if it has not been registered.
        /// </summary>
        public T Get<T>(string configId = null) where T : class
        {
            configId ??= typeof(T).Name;
            if (_cache.TryGetValue(configId, out object cached))
            {
                return cached as T;
            }
            throw new InvalidOperationException($"Config '{configId}' not registered. Call ConfigService.Instance.Register first.");
        }

        /// <summary>
        /// Try to get a config. Returns false if not registered.
        /// </summary>
        public bool TryGet<T>(out T config, string configId = null) where T : class
        {
            configId ??= typeof(T).Name;
            if (_cache.TryGetValue(configId, out object cached) && cached is T typed)
            {
                config = typed;
                return true;
            }
            config = null;
            return false;
        }

        /// <summary>
        /// Clear all cached configs. Useful for hot-reload or testing.
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
