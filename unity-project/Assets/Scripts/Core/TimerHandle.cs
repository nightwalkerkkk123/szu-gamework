using System;
using UnityEngine;

namespace SugarRush.Core
{
    /// <summary>
    /// A lightweight countdown timer that can be ticked by a central TimerService.
    /// Supports one-shot expiration callbacks and optional per-frame tick callbacks.
    /// </summary>
    public class TimerHandle
    {
        public float Duration { get; private set; }
        public float RemainingTime { get; private set; }
        public bool IsExpired => RemainingTime <= 0f;
        public bool IsRunning => !IsExpired;

        public event Action OnExpired;
        public event Action<float> OnTick; // remaining time

        public TimerHandle(float duration)
        {
            Duration = Mathf.Max(0f, duration);
            RemainingTime = Duration;
        }

        /// <summary>
        /// Advance the timer by deltaTime. Returns true if the timer expired this tick.
        /// </summary>
        public bool Tick(float deltaTime)
        {
            if (IsExpired) return false;

            RemainingTime -= deltaTime;
            OnTick?.Invoke(RemainingTime);

            if (RemainingTime <= 0f)
            {
                RemainingTime = 0f;
                OnExpired?.Invoke();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reset the timer to a new duration (or the original duration if omitted).
        /// </summary>
        public void Reset(float? duration = null)
        {
            Duration = duration.HasValue ? Mathf.Max(0f, duration.Value) : Duration;
            RemainingTime = Duration;
        }

        /// <summary>
        /// Force the timer to expire immediately and invoke callbacks.
        /// </summary>
        public void ForceExpire()
        {
            RemainingTime = 0f;
            OnExpired?.Invoke();
        }

        /// <summary>
        /// Normalized progress from 0 (just started) to 1 (expired).
        /// </summary>
        public float NormalizedProgress => Duration > 0f ? 1f - Mathf.Clamp01(RemainingTime / Duration) : 1f;
    }
}
