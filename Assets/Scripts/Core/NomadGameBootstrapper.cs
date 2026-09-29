using UnityEngine;
using NomadRun.Audio;
using NomadRun.Player;
using NomadRun.World;
using NomadRun.UI;

namespace NomadRun.Core
{
    /// <summary>
    /// Automatic game bootstrapper.
    /// Can initialize the whole game scene at runtime if components are missing,
    /// so you can press Play immediately in Unity!
    /// </summary>
    public class NomadGameBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoaded()
        {
            if (FindAnyObjectByType<GameManager>() == null)
            {
                var bootGo = new GameObject("--- NOMAD_RUN_BOOTSTRAPPER ---");
                bootGo.AddComponent<NomadGameBootstrapper>();
            }
        }

        private void Awake()
        {
            SetupGameEnvironment();
        }

        public void SetupGameEnvironment()
        {
            // 1. Setup Camera
            var mainCam = Camera.main;
            if (mainCam == null)
            {
                var camGo = new GameObject("Main Camera");
                mainCam = camGo.AddComponent<Camera>();
                camGo.tag = "MainCamera";
            }
            mainCam.orthographic = true;
            mainCam.orthographicSize = 5f;
            mainCam.backgroundColor = new Color(0.35f, 0.65f, 0.95f);
            mainCam.clearFlags = CameraClearFlags.SolidColor;

            var camCtrl = mainCam.GetComponent<CameraController>();
            if (camCtrl == null) camCtrl = mainCam.gameObject.AddComponent<CameraController>();

            // 2. Setup Core Managers
            if (LaneManager.Instance == null)
            {
                var laneGo = new GameObject("LaneManager");
                laneGo.AddComponent<LaneManager>();
            }

            if (SoundManager.Instance == null)
            {
                var soundGo = new GameObject("SoundManager");
                soundGo.AddComponent<SoundManager>();
            }

            if (GameManager.Instance == null)
            {
                var gmGo = new GameObject("GameManager");
                gmGo.AddComponent<GameManager>();
            }

            // 3. Setup Parallax Background & Kazakh Monuments
            if (FindAnyObjectByType<ParallaxBackground>() == null)
            {
                var bgGo = new GameObject("ParallaxBackground");
                bgGo.AddComponent<ParallaxBackground>();
            }

            // 4. Setup Player Camel
            PlayerController player = FindAnyObjectByType<PlayerController>();
            if (player == null)
            {
                var playerGo = new GameObject("Player_Camel");
                playerGo.transform.position = new Vector3(-3f, 0f, 0f);
                player = playerGo.AddComponent<PlayerController>();
            }

            // 5. Setup Police Chaser
            if (PoliceChaser.Instance == null)
            {
                var policeGo = new GameObject("DPS_PoliceCar");
                policeGo.transform.position = new Vector3(-17f, 0f, 0f);
                policeGo.AddComponent<PoliceChaser>();
            }

            // 6. Setup Obstacle Spawner
            if (FindAnyObjectByType<ObstacleSpawner>() == null)
            {
                var spawnerGo = new GameObject("ObstacleSpawner");
                spawnerGo.AddComponent<ObstacleSpawner>();
            }

            // 7. Setup UIManager
            if (UIManager.Instance == null)
            {
                var uiGo = new GameObject("UIManager");
                uiGo.AddComponent<UIManager>();
            }

            Debug.Log("<color=yellow>[NomadRun] Казахский степной раннер 'Nomad Drift' успешно инициализирован!</color>");
        }
    }
}
