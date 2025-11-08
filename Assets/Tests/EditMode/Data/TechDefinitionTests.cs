using NUnit.Framework;
using UnityEngine;
using Robotech.TBS.Data;
using System.Collections.Generic;

namespace Robotech.Tests.EditMode.Data
{
    /// <summary>
    /// EditMode tests for TechDefinition and related enums.
    /// Validates core data structures for the technology tree system.
    /// </summary>
    public class TechDefinitionTests
    {
        [Test]
        public void TechGeneration_CanBeInstantiated()
        {
            // Arrange & Act
            TechGeneration gen0 = TechGeneration.Gen0;
            TechGeneration gen1 = TechGeneration.Gen1;
            TechGeneration gen2 = TechGeneration.Gen2;
            TechGeneration gen3 = TechGeneration.Gen3;
            TechGeneration gen4 = TechGeneration.Gen4;
            TechGeneration gen5 = TechGeneration.Gen5;

            // Assert
            Assert.AreEqual(TechGeneration.Gen0, gen0);
            Assert.AreEqual(TechGeneration.Gen1, gen1);
            Assert.AreEqual(TechGeneration.Gen2, gen2);
            Assert.AreEqual(TechGeneration.Gen3, gen3);
            Assert.AreEqual(TechGeneration.Gen4, gen4);
            Assert.AreEqual(TechGeneration.Gen5, gen5);
        }

        [Test]
        public void TechCategory_CanBeInstantiated()
        {
            // Arrange & Act
            TechCategory power = TechCategory.Power;
            TechCategory mecha = TechCategory.Mecha;
            TechCategory weapons = TechCategory.Weapons;
            TechCategory defense = TechCategory.Defense;
            TechCategory aerospace = TechCategory.Aerospace;
            TechCategory special = TechCategory.Special;

            // Assert
            Assert.AreEqual(TechCategory.Power, power);
            Assert.AreEqual(TechCategory.Mecha, mecha);
            Assert.AreEqual(TechCategory.Weapons, weapons);
            Assert.AreEqual(TechCategory.Defense, defense);
            Assert.AreEqual(TechCategory.Aerospace, aerospace);
            Assert.AreEqual(TechCategory.Special, special);
        }

        [Test]
        public void TechDefinition_CanBeCreated()
        {
            // Arrange & Act
            TechDefinition tech = ScriptableObject.CreateInstance<TechDefinition>();

            // Assert
            Assert.IsNotNull(tech);
            Assert.IsInstanceOf<ScriptableObject>(tech);
            Assert.IsInstanceOf<TechDefinition>(tech);
        }

        [Test]
        public void TechDefinition_HasCorrectDefaultValues()
        {
            // Arrange & Act
            TechDefinition tech = ScriptableObject.CreateInstance<TechDefinition>();

            // Assert
            Assert.AreEqual(50, tech.costScience, "Default science cost should be 50");
            Assert.AreEqual(TechGeneration.Gen0, tech.generation, "Default generation should be Gen0");
            Assert.IsFalse(tech.isCriticalPath, "Default isCriticalPath should be false");
            Assert.IsFalse(tech.allowsEraTransition, "Default allowsEraTransition should be false");
        }

        [Test]
        public void TechDefinition_ListFieldsInitializeCorrectly()
        {
            // Arrange & Act
            TechDefinition tech = ScriptableObject.CreateInstance<TechDefinition>();

            // Assert
            Assert.IsNotNull(tech.prerequisites, "Prerequisites list should be initialized");
            Assert.IsNotNull(tech.unlocksUnits, "UnlocksUnits list should be initialized");
            Assert.IsNotNull(tech.unlocksDistricts, "UnlocksDistricts list should be initialized");
            Assert.IsNotNull(tech.unlocksAbilities, "UnlocksAbilities list should be initialized");

            Assert.AreEqual(0, tech.prerequisites.Count, "Prerequisites should start empty");
            Assert.AreEqual(0, tech.unlocksUnits.Count, "UnlocksUnits should start empty");
            Assert.AreEqual(0, tech.unlocksDistricts.Count, "UnlocksDistricts should start empty");
            Assert.AreEqual(0, tech.unlocksAbilities.Count, "UnlocksAbilities should start empty");
        }

        [Test]
        public void TechDefinition_CanBePopulatedWithAllFields()
        {
            // Arrange
            TechDefinition tech = ScriptableObject.CreateInstance<TechDefinition>();
            TechDefinition prereq = ScriptableObject.CreateInstance<TechDefinition>();
            UnitDefinition unit = ScriptableObject.CreateInstance<UnitDefinition>();
            DistrictDefinition district = ScriptableObject.CreateInstance<DistrictDefinition>();
            AbilityDefinition ability = ScriptableObject.CreateInstance<AbilityDefinition>();
            Sprite testIcon = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0, 0, 1, 1),
                Vector2.zero
            );

            // Act
            tech.techId = "test_tech";
            tech.displayName = "Test Technology";
            tech.costScience = 100;
            tech.description = "A test technology for unit testing";
            tech.generation = TechGeneration.Gen2;
            tech.category = TechCategory.Mecha;
            tech.prerequisites.Add(prereq);
            tech.unlocksUnits.Add(unit);
            tech.unlocksDistricts.Add(district);
            tech.unlocksAbilities.Add(ability);
            tech.protoculturePerTurn = 5.5f;
            tech.sciencePerTurn = 3.2f;
            tech.productionPerTurn = 10.0f;
            tech.hpBonus = 20;
            tech.armorBonus = 5;
            tech.movementBonus = 1;
            tech.attackBonus = 10;
            tech.isCriticalPath = true;
            tech.allowsEraTransition = true;
            tech.icon = testIcon;

            // Assert
            Assert.AreEqual("test_tech", tech.techId);
            Assert.AreEqual("Test Technology", tech.displayName);
            Assert.AreEqual(100, tech.costScience);
            Assert.AreEqual("A test technology for unit testing", tech.description);
            Assert.AreEqual(TechGeneration.Gen2, tech.generation);
            Assert.AreEqual(TechCategory.Mecha, tech.category);
            Assert.AreEqual(1, tech.prerequisites.Count);
            Assert.AreEqual(prereq, tech.prerequisites[0]);
            Assert.AreEqual(1, tech.unlocksUnits.Count);
            Assert.AreEqual(unit, tech.unlocksUnits[0]);
            Assert.AreEqual(1, tech.unlocksDistricts.Count);
            Assert.AreEqual(district, tech.unlocksDistricts[0]);
            Assert.AreEqual(1, tech.unlocksAbilities.Count);
            Assert.AreEqual(ability, tech.unlocksAbilities[0]);
            Assert.AreEqual(5.5f, tech.protoculturePerTurn, 0.01f);
            Assert.AreEqual(3.2f, tech.sciencePerTurn, 0.01f);
            Assert.AreEqual(10.0f, tech.productionPerTurn, 0.01f);
            Assert.AreEqual(20, tech.hpBonus);
            Assert.AreEqual(5, tech.armorBonus);
            Assert.AreEqual(1, tech.movementBonus);
            Assert.AreEqual(10, tech.attackBonus);
            Assert.IsTrue(tech.isCriticalPath);
            Assert.IsTrue(tech.allowsEraTransition);
            Assert.AreEqual(testIcon, tech.icon);
        }

        [Test]
        public void TechDefinition_PrerequisitesListCanHoldMultipleTechs()
        {
            // Arrange
            TechDefinition tech = ScriptableObject.CreateInstance<TechDefinition>();
            TechDefinition prereq1 = ScriptableObject.CreateInstance<TechDefinition>();
            TechDefinition prereq2 = ScriptableObject.CreateInstance<TechDefinition>();
            TechDefinition prereq3 = ScriptableObject.CreateInstance<TechDefinition>();

            prereq1.techId = "prereq_1";
            prereq2.techId = "prereq_2";
            prereq3.techId = "prereq_3";

            // Act
            tech.prerequisites.Add(prereq1);
            tech.prerequisites.Add(prereq2);
            tech.prerequisites.Add(prereq3);

            // Assert
            Assert.AreEqual(3, tech.prerequisites.Count);
            Assert.Contains(prereq1, tech.prerequisites);
            Assert.Contains(prereq2, tech.prerequisites);
            Assert.Contains(prereq3, tech.prerequisites);
        }

        [Test]
        public void TechDefinition_YieldBonusesDefaultToZero()
        {
            // Arrange & Act
            TechDefinition tech = ScriptableObject.CreateInstance<TechDefinition>();

            // Assert
            Assert.AreEqual(0f, tech.protoculturePerTurn);
            Assert.AreEqual(0f, tech.sciencePerTurn);
            Assert.AreEqual(0f, tech.productionPerTurn);
        }

        [Test]
        public void TechDefinition_UnitStatBonusesDefaultToZero()
        {
            // Arrange & Act
            TechDefinition tech = ScriptableObject.CreateInstance<TechDefinition>();

            // Assert
            Assert.AreEqual(0, tech.hpBonus);
            Assert.AreEqual(0, tech.armorBonus);
            Assert.AreEqual(0, tech.movementBonus);
            Assert.AreEqual(0, tech.attackBonus);
        }
    }
}
