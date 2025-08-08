using System;
using System.Collections.Generic;
using UnityEngine;
using Robotech.TBS.Hex;
using Robotech.TBS.Units;
using Robotech.TBS.Map;
using Robotech.TBS.Systems;
using Robotech.TBS.Data;
using Robotech.TBS.Combat;

namespace Robotech.TBS.Inputs
{
    public class SelectionController : MonoBehaviour
    {
        public HexGrid grid;
        public MapGenerator mapGen;
        public CityManager cityManager;

        public Unit SelectedUnit { get; private set; }
        public HexCoord HoverHex { get; private set; }
        public HashSet<HexCoord> ReachableHexes { get; private set; } = new();
        public HashSet<HexCoord> AttackableHexes { get; private set; } = new();
        public bool AttackMode { get; private set; } = false;

        public static event Action<Unit> OnUnitSelected;
        public static event Action OnSelectionCleared;

        Camera cam;
        Plane ground = new Plane(Vector3.up, Vector3.zero);

        void Awake()
        {
            cam = Camera.main;
            if (grid == null) grid = FindObjectOfType<HexGrid>();
            if (mapGen == null) mapGen = FindObjectOfType<MapGenerator>();
            if (cityManager == null) cityManager = FindObjectOfType<CityManager>();
        }

        void Update()
        {
            UpdateHover();
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                HandleLeftClick();
            }
            if (UnityEngine.Input.GetMouseButtonDown(1))
            {
                ClearSelection();
            }
            // Hotkey: Found City (B)
            if (UnityEngine.Input.GetKeyDown(KeyCode.B))
            {
                TryFoundCity();
            }
        }

        void UpdateHover()
        {
            var ray = cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
            float enter;
            if (ground.Raycast(ray, out enter))
            {
                var hit = ray.origin + ray.direction * enter;
                HoverHex = HexMath.AxialFromWorld(hit, grid.hexSize);
            }
        }

        void HandleLeftClick()
        {
            // Try raycast against colliders to select a unit first
            var ray = cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 1000f))
            {
                var unit = hit.collider.GetComponentInParent<Unit>();
                if (unit != null)
                {
                    SelectUnit(unit);
                    return;
                }
            }

            if (SelectedUnit != null)
            {
                if (AttackMode)
                {
                    // Attack if target hex has enemy and is attackable
                    if (AttackableHexes.Contains(HoverHex))
                    {
                        var target = FindUnitAt(HoverHex);
                        if (target != null && target.definition.faction != SelectedUnit.definition.faction)
                        {
                            CombatResolver.ResolveAttack(SelectedUnit, target);
                            // After attack, recompute ranges
                            RecomputeRanges();
                        }
                    }
                }
                else
                {
                    // Move if reachable and passable
                    if (ReachableHexes.Contains(HoverHex) && IsPassable(SelectedUnit.definition, HoverHex))
                    {
                        SelectedUnit.MoveTo(HoverHex, grid.hexSize);
                        RecomputeRanges();
                    }
                }
            }
        }

        bool IsPassable(UnitDefinition def, HexCoord c)
        {
            var t = mapGen.GetTerrain(c);
            return MapRules.IsPassable(def, t);
        }

        public void SelectUnit(Unit u)
        {
            SelectedUnit = u;
            AttackMode = false;
            RecomputeRanges();
            OnUnitSelected?.Invoke(u);
        }

        public void ClearSelection()
        {
            SelectedUnit = null;
            ReachableHexes.Clear();
            AttackableHexes.Clear();
            AttackMode = false;
            OnSelectionCleared?.Invoke();
        }

        public void RecomputeRanges()
        {
            ReachableHexes.Clear();
            AttackableHexes.Clear();
            if (SelectedUnit == null) return;

            // Simple 1-step movement for now
            foreach (var n in grid.Neighbors(SelectedUnit.coord))
            {
                if (IsPassable(SelectedUnit.definition, n))
                    ReachableHexes.Add(n);
            }
            // Simple attack: adjacent enemies only for now
            var all = FindObjectsOfType<Unit>();
            foreach (var other in all)
            {
                if (other == SelectedUnit) continue;
                if (other.definition.faction == SelectedUnit.definition.faction) continue;
                if (SelectedUnit.coord.Distance(other.coord) == 1)
                {
                    AttackableHexes.Add(other.coord);
                }
            }
        }

        public void SetAttackMode(bool enabled)
        {
            AttackMode = enabled;
        }

        private Unit FindUnitAt(HexCoord c)
        {
            foreach (var u in FindObjectsOfType<Unit>())
            {
                if (u.coord.q == c.q && u.coord.r == c.r)
                    return u;
            }
            return null;
        }

        public bool CanFoundCityHere()
        {
            if (SelectedUnit == null || !SelectedUnit.definition.canFoundCity) return false;
            if (!grid.InBounds(SelectedUnit.coord)) return false;
            if (cityManager == null) return false;
            if (cityManager.IsOwned(SelectedUnit.coord)) return false;
            if (cityManager.IsTooCloseToAnyCity(SelectedUnit.coord, 3)) return false;
            var t = mapGen.GetTerrain(SelectedUnit.coord);
            // disallow on water/impassable
            if (t.isWater || t.isImpassable) return false;
            return true;
        }

        public bool TryFoundCity()
        {
            if (!CanFoundCityHere()) return false;
            var count = cityManager.Cities.Count + 1;
            var city = cityManager.FoundCity($"City {count}", SelectedUnit.coord, grid.hexSize, SelectedUnit.definition.faction);
            if (city != null)
            {
                Destroy(SelectedUnit.gameObject);
                ClearSelection();
                return true;
            }
            return false;
        }
    }
}
