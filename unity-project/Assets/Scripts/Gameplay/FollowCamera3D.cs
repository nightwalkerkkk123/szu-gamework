using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Simple 3D follow camera for the fused endless-runner mode.
    /// </summary>
    public class FollowCamera3D : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(-8f, 5f, -12f);
        [SerializeField] private float _smoothTime = 0.2f;

        private Vector3 _velocity;

        private void LateUpdate()
        {
            if (_target == null) return;

            Vector3 targetPosition = _target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
            transform.LookAt(_target.position + Vector3.up * 1.5f);
        }
    }
}
