using SugarRush.Gameplay;
using System.Collections;
using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Freeze-frame hitstop triggered on crash.
    /// </summary>
    public class HitstopController : MonoBehaviour
    {
        [SerializeField] private SkiingController _skiingController;
        [SerializeField] private float _hitstopDuration = 0.06f;

        private static WaitForSecondsRealtime _waitForHitstop;

        private void OnEnable()
        {
            if (_skiingController != null)
            {
                _skiingController.OnCrashed += HandleCrashed;
            }

            if (_waitForHitstop == null)
            {
                _waitForHitstop = new WaitForSecondsRealtime(_hitstopDuration);
            }
        }

        private void OnDisable()
        {
            if (_skiingController != null)
            {
                _skiingController.OnCrashed -= HandleCrashed;
            }
        }

        private void HandleCrashed()
        {
            // Don't unfreeze a game that was already paused.
            if (Time.timeScale == 0f) return;

            StopAllCoroutines();
            StartCoroutine(HitstopCoroutine());
        }

        private IEnumerator HitstopCoroutine()
        {
            Time.timeScale = 0f;
            yield return _waitForHitstop;
            Time.timeScale = 1f;
        }
    }
}