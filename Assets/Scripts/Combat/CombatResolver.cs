using UnityEngine;
using Robotech.TBS.Units;
using Robotech.TBS.Data;

namespace Robotech.TBS.Combat
{
    public static class CombatResolver
    {
        // Returns true if target destroyed
        public static bool ResolveAttack(Unit attacker, Unit target)
        {
            if (attacker.definition.weapons == null || attacker.definition.weapons.Length == 0)
                return false;

            int totalDamage = 0;
            foreach (var w in attacker.definition.weapons)
            {
                if (w == null) continue;
                // very basic accuracy calculation; expand with range/cover later
                float hitChance = Mathf.Clamp01(w.accuracyBase);
                int hits = 0;
                for (int i = 0; i < Mathf.Max(1, w.salvoCount); i++)
                {
                    if (Random.value <= hitChance) hits++;
                }
                totalDamage += hits * Mathf.Max(0, w.damage);
            }

            target.TakeDamage(totalDamage);
            return target == null; // will be null after Destroy in TakeDamage
        }
    }
}
