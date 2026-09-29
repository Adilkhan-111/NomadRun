using System.Collections;
using UnityEngine;
using NomadRun.Core;
using NomadRun.World;
using NomadRun.Audio;

namespace NomadRun.Player
{
    /// <summary>
    /// Player controller for the camel rider in the 5-lane steppe runner.
    /// Handles lane switching, kumis drunkenness/boost, baursak energy,
    /// spit projectiles, stunts on kurgans, and road collisions.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Speed Settings")]
        [SerializeField] private float _baseSpeed = 16f;
        [SerializeField] private float _kumisBoostSpeed = 28f;
        [SerializeField] private float _laneSwitchSpeed = 12f;

        [Header("State")]
        [SerializeField] private int _currentLane = 2; // Start on center lane
        [SerializeField] private float _kumisDrunk = 0f; // 0 to 100
        [SerializeField] private float _boostTimer = 0f;
        [SerializeField] private int _spitAmmo = 3;
        [SerializeField] private int _maxSpitAmmo = 5;

        [Header("Jump / Flip")]
        [SerializeField] private bool _isJumping = false;
        [SerializeField] private float _jumpDuration = 1.0f;
        [SerializeField] private float _jumpHeight = 2.2f;

        // Visuals & Components
        private SpriteRenderer _spriteRenderer;
        private BoxCollider2D _collider;
        private Transform _visualRoot;

        // Sprite animation frames
        private Sprite[] _walkSprites;
        private Sprite _jumpSprite;
        private float _animTimer = 0f;
        private int _currentFrame = 0;

        // Hold-to-shift lanes support
        private float _laneHoldTimer = 0f;
        private const float LaneRepeatDelay = 0.22f;

        // Public properties for HUD and Game Managers
        public float CurrentSpeed => (_boostTimer > 0f) ? _kumisBoostSpeed : _baseSpeed;
        public int CurrentLane => _currentLane;
        public float KumisDrunkPercent => _kumisDrunk;
        public bool IsKumisBoosted => _boostTimer > 0f;
        public int SpitAmmo => _spitAmmo;
        public int MaxSpitAmmo => _maxSpitAmmo;
        public bool IsJumping => _isJumping;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            SetupComponents();
        }

        private void SetupComponents()
        {
            tag = "Player";

            // Child visual root so we can rotate / jump visually without breaking physics/Y position
            var visualGo = new GameObject("VisualRoot");
            visualGo.transform.SetParent(transform, false);
            _visualRoot = visualGo.transform;

            _spriteRenderer = visualGo.AddComponent<SpriteRenderer>();
            _spriteRenderer.sortingOrder = 10;

            _collider = gameObject.AddComponent<BoxCollider2D>();
            _collider.size = new Vector2(1.2f, 0.7f);
            _collider.offset = new Vector2(0f, 0.35f);

            var rb = gameObject.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            // Load procedural sprites
            _walkSprites = new Sprite[3]
            {
                NomadArtFactory.GetSprite("Camel_0"),
                NomadArtFactory.GetSprite("Camel_1"),
                NomadArtFactory.GetSprite("Camel_2")
            };
            _jumpSprite = NomadArtFactory.GetSprite("Camel_Jump");
            _spriteRenderer.sprite = _walkSprites[0];
        }

        private void Start()
        {
            if (LaneManager.Instance != null)
            {
                float startY = LaneManager.Instance.GetLaneY(_currentLane);
                transform.position = new Vector3(transform.position.x, startY, 0f);
            }
        }

        private void Update()
        {
            if (GameManager.Instance != null && !GameManager.Instance.IsPlaying)
                return;

            HandleForwardMovement();
            HandleLaneInput();
            HandleLaneMovement();
            HandleSpitInput();
            HandleKumisDecay();
            UpdateAnimation();
            UpdateSorting();
        }

        private void HandleForwardMovement()
        {
            // Move steadily to the right
            transform.position += Vector3.right * (CurrentSpeed * Time.deltaTime);

            // Update hoofbeat pitch with speed
            float speedRatio = CurrentSpeed / _baseSpeed;
            SoundManager.Instance?.SetHoofbeatPitch(speedRatio);
        }

        private void HandleLaneInput()
        {
            // W / Up Arrow: move to upper lane (index - 1)
            // S / Down Arrow: move to lower lane (index + 1)
            bool upPressed = NomadInput.GetUpPressed();
            bool downPressed = NomadInput.GetDownPressed();

            bool upHeld = NomadInput.GetUpHeld();
            bool downHeld = NomadInput.GetDownHeld();

            if (upPressed)
            {
                TryChangeLane(-1);
                _laneHoldTimer = 0f;
            }
            else if (downPressed)
            {
                TryChangeLane(1);
                _laneHoldTimer = 0f;
            }
            else if (upHeld || downHeld)
            {
                _laneHoldTimer += Time.deltaTime;
                if (_laneHoldTimer >= LaneRepeatDelay)
                {
                    TryChangeLane(upHeld ? -1 : 1);
                    _laneHoldTimer = 0f;
                }
            }
            else
            {
                _laneHoldTimer = 0f;
            }
        }

        private void TryChangeLane(int delta)
        {
            if (_isJumping) return; // Locked in jump trajectory

            int targetDelta = delta;

            // "Кумысная контузия" mechanic from GDD:
            // When drunk > 60%, 35% chance to overshoot by an extra line!
            if (_kumisDrunk > 60f && Random.value < 0.35f)
            {
                targetDelta += (delta > 0 ? 1 : -1);
                GameManager.Instance?.ShowFloatingText("КУМЫС ТАРТТЫ! (Овершут!)", transform.position, Color.yellow);
            }

            int newLane = Mathf.Clamp(_currentLane + targetDelta, 0, LaneManager.Instance.LaneCount - 1);
            if (newLane != _currentLane)
            {
                _currentLane = newLane;
            }
        }

        private void HandleLaneMovement()
        {
            if (LaneManager.Instance == null) return;

            float targetY = LaneManager.Instance.GetLaneY(_currentLane);
            float currentY = transform.position.y;
            float newY = Mathf.MoveTowards(currentY, targetY, _laneSwitchSpeed * Time.deltaTime);

            transform.position = new Vector3(transform.position.x, newY, transform.position.z);

            // Drunk wobble — subtle
            if (!_isJumping)
            {
                if (_kumisDrunk > 40f)
                {
                    float wobbleAngle = Mathf.Sin(Time.time * 6f) * (_kumisDrunk / 15f);
                    _visualRoot.localRotation = Quaternion.Euler(0f, 0f, wobbleAngle);
                }
                else
                {
                    _visualRoot.localRotation = Quaternion.identity;
                }
            }
        }

        private void HandleSpitInput()
        {
            // Space or Left Ctrl to spit backwards
            if (NomadInput.GetSpitPressed())
            {
                if (_spitAmmo > 0)
                {
                    FireSpit();
                }
                else
                {
                    GameManager.Instance?.ShowFloatingText("БАУРСАК ЖОҚ! (Нет плевков)", transform.position, Color.red);
                }
            }
        }

        private void FireSpit()
        {
            _spitAmmo--;
            SoundManager.Instance?.PlaySfx("Spit");

            // Spawn spit behind the camel
            var spitGo = new GameObject("CamelSpit");
            var proj = spitGo.AddComponent<SpitProjectile>();
            Vector2 spawnPos = (Vector2)transform.position + new Vector2(-0.8f, 0.4f);
            proj.Initialize(spawnPos, Vector2.left);

            GameManager.Instance?.ShowFloatingText("ТЬФУ! ПЛЕВОК!", transform.position, new Color(0.4f, 1f, 0.4f));
        }

        private void HandleKumisDecay()
        {
            // Decay drunkenness slowly
            if (_kumisDrunk > 0f)
            {
                _kumisDrunk = Mathf.Max(0f, _kumisDrunk - 4.5f * Time.deltaTime);
            }

            // Speed boost countdown
            if (_boostTimer > 0f)
            {
                _boostTimer -= Time.deltaTime;
            }
        }

        public void AddBaursak()
        {
            if (_spitAmmo < _maxSpitAmmo) _spitAmmo++;
            SoundManager.Instance?.PlaySfx("Baursak");
            GameManager.Instance?.RecordBaursak();
            GameManager.Instance?.ShowFloatingText("+1 БАУРСАК!", transform.position, new Color(1f, 0.85f, 0.2f));
        }

        public void AddKumis()
        {
            _kumisDrunk = Mathf.Clamp(_kumisDrunk + 28f, 0f, 100f);
            _boostTimer = 4.5f; // 4.5 seconds of hyperspeed!
            SoundManager.Instance?.PlaySfx("Kumis");
            GameManager.Instance?.RecordKumis();
            GameManager.Instance?.ShowFloatingText("ҚЫМЫЗ БУСТ! КЕТТІК!", transform.position, Color.cyan);
        }

        public void TriggerKurganJump()
        {
            if (!_isJumping)
            {
                StartCoroutine(KurganJumpRoutine());
            }
        }

        private IEnumerator KurganJumpRoutine()
        {
            _isJumping = true;
            SoundManager.Instance?.PlaySfx("Jump");
            GameManager.Instance?.RecordAuthority(250);
            GameManager.Instance?.ShowFloatingText("ТҰЛПАР-ФЛИП! +250 АВТОРИТЕТ!", transform.position, Color.yellow);

            float elapsed = 0f;
            _spriteRenderer.sprite = _jumpSprite;

            while (elapsed < _jumpDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / _jumpDuration;

                // Parabolic trajectory for the visual root
                float height = Mathf.Sin(t * Mathf.PI) * _jumpHeight;
                _visualRoot.localPosition = new Vector3(0f, height, 0f);

                // 360 degree spin!
                float spinAngle = Mathf.Lerp(0f, -360f, t);
                _visualRoot.localRotation = Quaternion.Euler(0f, 0f, spinAngle);

                yield return null;
            }

            _visualRoot.localPosition = Vector3.zero;
            _visualRoot.localRotation = Quaternion.identity;
            _isJumping = false;
        }

        private void UpdateAnimation()
        {
            if (_isJumping) return;

            _animTimer += Time.deltaTime * (CurrentSpeed / _baseSpeed);
            if (_animTimer >= 0.20f)
            {
                _animTimer = 0f;
                _currentFrame = (_currentFrame + 1) % _walkSprites.Length;
                _spriteRenderer.sprite = _walkSprites[_currentFrame];
            }
        }

        private void UpdateSorting()
        {
            // Depth sorting: lower Y renders in front
            _spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 30);
        }

        public void Crash(string reason)
        {
            if (_isJumping) return; // Invulnerable in air

            SoundManager.Instance?.PlaySfx("Crash");
            GameManager.Instance?.OnPlayerCrash(reason);
        }
    }
}
