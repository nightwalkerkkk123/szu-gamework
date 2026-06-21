using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Minimal script that kicks off the AudioSource on Start.
    /// Reliable alternative to playOnAwake which can be
    /// unreliable when the AudioSource is added in editor.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class BgmStarter : MonoBehaviour
    {
        private void Start()
        {
            AudioSource src = GetComponent<AudioSource>();
            if (src != null && src.clip != null && !src.isPlaying)
            {
                src.Play();
            }
        }
    }
}
