using SugarRush.Core;
using SugarRush.Foundation;
using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// Spawns simple 2D particles for landing, pickups, hits and death.
    /// Creates particles on the fly using a shared white sprite.
    /// </summary>
    public class SimpleParticleSpawner : MonoBehaviour
    {
        [SerializeField] private Sprite _particleSprite;

        [Header("Landing Snow")]
        [SerializeField] private int _landingParticleCount = 8;
        [SerializeField] private float _landingParticleLife = 0.4f;
        [SerializeField] private float _landingParticleSpeed = 3f;
        [SerializeField] private Color _landingColor = new Color(0.9f, 0.95f, 1f, 0.8f);

        [Header("Pickup Sparkle")]
        [SerializeField] private int _pickupParticleCount = 12;
        [SerializeField] private float _pickupParticleLife = 0.5f;
        [SerializeField] private float _pickupParticleSpeed = 4f;

        [Header("Hit Burst")]
        [SerializeField] private int _hitParticleCount = 10;
        [SerializeField] private float _hitParticleLife = 0.35f;
        [SerializeField] private float _hitParticleSpeed = 5f;
        [SerializeField] private Color _hitColor = new Color(1f, 0.2f, 0.2f, 0.9f);

        [Header("Death Burst")]
        [SerializeField] private int _deathParticleCount = 24;
        [SerializeField] private float _deathParticleLife = 0.7f;
        [SerializeField] private float _deathParticleSpeed = 6f;

        [SerializeField] private float _gravity = -10f;

        private void Start()
        {
            if (_particleSprite == null)
            {
                Debug.LogWarning("[SimpleParticleSpawner] Particle sprite not assigned.", this);
            }

            var skiing = GetComponent<SkiingController>();
            if (skiing != null)
            {
                skiing.OnGroundedChanged += HandleGroundedChanged;
            }

            PickupItem.OnItemPickedUp += HandleItemPickedUp;

            var obstacle = GetComponent<Obstacle>(); // won't exist on player, but harmless
        }

        private void OnDestroy()
        {
            var skiing = GetComponent<SkiingController>();
            if (skiing != null)
            {
                skiing.OnGroundedChanged -= HandleGroundedChanged;
            }

            PickupItem.OnItemPickedUp -= HandleItemPickedUp;
        }

        public void SpawnLandingParticles(Vector3 position)
        {
            for (int i = 0; i < _landingParticleCount; i++)
            {
                float angle = Random.Range(80f, 100f) * Mathf.Deg2Rad;
                float speed = Random.Range(_landingParticleSpeed * 0.5f, _landingParticleSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _landingParticleLife, _landingColor, 0.12f, 0.04f);
            }
        }

        public void SpawnPickupParticles(Vector3 position, Color color)
        {
            for (int i = 0; i < _pickupParticleCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_pickupParticleSpeed * 0.3f, _pickupParticleSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _pickupParticleLife, color, 0.15f, 0.05f);
            }
        }

        public void SpawnHitParticles(Vector3 position)
        {
            for (int i = 0; i < _hitParticleCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_hitParticleSpeed * 0.3f, _hitParticleSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _hitParticleLife, _hitColor, 0.14f, 0.05f);
            }
        }

        public void SpawnDeathParticles(Vector3 position, Color color)
        {
            for (int i = 0; i < _deathParticleCount; i++)
            {
                float angle = Random.Range(0f, Mathf.PI * 2f);
                float speed = Random.Range(_deathParticleSpeed * 0.3f, _deathParticleSpeed);
                Vector2 vel = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
                SpawnParticle(position, vel, _deathParticleLife, color, 0.2f, 0.05f);
            }
        }

        private void SpawnParticle(Vector3 position, Vector2 velocity, float life, Color color, float startScale, float endScale)
        {
            var go = new GameObject("Particle");
            go.transform.position = position;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = _particleSprite;
            sr.color = color;
            sr.sortingOrder = 10;

            var particle = go.AddComponent<SimpleParticle>();
            particle.Init(velocity, life, _gravity, color, startScale, endScale);
        }

        private void HandleGroundedChanged(bool isGrounded)
        {
            if (isGrounded)
            {
                SpawnLandingParticles(transform.position);
            }
        }

        private void HandleItemPickedUp(PickupItem pickup, ItemEffect effect, GameObject player)
        {
            if (player != gameObject) return;
            Color color = Color.yellow;
            if (effect != null)
            {
                // Try to infer color from effect name or use a default.
                string name = effect.DisplayName.ToLowerInvariant();
                if (name.Contains("insulin")) color = Color.blue;
                else if (name.Contains("pill")) color = Color.green;
                else if (name.Contains("snowflake")) color = Color.yellow;
            }
            SpawnPickupParticles(player.transform.position, color);
        }
    }
}
