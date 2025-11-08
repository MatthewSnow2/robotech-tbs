using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Robotech.TBS.Bootstrap;
using Robotech.TBS.Systems;
using Robotech.TBS.Data;

namespace Robotech.Tests.PlayMode.Systems
{
    public class TechIntegrationTests
    {
        private GameObject gameObject;
        private GameBootstrap bootstrap;
        private TechManager techManager;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            // Create a fresh game object with GameBootstrap
            gameObject = new GameObject("TestGameBootstrap");
            bootstrap = gameObject.AddComponent<GameBootstrap>();

            // Wait for Awake to complete
            yield return null;

            // Get reference to tech manager
            techManager = bootstrap.techManager;
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

        [UnityTest]
        public IEnumerator GameStartsWithoutErrors()
        {
            // Test passes if we get here without exceptions
            Assert.IsNotNull(bootstrap);
            Assert.IsNotNull(techManager);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AllSixteenTechsAreCreated()
        {
            Assert.IsNotNull(techManager.allTechs, "allTechs list should not be null");
            Assert.AreEqual(16, techManager.allTechs.Count, "Should have exactly 16 Gen 0-1 techs");

            // Verify no null techs
            foreach (var tech in techManager.allTechs)
            {
                Assert.IsNotNull(tech, "No tech should be null");
                Assert.IsNotEmpty(tech.techId, "All techs should have an ID");
                Assert.IsNotEmpty(tech.displayName, "All techs should have a display name");
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Gen0TechsAreImmediatelyAvailable()
        {
            var gen0Techs = techManager.GetTechsByGeneration(TechGeneration.Gen0);

            Assert.AreEqual(8, gen0Techs.Count, "Should have 8 Gen 0 techs");

            // All Gen 0 techs should be available at start
            foreach (var tech in gen0Techs)
            {
                Assert.IsTrue(techManager.availableTechs.Contains(tech),
                    $"Gen 0 tech '{tech.displayName}' should be available at game start");
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator Gen1TechsAreLockedUntilPrereqsResearched()
        {
            var gen1Techs = techManager.GetTechsByGeneration(TechGeneration.Gen1);

            Assert.AreEqual(8, gen1Techs.Count, "Should have 8 Gen 1 techs");

            // All Gen 1 techs should be locked at start (not available)
            foreach (var tech in gen1Techs)
            {
                Assert.IsFalse(techManager.availableTechs.Contains(tech),
                    $"Gen 1 tech '{tech.displayName}' should NOT be available at game start");
            }

            yield return null;
        }

        [UnityTest]
        public IEnumerator ProtocultureDiscoveryAdvancesGeneration()
        {
            // Find Protoculture Discovery tech
            var protocultureDiscovery = techManager.allTechs
                .FirstOrDefault(t => t.techId == "protoculture_discovery");

            Assert.IsNotNull(protocultureDiscovery, "Protoculture Discovery tech should exist");
            Assert.IsTrue(protocultureDiscovery.allowsEraTransition,
                "Protoculture Discovery should have allowsEraTransition flag");

            // Current generation should be Gen0
            Assert.AreEqual(TechGeneration.Gen0, techManager.currentGeneration,
                "Should start at Gen 0");

            // Research the tech
            techManager.SetResearch(protocultureDiscovery);
            techManager.AddScience(protocultureDiscovery.costScience);

            yield return null;

            // Generation should advance to Gen1
            Assert.AreEqual(TechGeneration.Gen1, techManager.currentGeneration,
                "Should advance to Gen 1 after researching Protoculture Discovery");

            // Tech should be in researched list
            Assert.IsTrue(techManager.researchedTechs.Contains(protocultureDiscovery),
                "Protoculture Discovery should be in researched techs");
        }

        [UnityTest]
        public IEnumerator ReactorMk2ProvidesProtocultureBonus()
        {
            var reactorMk2 = techManager.allTechs
                .FirstOrDefault(t => t.techId == "reactor_mk2");

            Assert.IsNotNull(reactorMk2, "Reactor Mk II tech should exist");
            Assert.AreEqual(15f, reactorMk2.protoculturePerTurn,
                "Reactor Mk II should provide +15 protoculture per turn");

            yield return null;
        }

        [UnityTest]
        public IEnumerator TransformationEngineeringAvailableAfterMechaChassis()
        {
            // Find the techs
            var chassisI = techManager.allTechs
                .FirstOrDefault(t => t.techId == "chassis_i");
            var transformationI = techManager.allTechs
                .FirstOrDefault(t => t.techId == "transformation_i");

            Assert.IsNotNull(chassisI, "Mecha Chassis I should exist");
            Assert.IsNotNull(transformationI, "Transformation Engineering I should exist");

            // Transformation should not be available at start
            Assert.IsFalse(techManager.IsTechAvailable(transformationI),
                "Transformation Engineering I should not be available at start");

            // Research Mecha Chassis I
            techManager.SetResearch(chassisI);
            techManager.AddScience(chassisI.costScience);

            yield return null;

            // Transformation should now be available
            Assert.IsTrue(techManager.IsTechAvailable(transformationI),
                "Transformation Engineering I should be available after researching Mecha Chassis I");
        }

        [UnityTest]
        public IEnumerator AllGen0TechsHaveCorrectProperties()
        {
            var gen0Techs = techManager.GetTechsByGeneration(TechGeneration.Gen0);

            // Verify specific Gen 0 tech properties
            var jetPropulsion = gen0Techs.FirstOrDefault(t => t.techId == "jet_propulsion");
            Assert.IsNotNull(jetPropulsion);
            Assert.AreEqual(10, jetPropulsion.costScience);
            Assert.IsTrue(jetPropulsion.isCriticalPath);

            var reactorMk1 = gen0Techs.FirstOrDefault(t => t.techId == "reactor_mk1");
            Assert.IsNotNull(reactorMk1);
            Assert.AreEqual(10f, reactorMk1.protoculturePerTurn);

            var metallurgyI = gen0Techs.FirstOrDefault(t => t.techId == "metallurgy_i");
            Assert.IsNotNull(metallurgyI);
            Assert.AreEqual(5, metallurgyI.armorBonus);

            var globalComms = gen0Techs.FirstOrDefault(t => t.techId == "global_comms");
            Assert.IsNotNull(globalComms);
            Assert.AreEqual(5f, globalComms.sciencePerTurn);

            yield return null;
        }

        [UnityTest]
        public IEnumerator AllGen1TechsHaveCorrectPrerequisites()
        {
            var gen1Techs = techManager.GetTechsByGeneration(TechGeneration.Gen1);

            // Each Gen 1 tech should have exactly one prerequisite
            foreach (var tech in gen1Techs)
            {
                Assert.IsNotNull(tech.prerequisites,
                    $"{tech.displayName} should have prerequisites list");
                Assert.AreEqual(1, tech.prerequisites.Count,
                    $"{tech.displayName} should have exactly 1 prerequisite");
                Assert.IsNotNull(tech.prerequisites[0],
                    $"{tech.displayName} prerequisite should not be null");
            }

            // Verify specific prerequisite chains
            var transformationI = gen1Techs.FirstOrDefault(t => t.techId == "transformation_i");
            Assert.AreEqual("chassis_i", transformationI.prerequisites[0].techId);

            var sensorsI = gen1Techs.FirstOrDefault(t => t.techId == "sensors_i");
            Assert.AreEqual("jet_propulsion", sensorsI.prerequisites[0].techId);

            var reactorMk2 = gen1Techs.FirstOrDefault(t => t.techId == "reactor_mk2");
            Assert.AreEqual("reactor_mk1", reactorMk2.prerequisites[0].techId);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TechCompletionTriggersEvent()
        {
            bool eventTriggered = false;
            TechDefinition completedTech = null;

            techManager.OnTechCompleted += tech =>
            {
                eventTriggered = true;
                completedTech = tech;
            };

            var jetPropulsion = techManager.allTechs
                .FirstOrDefault(t => t.techId == "jet_propulsion");

            techManager.SetResearch(jetPropulsion);
            techManager.AddScience(jetPropulsion.costScience);

            yield return null;

            Assert.IsTrue(eventTriggered, "OnTechCompleted event should be triggered");
            Assert.AreEqual(jetPropulsion, completedTech, "Event should pass the completed tech");
        }
    }
}
