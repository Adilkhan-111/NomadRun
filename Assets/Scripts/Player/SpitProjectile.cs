using UnityEngine;
using NomadRun.World;
using NomadRun.Audio;

namespace NomadRun.Player
{
    /// <summary>
    /// Camel spit projectile fired backwards towards pursuing police or hazards.
    /// </summary>
    public class SpitProjectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 22f;
        [SerializeField] private float _lifetime = 2.5f;

        private Vector2 _direction = Vector2.left;
        private SpriteRenderer _renderer;

        public void Initialize(Vector2 spawnPos, Vector2 direction)
        {
            transform.position = spawnPos;
            _direction = direction.normalized;

            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null) _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = NomadArtFactory.GetSprite("Spit");
            _renderer.sortingOrder = 50;

            var col = GetComponent<CircleCollider2D>();
            if (col == null)
            {
                col = gameObject.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.3f;
            }

            Destroy(gameObject, _lifetime);
        }

        private void Update()
        {
            transform.position += (Vector3)(_direction * _speed * Time.deltaTime);
            // Slight spin
            transform.Rotate(0f, 0f, 360f * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return;

            // Hit police car!
            var police = other.GetComponent<NomadRun.World.PoliceChaser>();
            if (police != null)
            {
                police.TakeSpitHit();
                SoundManager.Instance?.PlaySfx("Crash");
                Destroy(gameObject);
                return;
            }

            // Hit obstacle
            var obstacle = other.GetComponent<NomadRun.Obstacles.Obstacle>();
            if (obstacle != null)
            {
                obstacle.OnHitBySpit();
                SoundManager.Instance?.PlaySfx("Crash");
                Destroy(gameObject);
                return;
            }
        }
    }
}
