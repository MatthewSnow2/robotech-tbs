using System.Collections.Generic;
using UnityEngine;
using Robotech.TBS.Cities;
using Robotech.TBS.Hex;
using Robotech.TBS.Map;
using Robotech.TBS.Data;

namespace Robotech.TBS.Systems
{
    public class CityManager : MonoBehaviour
    {
        public HexGrid grid;
        public MapGenerator mapGen;

        private readonly List<City> cities = new();
        private readonly Dictionary<(int,int), City> ownership = new();

        public IReadOnlyList<City> Cities => cities;
        public bool IsOwned(HexCoord c) => ownership.ContainsKey((c.q, c.r));
        public City GetOwner(HexCoord c) => ownership.TryGetValue((c.q, c.r), out var city) ? city : null;

        void Awake()
        {
            if (grid == null) grid = FindObjectOfType<HexGrid>();
            if (mapGen == null) mapGen = FindObjectOfType<MapGenerator>();
        }

        public City FoundCity(string name, HexCoord c, float hexSize, Faction faction)
        {
            if (!grid.InBounds(c)) return null;
            if (IsOwned(c)) return null;
            if (IsTooCloseToAnyCity(c, 3)) return null;

            var go = new GameObject(name);
            var city = go.AddComponent<City>();
            city.Init(name, c, hexSize, faction);
            cities.Add(city);

            Claim(c, city);
            // initial territory radius 1
            foreach (var n in grid.Neighbors(c)) Claim(n, city);
            return city;
        }

        public bool IsTooCloseToAnyCity(HexCoord c, int minDistance)
        {
            foreach (var city in cities)
            {
                if (city.coord.Distance(c) < minDistance) return true;
            }
            return false;
        }

        public void GrowBorders(int tilesPerTurn = 1)
        {
            // very simple: for each city, claim up to N neighboring tiles unowned
            foreach (var city in cities)
            {
                int claimed = 0;
                var frontier = new List<HexCoord>();
                foreach (var owned in ownership)
                {
                    if (owned.Value != city) continue;
                    foreach (var n in grid.Neighbors(new HexCoord(owned.Key.Item1, owned.Key.Item2)))
                    {
                        if (!IsOwned(n)) frontier.Add(n);
                    }
                }
                foreach (var f in frontier)
                {
                    Claim(f, city);
                    claimed++;
                    if (claimed >= tilesPerTurn) break;
                }
            }
        }

        public void ApplyCityYields(ResourceManager res)
        {
            foreach (var city in cities)
            {
                var yields = city.GetYields();
                // base yields
                int baseProd = 2;
                int baseSci = 1;
                int baseInfl = 1;
                int prodPoints = baseProd + yields.prod;
                // Advance city production with production points
                city.AdvanceProduction(prodPoints, grid, mapGen);
                // Add non-production yields to global resources (science/influence)
                res.AddTurnYields(0, baseSci + yields.sci, baseInfl + yields.infl);
            }
        }

        private void Claim(HexCoord c, City city)
        {
            ownership[(c.q, c.r)] = city;
        }
    }
}
