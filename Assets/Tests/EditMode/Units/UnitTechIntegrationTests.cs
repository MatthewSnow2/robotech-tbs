using NUnit.Framework;
using UnityEngine;
using Robotech.TBS.Bootstrap;
using Robotech.TBS.Data;
using Robotech.TBS.Hex;
using Robotech.TBS.Units;
using Robotech.TBS.Systems;

namespace Robotech.TBS.Tests.EditMode.Units
{
    [TestFixture]
    public class UnitTechIntegrationTests
    {
        private GameObject techManagerObj;
        private TechManager techManager;

        [SetUp]
        public void SetUp()
        {
            // Create TechManager for tests
            techManagerObj = new GameObject("TechManager");
            techManager = techManagerObj.AddComponent<TechManager>();
            techManager.researchedTechs.Clear();
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up all GameObjects created during tests
            var allObjects = Object.FindObjectsOfType<GameObject>();
            foreach (var obj in allObjects)
            {
                Object.DestroyImmediate(obj);
            }
        }

        [Test]
        public void UnitFactory_RejectsSpawning_WithoutRequiredTech()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Advanced Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var requiredTech = ScriptableObject.CreateInstance<TechDefinition>();
            requiredTech.displayName = "Advanced Aeronautics";
            unitDef.requiredTech = requiredTech;

            var coord = new HexCoord(0, 0);

            // Act
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: false);

            // Assert
            Assert.IsNull(unit, "UnitFactory should return null when required tech is not researched");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(requiredTech);
        }

        [Test]
        public void UnitFactory_AllowsSpawning_WhenRequiredTechIsResearched()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Advanced Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var requiredTech = ScriptableObject.CreateInstance<TechDefinition>();
            requiredTech.displayName = "Advanced Aeronautics";
            unitDef.requiredTech = requiredTech;

            // Research the tech
            techManager.researchedTechs.Add(requiredTech);

            var coord = new HexCoord(0, 0);

            // Act
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: false);

            // Assert
            Assert.IsNotNull(unit, "UnitFactory should spawn unit when required tech is researched");
            Assert.AreEqual(unitDef, unit.definition);

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(requiredTech);
        }

        [Test]
        public void ApplyTechUpgrade_IncreasesUnitArmor_ByCorrectAmount()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var coord = new HexCoord(0, 0);
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            var tech = ScriptableObject.CreateInstance<TechDefinition>();
            tech.displayName = "Improved Armor";
            tech.armorBonus = 8;

            // Act
            unit.ApplyTechUpgrade(tech);

            // Assert
            Assert.AreEqual(13, unit.GetArmor(), "Unit armor should be base (5) + bonus (8) = 13");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech);
        }

        [Test]
        public void ApplyTechUpgrade_IncreasesUnitHP_ByCorrectAmount()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var coord = new HexCoord(0, 0);
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            var tech = ScriptableObject.CreateInstance<TechDefinition>();
            tech.displayName = "Enhanced Durability";
            tech.hpBonus = 25;

            // Act
            int initialHP = unit.currentHP;
            unit.ApplyTechUpgrade(tech);

            // Assert
            Assert.AreEqual(125, unit.GetMaxHP(), "Max HP should be base (100) + bonus (25) = 125");
            Assert.AreEqual(initialHP + 25, unit.currentHP, "Current HP should also increase by bonus amount");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech);
        }

        [Test]
        public void ApplyTechUpgrade_PreventsDuplicateApplication()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var coord = new HexCoord(0, 0);
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            var tech = ScriptableObject.CreateInstance<TechDefinition>();
            tech.displayName = "Improved Armor";
            tech.armorBonus = 8;

            // Act
            unit.ApplyTechUpgrade(tech);
            bool hasUpgradeBeforeSecondApply = unit.HasTechUpgrade(tech);
            int armorAfterFirst = unit.GetArmor();

            unit.ApplyTechUpgrade(tech); // Try to apply again
            int armorAfterSecond = unit.GetArmor();

            // Assert
            Assert.IsTrue(hasUpgradeBeforeSecondApply, "HasTechUpgrade should return true after first application");
            Assert.AreEqual(armorAfterFirst, armorAfterSecond, "Armor should not increase on duplicate application");
            Assert.AreEqual(13, armorAfterSecond, "Armor should still be 5 + 8 = 13, not 5 + 8 + 8 = 21");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech);
        }

        [Test]
        public void MultipleTechUpgrades_Stack()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var coord = new HexCoord(0, 0);
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            var tech1 = ScriptableObject.CreateInstance<TechDefinition>();
            tech1.displayName = "Armor Tech 1";
            tech1.armorBonus = 5;

            var tech2 = ScriptableObject.CreateInstance<TechDefinition>();
            tech2.displayName = "Armor Tech 2";
            tech2.armorBonus = 8;

            // Act
            unit.ApplyTechUpgrade(tech1);
            unit.ApplyTechUpgrade(tech2);

            // Assert
            Assert.AreEqual(18, unit.GetArmor(), "Armor should be base (5) + tech1 (5) + tech2 (8) = 18");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech1);
            Object.DestroyImmediate(tech2);
        }

        [Test]
        public void BypassTechCheck_AllowsSpawningLockedUnits()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Advanced Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var requiredTech = ScriptableObject.CreateInstance<TechDefinition>();
            requiredTech.displayName = "Advanced Aeronautics";
            unitDef.requiredTech = requiredTech;

            var coord = new HexCoord(0, 0);

            // Act - tech NOT researched, but bypass is true
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            // Assert
            Assert.IsNotNull(unit, "UnitFactory should spawn unit when bypassTechCheck is true");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(requiredTech);
        }

        [Test]
        public void UnitFactory_AppliesAllRelevantResearchedTechs()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var tech1 = ScriptableObject.CreateInstance<TechDefinition>();
            tech1.displayName = "Armor Tech";
            tech1.armorBonus = 5;

            var tech2 = ScriptableObject.CreateInstance<TechDefinition>();
            tech2.displayName = "HP Tech";
            tech2.hpBonus = 20;

            var tech3 = ScriptableObject.CreateInstance<TechDefinition>();
            tech3.displayName = "Movement Tech";
            tech3.movementBonus = 1;

            // Research all techs before spawning
            techManager.researchedTechs.Add(tech1);
            techManager.researchedTechs.Add(tech2);
            techManager.researchedTechs.Add(tech3);

            var coord = new HexCoord(0, 0);

            // Act
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            // Assert
            Assert.AreEqual(10, unit.GetArmor(), "Armor should include tech bonus");
            Assert.AreEqual(120, unit.GetMaxHP(), "Max HP should include tech bonus");
            Assert.AreEqual(4, unit.GetMovement(), "Movement should include tech bonus");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech1);
            Object.DestroyImmediate(tech2);
            Object.DestroyImmediate(tech3);
        }

        [Test]
        public void TechBonuses_PersistForUnitLifetime()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var coord = new HexCoord(0, 0);
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            var tech = ScriptableObject.CreateInstance<TechDefinition>();
            tech.displayName = "Armor Tech";
            tech.armorBonus = 10;

            // Act
            unit.ApplyTechUpgrade(tech);
            int armorAfterUpgrade = unit.GetArmor();

            // Simulate some game actions
            unit.NewTurn();
            unit.TakeDamage(10);

            int armorAfterActions = unit.GetArmor();

            // Assert
            Assert.AreEqual(15, armorAfterUpgrade, "Armor should be 5 + 10 = 15 after upgrade");
            Assert.AreEqual(15, armorAfterActions, "Armor should persist through game actions");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech);
        }

        [Test]
        public void TechManager_AppliesRetroactiveTechUpgrades_ToExistingUnits()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            // Spawn units BEFORE tech is researched
            var coord1 = new HexCoord(0, 0);
            var coord2 = new HexCoord(1, 1);
            var unit1 = UnitFactory.SpawnUnit("Test1", unitDef, coord1, 1.0f, bypassTechCheck: true);
            var unit2 = UnitFactory.SpawnUnit("Test2", unitDef, coord2, 1.0f, bypassTechCheck: true);

            var tech = ScriptableObject.CreateInstance<TechDefinition>();
            tech.displayName = "Global Armor Upgrade";
            tech.armorBonus = 7;
            tech.costScience = 50;

            // Set up tech research
            techManager.currentResearch = tech;
            techManager.scienceProgress = 0;

            // Act - Complete the tech (this should trigger retroactive upgrades)
            techManager.AddScience(50);

            // Assert
            Assert.IsTrue(unit1.HasTechUpgrade(tech), "Unit1 should have the tech upgrade applied");
            Assert.IsTrue(unit2.HasTechUpgrade(tech), "Unit2 should have the tech upgrade applied");
            Assert.AreEqual(12, unit1.GetArmor(), "Unit1 armor should be 5 + 7 = 12");
            Assert.AreEqual(12, unit2.GetArmor(), "Unit2 armor should be 5 + 7 = 12");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech);
        }

        [Test]
        public void ApplyTechUpgrade_IncreasesMovement_ByCorrectAmount()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var coord = new HexCoord(0, 0);
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            var tech = ScriptableObject.CreateInstance<TechDefinition>();
            tech.displayName = "Enhanced Engines";
            tech.movementBonus = 2;

            // Act
            unit.ApplyTechUpgrade(tech);

            // Assert
            Assert.AreEqual(5, unit.GetMovement(), "Movement should be base (3) + bonus (2) = 5");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech);
        }

        [Test]
        public void ApplyTechUpgrade_IncreasesAttack_ByCorrectAmount()
        {
            // Arrange
            var unitDef = ScriptableObject.CreateInstance<UnitDefinition>();
            unitDef.displayName = "Fighter";
            unitDef.maxHP = 100;
            unitDef.armor = 5;
            unitDef.movement = 3;
            unitDef.faction = Faction.RDF;

            var coord = new HexCoord(0, 0);
            var unit = UnitFactory.SpawnUnit("Test", unitDef, coord, 1.0f, bypassTechCheck: true);

            var tech = ScriptableObject.CreateInstance<TechDefinition>();
            tech.displayName = "Weapon Upgrades";
            tech.attackBonus = 15;

            // Act
            unit.ApplyTechUpgrade(tech);

            // Assert
            Assert.AreEqual(15, unit.GetAttackBonus(), "Attack bonus should be 15");

            // Cleanup
            Object.DestroyImmediate(unitDef);
            Object.DestroyImmediate(tech);
        }
    }
}
