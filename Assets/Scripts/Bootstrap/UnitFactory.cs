using UnityEngine;
using Robotech.TBS.Units;
using Robotech.TBS.Data;
using Robotech.TBS.Hex;
using Robotech.TBS.Systems;

namespace Robotech.TBS.Bootstrap
{
    public static class UnitFactory
    {
        /// <summary>
        /// Spawns a unit at the specified location. Validates tech requirements and applies all relevant tech bonuses.
        /// </summary>
        /// <param name="namePrefix">Prefix for the GameObject name</param>
        /// <param name="def">Unit definition to spawn</param>
        /// <param name="coord">Hex coordinate to spawn at</param>
        /// <param name="hexSize">Size of hexes for world position calculation</param>
        /// <param name="bypassTechCheck">If true, skips tech requirement validation (for testing/admin spawning)</param>
        /// <returns>The spawned unit, or null if tech requirements are not met</returns>
        public static Unit SpawnUnit(string namePrefix, UnitDefinition def, HexCoord coord, float hexSize, bool bypassTechCheck = false)
        {
            // Check if unit requires a tech
            if (!bypassTechCheck && def.requiredTech != null)
            {
                var techManager = Object.FindObjectOfType<TechManager>();
                if (techManager == null || !techManager.researchedTechs.Contains(def.requiredTech))
                {
                    Debug.LogError($"Cannot produce {def.displayName} - requires {def.requiredTech.displayName}");
                    return null;
                }
            }

            // Create the unit GameObject
            var go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            go.name = $"{namePrefix}_{def.displayName}";
            var unit = go.AddComponent<Unit>();

            // Ensure collider exists (Primitive adds CapsuleCollider)
            var col = go.GetComponent<Collider>();
            if (col != null) col.isTrigger = false;

            // Simple faction color
            var mr = go.GetComponent<MeshRenderer>();
            if (mr != null)
            {
                mr.material = new Material(Shader.Find("Standard"));
                mr.material.color = def.faction == Faction.RDF ? new Color(0.2f,0.6f,1f) : new Color(0.9f,0.3f,0.3f);
            }

            // Initialize the unit
            unit.Init(def, coord, hexSize);

            // Apply any applicable tech upgrades
            var techManager2 = Object.FindObjectOfType<TechManager>();
            if (techManager2 != null)
            {
                foreach (var tech in techManager2.researchedTechs)
                {
                    // Apply any tech that has unit stat bonuses
                    if (tech.hpBonus > 0 || tech.armorBonus > 0 || tech.movementBonus > 0 || tech.attackBonus > 0)
                    {
                        unit.ApplyTechUpgrade(tech);
                    }
                }
            }

            return unit;
        }
    }
}
