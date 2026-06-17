using SugarRush.Core;
using SugarRush.Gameplay;
using System;
using UnityEngine;

namespace SugarRush.Foundation
{
    /// <summary>
    /// MonoBehaviour subscriber that bridges the event bus to AudioManager.
    /// Drop onto any persistent GameObject (e.g. GameManager) in the scene.
    /// All subscriptions are cleared in OnDisable to survive scene reloads.
    /// </summary>
    public sealed class AudioFeedback : MonoBehaviour
    {
        [SerializeField] private SkiingController _skiingController;
        [SerializeField] private GlucoseSystem _glucoseSystem;

        private void OnEnable()
        {
            if (_skiingController != null)
            {
                _skiingController.OnJumped += HandleJumped;
                _skiingController.OnLanded += HandleLanded;
                _skiingController.OnRollingChanged += HandleRollingChanged;
                _skiingController.OnStumbledChanged += HandleStumbledChanged;
                _skiingController.OnCrashed += HandleCrashed;
                _skiingController.OnShieldActivated += HandleShieldActivated;
                _skiingController.OnShieldConsumed += HandleShieldConsumed;
            }

            PickupItem.OnItemPickedUp += HandleItemPickedUp;

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged += HandleZoneChanged;
            }

            GameEvents.OnGameFinished += HandleGameFinished;
        }

        private void OnDisable()
        {
            if (_skiingController != null)
            {
                _skiingController.OnJumped -= HandleJumped;
                _skiingController.OnLanded -= HandleLanded;
                _skiingController.OnRollingChanged -= HandleRollingChanged;
                _skiingController.OnStumbledChanged -= HandleStumbledChanged;
                _skiingController.OnCrashed -= HandleCrashed;
                _skiingController.OnShieldActivated -= HandleShieldActivated;
                _skiingController.OnShieldConsumed -= HandleShieldConsumed;
            }

            PickupItem.OnItemPickedUp -= HandleItemPickedUp;

            if (_glucoseSystem != null)
            {
                _glucoseSystem.OnZoneChanged -= HandleZoneChanged;
            }

            GameEvents.OnGameFinished -= HandleGameFinished;
        }

        private void Start()
        {
            // Auto-resolve references at runtime if not assigned in inspector.
            if (_skiingController == null)
                _skiingController = FindAnyObjectByType<SkiingController>();
            if (_glucoseSystem == null)
                _glucoseSystem = FindAnyObjectByType<GlucoseSystem>();
        }

        // ── Handlers ─────────────────────────────────────────────────────────────

        private void HandleJumped()
        {
            AudioManager.PlaySfx(PlaceholderSfx.GenerateJump(), volume: 0.4f, pitchVariation: 0.1f);
        }

        private void HandleLanded()
        {
            AudioManager.PlaySfx(PlaceholderSfx.GenerateLand(), volume: 0.5f, pitchVariation: 0.08f);
        }

        private void HandleRollingChanged(bool isRolling)
        {
            if (!isRolling) return;
            AudioManager.PlaySfx(PlaceholderSfx.GenerateRoll(), volume: 0.3f, pitchVariation: 0.1f);
        }

        private void HandleStumbledChanged(bool isStumbled)
        {
            if (!isStumbled) return;
            AudioManager.PlaySfx(PlaceholderSfx.GenerateStumble(), volume: 0.45f, pitchVariation: 0.1f);
        }

        private void HandleCrashed()
        {
            AudioManager.PlaySfx(PlaceholderSfx.GenerateCrash(), volume: 0.55f, pitchVariation: 0.08f);
        }

        private void HandleItemPickedUp(PickupItem pickup, ItemEffect effect, GameObject player)
        {
            AudioManager.PlaySfx(PlaceholderSfx.GeneratePickup(), volume: 0.5f, pitchVariation: 0.1f);
        }

        private void HandleShieldActivated()
        {
            AudioManager.PlaySfx(PlaceholderSfx.GenerateShieldActivate(), volume: 0.4f, pitchVariation: 0.1f);
        }

        private void HandleShieldConsumed()
        {
            AudioManager.PlaySfx(PlaceholderSfx.GenerateShieldConsume(), volume: 0.35f, pitchVariation: 0.1f);
        }

        private void HandleZoneChanged(GlucoseZone zone)
        {
            // GlucoseSystem.OnZoneChanged is the authoritative source; no bus fallback needed.
            AudioManager.PlaySfx(PlaceholderSfx.GenerateZoneChange((int)zone), volume: 0.25f, pitchVariation: 0.05f);
        }

        private void HandleGameFinished(bool win)
        {
            if (win)
                AudioManager.PlaySfx(PlaceholderSfx.GenerateWin(), volume: 0.5f, pitchVariation: 0.08f);
            else
                AudioManager.PlaySfx(PlaceholderSfx.GenerateLose(), volume: 0.5f, pitchVariation: 0.08f);
        }
    }
}
