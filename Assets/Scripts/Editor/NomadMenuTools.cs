#if UNITY_EDITOR
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using NomadRun.Core;
using NomadRun.World;
using NomadRun.Audio;
using NomadRun.Player;
using NomadRun.UI;

namespace NomadRun.Editor
{
    public static class NomadMenuTools
    {
        [MenuItem("Nomad Drift/1. Build & Setup Scene", false, 1)]
        public static void SetupScene()
        {
            // Clear existing runners
            var existing = Object.FindObjectsByType<NomadGameBootstrapper>(FindObjectsSortMode.None);
            foreach (var b in existing) Object.DestroyImmediate(b.gameObject);

            var bootGo = new GameObject("--- NOMAD_RUN_BOOTSTRAPPER ---");
            var boot = bootGo.AddComponent<NomadGameBootstrapper>();
            boot.SetupGameEnvironment();

            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            Debug.Log("[NomadRun] Сцена успешно настроена! Нажмите кнопку Play для запуска игры.");
        }

        [MenuItem("Nomad Drift/2. Export Sprites to PNG Files", false, 2)]
        public static void ExportSpritesToPNG()
        {
            string folder = "Assets/Art/Sprites";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string[] spriteNames = new string[]
            {
                "Camel_0", "Camel_1", "Camel_2", "Camel_Jump",
                "Baursak", "Kumis", "Sheep_0", "Sheep_1",
                "PoliceCar_0", "PoliceCar_1", "Spit", "Pothole",
                "Kazan", "Watermelon", "Kurgan", "Yurt",
                "Aksakal", "ToyBillboard", "RoadSign",
                "Baiterek", "KhanShatyr", "YasawiMausoleum", "GoldenMan",
                "AlatauMountains", "SteppeCloud"
            };

            NomadArtFactory.ClearCache();

            int count = 0;
            foreach (var name in spriteNames)
            {
                Sprite s = NomadArtFactory.GetSprite(name);
                if (s != null && s.texture != null)
                {
                    byte[] pngBytes = s.texture.EncodeToPNG();
                    string path = Path.Combine(folder, $"{name}.png");
                    File.WriteAllBytes(path, pngBytes);
                    count++;
                }
            }

            AssetDatabase.Refresh();
            Debug.Log($"[NomadRun] Экспортировано {count} спрайтов в {folder}!");
        }
    }
}
#endif
