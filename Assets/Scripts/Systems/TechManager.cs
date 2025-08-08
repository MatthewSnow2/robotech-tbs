using System.Collections.Generic;
using UnityEngine;
using Robotech.TBS.Data;

namespace Robotech.TBS.Systems
{
    public class TechManager : MonoBehaviour
    {
        public List<TechDefinition> techTree = new();
        public TechDefinition currentResearch;
        public int scienceProgress;

        public System.Action<TechDefinition> OnTechCompleted;

        // Persistent unlock state for prototype
        public bool hasArmoredVeritech;
        public bool hasSuperVeritech;
        public bool hasECMBonus;
        public bool hasAABonus;

        public void AddScience(int amount)
        {
            if (currentResearch == null) return;
            scienceProgress += amount;
            if (scienceProgress >= currentResearch.costScience)
            {
                CompleteCurrentTech();
            }
        }

        public void SetResearch(TechDefinition tech)
        {
            currentResearch = tech;
            scienceProgress = 0;
        }

        void CompleteCurrentTech()
        {
            // Set persistent flags
            if (currentResearch.unlockArmoredVeritech) hasArmoredVeritech = true;
            if (currentResearch.unlockSuperVeritech) hasSuperVeritech = true;
            if (currentResearch.unlockECMBonus) hasECMBonus = true;
            if (currentResearch.unlockAABonus) hasAABonus = true;

            OnTechCompleted?.Invoke(currentResearch);
            // Remove from tree and clear
            techTree.Remove(currentResearch);
            currentResearch = null;
            scienceProgress = 0;
        }
    }
}
