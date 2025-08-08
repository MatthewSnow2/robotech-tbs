using Robotech.TBS.Data;

namespace Robotech.TBS.Systems
{
    public static class MapRules
    {
        public static bool IsPassable(UnitDefinition unit, TerrainType terrain)
        {
            if (terrain == null) return true;
            if (terrain.isImpassable) return false;
            if (unit.layer == UnitLayer.Ground && terrain.isWater) return false;
            // Air can pass over anything for now
            return true;
        }
    }
}
