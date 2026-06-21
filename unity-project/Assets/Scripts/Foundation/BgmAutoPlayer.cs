using UnityEngine;

namespace SugarRush.Foundation
{
    public class BgmAutoPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip _bgm;
        [SerializeField] private float _volume = 3f;

        private AudioSource _source;

        private void Awake()
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.clip = _bgm;
            _source.loop = true;
            _source.spatialBlend = 0f;
            _source.priority = 0;
        }

        private void Start()
        {
            AudioListener.volume = _volume;
            _source.volume = 1f;
            _source.Play();
        }

        private void Update()
        {
            // Retry if failed to start (audio system timing)
            if (!_source.isPlaying && _source.clip != null)
            {
                _source.Play();
            }
        }
    }
}
