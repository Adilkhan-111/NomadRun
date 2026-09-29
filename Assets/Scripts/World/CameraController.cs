using UnityEngine;
using NomadRun.Player;

namespace NomadRun.World
{
    /// <summary>
    /// Smooth follow camera with support for screen shake and drunken sway.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance { get; private set; }

        [Header("Target & Offsets")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(4.5f, 0f, -10f);
        [SerializeField] private float _smoothTimeX = 0.08f;

        private float _currentVelocityX;
        private float _shakeTimer = 0f;
        private float _shakeIntensity = 0f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (_target == null && PlayerController.Instance != null)
            {
                _target = PlayerController.Instance.transform;
            }
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                if (PlayerController.Instance != null)
                    _target = PlayerController.Instance.transform;
                else
                    return;
            }

            float targetX = _target.position.x + _offset.x;
            float smoothX = Mathf.SmoothDamp(transform.position.x, targetX, ref _currentVelocityX, _smoothTimeX);

            Vector3 newPos = new Vector3(smoothX, _offset.y, _offset.z);

            // Screen shake
            if (_shakeTimer > 0f)
            {
                _shakeTimer -= Time.deltaTime;
                Vector3 shakeOffset = (Vector3)(Random.insideUnitCircle * _shakeIntensity);
                newPos += shakeOffset;
            }

            // Drunken sway when Kumis is high
            if (PlayerController.Instance != null && PlayerController.Instance.KumisDrunkPercent > 50f)
            {
                float drunkRatio = PlayerController.Instance.KumisDrunkPercent / 100f;
                float swayY = Mathf.Sin(Time.time * 3f) * 0.45f * drunkRatio;
                float rollZ = Mathf.Sin(Time.time * 2.2f) * 1.8f * drunkRatio;
                newPos.y += swayY;
                transform.rotation = Quaternion.Euler(0f, 0f, rollZ);
            }
            else
            {
                transform.rotation = Quaternion.identity;
            }

            transform.position = newPos;
        }

        public void Shake(float duration, float intensity)
        {
            _shakeTimer = duration;
            _shakeIntensity = intensity;
        }
    }
}
