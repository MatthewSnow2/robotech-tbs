using UnityEngine;

namespace Robotech.TBS.Data
{
    public enum UnitLayer { Ground, Air }
    public enum Faction { RDF, Zentradi }

    [CreateAssetMenu(fileName = "UnitDefinition", menuName = "Robotech/Data/Unit Definition", order = 1)]
    public class UnitDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string unitId;
        public string displayName;
        public Faction faction;
        public UnitLayer layer;

        [Header("Stats (normalized)")]
        public int maxHP = 100;
        public int armor = 0;           // flat damage reduction bands
        public int movement = 3;        // tiles per turn
        public int vision = 2;          // tiles

        [Header("Combat")]
        public WeaponDefinition[] weapons;
        public bool antiAirSpecialist;  // receives bonuses vs air
        public bool siegeSpecialist;    // bonuses vs cities/structures

        [Header("Abilities")]
        public bool canOverwatch;
        public bool hasECM;
        public bool hasJumpJets;        // for power armor style units
        public bool canTransform;       // Veritech mode switch
        public bool canFoundCity;       // Settler-like ability

        [Header("Economy")]
        public int buildCostMaterials = 20;
        public int upkeepProtoculture = 0;

        [TextArea]
        public string description;
    }
}
