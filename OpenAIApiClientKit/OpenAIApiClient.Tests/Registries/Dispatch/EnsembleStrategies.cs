// <copyright file="EnsembleStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries.Dispatch
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.AiModels;
    using testClass = OpenAIApiClient.Registries.Dispatch.EnsembleStrategies;

    /// <summary>
    /// Tests for the <see cref="EnsembleStrategies"/> class.
    /// </summary>
    [TestClass]
    public class EnsembleStrategies
    {
        private IReadOnlyDictionary<OpenAIModel, AiModelDescriptor>? modelRegistry;

        [TestInitialize]
        public void Setup()
        {
            // Important: Cleanup of custom handler registries on every test method end ..
            testClass.ClearCustomHandlers();

            // Initialize the model registry for use in strategy handlers ..
            this.modelRegistry = new OpenAIModels().GetRegistry();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Important: Cleanup of custom handler registries on every test method end ..
            testClass.ClearCustomHandlers();
        }

        // ---------------------------------------------------------
        // Registry Structure Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Registry_ShouldContainAllExpectedStrategies()
        {
            EnsembleStrategy[] expected =
            [
                EnsembleStrategy.Reasoning,
                EnsembleStrategy.Vision,
                EnsembleStrategy.CostOptimized,
            ];

            foreach (EnsembleStrategy strategy in expected)
            {
                Assert.IsTrue(testClass.DefaultHandlerStrategies.ContainsKey(strategy), $"Strategy {strategy} should be registered.");
            }
        }

        [TestMethod]
        public void Get_ShouldReturnStrategyDelegate()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Reasoning);
            Assert.IsNotNull(strategy);
        }

        [TestMethod]
        public void Get_ShouldThrowForMissingStrategy()
        {
            try
            {
                // Fake strategy not in registry
                testClass.Get((EnsembleStrategy)999);
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
        public void ReasoningEnsemble_ShouldReturnThreeAiModels()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Reasoning);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);
            Assert.HasCount(3, result.Models);
        }

        [TestMethod]
        public void ReasoningEnsemble_ShouldContainAReasoningAiModel()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Reasoning);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.Contains(m => m.Capabilities.Contains(AiModelCapability.Reasoning), result.Models, "Reasoning ensemble must include at least one reasoning model.");
        }

        [TestMethod]
        public void ReasoningEnsemble_ShouldContainAFastChatAiModel()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Reasoning);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.Contains(m => m.Capabilities.Contains(AiModelCapability.Chat), result.Models, "Reasoning ensemble must include a fast chat model.");
        }

        // ---------------------------------------------------------
        // Vision Ensemble Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void VisionEnsemble_ShouldReturnTwoAiModels()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Vision);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.HasCount(2, result.Models);
        }

        [TestMethod]
        public void VisionEnsemble_ShouldContainVisionAiModel()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Vision);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.Contains(m => m.Capabilities.Contains(AiModelCapability.Vision), result.Models, "Vision ensemble must include a vision-capable model.");
        }

        [TestMethod]
        public void VisionEnsemble_ShouldContainFastChatAiModel()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Vision);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.Contains(m => m.Capabilities.Contains(AiModelCapability.Chat), result.Models, "Vision ensemble must include a fast chat model.");
        }

        // ---------------------------------------------------------
        // Cost-Optimized Ensemble Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void CostOptimizedEnsemble_ShouldReturnThreeModels()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.CostOptimized);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.HasCount(3, result.Models);
        }

        [TestMethod]
        public void CostOptimizedEnsemble_ShouldContainLowCostModel()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.CostOptimized);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.Contains(m => m.Capabilities.Contains(AiModelCapability.LowCost), result.Models, "Cost-optimized ensemble must include a low-cost model.");
        }

        [TestMethod]
        public void CostOptimizedEnsemble_ShouldContainHighPerformanceModel()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.CostOptimized);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            Assert.Contains(m => m.Capabilities.Contains(AiModelCapability.HighPerformance), result.Models, "Cost-optimized ensemble must include a high-performance model.");
        }

        // ---------------------------------------------------------
        // Ordering Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void ReasoningEnsemble_FirstModelShouldBeHighPerformanceReasoning()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Reasoning);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            AiModelDescriptor first = result.Models[0];

            Assert.Contains(AiModelCapability.Reasoning, first.Capabilities);
            Assert.Contains(AiModelCapability.HighPerformance, first.Capabilities);
        }

        [TestMethod]
        public void VisionEnsemble_FirstAiModelShouldBeHighPerformanceVision()
        {
            EnsembleStrategyHandler strategy = testClass.Get(EnsembleStrategy.Vision);
            EnsembleDispatchResult result = strategy(modelRegistry: this.modelRegistry!);

            AiModelDescriptor first = result.Models[0];

            Assert.Contains(AiModelCapability.Vision, first.Capabilities);
            Assert.Contains(AiModelCapability.HighPerformance, first.Capabilities);
        }
    }
}