using UnityEngine;

namespace SnowShader
{
    public class Raycast : MonoBehaviour
    {
        [SerializeField]
        private Camera cam;

        [SerializeField]
        private float texScale = 0.5f;

        [SerializeField]
        private float rayCastLength = 1000;

        [SerializeField]
        private Texture2D tex;

        private RaycastHit _hit;
        private Ray _ray;

        private void Update()
        {
            Cast();
        }

        private void Cast()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            _ray = cam.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(_ray, out _hit, rayCastLength))
                return;

            if (_hit.collider.TryGetComponent(out Snow snow))
            {
                snow.Draw(_hit.textureCoord2, 0, texScale, tex);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawRay(_ray);
        }
    }
}