using SugarRush.Core;
using SugarRush.Foundation;
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
        [SerializeField] private MonoBehaviour _playerController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        private IPlayerController PlayerController => _playerController as IPlayerController;

        [Header("Timing")]
        [SerializeField] private float _introDuration = 1.5f;

        public GameState CurrentState { get; private set; } = GameState.Intro;
        public bool IsPlaying => CurrentState == GameState.Playing;

        private float _introTimer;
        private bool _resultReported;

        private void Awake()
        {
            ResolveReferences();
            InitializeConfig();
            SubscribeEvents();
        }

        private void ResolveReferences()
        {
            if (_input == null) _input = FindObjectOfType<SugarRushInput>();
            if (_playerController == null)
            {
                var skiing = FindObjectOfType<SkiingController>();
                if (skiing != null)
                {
                    _playerController = skiing;
                }
                else
                {
                    var sphere = FindObjectOfType<SpherePlayerController>();
                    _playerController = sphere != null ? sphere : FindObjectOfType<EntitasPlayerBridge>();
                }
            }
            if (_glucoseSystem == null) _glucoseSystem = FindObjectOfType<GlucoseSystem>();
        }

        private void InitializeConfig()
        {
            if (ConfigService.Instance.TryGet<GlucoseConfig>(out _)) return;

            var skiingController = _playerController as SkiingController;
            var skiingConfig = skiingController != null ? skiingController.Config : null;
            var glucoseConfig = _glucoseSystem != null ? _glucoseSystem.Config : null;
            var levelManager = FindObjectOfType<LevelManager>();
            var levelData = levelManager != null ? levelManager.Data : null;

            GameConfig.Initialize(glucoseConfig, skiingConfig, levelData);
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

            PlayerController?.SetEnabled(false);

            Debug.Log("[GameFlow] Intro started.", this);
        }

        public void StartPlaying()
        {
            if (CurrentState == GameState.Result) return;

            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
            _resultReported = false;

            PlayerController?.SetEnabled(true);
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

            PlayerController?.SetEnabled(false);
            if (_glucoseSystem != null)
            {
                _glucoseSystem.enabled = false;
            }

            RecordResult(win);

            Debug.Log($"[GameFlow] Result: {(win ? "Win" : "Lose")}.", this);
        }

        private void RecordResult(bool win)
        {
            var levelManager = FindObjectOfType<LevelManager>();
            var level = levelManager != null ? levelManager.Data : null;

            string levelId = level?.LevelId ?? "L1";
            float timeSeconds = levelManager != null ? levelManager.ElapsedTime : 0f;
            float distance = levelManager != null ? levelManager.DistanceTraveled : 0f;
            int score = CalculateScore(win, timeSeconds, distance, level);

            SaveService.Instance.RecordRun(levelId, win, timeSeconds, score, distance);
            SaveService.Instance.Save();
        }

        private int CalculateScore(bool win, float timeSeconds, float distance, LevelData level)
        {
            if (level == null) return 0;

            int score = level.BaseScore;
            if (win)
            {
                float timeBonus = Mathf.Max(0f, level.ParTimeSeconds - timeSeconds) * level.TimeBonusMultiplier;
                score += Mathf.RoundToInt(timeBonus);
            }
            return score;
        }

        private void HandleGameFinished(bool win)
        {
            ForceResult(win);
        }

        private void HandleCrisisFailed()
        {
            GameEvents.RaiseGameFinished(false);
        }
    }
}
