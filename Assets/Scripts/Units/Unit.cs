using System.Collections.Generic;
using UnityEngine;
using Robotech.TBS.Hex;
using Robotech.TBS.Data;

namespace Robotech.TBS.Units
{
    [RequireComponent(typeof(Collider))]
    public class Unit : MonoBehaviour
    {
        public UnitDefinition definition;
        public HexCoord coord;
        public int currentHP;
        public int movesLeft;

        // Tech upgrade tracking
        private List<TechDefinition> appliedTechUpgrades = new();
        private int maxHPBonus = 0;
        private int armorBonus = 0;
        private int movementBonus = 0;
        private int attackBonus = 0;

        public void Init(UnitDefinition def, HexCoord spawn, float hexSize)
        {
            definition = def;
            coord = spawn;
            currentHP = definition.maxHP;
            movesLeft = definition.movement;
            transform.position = coord.ToWorld(hexSize) + Vector3.up * 0.5f;
            name = $"Unit_{definition.displayName}_{coord.q}_{coord.r}";
        }

        public void NewTurn()
        {
            movesLeft = definition.movement + movementBonus;
        }

        public bool CanMoveTo(HexCoord target, System.Func<HexCoord, bool> passable)
        {
            if (coord.Distance(target) != 1) return false; // simple adjacent step
            if (!passable(target)) return false;
            return movesLeft > 0;
        }

        public void MoveTo(HexCoord target, float hexSize)
        {
            coord = target;
            movesLeft = Mathf.Max(0, movesLeft - 1);
            transform.position = coord.ToWorld(hexSize) + Vector3.up * 0.5f;
        }

        public void TakeDamage(int amount)
        {
            int final = Mathf.Max(0, amount - (definition.armor + armorBonus));
            currentHP -= final;
            if (currentHP <= 0)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Apply a technology upgrade to this unit. Bonuses stack and are applied immediately.
        /// Tech upgrades are only applied once per tech to prevent duplicate bonuses.
        /// </summary>
        /// <param name="tech">The technology to apply</param>
        public void ApplyTechUpgrade(TechDefinition tech)
        {
            if (tech == null) return;

            // Check if tech has already been applied
            if (appliedTechUpgrades.Contains(tech))
            {
                return;
            }

            // Apply HP bonus
            if (tech.hpBonus > 0)
            {
                maxHPBonus += tech.hpBonus;
                currentHP += tech.hpBonus;
                Debug.Log($"Unit {definition.displayName} upgraded by {tech.displayName}: +{tech.hpBonus} HP (total bonus: +{maxHPBonus})");
            }

            // Apply armor bonus
            if (tech.armorBonus > 0)
            {
                armorBonus += tech.armorBonus;
                Debug.Log($"Unit {definition.displayName} upgraded by {tech.displayName}: +{tech.armorBonus} armor (total bonus: +{armorBonus})");
            }

            // Apply movement bonus
            if (tech.movementBonus > 0)
            {
                movementBonus += tech.movementBonus;
                movesLeft += tech.movementBonus; // Apply to current turn as well
                Debug.Log($"Unit {definition.displayName} upgraded by {tech.displayName}: +{tech.movementBonus} movement (total bonus: +{movementBonus})");
            }

            // Apply attack bonus
            if (tech.attackBonus > 0)
            {
                attackBonus += tech.attackBonus;
                Debug.Log($"Unit {definition.displayName} upgraded by {tech.displayName}: +{tech.attackBonus} attack (total bonus: +{attackBonus})");
            }

            // Add tech to applied upgrades list
            appliedTechUpgrades.Add(tech);
        }

        /// <summary>
        /// Check if a specific tech upgrade has already been applied to this unit.
        /// </summary>
        /// <param name="tech">The technology to check</param>
        /// <returns>True if the tech upgrade has been applied, false otherwise</returns>
        public bool HasTechUpgrade(TechDefinition tech)
        {
            return appliedTechUpgrades.Contains(tech);
        }

        /// <summary>
        /// Get the total maximum HP including base and all tech bonuses.
        /// </summary>
        public int GetMaxHP()
        {
            return definition.maxHP + maxHPBonus;
        }

        /// <summary>
        /// Get the total armor including base and all tech bonuses.
        /// </summary>
        public int GetArmor()
        {
            return definition.armor + armorBonus;
        }

        /// <summary>
        /// Get the total movement including base and all tech bonuses.
        /// </summary>
        public int GetMovement()
        {
            return definition.movement + movementBonus;
        }

        /// <summary>
        /// Get the attack bonus from tech upgrades.
        /// </summary>
        public int GetAttackBonus()
        {
            return attackBonus;
        }
    }
}
