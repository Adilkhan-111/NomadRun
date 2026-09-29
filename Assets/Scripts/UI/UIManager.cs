using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NomadRun.Core;
using NomadRun.Player;
using NomadRun.World;

namespace NomadRun.UI
{
    /// <summary>
    /// Builds and drives the game HUD and Score Ceremony screen.
    /// Styled with traditional golden/turquoise Kazakh cultural elements.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Canvas _canvas;

        // HUD Elements
        private TextMeshProUGUI _timerText;
        private TextMeshProUGUI _speedText;
        private TextMeshProUGUI _baursakText;
        private TextMeshProUGUI _kumisText;
        private TextMeshProUGUI _policeText;
        private Slider _progressBar;
        private Slider _kumisSlider;
        private GameObject _toastPrompt;

        // Ceremony Screen
        private GameObject _ceremonyPanel;
        private TextMeshProUGUI _ceremonyTitle;
        private TextMeshProUGUI _ceremonyRank;
        private TextMeshProUGUI _ceremonyStats;
        private TMP_InputField _guestBookInput;
        private TextMeshProUGUI _guestBookSavedText;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                BuildCanvas();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void BuildCanvas()
        {
            var canvasGo = new GameObject("NomadCanvas");
            canvasGo.transform.SetParent(transform, false);

            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 500;

            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720);

            canvasGo.AddComponent<GraphicRaycaster>();

            BuildHUD(canvasGo.transform);
            BuildCeremonyPanel(canvasGo.transform);
        }

        private void BuildHUD(Transform parent)
        {
            // 1. Top Center: 120s Countdown Timer Banner
            var timerBox = CreatePanel(parent, "TimerBanner", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -40f), new Vector2(360f, 60f), new Color(0.12f, 0.15f, 0.22f, 0.85f));
            _timerText = CreateText(timerBox.transform, "TimerText", "ТОЙҒА ДЕЙІН: 120 СЕК", 24, Color.yellow, TextAlignmentOptions.Center);
            _timerText.rectTransform.anchoredPosition = Vector2.zero;
            _timerText.rectTransform.sizeDelta = new Vector2(350f, 50f);

            // Progress Bar to Toy under timer
            var barGo = new GameObject("ProgressBar");
            barGo.transform.SetParent(timerBox.transform, false);
            _progressBar = barGo.AddComponent<Slider>();
            var barRect = barGo.GetComponent<RectTransform>();
            barRect.anchoredPosition = new Vector2(0f, -24f);
            barRect.sizeDelta = new Vector2(320f, 12f);

            var bgImg = barGo.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.2f, 0.25f);
            var fillGo = new GameObject("Fill");
            fillGo.transform.SetParent(barGo.transform, false);
            var fillImg = fillGo.AddComponent<Image>();
            fillImg.color = new Color(0.98f, 0.82f, 0.25f);
            _progressBar.fillRect = fillImg.rectTransform;
            _progressBar.minValue = 0f;
            _progressBar.maxValue = 1f;

            // 2. Top Left: Baursaks & Kumis stats
            var leftBox = CreatePanel(parent, "LeftStats", new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(180f, -65f), new Vector2(320f, 95f), new Color(0.08f, 0.12f, 0.18f, 0.8f));

            _baursakText = CreateText(leftBox.transform, "BaursakText", "Баурсаки: 0 | Плевки: 3/5", 18, Color.white, TextAlignmentOptions.Left);
            _baursakText.rectTransform.anchoredPosition = new Vector2(-10f, 22f);

            _kumisText = CreateText(leftBox.transform, "KumisText", "Кумыс / Мастық: 0%", 18, Color.cyan, TextAlignmentOptions.Left);
            _kumisText.rectTransform.anchoredPosition = new Vector2(-10f, -8f);

            // Kumis drunkenness mini-bar
            var kBarGo = new GameObject("KumisBar");
            kBarGo.transform.SetParent(leftBox.transform, false);
            _kumisSlider = kBarGo.AddComponent<Slider>();
            var kBarRect = kBarGo.GetComponent<RectTransform>();
            kBarRect.anchoredPosition = new Vector2(0f, -32f);
            kBarRect.sizeDelta = new Vector2(280f, 8f);

            var kBg = kBarGo.AddComponent<Image>();
            kBg.color = new Color(0.15f, 0.25f, 0.35f);
            var kFill = new GameObject("Fill");
            kFill.transform.SetParent(kBarGo.transform, false);
            var kFillImg = kFill.AddComponent<Image>();
            kFillImg.color = Color.cyan;
            _kumisSlider.fillRect = kFillImg.rectTransform;
            _kumisSlider.minValue = 0f;
            _kumisSlider.maxValue = 100f;

            // 3. Top Right: Police Distance & Speedometer
            var rightBox = CreatePanel(parent, "RightStats", new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-180f, -65f), new Vector2(320f, 95f), new Color(0.08f, 0.12f, 0.18f, 0.8f));

            _policeText = CreateText(rightBox.transform, "PoliceText", "[ДПС] Полиция: 14м артта", 18, new Color(1f, 0.45f, 0.45f), TextAlignmentOptions.Right);
            _policeText.rectTransform.anchoredPosition = new Vector2(10f, 22f);

            _speedText = CreateText(rightBox.transform, "SpeedText", "Жылдамдық: 65 км/сағ", 18, Color.yellow, TextAlignmentOptions.Right);
            _speedText.rectTransform.anchoredPosition = new Vector2(10f, -12f);

            // UI simplified: removed toast prompt and bottom controls bar for a cleaner look.
        }

        private void BuildCeremonyPanel(Transform parent)
        {
            _ceremonyPanel = CreatePanel(parent, "CeremonyPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                Vector2.zero, new Vector2(680f, 520f), new Color(0.10f, 0.12f, 0.18f, 0.96f));

            // Gold ornament border
            var outline = _ceremonyPanel.AddComponent<Outline>();
            outline.effectColor = new Color(0.98f, 0.82f, 0.25f);
            outline.effectDistance = new Vector2(3f, 3f);

            // Title
            _ceremonyTitle = CreateText(_ceremonyPanel.transform, "CeremonyTitle", "ФИНАЛЬНАЯ ЦЕРЕМОНИЯ: ДАСТАРХАН У БӘКЕ",
                26, Color.yellow, TextAlignmentOptions.Center);
            _ceremonyTitle.rectTransform.anchoredPosition = new Vector2(0f, 215f);
            _ceremonyTitle.rectTransform.sizeDelta = new Vector2(640f, 50f);

            // Rank / Status
            _ceremonyRank = CreateText(_ceremonyPanel.transform, "CeremonyRank", "👑 ТОЙ СҰЛТАНЫ",
                22, Color.cyan, TextAlignmentOptions.Center);
            _ceremonyRank.rectTransform.anchoredPosition = new Vector2(0f, 165f);
            _ceremonyRank.rectTransform.sizeDelta = new Vector2(600f, 40f);

            // Detailed Statistics
            _ceremonyStats = CreateText(_ceremonyPanel.transform, "CeremonyStats", "Статистика тоя...",
                19, Color.white, TextAlignmentOptions.Left);
            _ceremonyStats.rectTransform.anchoredPosition = new Vector2(0f, 45f);
            _ceremonyStats.rectTransform.sizeDelta = new Vector2(580f, 170f);

            // Guest Book Input: "Книга почетных гостей Тоя"
            var bookLabel = CreateText(_ceremonyPanel.transform, "BookLabel", "📖 Книга почетных гостей Тоя (Введите имя):",
                17, new Color(0.95f, 0.85f, 0.45f), TextAlignmentOptions.Center);
            bookLabel.rectTransform.anchoredPosition = new Vector2(0f, -80f);
            bookLabel.rectTransform.sizeDelta = new Vector2(500f, 30f);

            var inputGo = CreatePanel(_ceremonyPanel.transform, "GuestInput", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-50f, -120f), new Vector2(320f, 40f), new Color(0.18f, 0.22f, 0.32f));
            _guestBookInput = inputGo.AddComponent<TMP_InputField>();
            var inputText = CreateText(inputGo.transform, "Text", "Батыр / Хан / Шабандоз", 18, Color.white, TextAlignmentOptions.Left);
            inputText.rectTransform.anchoredPosition = new Vector2(10f, 0f);
            inputText.rectTransform.sizeDelta = new Vector2(300f, 35f);
            _guestBookInput.textComponent = inputText;

            // Sign Button
            var signBtnGo = CreateButton(_ceremonyPanel.transform, "SignBtn", new Vector2(160f, -120f), new Vector2(140f, 40f),
                "ҚОЛ ҚОЮ", () =>
                {
                    if (_guestBookSavedText != null)
                    {
                        string name = string.IsNullOrEmpty(_guestBookInput.text) ? "Құрметті Қонақ" : _guestBookInput.text;
                        _guestBookSavedText.text = $"✓ {name} жазылды! Бәке риза болды!";
                    }
                });

            _guestBookSavedText = CreateText(_ceremonyPanel.transform, "SavedText", "", 16, Color.green, TextAlignmentOptions.Center);
            _guestBookSavedText.rectTransform.anchoredPosition = new Vector2(0f, -165f);
            _guestBookSavedText.rectTransform.sizeDelta = new Vector2(550f, 30f);

            // Play Again Button
            CreateButton(_ceremonyPanel.transform, "RestartBtn", new Vector2(0f, -215f), new Vector2(280f, 50f),
                "ҚАЙТА ШАБУ (Играть снова)", () => GameManager.Instance?.RestartGame());

            _ceremonyPanel.SetActive(false);
        }

        private void Update()
        {
            if (GameManager.Instance == null || PlayerController.Instance == null)
                return;

            if (GameManager.Instance.IsPlaying)
            {
                // Update Timer
                float time = GameManager.Instance.TimeRemaining;
                _timerText.text = $"ТОЙҒА ДЕЙІН: {time:F1} СЕК";
                _timerText.color = (time < 20f) ? Color.red : Color.yellow;

                // Update Progress
                _progressBar.value = GameManager.Instance.ProgressPercent;

                // Update Baursak & Spit
                int spit = PlayerController.Instance.SpitAmmo;
                int maxSpit = PlayerController.Instance.MaxSpitAmmo;
                int baursaks = GameManager.Instance.BaursaksDelivered;
                _baursakText.text = $"Баурсаки: {baursaks}  |  Плевки: {spit}/{maxSpit}";

                // Update Kumis
                float kumis = PlayerController.Instance.KumisDrunkPercent;
                _kumisSlider.value = kumis;
                string boostStatus = PlayerController.Instance.IsKumisBoosted ? " [БУСТ!]" : "";
                _kumisText.text = $"Кумыс / Мастық: {kumis:F0}%{boostStatus}";

                // Update Police Distance
                if (PoliceChaser.Instance != null)
                {
                    float dist = PoliceChaser.Instance.DistanceBehind;
                    _policeText.text = $"[ДПС] Полиция: {dist:F1}м артта";
                }

                // Update Speedometer
                float speedKmh = PlayerController.Instance.CurrentSpeed * 3.6f;
                _speedText.text = $"Жылдамдық: {speedKmh:F0} км/сағ";

                // Quick restart shortcut (kept minimal UI)
                if (NomadInput.GetRestartPressed())
                {
                    GameManager.Instance.RestartGame();
                }
            }
        }

        public void SetToastPromptVisible(bool visible)
        {
            if (_toastPrompt != null)
                _toastPrompt.SetActive(visible);
        }

        public void ShowCeremonyScreen(bool isVictory)
        {
            if (_ceremonyPanel == null) return;
            _ceremonyPanel.SetActive(true);

            if (_toastPrompt != null) _toastPrompt.SetActive(false);

            if (isVictory)
            {
                _ceremonyTitle.text = "[!] АМАН-ЕСЕН ЖЕТТІК! ТОЙ БАСТАЛДЫ! [!]";
                _ceremonyTitle.color = Color.yellow;
            }
            else
            {
                _ceremonyTitle.text = "[X] " + GameManager.Instance.EndGameMessage + " [X]";
                _ceremonyTitle.color = Color.red;
            }

            _ceremonyRank.text = GameManager.Instance.GetHonoraryTitle();

            _ceremonyStats.text =
                $"* Баурсаков довезено до дастархана: <b>{GameManager.Instance.BaursaksDelivered}</b>\n" +
                $"* Кумыса выпито по пути: <b>{GameManager.Instance.KumisBottlesDrunk}</b>\n" +
                $"* Овец успешно объехали: <b>{GameManager.Instance.SheepDodged}</b>\n" +
                $"* Заработанный Авторитет (Уят-контроль): <b>{GameManager.Instance.AuthorityScore} очков</b>\n" +
                $"* Оставшееся время до банкета: <b>{GameManager.Instance.TimeRemaining:F1} сек</b>";
        }

        // ====================================================================
        // UI BUILDER HELPERS
        // ====================================================================
        private GameObject CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, Vector2 size, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            var img = go.AddComponent<Image>();
            img.color = color;
            return go;
        }

        private TextMeshProUGUI CreateText(Transform parent, string name, string text, float size, Color color, TextAlignmentOptions align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.alignment = align;
            return tmp;
        }

        private GameObject CreateButton(Transform parent, string name, Vector2 pos, Vector2 size, string label, System.Action onClick)
        {
            var go = CreatePanel(parent, name, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size, new Color(0.85f, 0.65f, 0.15f));
            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(() => onClick?.Invoke());

            var txt = CreateText(go.transform, "Label", label, 18, Color.black, TextAlignmentOptions.Center);
            txt.rectTransform.anchoredPosition = Vector2.zero;
            txt.rectTransform.sizeDelta = size;

            return go;
        }
    }
}
