using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SugarRush.Foundation
{
    /// <summary>
    /// Thin wrapper over the Input System action asset.
    /// Exposes gameplay events and handles enable/disable lifecycle.
    /// </summary>
    public class SugarRushInput : MonoBehaviour
    {
        [SerializeField] private InputActionAsset _inputAsset;

        private InputActionMap _gameplayMap;
        private readonly List<(InputAction action, Action<InputAction.CallbackContext> callback)> _bindings = new();

        public event Action OnJumpPressed;
        public event Action OnRollPressed;
        public event Action OnUseItemPressed;
        public event Action OnPausePressed;

        private void Awake()
        {
            if (_inputAsset == null)
            {
                Debug.LogError("[SugarRushInput] InputActionAsset not assigned.", this);
                enabled = false;
                return;
            }

            _gameplayMap = _inputAsset.FindActionMap("Gameplay", throwIfNotFound: true);

            Bind("Jump", () => OnJumpPressed?.Invoke());
            Bind("Roll", () => OnRollPressed?.Invoke());
            Bind("UseItem", () => OnUseItemPressed?.Invoke());
            Bind("Pause", () => OnPausePressed?.Invoke());
        }

        private void Start()
        {
            _gameplayMap?.Enable();
        }

        private void OnDisable()
        {
            _gameplayMap?.Disable();

            foreach (var (action, callback) in _bindings)
            {
                if (action != null)
                {
                    action.performed -= callback;
                }
            }
            _bindings.Clear();
        }

        private void Bind(string actionName, Action callback)
        {
            var action = _gameplayMap.FindAction(actionName, throwIfNotFound: true);
            Action<InputAction.CallbackContext> wrapper = _ =>
            {
                Debug.Log($"[SugarRushInput-DIAG] {actionName} triggered");
                callback();
            };
            _bindings.Add((action, wrapper));
            action.performed += wrapper;
            Debug.Log($"[SugarRushInput-DIAG] Bound action '{actionName}' to map '{_gameplayMap.name}'");
        }
    }
}
