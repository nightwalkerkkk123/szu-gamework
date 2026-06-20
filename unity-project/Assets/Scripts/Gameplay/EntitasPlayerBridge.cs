using Entitas;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Bridges the reference Entitas ECS player to our IPlayerController interface
    /// so GameFlowManager and HUD can talk to it generically.
    /// </summary>
    public class EntitasPlayerBridge : MonoBehaviour, IPlayerController
    {
        private GameEntity _playerEntity;
        private Transform _playerTransform;
        private IRigidbody _rigidbody;

        public Transform PlayerTransform => _playerTransform;

        public float Speed => _rigidbody?.Velocity.magnitude ?? 0f;

        public void SetEnabled(bool enabled)
        {
            ResolvePlayer();
            if (_playerEntity != null && _playerEntity.hasView)
            {
                _playerEntity.view.Value.Enabled = enabled;
            }
        }

        private void Update()
        {
            ResolvePlayer();
        }

        private void ResolvePlayer()
        {
            if (_playerEntity != null && _playerEntity.isEnabled) return;

            var context = Contexts.sharedInstance?.game;
            if (context == null) return;
            if (!context.isPlayer) return;

            _playerEntity = context.playerEntity;
            if (_playerEntity == null) return;

            if (_playerEntity.hasView)
            {
                _playerTransform = _playerEntity.view.Value.Transform;
            }

            if (_playerEntity.hasRigidbody)
            {
                _rigidbody = _playerEntity.rigidbody.Value;
            }
        }
    }
}
