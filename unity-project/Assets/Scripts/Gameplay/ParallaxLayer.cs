using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Lightweight parallax: the layer tracks the camera by <see cref="_factor"/> of its
    /// movement. factor=1 pins the layer to the camera (reads as infinitely far / barely moves
    /// on screen); factor=0 leaves it world-fixed (foreground). Far layers use a high factor.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] private Transform _camera;
        [SerializeField, Range(0f, 1f)] private float _factor = 0.9f;

        private Vector3 _startPos;
        private Vector3 _camStart;

        private void Start()
        {
            if (_camera == null && Camera.main != null) _camera = Camera.main.transform;

            _startPos = transform.position;
            if (_camera != null) _camStart = _camera.position;
        }

        private void LateUpdate()
        {
            if (_camera == null) return;

            Vector3 d = _camera.position - _camStart;
            transform.position = new Vector3(
                _startPos.x + d.x * _factor,
                _startPos.y + d.y * _factor,
                _startPos.z);
        }
    }
}
