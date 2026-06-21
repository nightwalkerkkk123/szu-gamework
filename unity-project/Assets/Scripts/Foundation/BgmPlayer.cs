using UnityEngine;

namespace SugarRush.Foundation
{
    public class BgmPlayer : MonoBehaviour
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
            _source.playOnAwake = false;
        }

        private void Start()
        {
            if (_source != null && _source.clip != null)
            {
                _source.volume = Mathf.Min(_volume, 1f);
                Invoke(nameof(DelayedPlay), 0.2f);
            }
        }

        private void DelayedPlay()
        {
            AudioListener.volume = _volume;
            _source.Play();
        }
    }
}
