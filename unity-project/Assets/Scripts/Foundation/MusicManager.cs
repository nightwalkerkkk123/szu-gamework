using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Scene-level background music. Requires an AudioSource on the same GameObject
    /// (pre-configured in editor so playOnAwake is processed correctly).
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        private AudioSource _source;

        private void Awake()
        {
            MusicManager[] existing = FindObjectsOfType<MusicManager>();
            if (existing.Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            _source = GetComponent<AudioSource>();
            _source.loop = true;
            _source.spatialBlend = 0f;
            _source.volume = 1f;
            _source.priority = 0;
            _source.playOnAwake = true;
        }
    }
}
