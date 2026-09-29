using UnityEngine;
using NomadRun.Player;
using NomadRun.World;
using NomadRun.Core;

namespace NomadRun.Obstacles
{
    public enum ObstacleType
    {
        Sheep,
        Pothole,
        Kazan,
        Watermelon
    }

    /// <summary>
    /// Base class for road hazards.
    /// </summary>
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private ObstacleType _type;
        private SpriteRenderer _renderer;
        private BoxCollider2D _collider;
        private bool _hasBeenAvoided = false;

        public ObstacleType Type => _type;

        public void Initialize(ObstacleType type, int laneIndex)
        {
            _type = type;

            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null) _renderer = gameObject.AddComponent<SpriteRenderer>();

            string spriteKey = GetSpriteKey(type);
            _renderer.sprite = NomadArtFactory.GetSprite(spriteKey);

            _collider = GetComponent<BoxCollider2D>();
            if (_collider == null) _collider = gameObject.AddComponent<BoxCollider2D>();
            _collider.isTrigger = true;

            AdjustColliderForType(type);
            UpdateSorting();
        }

        private string GetSpriteKey(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.Sheep: return "Sheep_0";
                case ObstacleType.Pothole: return "Pothole";
                case ObstacleType.Kazan: return "Kazan";
                case ObstacleType.Watermelon: return "Watermelon";
                default: return "Pothole";
            }
        }

        private void AdjustColliderForType(ObstacleType type)
        {
            switch (type)
            {
                case ObstacleType.Sheep:
                    _collider.size = new Vector2(0.9f, 0.6f);
                    _collider.offset = new Vector2(0f, 0.3f);
                    break;
                case ObstacleType.Pothole:
                    _collider.size = new Vector2(1.0f, 0.4f);
                    _collider.offset = Vector2.zero;
                    break;
                case ObstacleType.Kazan:
                case ObstacleType.Watermelon:
                    _collider.size = new Vector2(0.8f, 0.6f);
                    _collider.offset = new Vector2(0f, 0.3f);
                    break;
            }
        }

        private void Update()
        {
            UpdateSorting();

            // Track if player successfully passed this obstacle
            if (!_hasBeenAvoided && PlayerController.Instance != null)
            {
                if (transform.position.x < PlayerController.Instance.transform.position.x - 1.5f)
                {
                    _hasBeenAvoided = true;
                    if (_type == ObstacleType.Sheep)
                    {
                        GameManager.Instance?.RecordSheepDodged();
                    }
                }
            }

            // Despawn if far behind player
            if (PlayerController.Instance != null &&
                transform.position.x < PlayerController.Instance.transform.position.x - 25f)
            {
                Destroy(gameObject);
            }
        }

        private void UpdateSorting()
        {
            if (_renderer != null)
            {
                _renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 30);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player != null && !player.IsJumping)
                {
                    string msg = _type == ObstacleType.Sheep ? "ҚОЙҒА СОҚТЫҚ! (Сбил овцу!)" :
                                 _type == ObstacleType.Pothole ? "ШҰҢҚЫР! (Яма на асфальте!)" :
                                 _type == ObstacleType.Kazan ? "ҚАЗАНҒА КІРДІК! (Врезались в казан!)" :
                                 "ҚАРБЫЗДАР ҰШТЫ! (Арбузный краш!)";

                    player.Crash(msg);
                }
            }
        }

        public void OnHitBySpit()
        {
            // Knock obstacle away
            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2f;
            rb.linearVelocity = new Vector2(Random.Range(-5f, -2f), Random.Range(4f, 8f));
            rb.angularVelocity = Random.Range(-300f, 300f);
            Destroy(gameObject, 1.5f);
        }
    }
}
