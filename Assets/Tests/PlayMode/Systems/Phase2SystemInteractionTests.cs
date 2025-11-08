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
    public class Phase2SystemInteractionTests
    {
        private GameObject gameObject;
        private GameBootstrap bootstrap;
        private TechManager techManager;
        private ResourceManager resourceManager;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            gameObject = new GameObject("TestGameBootstrap");
            bootstrap = gameObject.AddComponent<GameBootstrap>();
            yield return null;

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

        #region System Communication Tests

        [UnityTest]
        public IEnumerator TechManager_To_ResourceManager_Communication()
        {
            // Arrange
            var reactorMk1 = techManager.allTechs.FirstOrDefault(t => t.techId == "reactor_mk1");
            Assert.IsNotNull(reactorMk1, "Reactor Mk I should exist");

            int initialProtoculture = resourceManager.protoculture;
            bool eventReceived = false;
            TechDefinition receivedTech = null;

            // Subscribe to OnTechCompleted event
            techManager.OnTechCompleted += tech =>
            {
                eventReceived = true;
                receivedTech = tech;
                // Simulate ResourceManager listening to this event
                resourceManager.protoculture += (int)tech.protoculturePerTurn;
            };

            // Act - Complete tech research
            techManager.SetResearch(reactorMk1);
            techManager.AddScience(reactorMk1.costScience);

            yield return null;

            // Assert
            Assert.IsTrue(eventReceived, "OnTechCompleted event should be fired");
            Assert.AreEqual(reactorMk1, receivedTech, "Event should pass correct tech");
            Assert.AreEqual(initialProtoculture + 10, resourceManager.protoculture,
                "ResourceManager should receive and apply bonus via event");
        }

        [UnityTest]
        public IEnumerator TechManager_To_Unit_Communication()
        {
            // Arrange
            var metallurgyI = techManager.allTechs.FirstOrDefault(t => t.techId == "metallurgy_i");
            Assert.IsNotNull(metallurgyI, "Metallurgy I should exist");

            var unitDef = DefinitionsFactory.CreateUnit(
                "test_unit", "Test Unit", Faction.RDF, UnitLayer.Ground, 100, 10, 3, 3,
                new WeaponDefinition[] { });
            var unit = UnitFactory.SpawnUnit("Test", unitDef, new HexCoord(0, 0), 1.0f);

            int initialArmor = unit.definition.armor;
            bool eventReceived = false;

            // Subscribe to OnTechCompleted event to upgrade units
            techManager.OnTechCompleted += tech =>
            {
                eventReceived = true;
                // Simulate unit system listening to this event
                unit.definition.armor += tech.armorBonus;
            };

            // Act
            techManager.SetResearch(metallurgyI);
            techManager.AddScience(metallurgyI.costScience);

            yield return null;

            // Assert
            Assert.IsTrue(eventReceived, "OnTechCompleted event should be fired");
            Assert.AreEqual(initialArmor + 5, unit.definition.armor,
                "Unit should receive upgrade via event system");

            // Cleanup
            Object.Destroy(unit.gameObject);
        }

        [UnityTest]
        public IEnumerator ResourceManager_Bonus_Accumulation()
        {
            // Arrange - Multiple techs with various bonuses
            var reactor1 = techManager.allTechs.FirstOrDefault(t => t.techId == "reactor_mk1");
            var globalComms = techManager.allTechs.FirstOrDefault(t => t.techId == "global_comms");

            Assert.IsNotNull(reactor1, "Reactor Mk I should exist");
            Assert.IsNotNull(globalComms, "Global Comms should exist");

            int initialProtoculture = resourceManager.protoculture;
            int initialScience = resourceManager.science;

            // Subscribe to accumulate all bonuses
            techManager.OnTechCompleted += tech =>
            {
                resourceManager.protoculture += (int)tech.protoculturePerTurn;
                resourceManager.science += (int)tech.sciencePerTurn;
                resourceManager.materials += (int)tech.productionPerTurn;
            };

            // Act - Research multiple techs
            techManager.SetResearch(reactor1);
            techManager.AddScience(reactor1.costScience);
            yield return null;

            techManager.SetResearch(globalComms);
            techManager.AddScience(globalComms.costScience);
            yield return null;

            // Assert - All bonuses accumulated
            Assert.AreEqual(initialProtoculture + 10, resourceManager.protoculture,
                "Protoculture bonus from Reactor Mk I");
            Assert.AreEqual(initialScience + 5, resourceManager.science,
                "Science bonus from Global Comms");
        }

        [UnityTest]
        public IEnumerator Unit_Bonus_Accumulation()
        {
            // Arrange
            var metallurgyI = techManager.allTechs.FirstOrDefault(t => t.techId == "metallurgy_i");
            var advancedMaterials = techManager.allTechs.FirstOrDefault(t => t.techId == "advanced_materials");

            Assert.IsNotNull(metallurgyI, "Metallurgy I should exist");
            Assert.IsNotNull(advancedMaterials, "Advanced Materials should exist");

            var unitDef = DefinitionsFactory.CreateUnit(
                "test_unit", "Test Unit", Faction.RDF, UnitLayer.Ground, 100, 10, 3, 3,
                new WeaponDefinition[] { });
            var unit = UnitFactory.SpawnUnit("Test", unitDef, new HexCoord(0, 0), 1.0f);

            int initialArmor = unit.definition.armor;

            // Subscribe to accumulate unit bonuses
            techManager.OnTechCompleted += tech =>
            {
                unit.definition.armor += tech.armorBonus;
                unit.definition.maxHP += tech.hpBonus;
                unit.definition.movement += tech.movementBonus;
            };

            // Act - Research both armor techs
            techManager.SetResearch(metallurgyI);
            techManager.AddScience(metallurgyI.costScience);
            yield return null;

            techManager.SetResearch(advancedMaterials);
            techManager.AddScience(advancedMaterials.costScience);
            yield return null;

            // Assert - Multiple bonuses accumulated on same unit
            Assert.AreEqual(initialArmor + 5 + 8, unit.definition.armor,
                "Both armor bonuses should accumulate: +5 from Metallurgy I, +8 from Advanced Materials");

            // Cleanup
            Object.Destroy(unit.gameObject);
        }

        #endregion

        #region Edge Case Tests

        [UnityTest]
        public IEnumerator Edge_Case_NullTechManager()
        {
            // Arrange - Create a scenario where TechManager might be null
            var isolatedResourceManager = new GameObject("IsolatedRM").AddComponent<ResourceManager>();
            int initialProtoculture = isolatedResourceManager.protoculture;

            // Act - Try to operate without TechManager
            isolatedResourceManager.protoculture += 10;

            yield return null;

            // Assert - Should handle gracefully
            Assert.AreEqual(initialProtoculture + 10, isolatedResourceManager.protoculture,
                "ResourceManager should work independently");

            // Cleanup
            Object.Destroy(isolatedResourceManager.gameObject);
        }

        [UnityTest]
        public IEnumerator Edge_Case_SpawnUnit_With_NullTech()
        {
            // Arrange - Unit definition with null requiredTech
            var unitDef = DefinitionsFactory.CreateUnit(
                "basic_unit", "Basic Unit", Faction.RDF, UnitLayer.Ground, 100, 5, 3, 3,
                new WeaponDefinition[] { });
            unitDef.requiredTech = null;

            // Act - Should spawn without tech requirement
            var unit = UnitFactory.SpawnUnit("Test", unitDef, new HexCoord(0, 0), 1.0f);

            yield return null;

            // Assert
            Assert.IsNotNull(unit, "Unit should spawn successfully with null requiredTech");
            Assert.AreEqual(unitDef, unit.definition, "Unit should have correct definition");

            // Cleanup
            Object.Destroy(unit.gameObject);
        }

        [UnityTest]
        public IEnumerator Bonus_Calculation_With_10Plus_Techs()
        {
            // Arrange - Research many techs to test system stability
            var gen0Techs = techManager.GetTechsByGeneration(TechGeneration.Gen0);
            Assert.GreaterOrEqual(gen0Techs.Count, 8, "Should have at least 8 Gen 0 techs");

            int initialProtoculture = resourceManager.protoculture;
            int initialScience = resourceManager.science;

            techManager.OnTechCompleted += tech =>
            {
                resourceManager.protoculture += (int)tech.protoculturePerTurn;
                resourceManager.science += (int)tech.sciencePerTurn;
                resourceManager.materials += (int)tech.productionPerTurn;
            };

            // Act - Research all Gen 0 techs
            foreach (var tech in gen0Techs)
            {
                techManager.SetResearch(tech);
                techManager.AddScience(tech.costScience);
                yield return null;
            }

            // Assert - System should handle many techs gracefully
            Assert.AreEqual(8, techManager.researchedTechs.Count, "All 8 Gen 0 techs researched");
            Assert.IsNotNull(resourceManager, "ResourceManager should still be valid");

            // Calculate expected bonuses
            float expectedProtoculture = initialProtoculture;
            float expectedScience = initialScience;
            foreach (var tech in techManager.researchedTechs)
            {
                expectedProtoculture += tech.protoculturePerTurn;
                expectedScience += tech.sciencePerTurn;
            }

            Assert.AreEqual((int)expectedProtoculture, resourceManager.protoculture,
                "Protoculture should match expected total from all techs");
            Assert.AreEqual((int)expectedScience, resourceManager.science,
                "Science should match expected total from all techs");
        }

        [UnityTest]
        public IEnumerator Event_Unsubscription_Prevents_Leaks()
        {
            // Arrange
            var reactorMk1 = techManager.allTechs.FirstOrDefault(t => t.techId == "reactor_mk1");
            Assert.IsNotNull(reactorMk1, "Reactor Mk I should exist");

            int callCount = 0;
            System.Action<TechDefinition> handler = tech => { callCount++; };

            // Subscribe
            techManager.OnTechCompleted += handler;

            // Act - Trigger event
            techManager.SetResearch(reactorMk1);
            techManager.AddScience(reactorMk1.costScience);
            yield return null;

            Assert.AreEqual(1, callCount, "Handler should be called once");

            // Unsubscribe
            techManager.OnTechCompleted -= handler;

            // Research another tech
            var globalComms = techManager.allTechs.FirstOrDefault(t => t.techId == "global_comms");
            techManager.SetResearch(globalComms);
            techManager.AddScience(globalComms.costScience);
            yield return null;

            // Assert - Handler should not be called again
            Assert.AreEqual(1, callCount, "Handler should not be called after unsubscription");
        }

        #endregion

        #region Complex Integration Tests

        [UnityTest]
        public IEnumerator Full_System_Integration_Research_And_Spawn()
        {
            // Arrange - Complex scenario: research techs, spawn units, verify all interactions
            var chassisI = techManager.allTechs.FirstOrDefault(t => t.techId == "chassis_i");
            var transformationI = techManager.allTechs.FirstOrDefault(t => t.techId == "transformation_i");
            var metallurgyI = techManager.allTechs.FirstOrDefault(t => t.techId == "metallurgy_i");

            Assert.IsNotNull(chassisI, "Chassis I should exist");
            Assert.IsNotNull(transformationI, "Transformation I should exist");
            Assert.IsNotNull(metallurgyI, "Metallurgy I should exist");

            int initialProtoculture = resourceManager.protoculture;

            techManager.OnTechCompleted += tech =>
            {
                resourceManager.protoculture += (int)tech.protoculturePerTurn;
            };

            // Act - Research prerequisite chain
            techManager.SetResearch(chassisI);
            techManager.AddScience(chassisI.costScience);
            yield return null;

            techManager.SetResearch(metallurgyI);
            techManager.AddScience(metallurgyI.costScience);
            yield return null;

            techManager.SetResearch(transformationI);
            techManager.AddScience(transformationI.costScience);
            yield return null;

            // Create unit requiring Transformation I
            var vf0 = DefinitionsFactory.CreateUnit(
                "vf0", "VF-0 Phoenix", Faction.RDF, UnitLayer.Air, 100, 5, 4, 3,
                new WeaponDefinition[] { });
            vf0.requiredTech = transformationI;

            bool canSpawn = techManager.researchedTechs.Contains(transformationI);
            Assert.IsTrue(canSpawn, "Should be able to spawn VF-0 after researching Transformation I");

            var unit = UnitFactory.SpawnUnit("RDF", vf0, new HexCoord(5, 5), 1.0f);

            // Apply tech bonuses to spawned unit
            foreach (var tech in techManager.researchedTechs)
            {
                unit.definition.armor += tech.armorBonus;
            }

            // Assert - All systems working together
            Assert.AreEqual(3, techManager.researchedTechs.Count, "3 techs researched");
            Assert.IsNotNull(unit, "Unit spawned successfully");
            Assert.AreEqual(10, unit.definition.armor, "Unit has base armor (5) + Metallurgy I (5)");

            // Cleanup
            Object.Destroy(unit.gameObject);
        }

        [UnityTest]
        public IEnumerator Multiple_Units_Receive_Same_Tech_Bonus()
        {
            // Arrange - Spawn multiple units, then research tech
            var unitDef1 = DefinitionsFactory.CreateUnit("unit1", "Unit 1", Faction.RDF, UnitLayer.Ground, 100, 10, 3, 3, new WeaponDefinition[] { });
            var unitDef2 = DefinitionsFactory.CreateUnit("unit2", "Unit 2", Faction.RDF, UnitLayer.Air, 80, 8, 4, 3, new WeaponDefinition[] { });
            var unitDef3 = DefinitionsFactory.CreateUnit("unit3", "Unit 3", Faction.Zentradi, UnitLayer.Ground, 110, 12, 3, 2, new WeaponDefinition[] { });

            var unit1 = UnitFactory.SpawnUnit("RDF1", unitDef1, new HexCoord(0, 0), 1.0f);
            var unit2 = UnitFactory.SpawnUnit("RDF2", unitDef2, new HexCoord(1, 1), 1.0f);
            var unit3 = UnitFactory.SpawnUnit("ZENT1", unitDef3, new HexCoord(2, 2), 1.0f);

            var units = new[] { unit1, unit2, unit3 };

            var metallurgyI = techManager.allTechs.FirstOrDefault(t => t.techId == "metallurgy_i");
            Assert.IsNotNull(metallurgyI, "Metallurgy I should exist");

            // Subscribe to apply bonuses to ALL units
            techManager.OnTechCompleted += tech =>
            {
                foreach (var unit in units)
                {
                    if (unit != null && unit.definition != null)
                    {
                        unit.definition.armor += tech.armorBonus;
                    }
                }
            };

            // Act - Research tech
            techManager.SetResearch(metallurgyI);
            techManager.AddScience(metallurgyI.costScience);
            yield return null;

            // Assert - All units should receive bonus
            Assert.AreEqual(15, unit1.definition.armor, "Unit 1: 10 + 5");
            Assert.AreEqual(13, unit2.definition.armor, "Unit 2: 8 + 5");
            Assert.AreEqual(17, unit3.definition.armor, "Unit 3: 12 + 5");

            // Cleanup
            foreach (var unit in units)
            {
                if (unit != null && unit.gameObject != null)
                {
                    Object.Destroy(unit.gameObject);
                }
            }
        }

        #endregion
    }
}
