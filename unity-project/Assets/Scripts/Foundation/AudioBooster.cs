using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Boosts AudioSource volume beyond the editor's [0,1] clamp.
    /// Attach to same GameObject as the AudioSource.
    /// </summary>
    public class AudioBooster : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float _boostVolume = 3f;

        private void Start()
        {
            AudioSource src = GetComponent<AudioSource>();
            if (src != null)
            {
                src.volume = _boostVolume;
            }
        }
    }
}
