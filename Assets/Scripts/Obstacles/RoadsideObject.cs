using UnityEngine;
using NomadRun.Player;
using NomadRun.World;
using NomadRun.Core;
using NomadRun.Audio;

namespace NomadRun.Obstacles
{
    public enum RoadsideType
    {
        Yurt,
        Aksakal,
        ToyBillboard,
        RoadSign
    }

    /// <summary>
    /// Interactive roadside objects on shoulders (Lane 0 and 4).
    /// Player can press 'F' to shout a toast and receive blessings (+Authority).
    /// </summary>
    public class RoadsideObject : MonoBehaviour
    {
        [SerializeField] private RoadsideType _type;
        private SpriteRenderer _renderer;
        private bool _blessed = false;
        private bool _playerInRange = false;

        public void Initialize(RoadsideType type)
        {
            _type = type;

            _renderer = GetComponent<SpriteRenderer>();
            if (_renderer == null) _renderer = gameObject.AddComponent<SpriteRenderer>();

            string key = type == RoadsideType.Yurt ? "Yurt" :
                         type == RoadsideType.Aksakal ? "Aksakal" :
                         type == RoadsideType.ToyBillboard ? "ToyBillboard" : "RoadSign";

            _renderer.sprite = NomadArtFactory.GetSprite(key);
            _renderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 30);

            var col = gameObject.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 2.2f;
        }

        private void Update()
        {
            if (_playerInRange && !_blessed)
            {
                if (NomadInput.GetToastPressed())
                {
                    GiveBlessing();
                }
            }

            if (PlayerController.Instance != null &&
                transform.position.x < PlayerController.Instance.transform.position.x - 25f)
            {
                Destroy(gameObject);
            }
        }

        private void GiveBlessing()
        {
            _blessed = true;
            SoundManager.Instance?.PlaySfx("Blessing");
            GameManager.Instance?.RecordAuthority(100);

            string toastText = (_type == RoadsideType.Aksakal) ? "АҚСАҚАЛДЫҢ БАТАСЫ! +100 АВТОРИТЕТ!" :
                               (_type == RoadsideType.Yurt) ? "САУ БОЛЫҢЫЗДАР! +100 АВТОРИТЕТ!" :
                               "ТОЙҒА СӘЛЕМ! +100 АВТОРИТЕТ!";

            GameManager.Instance?.ShowFloatingText(toastText, transform.position + Vector3.up * 1.5f, Color.green);

            // Shouting toasts alerts the police patrol!
            PoliceChaser.Instance?.AlertByToast();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !_blessed)
            {
                _playerInRange = true;
                GameManager.Instance?.ShowToastPrompt(true);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                _playerInRange = false;
                GameManager.Instance?.ShowToastPrompt(false);
            }
        }
    }
}
