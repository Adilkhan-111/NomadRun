using System.Collections.Generic;
using UnityEngine;
using NomadRun.Core;
using NomadRun.Obstacles;
using NomadRun.Player;

namespace NomadRun.World
{
    /// <summary>
    /// Procedurally spawns obstacles, pickups, ramps, and roadside decor ahead of the player.
    /// Ensures there is always a passable route.
    /// </summary>
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private float _spawnAheadDistance = 28f;
        [SerializeField] private float _spawnInterval = 7.5f;

        private float _nextSpawnX = 15f;

        private void Update()
        {
            if (PlayerController.Instance == null || GameManager.Instance == null || !GameManager.Instance.IsPlaying)
                return;

            float playerX = PlayerController.Instance.transform.position.x;
            while (_nextSpawnX < playerX + _spawnAheadDistance)
            {
                SpawnSegment(_nextSpawnX);
                _nextSpawnX += _spawnInterval;
            }
        }

        private void SpawnSegment(float x)
        {
            if (LaneManager.Instance == null) return;

            // 1. Shoulder spawns (Lane 0 and Lane 4)
            SpawnShoulderContent(0, x);
            SpawnShoulderContent(4, x);

            // 2. Highway spawns (Lanes 1, 2, 3)
            // Pick at most 1 or 2 lanes to place obstacles, ensuring at least one asphalt lane is open!
            List<int> asphaltLanes = new List<int> { 1, 2, 3 };

            // Decide pattern:
            float roll = Random.value;
            if (roll < 0.40f)
            {
                // Single obstacle on one asphalt lane
                int blockedLane = asphaltLanes[Random.Range(0, asphaltLanes.Count)];
                SpawnAsphaltObstacle(blockedLane, x);
                asphaltLanes.Remove(blockedLane);

                // Maybe place a pickup on one of the open lanes
                int pickupLane = asphaltLanes[Random.Range(0, asphaltLanes.Count)];
                SpawnPickup(pickupLane, x);
            }
            else if (roll < 0.70f)
            {
                // Double obstacle (leaves 1 clear lane)
                int safeLane = asphaltLanes[Random.Range(0, asphaltLanes.Count)];
                asphaltLanes.Remove(safeLane);

                foreach (int lane in asphaltLanes)
                {
                    SpawnAsphaltObstacle(lane, x);
                }

                // Reward player on safe lane
                SpawnPickup(safeLane, x);
            }
            else
            {
                // Bonus stretch! Baursak row or Kumis on asphalt lanes
                int bonusLane = asphaltLanes[Random.Range(0, asphaltLanes.Count)];
                SpawnBaursakRow(bonusLane, x);

                if (Random.value < 0.45f)
                {
                    int kumisLane = (bonusLane == 2) ? 1 : 2;
                    SpawnPickup(kumisLane, x, forceKumis: true);
                }
            }
        }

        private void SpawnShoulderContent(int shoulderLane, float x)
        {
            float roll = Random.value;
            float y = LaneManager.Instance.GetLaneY(shoulderLane);

            if (roll < 0.28f)
            {
                // Kurgan Ramp for stunts!
                var rampGo = new GameObject("KurganRamp");
                rampGo.transform.position = new Vector3(x, y, 0f);
                var ramp = rampGo.AddComponent<KurganRamp>();
                ramp.Initialize();
            }
            else if (roll < 0.55f)
            {
                // Roadside interactive object (Yurt, Aksakal, or Billboard)
                var roadGo = new GameObject("Roadside");
                roadGo.transform.position = new Vector3(x, y, 0f);
                var roadObj = roadGo.AddComponent<RoadsideObject>();

                RoadsideType type = (roll < 0.38f) ? RoadsideType.Aksakal :
                                    (roll < 0.48f) ? RoadsideType.Yurt : RoadsideType.ToyBillboard;
                roadObj.Initialize(type);
            }
            else if (roll < 0.70f)
            {
                // Road sign
                var signGo = new GameObject("RoadSign");
                signGo.transform.position = new Vector3(x, y, 0f);
                var roadObj = signGo.AddComponent<RoadsideObject>();
                roadObj.Initialize(RoadsideType.RoadSign);
            }
        }

        private void SpawnAsphaltObstacle(int lane, float x)
        {
            float y = LaneManager.Instance.GetLaneY(lane);
            var obsGo = new GameObject("Obstacle_Lane" + lane);
            obsGo.transform.position = new Vector3(x, y, 0f);
            var obstacle = obsGo.AddComponent<Obstacle>();

            float roll = Random.value;
            if (roll < 0.40f)
            {
                obstacle.Initialize(ObstacleType.Sheep, lane);
                var sheep = obsGo.AddComponent<SheepController>();
                sheep.SetupWandering(lane);
            }
            else if (roll < 0.65f)
            {
                obstacle.Initialize(ObstacleType.Pothole, lane);
            }
            else if (roll < 0.85f)
            {
                obstacle.Initialize(ObstacleType.Kazan, lane);
            }
            else
            {
                obstacle.Initialize(ObstacleType.Watermelon, lane);
            }
        }

        private void SpawnPickup(int lane, float x, bool forceKumis = false)
        {
            float y = LaneManager.Instance.GetLaneY(lane);
            var pickGo = new GameObject("Pickup_Lane" + lane);
            pickGo.transform.position = new Vector3(x, y, 0f);
            var pickup = pickGo.AddComponent<Pickup>();

            PickupType type = (forceKumis || Random.value < 0.30f) ? PickupType.Kumis : PickupType.Baursak;
            pickup.Initialize(type);
        }

        private void SpawnBaursakRow(int lane, float startX)
        {
            float y = LaneManager.Instance.GetLaneY(lane);
            for (int i = 0; i < 3; i++)
            {
                var pickGo = new GameObject($"Baursak_{i}");
                pickGo.transform.position = new Vector3(startX + i * 1.5f, y, 0f);
                var pickup = pickGo.AddComponent<Pickup>();
                pickup.Initialize(PickupType.Baursak);
            }
        }
    }
}
