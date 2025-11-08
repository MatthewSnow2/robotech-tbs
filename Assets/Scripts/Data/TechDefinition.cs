using UnityEngine;

namespace Robotech.TBS.Data
{
    public enum TechGeneration
    {
        Gen0,  // Initial/Starting era
        Gen1,  // Early game
        Gen2,  // Mid game
        Gen3   // Late game
    }

    public enum TechCategory
    {
        Military,
        Science,
        Infrastructure,
        Special
    }

    [CreateAssetMenu(fileName = "TechDefinition", menuName = "Robotech/Data/Tech Definition", order = 10)]
    public class TechDefinition : ScriptableObject
    {
        public string techId;
        public string displayName;
        public int costScience = 50;
        [TextArea] public string description;

        // Simple unlock flags for prototype
        public bool unlockArmoredVeritech;
        public bool unlockSuperVeritech;
        public bool unlockECMBonus;
        public bool unlockAABonus;
    }
}
