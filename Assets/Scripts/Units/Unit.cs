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
            movesLeft = definition.movement;
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
            int final = Mathf.Max(0, amount - definition.armor);
            currentHP -= final;
            if (currentHP <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
