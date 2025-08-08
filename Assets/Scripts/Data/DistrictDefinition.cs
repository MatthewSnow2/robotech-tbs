using UnityEngine;

namespace Robotech.TBS.Data
{
    public enum DistrictType { Factory, Lab, Outpost }

    [CreateAssetMenu(fileName = "DistrictDefinition", menuName = "Robotech/Data/District Definition", order = 3)]
    public class DistrictDefinition : ScriptableObject
    {
        [Header("Identity")]
        public string districtId;
        public string displayName;
        public DistrictType type;

        [Header("Yields/Effects")]
        public int bonusProduction;
        public int bonusScience;
        public int bonusInfluence; // border growth
        public int defenseBonus;   // outpost/city defense contribution
        public int visionBonus;    // extra tiles of vision

        [Header("Upkeep")]
        public int protocultureUpkeep; // per turn

        [TextArea]
        public string description;
    }
}
