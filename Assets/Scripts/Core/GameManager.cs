using UnityEngine;
using UnityEngine.SceneManagement;
using NomadRun.Audio;
using NomadRun.World;
using NomadRun.UI;

namespace NomadRun.Core
{
    public enum GameState
    {
        Playing,
        Victory,
        GameOver
    }

    /// <summary>
    /// Central game manager controlling the 120-second Toy countdown,
    /// distance tracking to the banquet, statistics, and the Score Ceremony.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Race & Time Settings")]
        [SerializeField] private float _timeRemaining = 120f;
        [SerializeField] private float _totalDistance = 2400f; // Meters to Toy restaurant
        [SerializeField] private float _currentDistance = 0f;

        [Header("Statistics")]
        [SerializeField] private int _baursaksDelivered = 0;
        [SerializeField] private int _kumisBottlesDrunk = 0;
        [SerializeField] private int _sheepDodged = 0;
        [SerializeField] private int _authorityScore = 0;

        private GameState _state = GameState.Playing;
        private string _endGameMessage = "";

        public GameState State => _state;
        public bool IsPlaying => _state == GameState.Playing;
        public float TimeRemaining => Mathf.Max(0f, _timeRemaining);
        public float CurrentDistance => _currentDistance;
        public float TotalDistance => _totalDistance;
        public float ProgressPercent => Mathf.Clamp01(_currentDistance / _totalDistance);

        public int BaursaksDelivered => _baursaksDelivered;
        public int KumisBottlesDrunk => _kumisBottlesDrunk;
        public int SheepDodged => _sheepDodged;
        public int AuthorityScore => _authorityScore;
        public string EndGameMessage => _endGameMessage;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (!IsPlaying) return;

            // 120s countdown
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0f)
            {
                OnTimeExpired();
                return;
            }

            // Track distance
            if (NomadRun.Player.PlayerController.Instance != null)
            {
                _currentDistance = NomadRun.Player.PlayerController.Instance.transform.position.x;
                if (_currentDistance >= _totalDistance)
                {
                    OnToyReached();
                }
            }
        }

        public void RecordBaursak()
        {
            _baursaksDelivered++;
            _authorityScore += 50;
        }

        public void RecordKumis()
        {
            _kumisBottlesDrunk++;
            _authorityScore += 75;
        }

        public void RecordSheepDodged()
        {
            _sheepDodged++;
            _authorityScore += 30;
        }

        public void RecordAuthority(int points)
        {
            _authorityScore += points;
        }

        public void ShowFloatingText(string message, Vector3 worldPos, Color color)
        {
            var go = new GameObject("FloatingText");
            go.transform.position = worldPos;
            var ft = go.AddComponent<FloatingText>();
            ft.Initialize(message, color);
        }

        public void ShowToastPrompt(bool visible)
        {
            NomadRun.UI.UIManager.Instance?.SetToastPromptVisible(visible);
        }

        public void OnPlayerCrash(string reason)
        {
            CameraController.Instance?.Shake(0.35f, 0.45f);

            // Stumble penalty: lose some distance and baursaks
            if (_baursaksDelivered > 0) _baursaksDelivered--;
            _authorityScore = Mathf.Max(0, _authorityScore - 50);

            ShowFloatingText(reason, NomadRun.Player.PlayerController.Instance.transform.position, Color.red);
        }

        public void OnCaughtByPolice()
        {
            if (!IsPlaying) return;
            _state = GameState.GameOver;
            _endGameMessage = "ДПС ҰСТАП АЛДЫ! ШТРАФ ЖӘНЕ УЯТ БОЛДЫ!";
            SoundManager.Instance?.StopHoofbeats();
            SoundManager.Instance?.PlaySfx("Crash");
            NomadRun.UI.UIManager.Instance?.ShowCeremonyScreen(isVictory: false);
        }

        private void OnTimeExpired()
        {
            if (!IsPlaying) return;
            _state = GameState.GameOver;
            _endGameMessage = "УАҚЫТ БІТТІ! БӘКЕ РЕНЖІДІ: ОПОЗДАЛИ НА ТОЙ!";
            SoundManager.Instance?.StopHoofbeats();
            SoundManager.Instance?.PlaySfx("Crash");
            NomadRun.UI.UIManager.Instance?.ShowCeremonyScreen(isVictory: false);
        }

        private void OnToyReached()
        {
            if (!IsPlaying) return;
            _state = GameState.Victory;
            _endGameMessage = "АМАН-ЕСЕН ЖЕТТІК! БӘКЕНІҢ ТОЙЫ БАСТАЛДЫ!";
            SoundManager.Instance?.StopHoofbeats();
            SoundManager.Instance?.PlaySfx("Win");
            NomadRun.UI.UIManager.Instance?.ShowCeremonyScreen(isVictory: true);
        }

        public string GetHonoraryTitle()
        {
            if (_authorityScore >= 1500 && _baursaksDelivered >= 12)
                return "👑 ТОЙ СҰЛТАНЫ (Почетный Гость №1)";
            if (_authorityScore >= 900)
                return "🌟 ҚҰРМЕТТІ АҚСАҚАЛ / ҚОНАҚ";
            if (_authorityScore >= 400)
                return "🐎 СТЕПНОЙ ШАБАНДОЗ";
            return "😅 АУЫЛДЫҢ БҰЗЫҚ БАЛАСЫ (Уят болмасын)";
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
