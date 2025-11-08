using UnityEngine;
using Robotech.TBS.Data;

namespace Robotech.TBS.Systems
{
    public class ResourceManager : MonoBehaviour
    {
        public int protoculture = 10;
        public int materials = 20;
        public int credits = 50;
        public int science = 0;

        // Tech bonus fields
        private float protocultureTechBonus = 0f;
        private float scienceTechBonus = 0f;
        private float productionTechBonus = 0f;

        // Tech bonus getters
        public float GetProtocultureBonus() { return protocultureTechBonus; }
        public float GetScienceBonus() { return scienceTechBonus; }
        public float GetProductionBonus() { return productionTechBonus; }

        // Tech bonus setters
        public void SetProtocultureBonus(float amount) { protocultureTechBonus = amount; }
        public void SetScienceBonus(float amount) { scienceTechBonus = amount; }
        public void SetProductionBonus(float amount) { productionTechBonus = amount; }

        private void OnEnable()
        {
            // Subscribe to TechManager events
            var techManager = FindObjectOfType<TechManager>();
            if (techManager != null)
            {
                techManager.OnTechCompleted += OnTechCompleted;
            }
        }

        private void OnDisable()
        {
            // Unsubscribe from TechManager events to prevent memory leaks
            var techManager = FindObjectOfType<TechManager>();
            if (techManager != null)
            {
                techManager.OnTechCompleted -= OnTechCompleted;
            }
        }

        private void OnTechCompleted(TechDefinition tech)
        {
            ApplyTechBonus(tech);
        }

        public void ApplyTechBonus(TechDefinition tech)
        {
            if (tech == null) return;

            // Apply protoculture bonus
            if (tech.protoculturePerTurn > 0)
            {
                protocultureTechBonus += tech.protoculturePerTurn;
                Debug.Log($"Tech bonus applied: +{tech.protoculturePerTurn} protoculture/turn from {tech.displayName}");
            }

            // Apply science bonus
            if (tech.sciencePerTurn > 0)
            {
                scienceTechBonus += tech.sciencePerTurn;
                Debug.Log($"Tech bonus applied: +{tech.sciencePerTurn} science/turn from {tech.displayName}");
            }

            // Apply production bonus
            if (tech.productionPerTurn > 0)
            {
                productionTechBonus += tech.productionPerTurn;
                Debug.Log($"Tech bonus applied: +{tech.productionPerTurn} production/turn from {tech.displayName}");
            }
        }

        public float CalculateTotalProtoculturePerTurn()
        {
            // TODO: Add base protoculture calculation from cities when implemented
            return protocultureTechBonus;
        }

        public float CalculateTotalSciencePerTurn()
        {
            // TODO: Add base science calculation from cities when implemented
            return scienceTechBonus;
        }

        public float CalculateTotalProductionPerTurn()
        {
            // TODO: Add base production calculation from cities when implemented
            return productionTechBonus;
        }

        public void ApplyUpkeep(int protoCost)
        {
            protoculture -= protoCost;
            if (protoculture < 0) protoculture = 0; // TODO: deficit penalties
        }

        public void AddTurnYields(int prod, int sci, int infl)
        {
            materials += prod;
            science += sci;
            // influence would drive border growth (not yet implemented)
        }

        /// <summary>
        /// Applies all per-turn income including base yields and tech bonuses.
        /// This is the new unified method for applying turn income.
        /// </summary>
        public void ApplyIncome()
        {
            // Apply tech bonuses to resources
            // Note: Base yields from cities are currently applied via AddTurnYields in CityManager
            // In the future, this method will handle all income calculation

            float protocultureIncome = CalculateTotalProtoculturePerTurn();
            float scienceIncome = CalculateTotalSciencePerTurn();
            float productionIncome = CalculateTotalProductionPerTurn();

            protoculture += Mathf.RoundToInt(protocultureIncome);
            science += Mathf.RoundToInt(scienceIncome);
            materials += Mathf.RoundToInt(productionIncome);

            if (protocultureIncome > 0 || scienceIncome > 0 || productionIncome > 0)
            {
                Debug.Log($"Tech bonuses applied: +{protocultureIncome} protoculture, +{scienceIncome} science, +{productionIncome} production");
            }
        }
    }
}
