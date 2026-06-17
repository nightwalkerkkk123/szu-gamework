using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Runtime component spawned on the player by <see cref="Items.MagnetEffect"/>.
    /// Each frame, pulls any <see cref="PickupItem"/> within radius toward the player
    /// at the configured pull speed. Self-destructs when the timer expires.
    /// </summary>
    public class MagnetField : MonoBehaviour
    {
        private float _radius;
        private float _pullSpeed;
        private float _timer;

        public float Radius => _radius;
        public float TimeRemaining => _timer;

        public void Configure(float radius, float duration, float pullSpeed)
        {
            _radius = radius;
            _pullSpeed = pullSpeed;
            // Refresh on overlap — picking up another magnet resets the timer.
            _timer = duration;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                Destroy(this);
                return;
            }

            var hits = Physics2D.OverlapCircleAll(transform.position, _radius);
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                if (hit == null) continue;
                if (!hit.TryGetComponent<PickupItem>(out _)) continue;

                Vector3 dir = transform.position - hit.transform.position;
                float distSq = dir.sqrMagnitude;
                if (distSq < 0.01f) continue;

                hit.transform.position += dir.normalized * _pullSpeed * Time.deltaTime;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.8f, 0.4f, 1f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
