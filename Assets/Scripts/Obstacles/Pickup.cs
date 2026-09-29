using UnityEngine;
using NomadRun.Player;
using NomadRun.World;

namespace NomadRun.Obstacles
{
    public enum PickupType
    {
        Baursak,
        Kumis
    }

    /// <summary>
    /// Collectible items on the road (Baursak and Kumis).
    /// </summary>
    public class Pickup : MonoBehaviour
    {
        [SerializeField] private PickupType _type;
        private SpriteRenderer _renderer;
        private CircleCollider2D _collider;
        private float _bobOffset;
        private Vector3 _startPos;

        public void Initialize(PickupType type)
        {
            _type = type;
            _startPos = transform.position;
            _bobOffset = Random.Range(0f, Mathf.PI * 2f);

            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null) _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = NomadArtFactory.GetSprite(type == PickupType.Baursak ? "Baursak" : "Kumis");
            _renderer.sortingOrder = 25;

            _collider = GetComponent<CircleCollider2D>();
            if (_collider == null) _collider = gameObject.AddComponent<CircleCollider2D>();
            _collider.isTrigger = true;
            _collider.radius = 0.5f;
        }

        private void Update()
        {
            // Gentle floating bob
            float y = _startPos.y + Mathf.Sin(Time.time * 4f + _bobOffset) * 0.15f;
            transform.position = new Vector3(transform.position.x, y, transform.position.z);

            // Despawn if passed
            if (PlayerController.Instance != null &&
                transform.position.x < PlayerController.Instance.transform.position.x - 20f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    if (_type == PickupType.Baursak)
                    {
                        player.AddBaursak();
                    }
                    else
                    {
                        player.AddKumis();
                    }
                    Destroy(gameObject);
                }
            }
        }
    }
}
