using UnityEngine;
using NomadRun.Player;
using NomadRun.Core;
using NomadRun.Audio;

namespace NomadRun.World
{
    /// <summary>
    /// Pursuing police car (White DPS Priora with flashing lights).
    /// Chases the player from behind. Can be repelled by Camel Spit.
    /// </summary>
    public class PoliceChaser : MonoBehaviour
    {
        public static PoliceChaser Instance { get; private set; }

        [Header("Chase Settings")]
        [SerializeField] private float _distanceBehind = 14f;
        [SerializeField] private float _minCatchDistance = 1.0f;
        [SerializeField] private float _baseCatchSpeed = 1.2f;

        private SpriteRenderer _renderer;
        private Sprite[] _carSprites;
        private float _animTimer = 0f;
        private int _carFrame = 0;
        private bool _isStunned = false;
        private float _stunTimer = 0f;

        private float _shoutTimer = 0f;
        private readonly string[] _policeShouts = new string[]
        {
            "ТОҚТА, ҚАЙДА БАРАСЫҢ?!",
            "ҚҰЖАТТАРДЫ ДАЙЫНДА!",
            "120 СЕКУНД ДЕЙ МЕ?! ТОҚТА!",
            "ТҮЙЕҢЕ НӨМІР САЛМАҒАНСЫҢ!",
            "ТОЙҒА БІЗ ДЕ БАРАМЫЗ!"
        };

        public float DistanceBehind => _distanceBehind;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            SetupVisuals();
        }

        private void SetupVisuals()
        {
            _renderer = gameObject.AddComponent<SpriteRenderer>();
            _carSprites = new Sprite[]
            {
                NomadArtFactory.GetSprite("PoliceCar_0"),
                NomadArtFactory.GetSprite("PoliceCar_1")
            };
            _renderer.sprite = _carSprites[0];
            _renderer.sortingOrder = 15;

            var col = gameObject.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(2f, 0.8f);
        }

        private void Update()
        {
            if (PlayerController.Instance == null || GameManager.Instance == null || !GameManager.Instance.IsPlaying)
                return;

            HandleChaseMovement();
            HandleFlashingLights();
            HandleShouts();
        }

        private void HandleChaseMovement()
        {
            if (_isStunned)
            {
                _stunTimer -= Time.deltaTime;
                if (_stunTimer <= 0f) _isStunned = false;
                // Spin while stunned
                transform.Rotate(0f, 0f, -360f * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.identity;

                // Move closer if player is not boosted, fall back if player is boosted
                float playerSpeed = PlayerController.Instance.CurrentSpeed;
                float relativeApproach = (playerSpeed < 20f) ? _baseCatchSpeed : -2.5f;

                _distanceBehind = Mathf.Clamp(_distanceBehind - relativeApproach * Time.deltaTime, 0.5f, 25f);
            }

            // Position police car behind player
            Vector3 playerPos = PlayerController.Instance.transform.position;
            // Match player's lane Y smoothly
            float targetY = playerPos.y;
            float newY = Mathf.Lerp(transform.position.y, targetY, 4f * Time.deltaTime);

            transform.position = new Vector3(playerPos.x - _distanceBehind, newY, 0f);
            _renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 30);

            // Caught condition
            if (_distanceBehind <= _minCatchDistance && !_isStunned)
            {
                GameManager.Instance?.OnCaughtByPolice();
            }
        }

        private void HandleFlashingLights()
        {
            _animTimer += Time.deltaTime;
            if (_animTimer >= 0.15f)
            {
                _animTimer = 0f;
                _carFrame = (_carFrame + 1) % _carSprites.Length;
                _renderer.sprite = _carSprites[_carFrame];
            }
        }

        private void HandleShouts()
        {
            _shoutTimer += Time.deltaTime;
            if (_shoutTimer >= 7f && _distanceBehind < 18f && !_isStunned)
            {
                _shoutTimer = 0f;
                SoundManager.Instance?.PlaySfx("PoliceSiren");
                string shout = _policeShouts[Random.Range(0, _policeShouts.Length)];
                GameManager.Instance?.ShowFloatingText(shout, transform.position + Vector3.up * 1.4f, Color.red);
            }
        }

        public void TakeSpitHit()
        {
            _isStunned = true;
            _stunTimer = 2.2f;
            _distanceBehind = Mathf.Min(_distanceBehind + 12f, 26f);

            GameManager.Instance?.ShowFloatingText("ОЙБАЙ! КӨЗІМЕ ТҮКІРДІ! (ДПС отстает!)", transform.position + Vector3.up * 1.5f, Color.green);
        }

        public void AlertByToast()
        {
            // Shouting toasts draws police closer
            _distanceBehind = Mathf.Max(_distanceBehind - 2.5f, 2f);
            GameManager.Instance?.ShowFloatingText("ДПС ШУДЫ ЕСТІДІ! (Полиция приближается!)", transform.position + Vector3.up * 1.2f, Color.yellow);
        }
    }
}
