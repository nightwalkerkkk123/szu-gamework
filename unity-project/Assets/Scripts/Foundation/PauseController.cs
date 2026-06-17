using SugarRush.Core;
using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Bridges the Pause input action to GameEvents.RaiseGamePaused / Resume.
    /// PauseMenu and any other subscriber handle the actual UI / timeScale side
    /// of things.
    /// </summary>
    public class PauseController : MonoBehaviour
    {
        [SerializeField] private SugarRushInput _input;
        [SerializeField] private bool _startPaused = false;

        private bool _subscribed;

        private void Awake()
        {
            if (_input == null) _input = FindObjectOfType<SugarRushInput>();
        }

        private void OnEnable()
        {
            if (_input != null)
            {
                _input.OnPausePressed += OnPausePressed;
                _subscribed = true;
            }

            if (_startPaused)
            {
                // Use a small delay so subscribers (PauseMenu etc.) have time
                // to subscribe in their own OnEnable before the event fires.
                Invoke(nameof(RaisePauseNow), 0.05f);
            }
        }

        private void OnDisable()
        {
            if (_subscribed && _input != null)
            {
                _input.OnPausePressed -= OnPausePressed;
                _subscribed = false;
            }
        }

        private void OnPausePressed()
        {
            if (Time.timeScale <= 0f)
            {
                GameEvents.RaiseGameResumed();
            }
            else
            {
                GameEvents.RaiseGamePaused();
            }
        }

        private void RaisePauseNow() => GameEvents.RaiseGamePaused();
    }
}
