using UnityEngine;

namespace Robotech.TBS.Data
{
    [CreateAssetMenu(fileName = "TerrainType", menuName = "Robotech/Data/Terrain Type", order = 2)]
    public class TerrainType : ScriptableObject
    {
        [Header("Identity")]
        public string terrainId;
        public string displayName;

        [Header("Movement/Combat")]
        public int movementCost = 1; // higher = slower
        public int defenseBonus = 0;  // flat defense bonus
        public bool isWater;
        public bool isImpassable;
        public bool isUrban;
        public bool providesElevation; // e.g., Hills/Mountains

        [Header("Yields per worked tile")]
        public int yieldMaterials;
        public int yieldCredits;
        public int yieldPopulation;
        public int yieldScience;

        [Header("Resource Nodes")]
        [Tooltip("Chance 0..1 that this terrain can spawn a Protoculture node during generation")]
        [Range(0,1f)] public float protocultureSpawnChance = 0.0f;

        [TextArea]
        public string description;
    }
}
