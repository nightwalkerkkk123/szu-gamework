using System;
using System.Collections.Generic;
using SugarRush.Core;
using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Central glucose (blood sugar) simulation. 
    /// Translates the medical mechanic into a survival bar with speed/control/vision modifiers.
    /// </summary>
    public class GlucoseSystem : MonoBehaviour
    {
        [SerializeField] private GlucoseConfig _config;

        [Header("Runtime (read-only)")]
        [SerializeField, Range(0f, 100f)] private float _currentValue;
        [SerializeField] private GlucoseZone _currentZone;
        [SerializeField] private float _lowCrisisTimer;
        [SerializeField] private float _highCrisisTimer;

        private readonly List<GlucoseBuff> _activeBuffs = new();
        private float _passiveDeltaPerSecond;

        public GlucoseConfig Config => _config;
        public float CurrentValue => _currentValue;
        public GlucoseZone CurrentZone => _currentZone;

        /// <summary>Speed multiplier exposed to SkiingController (1.0 = baseline).</summary>
        public float SpeedModifier { get; private set; } = 1f;

        /// <summary>Control multiplier exposed to SkiingController (1.0 = baseline).</summary>
        public float ControlModifier { get; private set; } = 1f;

        /// <summary>Vision multiplier exposed to Camera/VFX (1.0 = baseline).</summary>
        public float VisionModifier { get; private set; } = 1f;

        public event Action<float> OnValueChanged;
        public event Action<GlucoseZone> OnZoneChanged;
        public event Action OnCrisisFailed;

        private void Awake()
        {
            if (_config == null)
            {
                _config = GameConfig.Glucose;
            }

            if (_config == null)
            {
                Debug.LogError("[GlucoseSystem] No GlucoseConfig assigned and none registered in GameConfig.", this);
                enabled = false;
                return;
            }

            _currentValue = _config.startValue;
            UpdateZone(true);
        }

        private void OnEnable()
        {
            foreach (var buff in _activeBuffs)
            {
                TimerService.Instance.Register(buff.Timer);
            }
        }

        private void OnDisable()
        {
            if (TimerService.Instance == null) return;

            foreach (var buff in _activeBuffs)
            {
                TimerService.Instance.Unregister(buff.Timer);
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            // Passive skiing decay
            ApplyDelta(-_config.passiveDecayPerSecond * deltaTime);

            // Environmental / hazard passive delta
            if (_passiveDeltaPerSecond != 0f)
            {
                ApplyDelta(_passiveDeltaPerSecond * deltaTime);
            }

            // Tick timed buffs
            bool anyBuffChanged = false;
            for (int i = _activeBuffs.Count - 1; i >= 0; i--)
            {
                var buff = _activeBuffs[i];
                if (buff.Timer.IsExpired)
                {
                    TimerService.Instance.Unregister(buff.Timer);
                    _activeBuffs.RemoveAt(i);
                    anyBuffChanged = true;
                    continue;
                }

                if (buff.DeltaPerSecond != 0f)
                {
                    ApplyDelta(buff.DeltaPerSecond * deltaTime, suppressEvents: true);
                    anyBuffChanged = true;
                }
            }

            if (anyBuffChanged)
            {
                RaiseValueChanged();
            }

            UpdateZone();
            UpdateModifiers();
            UpdateCrisisTimers(deltaTime);
        }

        /// <summary>
        /// Apply an instantaneous glucose change (positive or negative).
        /// </summary>
        public void ApplyDelta(float delta)
        {
            ApplyDelta(delta, suppressEvents: false);
        }

        private void ApplyDelta(float delta, bool suppressEvents)
        {
            if (delta == 0f) return;
            _currentValue = Mathf.Clamp(_currentValue + delta, 0f, 100f);
            if (!suppressEvents) RaiseValueChanged();
        }

        /// <summary>
        /// Apply a timed buff that changes glucose over duration.
        /// </summary>
        public void ApplyBuffOverTime(float deltaPerSecond, float duration)
        {
            if (duration <= 0f)
            {
                ApplyDelta(deltaPerSecond);
                return;
            }

            var buff = new GlucoseBuff(deltaPerSecond, duration);
            if (enabled && gameObject.activeInHierarchy)
            {
                TimerService.Instance.Register(buff.Timer);
            }
            _activeBuffs.Add(buff);
        }

        /// <summary>
        /// Set an environmental passive delta (e.g. hazard zones). Overrides previous value.
        /// </summary>
        public void SetPassiveDelta(float deltaPerSecond)
        {
            _passiveDeltaPerSecond = deltaPerSecond;
        }

        public void ResetPassiveDelta()
        {
            _passiveDeltaPerSecond = 0f;
        }

        private void RaiseValueChanged()
        {
            OnValueChanged?.Invoke(_currentValue);
            GameEvents.RaiseGlucoseChanged(_currentValue);
        }

        private void UpdateZone(bool force = false)
        {
            GlucoseZone newZone = _config.GetZone(_currentValue);
            if (force || newZone != _currentZone)
            {
                _currentZone = newZone;
                OnZoneChanged?.Invoke(_currentZone);
                GameEvents.RaiseGlucoseZoneChanged(_currentZone);

                if (_config.logZoneChanges)
                {
                    Debug.Log($"[GlucoseSystem] Zone changed to {_currentZone} at {_currentValue:F1}", this);
                }

                // Reset timers when leaving crisis zones
                if (_currentZone != GlucoseZone.LowCrisis) _lowCrisisTimer = 0f;
                if (_currentZone != GlucoseZone.HighCrisis) _highCrisisTimer = 0f;
            }
        }

        private void UpdateModifiers()
        {
            float t = _config.Normalize(_currentValue);
            SpeedModifier = _config.speedModifierCurve.Evaluate(t);
            ControlModifier = _config.controlModifierCurve.Evaluate(t);
            VisionModifier = _config.visionModifierCurve.Evaluate(t);
        }

        private void UpdateCrisisTimers(float deltaTime)
        {
            if (_currentZone == GlucoseZone.LowCrisis)
            {
                _lowCrisisTimer += deltaTime;
                if (_lowCrisisTimer >= _config.lowCrisisFailTime)
                {
                    Fail();
                }
            }
            else if (_currentZone == GlucoseZone.HighCrisis)
            {
                _highCrisisTimer += deltaTime;
                if (_highCrisisTimer >= _config.highCrisisFailTime)
                {
                    Fail();
                }
            }
        }

        private void Fail()
        {
            enabled = false;
            OnCrisisFailed?.Invoke();
            Debug.Log("[GlucoseSystem] Crisis failure triggered.", this);
        }

        private class GlucoseBuff
        {
            public float DeltaPerSecond { get; }
            public TimerHandle Timer { get; }

            public GlucoseBuff(float deltaPerSecond, float duration)
            {
                DeltaPerSecond = deltaPerSecond;
                Timer = new TimerHandle(duration);
            }
        }
    }
}
