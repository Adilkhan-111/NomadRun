using UnityEngine;

namespace NomadRun.Core
{
    /// <summary>
    /// Manages the 5 lanes of the steppe highway:
    /// Lane 0: Верхняя обочина (Курганы-трамплины, юрты, аксакалы)
    /// Lane 1: Левая / верхняя полоса асфальта
    /// Lane 2: Центральная полоса асфальта
    /// Lane 3: Правая / нижняя полоса асфальта
    /// Lane 4: Нижняя обочина (Курганы-трамплины, билборды)
    /// </summary>
    public class LaneManager : MonoBehaviour
    {
        public static LaneManager Instance { get; private set; }

        [Header("Lane Settings")]
        [SerializeField] private int _laneCount = 5;
        [SerializeField] private float _laneSpacing = 1.1f;
        [SerializeField] private float _baseCenterY = 0f;

        public int LaneCount => _laneCount;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        /// <summary>
        /// Converts lane index (0 to 4) to world Y position.
        /// Lane 0 is top (+Y), Lane 4 is bottom (-Y).
        /// </summary>
        public float GetLaneY(int laneIndex)
        {
            laneIndex = Mathf.Clamp(laneIndex, 0, _laneCount - 1);
            // Index 0 -> +2 * spacing, Index 2 -> 0, Index 4 -> -2 * spacing
            float offsetFromCenter = (2 - laneIndex) * _laneSpacing;
            return _baseCenterY + offsetFromCenter;
        }

        /// <summary>
        /// Checks if a lane is the shoulder (обочина).
        /// As specified in the design doc: first (0) and last (4) are shoulders.
        /// </summary>
        public bool IsShoulder(int laneIndex)
        {
            return laneIndex == 0 || laneIndex == _laneCount - 1;
        }

        /// <summary>
        /// Clamps lane index to [0, LaneCount - 1].
        /// </summary>
        public int ClampLane(int laneIndex)
        {
            return Mathf.Clamp(laneIndex, 0, _laneCount - 1);
        }
    }
}
