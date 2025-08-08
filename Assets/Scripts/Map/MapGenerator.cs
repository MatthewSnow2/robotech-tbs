using System.Collections.Generic;
using UnityEngine;
using Robotech.TBS.Hex;
using Robotech.TBS.Data;

namespace Robotech.TBS.Map
{
    // Very basic procedural terrain assignment (no visuals yet)
    public class MapGenerator : MonoBehaviour
    {
        public HexGrid grid;
        [Header("Terrain Catalog")] public TerrainType plains;
        public TerrainType forest;
        public TerrainType hills;
        public TerrainType mountains;
        public TerrainType desert;
        public TerrainType tundra;
        public TerrainType marsh;
        public TerrainType urban;
        public TerrainType coast;
        public TerrainType ocean;

        private Dictionary<HexCoord, TerrainType> terrain = new();
        public IReadOnlyDictionary<HexCoord, TerrainType> Terrain => terrain;

        [Range(0,1f)] public float forestChance = 0.2f;
        [Range(0,1f)] public float hillsChance = 0.15f;
        [Range(0,1f)] public float mountainsChance = 0.08f;
        [Range(0,1f)] public float desertChance = 0.15f;
        [Range(0,1f)] public float tundraChance = 0.1f;
        [Range(0,1f)] public float marshChance = 0.05f;
        [Range(0,1f)] public float urbanChance = 0.02f;

        [ContextMenu("Generate Map (Data)")]
        public void Generate()
        {
            terrain.Clear();
            var rng = new System.Random();
            foreach (var c in grid.AllCoords())
            {
                float roll = (float)rng.NextDouble();
                TerrainType t = plains;
                if (roll < urbanChance) t = urban;
                else if (roll < urbanChance + marshChance) t = marsh;
                else if (roll < urbanChance + marshChance + mountainsChance) t = mountains;
                else if (roll < urbanChance + marshChance + mountainsChance + hillsChance) t = hills;
                else if (roll < urbanChance + marshChance + mountainsChance + hillsChance + forestChance) t = forest;
                else if (roll < urbanChance + marshChance + mountainsChance + hillsChance + forestChance + desertChance) t = desert;
                else if (roll < urbanChance + marshChance + mountainsChance + hillsChance + forestChance + desertChance + tundraChance) t = tundra;
                // simple coastal/ocean banding on top/bottom rows
                if (c.r == 0 || c.r == grid.height - 1) t = coast;
                if (c.r < 0 || c.r > grid.height - 1) t = ocean;

                terrain[c] = t;
            }
            Debug.Log($"Map generated: {terrain.Count} tiles.");
        }

        public TerrainType GetTerrain(HexCoord c) => terrain.TryGetValue(c, out var t) ? t : plains;
    }
}
