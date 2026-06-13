using SugarRush.Core;
using SugarRush.Gameplay;
using UnityEngine;

namespace SugarRush.GameFlow
{
    public enum GameState
    {
        Intro,
        Playing,
        Paused,
        Result
    }

    /// <summary>
    /// Central game state machine. Coordinates start, pause, finish and failure flows.
    /// </summary>
    public class GameFlowManager : MonoBehaviour
    {
        [SerializeField] private SugarRushInput _input;
        [SerializeField] private SkiingController _playerController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        [Header("Timing")]
        [SerializeField] private float _introDuration = 1.5f;

        public GameState CurrentState { get; private set; } = GameState.Intro;
        public bool IsPlaying => CurrentState == GameState.Playing;

        private float _introTimer;
        private bool _resultReported;

        private void Awake()
        {
            SubscribeEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeEvents();
        }

        private void Start()
        {
            EnterIntro();
        }

        private void Update()
        {
            if (CurrentState == GameState.Intro)
            {
                _introTimer -= Time.deltaTime;
                if (_introTimer <= 0f)
                {
                    StartPlaying();
                }
            }
        }

        private void SubscribeEvents()
        {
            GameEvents.OnGameFinished += HandleGameFinished;

            if (_input != null)
            {
                _input.OnPausePressed += TogglePause;
            }

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnCrisisFailed += HandleCrisisFailed;
            }
        }

        private void UnsubscribeEvents()
        {
            GameEvents.OnGameFinished -= HandleGameFinished;

            if (_input != null)
            {
                _input.OnPausePressed -= TogglePause;
            }

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnCrisisFailed -= HandleCrisisFailed;
            }
        }

        private void EnterIntro()
        {
            CurrentState = GameState.Intro;
            _introTimer = _introDuration;
            Time.timeScale = 1f;

            _playerController?.SetEnabled(false);

            Debug.Log("[GameFlow] Intro started.", this);
        }

        public void StartPlaying()
        {
            if (CurrentState == GameState.Result) return;

            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            _resultReported = false;

            _playerController?.SetEnabled(true);
            GameEvents.RaiseGameStarted();

            Debug.Log("[GameFlow] Playing started.", this);
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Result) return;

            if (CurrentState == GameState.Playing)
            {
                CurrentState = GameState.Paused;
                Time.timeScale = 0f;
                GameEvents.RaiseGamePaused();
                Debug.Log("[GameFlow] Paused.", this);
            }
            else if (CurrentState == GameState.Paused)
            {
                CurrentState = GameState.Playing;
                Time.timeScale = 1f;
                GameEvents.RaiseGameResumed();
                Debug.Log("[GameFlow] Resumed.", this);
            }
        }

        public void ForceResult(bool win)
        {
            if (CurrentState == GameState.Result || _resultReported) return;

            CurrentState = GameState.Result;
            _resultReported = true;
            Time.timeScale = win ? 1f : 0f;

            _playerController?.SetEnabled(false);
            GameEvents.RaiseGameFinished(win);

            Debug.Log($"[GameFlow] Result: {(win ? "Win" : "Lose")}.", this);
        }

        private void HandleGameFinished(bool win)
        {
            ForceResult(win);
        }

        private void HandleCrisisFailed()
        {
            ForceResult(win: false);
        }
    }
}
