using UnityEngine;
using Robotech.TBS.Hex;
using Robotech.TBS.Map;
using Robotech.TBS.Fog;
using Robotech.TBS.Systems;
using Robotech.TBS.Bootstrap;
using Robotech.TBS.Data;
using Robotech.TBS.Units;
using Robotech.TBS.Core;
using Robotech.TBS.Rendering;

namespace Robotech.TBS.Bootstrap
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Core Objects")]
        public HexGrid grid;
        public MapGenerator mapGen;
        public FogOfWarSystem fog;
        public ResourceManager resources;
        public TurnManager turnManager;
        public CityManager cityManager;
        public TechManager techManager;

        [Header("Runtime Unit Defs (temp)")]
        public UnitDefinition rdfVF1A;
        public UnitDefinition zentTacticalPod;
        public UnitDefinition rdfSettler;
        public UnitDefinition zentSettler;
        public UnitDefinition rdfArmoredVeritech;
        public UnitDefinition rdfSuperVeritech;
        public UnitDefinition zentOfficerPod;

        void Awake()
        {
            // Ensure components exist
            if (grid == null) grid = gameObject.AddComponent<HexGrid>();
            if (mapGen == null) mapGen = gameObject.AddComponent<MapGenerator>();
            if (fog == null) fog = gameObject.AddComponent<FogOfWarSystem>();
            if (resources == null) resources = gameObject.AddComponent<ResourceManager>();
            if (turnManager == null) turnManager = gameObject.AddComponent<TurnManager>();
            if (cityManager == null) cityManager = gameObject.AddComponent<CityManager>();
            if (techManager == null) techManager = gameObject.AddComponent<TechManager>();
            // Auto-add debug renderer for quick visualization
            var debugRenderer = gameObject.AddComponent<HexDebugRenderer>();
            debugRenderer.grid = grid;
            debugRenderer.mapGen = mapGen;

            // Create minimal terrain ScriptableObjects for generator
            var plains   = DefinitionsFactory.CreateTerrain("plains","Plains",1,0);
            var forest   = DefinitionsFactory.CreateTerrain("forest","Forest",2,1);
            var hills    = DefinitionsFactory.CreateTerrain("hills","Hills",2,1,false,false,false,true);
            var mountains= DefinitionsFactory.CreateTerrain("mountains","Mountains",99,3,false,true,false,true);
            var desert   = DefinitionsFactory.CreateTerrain("desert","Desert",1,0);
            var tundra   = DefinitionsFactory.CreateTerrain("tundra","Tundra",1,0);
            var marsh    = DefinitionsFactory.CreateTerrain("marsh","Marsh",2,0);
            var urban    = DefinitionsFactory.CreateTerrain("urban","Urban/Ruins",1,1,false,false,true,false);
            var coast    = DefinitionsFactory.CreateTerrain("coast","Coast",1,0,true,false,false,false);
            var ocean    = DefinitionsFactory.CreateTerrain("ocean","Ocean",1,0,true,true,false,false);

            mapGen.grid = grid;
            mapGen.plains = plains; mapGen.forest=forest; mapGen.hills=hills; mapGen.mountains=mountains;
            mapGen.desert=desert; mapGen.tundra=tundra; mapGen.marsh=marsh; mapGen.urban=urban; mapGen.coast=coast; mapGen.ocean=ocean;
            mapGen.Generate();

            fog.grid = grid;

            // Create temporary weapon defs
            var gun = DefinitionsFactory.CreateWeapon("vf1a_gun","GU-11 Gun Pod","Kinetic",12,1,1,1,0.75f,false,false);
            var podGun = DefinitionsFactory.CreateWeapon("pod_gun","Battlepod Cannons","Kinetic",10,1,1,1,0.7f,false,false);
            var officerGun = DefinitionsFactory.CreateWeapon("officer_gun","Officer Pod Cannons","Kinetic",14,1,1,1,0.72f,false,false);

            // Create unit defs
            rdfVF1A = DefinitionsFactory.CreateUnit("vf1a","VF-1A Veritech", Faction.RDF, UnitLayer.Air, 100, 1, 4, 3, new[] { gun }, canTransform:true);
            zentTacticalPod = DefinitionsFactory.CreateUnit("tpod","Tactical Battlepod", Faction.Zentradi, UnitLayer.Ground, 110, 1, 3, 2, new[] { podGun });
            zentOfficerPod = DefinitionsFactory.CreateUnit("opod","Officer Battlepod", Faction.Zentradi, UnitLayer.Ground, 140, 2, 3, 2, new[] { officerGun });
            rdfSettler = DefinitionsFactory.CreateUnit("rdf_settler","RDF Engineer Corps", Faction.RDF, UnitLayer.Ground, 60, 0, 2, 2, new WeaponDefinition[] { }, false, false, false, true);
            zentSettler = DefinitionsFactory.CreateUnit("zent_colonizer","Zentradi Outpost Crew", Faction.Zentradi, UnitLayer.Ground, 60, 0, 2, 2, new WeaponDefinition[] { }, false, false, false, true);
            // Advanced RDF units unlocked by tech
            var heavyGun = DefinitionsFactory.CreateWeapon("arm_gun","Enhanced GU-11","Kinetic",16,1,1,1,0.75f,false,false);
            rdfArmoredVeritech = DefinitionsFactory.CreateUnit("vf1a_arm","Armored Veritech", Faction.RDF, UnitLayer.Air, 140, 2, 3, 3, new[] { heavyGun }, canTransform:true, ecm:false, jj:false);
            var superGun = DefinitionsFactory.CreateWeapon("super_gun","Super Veritech Cannons","Kinetic",18,1,1,1,0.78f,false,false);
            rdfSuperVeritech = DefinitionsFactory.CreateUnit("vf1a_sup","Super Veritech", Faction.RDF, UnitLayer.Air, 150, 2, 4, 3, new[] { superGun }, canTransform:true, ecm:true, jj:false);

            // Spawn two units
            var startA = new HexCoord(5, 5);
            var startB = new HexCoord(grid.width - 6, grid.height - 6);
            var uA = UnitFactory.SpawnUnit("RDF", rdfVF1A, startA, grid.hexSize);
            var uB = UnitFactory.SpawnUnit("ZENT", zentTacticalPod, startB, grid.hexSize);
            // Spawn settlers near starts if passable
            var settlerPosA = new HexCoord(startA.q + 1, startA.r);
            var settlerPosB = new HexCoord(startB.q - 1, startB.r);
            UnitFactory.SpawnUnit("RDF", rdfSettler, settlerPosA, grid.hexSize);
            UnitFactory.SpawnUnit("ZENT", zentSettler, settlerPosB, grid.hexSize);

            // Initial vision
            fog.ClearVisibility();
            fog.RevealFrom(startA, rdfVF1A.vision);

            // Hook turns
            TurnManager.OnTurnStarted += OnTurnStarted;
            // Minimal tech setup
            var techArmored = ScriptableObject.CreateInstance<TechDefinition>();
            techArmored.techId = "armored_veritech"; techArmored.displayName = "Armored Veritech"; techArmored.costScience = 60; techArmored.unlockArmoredVeritech = true;
            var techSuper = ScriptableObject.CreateInstance<TechDefinition>();
            techSuper.techId = "super_veritech"; techSuper.displayName = "Super Veritech"; techSuper.costScience = 90; techSuper.unlockSuperVeritech = true;
            techManager.techTree.Add(techArmored);
            techManager.techTree.Add(techSuper);
            techManager.SetResearch(techArmored);
            techManager.OnTechCompleted += t => Debug.Log($"Tech completed: {t.displayName}");
        }

        private void OnDestroy()
        {
            TurnManager.OnTurnStarted -= OnTurnStarted;
        }

        private void OnTurnStarted(int turn)
        {
            // Reset unit moves and refresh visibility for the simple prototype
            foreach (var unit in FindObjectsOfType<Unit>())
            {
                unit.NewTurn();
            }
            // Recompute visibility from all RDF units for demo
            fog.ClearVisibility();
            foreach (var unit in FindObjectsOfType<Unit>())
            {
                if (unit.definition.faction == Faction.RDF)
                    fog.RevealFrom(unit.coord, unit.definition.vision);
            }

            // City yields, border growth, and research progress
            if (cityManager != null && resources != null)
            {
                int sciBefore = resources.science;
                cityManager.GrowBorders(1);
                cityManager.ApplyCityYields(resources);
                int sciDelta = resources.science - sciBefore;
                if (techManager != null && sciDelta > 0) techManager.AddScience(sciDelta);
            }

            // Apply unit upkeep (protoculture)
            if (resources != null)
            {
                int totalUpkeep = 0;
                foreach (var unit in FindObjectsOfType<Unit>())
                {
                    if (unit != null && unit.definition != null)
                        totalUpkeep += unit.definition.upkeepProtoculture;
                }
                if (totalUpkeep > 0)
                    resources.ApplyUpkeep(totalUpkeep);
            }
        }
    }
}
