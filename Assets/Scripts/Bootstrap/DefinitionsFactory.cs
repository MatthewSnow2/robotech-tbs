using UnityEngine;
using Robotech.TBS.Data;

namespace Robotech.TBS.Bootstrap
{
    public static class DefinitionsFactory
    {
        public static TerrainType CreateTerrain(string id, string name, int move, int def, bool water=false, bool impass=false, bool urban=false, bool elev=false)
        {
            var t = ScriptableObject.CreateInstance<TerrainType>();
            t.terrainId = id; t.displayName = name;
            t.movementCost = move; t.defenseBonus = def;
            t.isWater = water; t.isImpassable = impass; t.isUrban = urban; t.providesElevation = elev;
            return t;
        }

        public static WeaponDefinition CreateWeapon(string id, string name, string wclass, int dmg, int salvo, int rmin, int rmax, float acc, bool aa=false, bool siege=false)
        {
            var w = ScriptableObject.CreateInstance<WeaponDefinition>();
            w.weaponId = id; w.displayName = name; w.weaponClass = wclass;
            w.damage = dmg; w.salvoCount = salvo; w.rangeMin = rmin; w.rangeMax = rmax; w.accuracyBase = acc;
            w.antiAir = aa; w.siege = siege;
            return w;
        }

        public static UnitDefinition CreateUnit(string id, string name, Faction faction, UnitLayer layer, int hp, int armor, int move, int vision, WeaponDefinition[] weapons, bool canTransform=false, bool ecm=false, bool jj=false, bool canFoundCity=false)
        {
            var u = ScriptableObject.CreateInstance<UnitDefinition>();
            u.unitId = id; u.displayName = name; u.faction = faction; u.layer = layer;
            u.maxHP = hp; u.armor = armor; u.movement = move; u.vision = vision;
            u.weapons = weapons; u.canTransform = canTransform; u.hasECM = ecm; u.hasJumpJets = jj; u.canFoundCity = canFoundCity;
            return u;
        }
    }
}
