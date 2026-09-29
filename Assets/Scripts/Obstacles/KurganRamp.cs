using UnityEngine;
using NomadRun.Player;
using NomadRun.World;

namespace NomadRun.Obstacles
{
    /// <summary>
    /// Kurgan / Sand Dune ramp located on shoulders (Lane 0 and 4).
    /// Triggers the airborne Tulpar-Flip stunt.
    /// </summary>
    public class KurganRamp : MonoBehaviour
    {
        private SpriteRenderer _renderer;
        private BoxCollider2D _collider;

        public void Initialize()
        {
            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null) _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = NomadArtFactory.GetSprite("Kurgan");
            _renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 30);

            _collider = GetComponent<BoxCollider2D>();
            if (_collider == null) _collider = gameObject.AddComponent<BoxCollider2D>();
            _collider.isTrigger = true;
            _collider.size = new Vector2(1.5f, 0.8f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.TriggerKurganJump();
                }
            }
        }

        private void Update()
        {
            if (PlayerController.Instance != null &&
                transform.position.x < PlayerController.Instance.transform.position.x - 20f)
            {
                Destroy(gameObject);
            }
        }
    }
}
