using System.Collections;
using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Singleton-ish audio pool. Manages 8 AudioSource components on a hidden
    /// host GameObject. All public API is static for zero-friction access from
    /// any call site.
    ///
    /// Subscribe to SettingsService.OnSettingsChanged to re-apply master volume
    /// whenever settings are saved.
    /// </summary>
    public static class AudioManager
    {
        private const int PoolSize = 8;

        private static AudioManagerHost _host;
        private static AudioSource[] _pool;
        private static bool _initialized;
        private static bool _enabled = true;

        /// <summary>Lazy-init the host, pool, and subscribe to settings.</summary>
        private static void EnsureInitialized()
        {
            if (_initialized) return;
            _initialized = true;

            var go = new GameObject("AudioManagerHost");
            Object.DontDestroyOnLoad(go);
            go.hideFlags = HideFlags.HideAndDontSave;

            _host = go.AddComponent<AudioManagerHost>();
            _host.StartCoroutine(HostCoroutine());

            _pool = new AudioSource[PoolSize];
            for (int i = 0; i < PoolSize; i++)
            {
                _pool[i] = go.AddComponent<AudioSource>();
                _pool[i].playOnAwake = false;
                _pool[i].spatialBlend = 0f; // 2D
            }

            SettingsService.OnSettingsChanged += OnSettingsChanged;
            AudioListener.volume = SettingsService.MasterVolume;
        }

        private static IEnumerator HostCoroutine()
        {
            // Runs once per frame: recycle any stopped sources.
            while (true)
            {
                yield return null;
                if (_host == null) yield break;
                for (int i = 0; i < PoolSize; i++)
                {
                    if (_pool[i].isPlaying) continue;
                    _host.StartCoroutine(ReturnAfterClip(_pool[i]));
                }
            }
        }

        private static IEnumerator ReturnAfterClip(AudioSource source)
        {
            if (source.clip == null) yield break;
            yield return new WaitForSeconds(source.clip.length + 0.05f);
            if (source == null) yield break;
            source.clip = null;
        }

        private static void OnSettingsChanged()
        {
            if (!_initialized) return;
            AudioListener.volume = SettingsService.MasterVolume;
        }

        /// <summary>
        /// Borrow an AudioSource from the pool, set clip + random pitch, play, return
        /// after clip length. Thread-safe only in the Unity main thread (standard for games).
        /// </summary>
        public static void PlaySfx(AudioClip clip, float volume = 0.5f, float pitchVariation = 0.1f)
        {
            if (!_enabled || clip == null) return;
            EnsureInitialized();

            // Find a free source
            AudioSource source = null;
            for (int i = 0; i < PoolSize; i++)
            {
                if (!_pool[i].isPlaying && _pool[i].clip == null)
                {
                    source = _pool[i];
                    break;
                }
            }

            // All in use — skip (drop rare frame)
            if (source == null) return;

            float pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            source.clip = clip;
            source.volume = Mathf.Clamp01(volume) * SettingsService.SfxVolume;
            source.pitch = Mathf.Clamp(pitch, 0.5f, 2f);
            source.Play();
        }

        /// <summary>
        /// Apply a new master volume to the global AudioListener.
        /// </summary>
        public static void SetMasterVolume(float v)
        {
            SettingsService.MasterVolume = v;
        }

        /// <summary>
        /// Global mute toggle. When disabled all PlaySfx calls become no-ops.
        /// </summary>
        public static void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        // ── Internal MonoBehaviour used only to host coroutines ──────────────────

        private sealed class AudioManagerHost : MonoBehaviour { }
    }
}
