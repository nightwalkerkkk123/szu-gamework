using System.Collections.Generic;
using UnityEngine;

namespace SugarRush.Core
{
    /// <summary>
    /// Central service that ticks all registered TimerHandles.
    /// Automatically respects Time.timeScale for pause/slow-motion.
    /// </summary>
    public class TimerService : MonoBehaviour
    {
        private static TimerService _instance;
        public static TimerService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TimerService>();
                    if (_instance == null && Application.isPlaying)
                    {
                        var go = new GameObject(nameof(TimerService));
                        _instance = go.AddComponent<TimerService>();
                    }
                }
                return _instance;
            }
        }

        private readonly List<TimerHandle> _timers = new();
        private readonly List<TimerHandle> _pendingAdd = new();
        private readonly List<TimerHandle> _pendingRemove = new();
        private bool _isIterating;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void Update()
        {
            if (Time.timeScale <= 0f) return;

            float dt = Time.deltaTime;

            _isIterating = true;
            for (int i = _timers.Count - 1; i >= 0; i--)
            {
                var timer = _timers[i];
                if (timer == null)
                {
                    _timers.RemoveAt(i);
                    continue;
                }

                timer.Tick(dt);
                if (timer.IsExpired)
                {
                    _timers.RemoveAt(i);
                }
            }
            _isIterating = false;

            FlushPending();
        }

        /// <summary>
        /// Register a timer to be ticked automatically. Safe to call during iteration.
        /// </summary>
        public void Register(TimerHandle timer)
        {
            if (timer == null) return;

            if (_isIterating)
            {
                _pendingAdd.Add(timer);
            }
            else
            {
                if (!_timers.Contains(timer))
                {
                    _timers.Add(timer);
                }
            }
        }

        /// <summary>
        /// Unregister a timer. Safe to call during iteration.
        /// </summary>
        public void Unregister(TimerHandle timer)
        {
            if (timer == null) return;

            if (_isIterating)
            {
                _pendingRemove.Add(timer);
            }
            else
            {
                _timers.Remove(timer);
            }
        }

        private void FlushPending()
        {
            foreach (var timer in _pendingRemove)
            {
                _timers.Remove(timer);
            }
            _pendingRemove.Clear();

            foreach (var timer in _pendingAdd)
            {
                if (!_timers.Contains(timer))
                {
                    _timers.Add(timer);
                }
            }
            _pendingAdd.Clear();
        }
    }
}
