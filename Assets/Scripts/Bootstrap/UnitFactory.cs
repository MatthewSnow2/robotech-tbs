using UnityEngine;
using Robotech.TBS.Units;
using Robotech.TBS.Data;
using Robotech.TBS.Hex;

namespace Robotech.TBS.Bootstrap
{
    public static class UnitFactory
    {
        public static Unit SpawnUnit(string namePrefix, UnitDefinition def, HexCoord coord, float hexSize)
        {
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

            unit.Init(def, coord, hexSize);
            return unit;
        }
    }
}
