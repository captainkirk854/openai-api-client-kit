// <copyright file="EnsembleStrategyRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.Ensemble;
    using testClass = OpenAIApiClient.Registries.EnsembleStrategyRegistry;

    [TestClass]
    public class EnsembleStrategyRegistry
    {
        private IReadOnlyDictionary<OpenAIModel, ModelDescriptor>? modelRegistry;

        [TestInitialize]
        public void Setup()
        {
            this.modelRegistry = new OpenAIApiClient.Registries.OpenAIModelRegistry().Registry;
        }

        // ---------------------------------------------------------
        // Registry Structure Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Registry_ShouldContainAllExpectedStrategies()
        {
            EnsembleRoutingStrategy[] expected =
            [
                EnsembleRoutingStrategy.Reasoning,
                EnsembleRoutingStrategy.Vision,
                EnsembleRoutingStrategy.CostOptimized,
            ];

            foreach (EnsembleRoutingStrategy strategy in expected)
            {
                Assert.IsTrue(testClass.Strategies.ContainsKey(strategy), $"Strategy {strategy} should be registered.");
            }
        }

        [TestMethod]
        public void Get_ShouldReturnStrategyDelegate()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Reasoning);
            Assert.IsNotNull(strategy);
        }

        [TestMethod]
        public void Get_ShouldThrowForMissingStrategy()
        {
            try
            {
                // Fake strategy not in registry
                testClass.Get((EnsembleRoutingStrategy)999);
                Assert.Fail("Expected KeyNotFoundException was not thrown.");
            }
            catch (KeyNotFoundException)
            {
                // Test passes
            }
        }

        // ---------------------------------------------------------
        // Reasoning Ensemble Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void ReasoningEnsemble_ShouldReturnThreeModels()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Reasoning);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);
            Assert.HasCount(3, result.Models);
        }

        [TestMethod]
        public void ReasoningEnsemble_ShouldContainAReasoningModel()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Reasoning);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.IsTrue(result.Models.Any(m => m.Capabilities.Contains(ModelCapability.Reasoning)), "Reasoning ensemble must include at least one reasoning model.");
        }

        [TestMethod]
        public void ReasoningEnsemble_ShouldContainAFastChatModel()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Reasoning);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.IsTrue(result.Models.Any(m => m.Capabilities.Contains(ModelCapability.Chat)), "Reasoning ensemble must include a fast chat model.");
        }

        // ---------------------------------------------------------
        // Vision Ensemble Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void VisionEnsemble_ShouldReturnTwoModels()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Vision);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.HasCount(2, result.Models);
        }

        [TestMethod]
        public void VisionEnsemble_ShouldContainVisionModel()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Vision);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.IsTrue(result.Models.Any(m => m.Capabilities.Contains(ModelCapability.Vision)), "Vision ensemble must include a vision-capable model.");
        }

        [TestMethod]
        public void VisionEnsemble_ShouldContainFastChatModel()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Vision);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.IsTrue(result.Models.Any(m => m.Capabilities.Contains(ModelCapability.Chat)), "Vision ensemble must include a fast chat model.");
        }

        // ---------------------------------------------------------
        // Cost-Optimized Ensemble Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void CostOptimizedEnsemble_ShouldReturnThreeModels()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.CostOptimized);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.HasCount(3, result.Models);
        }

        [TestMethod]
        public void CostOptimizedEnsemble_ShouldContainLowCostModel()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.CostOptimized);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.IsTrue(result.Models.Any(m => m.Capabilities.Contains(ModelCapability.LowCost)), "Cost-optimized ensemble must include a low-cost model.");
        }

        [TestMethod]
        public void CostOptimizedEnsemble_ShouldContainHighPerformanceModel()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.CostOptimized);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.IsTrue(result.Models.Any(m => m.Capabilities.Contains(ModelCapability.HighPerformance)), "Cost-optimized ensemble must include a high-performance model.");
        }

        // ---------------------------------------------------------
        // Ordering Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void ReasoningEnsemble_FirstModelShouldBeHighPerformanceReasoning()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Reasoning);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            ModelDescriptor first = result.Models[0];

            Assert.Contains(ModelCapability.Reasoning, first.Capabilities);
            Assert.Contains(ModelCapability.HighPerformance, first.Capabilities);
        }

        [TestMethod]
        public void VisionEnsemble_FirstModelShouldBeHighPerformanceVision()
        {
            EnsembleStrategy strategy = testClass.Get(EnsembleRoutingStrategy.Vision);
            EnsembleRouterResult result = strategy(modelRegistry: this.modelRegistry!);

            ModelDescriptor first = result.Models[0];

            Assert.Contains(ModelCapability.Vision, first.Capabilities);
            Assert.Contains(ModelCapability.HighPerformance, first.Capabilities);
        }
    }
}