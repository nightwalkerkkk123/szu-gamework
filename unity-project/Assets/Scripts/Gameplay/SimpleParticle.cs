using UnityEngine;

namespace SugarRush.Gameplay
{
    /// <summary>
    /// A tiny temporary particle: moves with velocity, fades out, then self-destructs.
    /// </summary>
    public class SimpleParticle : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;

        private Vector2 _velocity;
        private float _life;
        private float _maxLife;
        private float _gravity;
        private Color _startColor;
        private float _startScale;
        private float _endScale;

        public void Init(Vector2 velocity, float life, float gravity, Color color, float startScale, float endScale)
        {
            _velocity = velocity;
            _life = life;
            _maxLife = life;
            _gravity = gravity;
            _startColor = color;
            _startScale = startScale;
            _endScale = endScale;

            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            _renderer.color = color;
            transform.localScale = Vector3.one * startScale;
        }

        private void Update()
        {
            _life -= Time.deltaTime;
            if (_life <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            _velocity.y += _gravity * Time.deltaTime;
            transform.position += (Vector3)_velocity * Time.deltaTime;

            float t = _life / _maxLife;
            Color c = _startColor;
            c.a *= t;
            _renderer.color = c;

            float scale = Mathf.Lerp(_endScale, _startScale, t);
            transform.localScale = Vector3.one * scale;
        }
    }
}
