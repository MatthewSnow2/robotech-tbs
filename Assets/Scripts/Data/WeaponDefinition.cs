using UnityEngine;

namespace Robotech.TBS.Data
{
    [CreateAssetMenu(fileName = "WeaponDefinition", menuName = "Robotech/Data/Weapon Definition", order = 0)]
    public class WeaponDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string weaponId;
        public string displayName;

        [Header("Combat")]
        [Tooltip("Anti-Infantry, Anti-Armor, Anti-Air, Siege, etc.")]
        public string weaponClass;
        public int damage;              // normalized per-hit damage
        public int salvoCount = 1;      // number of projectiles per attack
        public int rangeMin = 1;        // in hexes
        public int rangeMax = 2;        // in hexes
        public float accuracyBase = 0.7f; // 0..1 base chance before modifiers

        [Header("Tags/Effects")]
        public bool antiAir;
        public bool siege;
        public bool highExplosive;
        public bool kinetic;

        [TextArea]
        public string description;
    }
}
