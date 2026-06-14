using UnityEngine;

namespace SugarRush.Core
{
    /// <summary>
    /// Repository backed by a Unity ScriptableObject asset.
    /// This is the default local config source for SugarRush.
    /// </summary>
    public class ScriptableObjectConfigRepository<T> : IConfigRepository<T> where T : ScriptableObject
    {
        private readonly T _asset;

        public string ConfigId { get; }

        public ScriptableObjectConfigRepository(T asset, string configId = null)
        {
            _asset = asset;
            ConfigId = configId ?? typeof(T).Name;
        }

        public T Load()
        {
            return _asset;
        }

        public void Save(T config)
        {
            // ScriptableObject assets are edited through the Editor; runtime writes are unsupported.
            Debug.LogWarning($"[{nameof(ScriptableObjectConfigRepository<T>)}] Save is not supported for {ConfigId}.");
        }
    }
}
