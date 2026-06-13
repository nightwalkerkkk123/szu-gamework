using Cinemachine;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Simple Cinemachine 2D follow setup. Assigns the target to a virtual camera.
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        [Header("Framing")]
        [SerializeField] private Vector3 _offset = new Vector3(4f, 2f, -10f);
        [SerializeField] private float _lookaheadTime = 0.3f;
        [SerializeField] private float _damping = 1f;

        private void Awake()
        {
            if (_virtualCamera == null)
            {
                _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            }

            if (_virtualCamera != null && _target != null)
            {
                _virtualCamera.Follow = _target;
            }
        }

        private void Start()
        {
            if (_virtualCamera == null)
            {
                Debug.LogWarning("[CameraFollow] No CinemachineVirtualCamera assigned.", this);
                return;
            }

            if (_target == null)
            {
                Debug.LogWarning("[CameraFollow] No follow target assigned.", this);
                return;
            }

            _virtualCamera.Follow = _target;

            if (_virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) is CinemachineFramingTransposer framing)
            {
                framing.m_ScreenX = 0.35f; // keep player slightly left of center
                framing.m_ScreenY = 0.5f;
                framing.m_DeadZoneWidth = 0.1f;
                framing.m_DeadZoneHeight = 0.15f;
                framing.m_LookaheadTime = _lookaheadTime;
                framing.m_XDamping = _damping;
                framing.m_YDamping = _damping;
            }
        }
    }
}
