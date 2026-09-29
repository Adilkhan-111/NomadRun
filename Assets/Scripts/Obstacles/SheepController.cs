using UnityEngine;
using NomadRun.Core;
using NomadRun.World;

namespace NomadRun.Obstacles
{
    /// <summary>
    /// AI for steppe sheep that wander across adjacent lanes.
    /// </summary>
    public class SheepController : MonoBehaviour
    {
        [SerializeField] private float _walkSpeed = 0.8f;
        private int _targetLane;
        private float _animTimer = 0f;
        private int _frame = 0;
        private SpriteRenderer _renderer;
        private Sprite[] _sprites;

        public void SetupWandering(int currentLane)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _sprites = new Sprite[]
            {
                NomadArtFactory.GetSprite("Sheep_0"),
                NomadArtFactory.GetSprite("Sheep_1")
            };

            // Pick an adjacent asphalt lane (1, 2, or 3)
            int dir = (Random.value < 0.5f) ? -1 : 1;
            _targetLane = Mathf.Clamp(currentLane + dir, 1, 3);
        }

        private void Update()
        {
            if (LaneManager.Instance == null) return;

            // Slowly migrate to target lane
            float targetY = LaneManager.Instance.GetLaneY(_targetLane);
            float newY = Mathf.MoveTowards(transform.position.y, targetY, _walkSpeed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // If reached target lane, pick a new adjacent lane after a brief pause
            if (Mathf.Abs(transform.position.y - targetY) < 0.05f)
            {
                int dir = (Random.value < 0.5f) ? -1 : 1;
                _targetLane = Mathf.Clamp(_targetLane + dir, 1, 3);
            }

            // Animate legs
            _animTimer += Time.deltaTime;
            if (_animTimer >= 0.2f && _renderer != null && _sprites != null && _sprites.Length > 1)
            {
                _animTimer = 0f;
                _frame = (_frame + 1) % _sprites.Length;
                _renderer.sprite = _sprites[_frame];
            }
        }
    }
}
