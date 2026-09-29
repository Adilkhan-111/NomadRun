using System.Collections.Generic;
using UnityEngine;
using NomadRun.Core;

namespace NomadRun.World
{
    /// <summary>
    /// Multi-layered parallax background featuring Kazakh monuments (Baiterek, Khan Shatyr, Yasawi, Golden Man),
    /// the snowy Alatau mountains, steppe sky, and repeating road surface for the 5 lanes.
    /// </summary>
    public class ParallaxBackground : MonoBehaviour
    {
        [Header("Camera Reference")]
        [SerializeField] private Camera _cam;

        // Background layer objects
        private readonly List<Transform> _mountainChunks = new List<Transform>();
        private readonly List<Transform> _monumentInstances = new List<Transform>();
        private readonly List<Transform> _cloudChunks = new List<Transform>();
        private readonly List<Transform> _roadChunks = new List<Transform>();

        private float _lastCamX;
        private float _monumentSpawnNextX = 20f;
        private int _monumentIndex = 0;
        private readonly string[] _monumentNames = new string[]
        {
            "Baiterek",
            "KhanShatyr",
            "YasawiMausoleum",
            "GoldenMan",
            "ExpoPavilion",
            "KokTobe",
            "Medeu"
        };

        private void Start()
        {
            if (_cam == null) _cam = Camera.main;
            if (_cam != null) _lastCamX = _cam.transform.position.x;

            SetupSky();
            SetupMountains();
            SetupRoadSegments();
        }

        private void SetupSky()
        {
            // Sky backdrop quad
            var skyGo = new GameObject("SkyBackdrop");
            skyGo.transform.SetParent(transform, false);
            var mr = skyGo.AddComponent<MeshRenderer>();
            var mf = skyGo.AddComponent<MeshFilter>();

            // Simple colored quad
            var mesh = new Mesh();
            mesh.vertices = new Vector3[]
            {
                new Vector3(-50f, -20f, 0f),
                new Vector3(50f, -20f, 0f),
                new Vector3(-50f, 30f, 0f),
                new Vector3(50f, 30f, 0f)
            };
            mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
            mesh.colors = new Color[]
            {
                new Color(0.95f, 0.85f, 0.65f), // Golden horizon
                new Color(0.95f, 0.85f, 0.65f),
                new Color(0.35f, 0.65f, 0.95f), // Blue sky
                new Color(0.35f, 0.65f, 0.95f)
            };
            mf.mesh = mesh;

            var mat = new Material(Shader.Find("Sprites/Default"));
            mr.material = mat;
            mr.sortingOrder = -500;

            // Spawn some initial clouds
            for (int i = 0; i < 6; i++)
            {
                var cloud = new GameObject($"Cloud_{i}");
                cloud.transform.SetParent(transform);
                cloud.transform.position = new Vector3(i * 14f - 10f, Random.Range(4.5f, 8f), 0f);
                var sr = cloud.AddComponent<SpriteRenderer>();
                sr.sprite = NomadArtFactory.GetSprite("SteppeCloud");
                sr.sortingOrder = -400;
                _cloudChunks.Add(cloud.transform);
            }
        }

        private void SetupMountains()
        {
            for (int i = 0; i < 4; i++)
            {
                var mtn = new GameObject($"Mountain_{i}");
                mtn.transform.SetParent(transform);
                mtn.transform.position = new Vector3(i * 24f - 15f, 4.0f, 0f);
                var sr = mtn.AddComponent<SpriteRenderer>();
                sr.sprite = NomadArtFactory.GetSprite("AlatauMountains");
                sr.sortingOrder = -300;
                _mountainChunks.Add(mtn.transform);
            }
        }

        private void SetupRoadSegments()
        {
            // Spawn repeating 5-lane road chunks
            for (int i = 0; i < 5; i++)
            {
                var chunk = CreateRoadChunk(i * 20f - 10f);
                _roadChunks.Add(chunk.transform);
            }
        }

        private GameObject CreateRoadChunk(float startX)
        {
            var chunk = new GameObject("RoadChunk");
            chunk.transform.SetParent(transform);
            chunk.transform.position = new Vector3(startX, 0f, 0f);

            // 5 full highway lanes visuals:
            // Lane 0: Y = +2.2f
            // Lane 1: Y = +1.1f
            // Lane 2: Y =  0.0f
            // Lane 3: Y = -1.1f
            // Lane 4: Y = -2.2f
            // Total asphalt height = 5.5f, spanning from Y = -2.75f to +2.75f!

            // 1. Steppe Shoulders (Sand/Grass) outside the 5-lane highway:
            // Top Shoulder: center Y = +3.45f, height 1.4f (spans +2.75f to +4.15f)
            CreateQuad(chunk.transform, new Vector3(10f, 3.45f, 0f), new Vector2(20f, 1.4f),
                new Color(0.72f, 0.68f, 0.35f), -100);

            // Bottom Shoulder: center Y = -3.45f, height 1.4f (spans -4.15f to -2.75f)
            CreateQuad(chunk.transform, new Vector3(10f, -3.45f, 0f), new Vector2(20f, 1.4f),
                new Color(0.72f, 0.68f, 0.35f), -100);

            // 2. Asphalt Road: 5 full lanes
            CreateQuad(chunk.transform, new Vector3(10f, 0f, 0f), new Vector2(20f, 5.5f),
                new Color(0.20f, 0.20f, 0.22f), -90);

            // 3. Dashed White Dividing Lines between the 5 lanes:
            // Divider 0 (between Lane 0 and 1): Y = +1.65f
            // Divider 1 (between Lane 1 and 2): Y = +0.55f
            // Divider 2 (between Lane 2 and 3): Y = -0.55f
            // Divider 3 (between Lane 3 and 4): Y = -1.65f
            CreateDashedLines(chunk.transform, +1.65f);
            CreateDashedLines(chunk.transform, +0.55f);
            CreateDashedLines(chunk.transform, -0.55f);
            CreateDashedLines(chunk.transform, -1.65f);

            // 4. Solid Road Edge Lines (Top and Bottom borders):
            // Border Top: Y = +2.75f
            // Border Bottom: Y = -2.75f
            CreateQuad(chunk.transform, new Vector3(10f, 2.75f, 0f), new Vector2(20f, 0.08f), Color.white, -80);
            CreateQuad(chunk.transform, new Vector3(10f, -2.75f, 0f), new Vector2(20f, 0.08f), Color.white, -80);

            return chunk;
        }

        private void CreateDashedLines(Transform parent, float y)
        {
            for (float x = 1f; x < 20f; x += 3.5f)
            {
                CreateQuad(parent, new Vector3(x, y, 0f), new Vector2(1.8f, 0.08f),
                    new Color(0.95f, 0.95f, 0.95f, 0.85f), -80);
            }
        }

        private void CreateQuad(Transform parent, Vector3 localPos, Vector2 size, Color c, int sortOrder)
        {
            var go = new GameObject("Plane");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;

            var mr = go.AddComponent<MeshRenderer>();
            var mf = go.AddComponent<MeshFilter>();
            var mesh = new Mesh();
            float hw = size.x * 0.5f, hh = size.y * 0.5f;
            mesh.vertices = new Vector3[]
            {
                new Vector3(-hw, -hh, 0f),
                new Vector3(hw, -hh, 0f),
                new Vector3(-hw, hh, 0f),
                new Vector3(hw, hh, 0f)
            };
            mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
            mesh.colors = new Color[] { c, c, c, c };
            mf.mesh = mesh;

            var mat = new Material(Shader.Find("Sprites/Default"));
            mr.material = mat;
            mr.sortingOrder = sortOrder;
        }

        private void LateUpdate()
        {
            if (_cam == null) return;

            float camX = _cam.transform.position.x;
            float deltaX = camX - _lastCamX;
            _lastCamX = camX;

            // 1. Mountains parallax (moves at 10% speed)
            foreach (var mtn in _mountainChunks)
            {
                mtn.position += Vector3.right * (deltaX * 0.90f);
                if (mtn.position.x < camX - 25f)
                {
                    mtn.position += Vector3.right * (4 * 24f);
                }
            }

            // 2. Clouds parallax (drifts very slowly)
            foreach (var cloud in _cloudChunks)
            {
                cloud.position += Vector3.right * (deltaX * 0.97f - Time.deltaTime * 0.15f);
                if (cloud.position.x < camX - 20f)
                {
                    cloud.position += Vector3.right * (6 * 14f);
                }
            }

            // 3. Repeating road chunks (re-cycles ahead of camera)
            foreach (var road in _roadChunks)
            {
                if (road.position.x < camX - 25f)
                {
                    road.position += Vector3.right * (5 * 20f);
                }
            }

            // 4. Kazakh Monuments Spawner
            // Spawns them every 70-100 units in the mid-background with parallax
            if (camX + 35f > _monumentSpawnNextX)
            {
                SpawnMonument(_monumentSpawnNextX);
                _monumentSpawnNextX += Random.Range(70f, 100f);
            }

            // Move monuments at parallax speed (35% speed)
            for (int i = _monumentInstances.Count - 1; i >= 0; i--)
            {
                var mon = _monumentInstances[i];
                if (mon != null)
                {
                    mon.position += Vector3.right * (deltaX * 0.65f);
                    if (mon.position.x < camX - 35f)
                    {
                        Destroy(mon.gameObject);
                        _monumentInstances.RemoveAt(i);
                    }
                }
            }
        }

        private void SpawnMonument(float x)
        {
            string monName = _monumentNames[_monumentIndex];
            _monumentIndex = (_monumentIndex + 1) % _monumentNames.Length;

            var monGo = new GameObject($"Monument_{monName}");
            monGo.transform.SetParent(transform);
            // Placed behind top shoulder
            monGo.transform.position = new Vector3(x, 2.7f, 0f);
            // Scale up for better visibility
            monGo.transform.localScale = new Vector3(1.8f, 1.8f, 1f);

            var sr = monGo.AddComponent<SpriteRenderer>();
            sr.sprite = NomadArtFactory.GetSprite(monName);
            sr.sortingOrder = -200; // Behind road and roadside decor

            _monumentInstances.Add(monGo.transform);
        }
    }
}
