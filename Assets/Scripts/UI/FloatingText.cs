using UnityEngine;
using TMPro;

namespace NomadRun.UI
{
    /// <summary>
    /// World-space floating comic/status text.
    /// </summary>
    public class FloatingText : MonoBehaviour
    {
        private TextMeshPro _tmp;
        private float _lifetime = 1.6f;
        private float _elapsed = 0f;
        private Vector3 _velocity = new Vector3(0.5f, 1.8f, 0f);

        public void Initialize(string message, Color color)
        {
            _tmp = gameObject.AddComponent<TextMeshPro>();
            _tmp.text = message;
            _tmp.fontSize = 4.5f;
            _tmp.alignment = TextAlignmentOptions.Center;
            _tmp.color = color;
            _tmp.sortingOrder = 100;
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;

            if (_tmp != null)
            {
                Color c = _tmp.color;
                c.a = Mathf.Clamp01(1f - (_elapsed / _lifetime));
                _tmp.color = c;
            }

            if (_elapsed >= _lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
