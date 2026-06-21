using UnityEngine;

namespace SugarRush.Foundation
{
    public class VolumeBoost : MonoBehaviour
    {
        [SerializeField] private AudioClip _bgm;
        [SerializeField] private float _listenerVolume = 3f;

        private AudioSource _source;

        private void Awake()
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.clip = _bgm;
            _source.loop = true;
            _source.spatialBlend = 0f;
            _source.playOnAwake = false;
        }

        private void Start()
        {
            AudioListener.volume = _listenerVolume;
            StartCoroutine(PlayBgmDelayed());
        }

        private System.Collections.IEnumerator PlayBgmDelayed()
        {
            yield return null;
            _source.Play();
        }
    }
}
