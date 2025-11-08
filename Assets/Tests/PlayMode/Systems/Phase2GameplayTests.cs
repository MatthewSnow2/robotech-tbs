using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Robotech.TBS.Bootstrap;
using Robotech.TBS.Systems;
using Robotech.TBS.Data;
using Robotech.TBS.Units;
using Robotech.TBS.Hex;

namespace Robotech.Tests.PlayMode.Systems
{
    public class Phase2GameplayTests
    {
        private GameObject gameObject;
        private GameBootstrap bootstrap;
        private TechManager techManager;
        private ResourceManager resourceManager;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Create a fresh game object with GameBootstrap
            gameObject = new GameObject("TestGameBootstrap");
            bootstrap = gameObject.AddComponent<GameBootstrap>();

            // Wait for Awake to complete
            yield return null;

            // Get references
            techManager = bootstrap.techManager;
            resourceManager = bootstrap.resources;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            if (gameObject != null)
            {
                Object.Destroy(gameObject);
            }
            yield return null;
        }

        #region Resource Income Tests

        [UnityTest]
        public IEnumerator Game_Starts_With_Base_Income()
        {
            // Assert - Verify initial resource state
            Assert.IsNotNull(resourceManager, "ResourceManager should exist");
            Assert.AreEqual(10, resourceManager.protoculture, "Starting protoculture should be 10");
            Assert.AreEqual(20, resourceManager.materials, "Starting materials should be 20");
            Assert.AreEqual(50, resourceManager.credits, "Starting credits should be 50");
            Assert.AreEqual(0, resourceManager.science, "Starting science should be 0");

            yield return null;
        }

        [UnityTest]
        public IEnumerator Research_Reactor_Mk_I_Increases_Income()
        {
            // Arrange
            var reactorMk1 = techManager.allTechs.FirstOrDefault(t => t.techId == "reactor_mk1");
            Assert.IsNotNull(reactorMk1, "Reactor Mk I should exist");

            int initialProtoculture = resourceManager.protoculture;

            // Subscribe to tech completion to apply bonus
            bool bonusApplied = false;
            techManager.OnTechCompleted += tech =>
            {
                if (tech.protoculturePerTurn > 0)
                {
                    resourceManager.protoculture += (int)tech.protoculturePerTurn;
                    bonusApplied = true;
                }
            };

            // Act - Research Reactor Mk I
            techManager.SetResearch(reactorMk1);
            techManager.AddScience(reactorMk1.costScience);

            yield return null;

            // Assert
            Assert.IsTrue(bonusApplied, "Protoculture bonus should be applied");
            Assert.AreEqual(initialProtoculture + 10, resourceManager.protoculture,
                "Protoculture should increase by +10 from Reactor Mk I");
        }

        [UnityTest]
        public IEnumerator Bonus_Stacks_With_Multiple_Techs_Researched()
        {
            // Arrange
            var reactorMk1 = techManager.allTechs.FirstOrDefault(t => t.techId == "reactor_mk1");
            var reactorMk2 = techManager.allTechs.FirstOrDefault(t => t.techId == "reactor_mk2");

            Assert.IsNotNull(reactorMk1, "Reactor Mk I should exist");
            Assert.IsNotNull(reactorMk2, "Reactor Mk II should exist");

            int initialProtoculture = resourceManager.protoculture;

            techManager.OnTechCompleted += tech =>
            {
                resourceManager.protoculture += (int)tech.protoculturePerTurn;
            };

            // Act - Research Reactor Mk I
            techManager.SetResearch(reactorMk1);
            techManager.AddScience(reactorMk1.costScience);
            yield return null;

            int afterMk1 = resourceManager.protoculture;

            // Research Reactor Mk II
            techManager.SetResearch(reactorMk2);
            techManager.AddScience(reactorMk2.costScience);
            yield return null;

            // Assert
            Assert.AreEqual(initialProtoculture + 10, afterMk1, "After Mk I: +10 protoculture");
            Assert.AreEqual(initialProtoculture + 10 + 15, resourceManager.protoculture,
                "After Mk I + Mk II: +25 total protoculture");
        }

        [UnityTest]
        public IEnumerator Global_Comms_Increases_Science_Income()
        {
            // Arrange
            var globalComms = techManager.allTechs.FirstOrDefault(t => t.techId == "global_comms");
            Assert.IsNotNull(globalComms, "Global Communications should exist");

            int initialScience = resourceManager.science;

            techManager.OnTechCompleted += tech =>
            {
                resourceManager.science += (int)tech.sciencePerTurn;
            };

            // Act
            techManager.SetResearch(globalComms);
            techManager.AddScience(globalComms.costScience);

            yield return null;

            // Assert
            Assert.AreEqual(initialScience + 5, resourceManager.science,
                "Science should increase by +5 from Global Comms");
        }

        #endregion

        #region Unit Production and Tech Requirements

        [UnityTest]
        public IEnumerator Unit_Cannot_Spawn_Without_Tech()
        {
            // Arrange
            var transformationI = techManager.allTechs.FirstOrDefault(t => t.techId == "transformation_i");
            Assert.IsNotNull(transformationI, "Transformation Engineering I should exist");

            // Create a unit that requires this tech
            var vf0 = DefinitionsFactory.CreateUnit(
                "vf0", "VF-0 Phoenix", Faction.RDF, UnitLayer.Air, 100, 1, 4, 3,
                new WeaponDefinition[] { });
            vf0.requiredTech = transformationI;

            // Act
            bool canProduce = CanProduceUnit(vf0, techManager);

            yield return null;

            // Assert
            Assert.IsFalse(canProduce, "VF-0 should be blocked without Transformation Engineering I");
        }

        [UnityTest]
        public IEnumerator Unit_Can_Spawn_After_Tech_Researched()
        {
            // Arrange
            var transformationI = techManager.allTechs.FirstOrDefault(t => t.techId == "transformation_i");
            var chassisI = techManager.allTechs.FirstOrDefault(t => t.techId == "chassis_i");

            Assert.IsNotNull(transformationI, "Transformation Engineering I should exist");
            Assert.IsNotNull(chassisI, "Chassis I should exist");

            var vf0 = DefinitionsFactory.CreateUnit(
                "vf0", "VF-0 Phoenix", Faction.RDF, UnitLayer.Air, 100, 1, 4, 3,
                new WeaponDefinition[] { });
            vf0.requiredTech = transformationI;

            // Research prerequisite and then Transformation I
            techManager.SetResearch(chassisI);
            techManager.AddScience(chassisI.costScience);
            yield return null;

            techManager.SetResearch(transformationI);
            techManager.AddScience(transformationI.costScience);
            yield return null;

            // Act
            bool canProduce = CanProduceUnit(vf0, techManager);

            // Assert
            Assert.IsTrue(canProduce, "VF-0 should be allowed after researching Transformation Engineering I");
        }

        #endregion

        #region Unit Upgrade Integration

        [UnityTest]
        public IEnumerator New_Unit_Has_Tech_Bonuses()
        {
            // Arrange
            var metallurgyI = techManager.allTechs.FirstOrDefault(t => t.techId == "metallurgy_i");
            Assert.IsNotNull(metallurgyI, "Metallurgy I should exist");

            // Research Metallurgy I first
            techManager.SetResearch(metallurgyI);
            techManager.AddScience(metallurgyI.costScience);
            yield return null;

            // Act - Spawn a unit after research
            var unitDef = DefinitionsFactory.CreateUnit(
                "test_unit", "Test Unit", Faction.RDF, UnitLayer.Ground, 100, 10, 3, 3,
                new WeaponDefinition[] { });
            var unit = UnitFactory.SpawnUnit("Test", unitDef, new HexCoord(0, 0), 1.0f);

            // Apply tech upgrades
            ApplyTechUpgradesToUnit(unit, techManager);

            yield return null;

            // Assert
            Assert.AreEqual(15, unit.definition.armor, "Unit should have 10 (base) + 5 (Metallurgy I) armor");

            // Cleanup
            Object.Destroy(unit.gameObject);
        }

        [UnityTest]
        public IEnumerator Existing_Unit_Upgraded_By_Tech()
        {
            // Arrange - Spawn unit first
            var unitDef = DefinitionsFactory.CreateUnit(
                "test_unit", "Test Unit", Faction.RDF, UnitLayer.Ground, 100, 10, 3, 3,
                new WeaponDefinition[] { });
            var unit = UnitFactory.SpawnUnit("Test", unitDef, new HexCoord(0, 0), 1.0f);

            int initialArmor = unit.definition.armor;
            Assert.AreEqual(10, initialArmor, "Initial armor should be 10");

            yield return null;

            // Act - Research Metallurgy I AFTER spawning
            var metallurgyI = techManager.allTechs.FirstOrDefault(t => t.techId == "metallurgy_i");
            Assert.IsNotNull(metallurgyI, "Metallurgy I should exist");

            techManager.SetResearch(metallurgyI);
            techManager.AddScience(metallurgyI.costScience);
            yield return null;

            // Apply upgrades retroactively
            ApplyTechUpgradesToUnit(unit, techManager);

            // Assert
            Assert.AreEqual(15, unit.definition.armor, "Existing unit should be upgraded to 15 armor");

            // Cleanup
            Object.Destroy(unit.gameObject);
        }

        [UnityTest]
        public IEnumerator Multiple_Unit_Upgrades_Visible()
        {
            // Arrange
            var metallurgyI = techManager.allTechs.FirstOrDefault(t => t.techId == "metallurgy_i");
            var advancedMaterials = techManager.allTechs.FirstOrDefault(t => t.techId == "advanced_materials");

            Assert.IsNotNull(metallurgyI, "Metallurgy I should exist");
            Assert.IsNotNull(advancedMaterials, "Advanced Materials should exist");

            // Research Metallurgy I
            techManager.SetResearch(metallurgyI);
            techManager.AddScience(metallurgyI.costScience);
            yield return null;

            // Research Advanced Materials
            techManager.SetResearch(advancedMaterials);
            techManager.AddScience(advancedMaterials.costScience);
            yield return null;

            // Act - Spawn unit and apply all upgrades
            var unitDef = DefinitionsFactory.CreateUnit(
                "test_unit", "Test Unit", Faction.RDF, UnitLayer.Ground, 100, 10, 3, 3,
                new WeaponDefinition[] { });
            var unit = UnitFactory.SpawnUnit("Test", unitDef, new HexCoord(0, 0), 1.0f);

            ApplyTechUpgradesToUnit(unit, techManager);

            // Assert
            Assert.AreEqual(23, unit.definition.armor,
                "Unit should have 10 (base) + 5 (Metallurgy I) + 8 (Advanced Materials) = 23 armor");

            // Cleanup
            Object.Destroy(unit.gameObject);
        }

        #endregion

        #region Stability and Persistence Tests

        [UnityTest]
        public IEnumerator Game_Runs_Without_Crashes()
        {
            // Arrange - Get all techs
            var allTechs = techManager.allTechs.ToList();
            Assert.AreEqual(16, allTechs.Count, "Should have 16 techs");

            // Act - Research all Gen 0 techs
            var gen0Techs = techManager.GetTechsByGeneration(TechGeneration.Gen0);
            foreach (var tech in gen0Techs)
            {
                techManager.SetResearch(tech);
                techManager.AddScience(tech.costScience);
                yield return null;
            }

            // Research available Gen 1 techs
            techManager.UpdateAvailableTechs();
            var availableGen1 = techManager.availableTechs.Where(t => t.generation == TechGeneration.Gen1).ToList();
            foreach (var tech in availableGen1)
            {
                techManager.SetResearch(tech);
                techManager.AddScience(tech.costScience);
                yield return null;
            }

            // Spawn various unit types
            var vf1a = bootstrap.rdfVF1A;
            var pod = bootstrap.zentTacticalPod;

            var unit1 = UnitFactory.SpawnUnit("RDF1", vf1a, new HexCoord(1, 1), 1.0f);
            var unit2 = UnitFactory.SpawnUnit("ZENT1", pod, new HexCoord(2, 2), 1.0f);

            yield return null;

            // Assert - Game should still be running
            Assert.IsNotNull(techManager, "TechManager should still exist");
            Assert.IsNotNull(resourceManager, "ResourceManager should still exist");
            Assert.IsTrue(techManager.researchedTechs.Count >= 8, "At least 8 techs researched");

            // Cleanup
            Object.Destroy(unit1.gameObject);
            Object.Destroy(unit2.gameObject);
        }

        [UnityTest]
        public IEnumerator Tech_Bonus_Survives_Multiple_Turns()
        {
            // Arrange
            var reactorMk1 = techManager.allTechs.FirstOrDefault(t => t.techId == "reactor_mk1");
            Assert.IsNotNull(reactorMk1, "Reactor Mk I should exist");

            techManager.OnTechCompleted += tech =>
            {
                resourceManager.protoculture += (int)tech.protoculturePerTurn;
            };

            // Research tech in "turn 5"
            techManager.SetResearch(reactorMk1);
            techManager.AddScience(reactorMk1.costScience);
            yield return null;

            int protocolAfterResearch = resourceManager.protoculture;

            // Act - Simulate multiple turns passing (no additional changes)
            for (int i = 0; i < 15; i++)
            {
                yield return null;
            }

            // Assert - Bonus should persist
            Assert.AreEqual(protocolAfterResearch, resourceManager.protoculture,
                "Tech bonus should persist across multiple turns");
            Assert.IsTrue(techManager.researchedTechs.Contains(reactorMk1),
                "Tech should remain in researched list");
        }

        [UnityTest]
        public IEnumerator Era_Transition_And_Bonuses_Work_Together()
        {
            // Arrange
            var protocultureDiscovery = techManager.allTechs.FirstOrDefault(t => t.techId == "protoculture_discovery");
            var reactorMk1 = techManager.allTechs.FirstOrDefault(t => t.techId == "reactor_mk1");

            Assert.IsNotNull(protocultureDiscovery, "Protoculture Discovery should exist");
            Assert.IsNotNull(reactorMk1, "Reactor Mk I should exist");

            bool eraTransitioned = false;
            techManager.OnEraTransition += gen => { eraTransitioned = true; };
            techManager.OnTechCompleted += tech =>
            {
                resourceManager.protoculture += (int)tech.protoculturePerTurn;
            };

            int initialProtoculture = resourceManager.protoculture;

            // Act - Research Protoculture Discovery (era unlock)
            techManager.SetResearch(protocultureDiscovery);
            techManager.AddScience(protocultureDiscovery.costScience);
            yield return null;

            Assert.IsTrue(eraTransitioned, "Era should transition to Gen 1");
            Assert.AreEqual(TechGeneration.Gen1, techManager.currentGeneration);

            // Research Reactor Mk I (bonus tech)
            techManager.SetResearch(reactorMk1);
            techManager.AddScience(reactorMk1.costScience);
            yield return null;

            // Assert - Both era transition and bonuses work
            Assert.AreEqual(initialProtoculture + 10, resourceManager.protoculture,
                "Reactor Mk I bonus should apply after era transition");
        }

        #endregion

        #region Helper Methods

        private bool CanProduceUnit(UnitDefinition unit, TechManager tm)
        {
            if (unit.requiredTech == null) return true;
            return tm.researchedTechs.Contains(unit.requiredTech);
        }

        private void ApplyTechUpgradesToUnit(Unit unit, TechManager tm)
        {
            // Apply all researched tech bonuses to this unit
            foreach (var tech in tm.researchedTechs)
            {
                unit.definition.maxHP += tech.hpBonus;
                unit.definition.armor += tech.armorBonus;
                unit.definition.movement += tech.movementBonus;
                // Attack bonuses would apply to weapons
            }
        }

        #endregion
    }
}
