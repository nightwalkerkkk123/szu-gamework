using SugarRush.Core;
using System.Collections;
using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Slow-motion effect on game win, triggered via GameEvents.OnGameFinished.
    /// </summary>
    public class FinishSlowMotion : MonoBehaviour
    {
        [SerializeField] private float _winTimeScale = 0.3f;
        [SerializeField] private float _winSlowDuration = 0.6f;

        private static WaitForSecondsRealtime _waitForWinSlow;

        private void OnEnable()
        {
            GameEvents.OnGameFinished += HandleGameFinished;

            if (_waitForWinSlow == null)
            {
                _waitForWinSlow = new WaitForSecondsRealtime(_winSlowDuration);
            }
        }

        private void OnDisable()
        {
            GameEvents.OnGameFinished -= HandleGameFinished;
        }

        private void HandleGameFinished(bool win)
        {
            if (!win) return;
            // Don't clobber an existing pause.
            if (Time.timeScale == 0f) return;

            StopAllCoroutines();
            StartCoroutine(WinSlowMoCoroutine());
        }

        private IEnumerator WinSlowMoCoroutine()
        {
            Time.timeScale = _winTimeScale;
            yield return _waitForWinSlow;
            Time.timeScale = 1f;
        }
    }
}