// <copyright file="SingleAiModelStrategies.cs" company="854 Things (tm)">
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
    using testClass = OpenAIApiClient.Registries.Dispatch.SingleAiModelStrategies;

    /// <summary>
    /// Tests for the <see cref="SingleAiModelStrategies"/> class.
    /// </summary>
    [TestClass]
    public class SingleAiModelStrategies
    {
        private IReadOnlyDictionary<OpenAIModel, AiModelDescriptor>? modelRegistry;

        [TestInitialize]
        public void Setup()
        {
            // Important: Cleanup of custom handler registries on every test method end ..
            testClass.ClearCustomHandlers();

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
            SingleAiModelStrategy[] expected =
            [
                SingleAiModelStrategy.Explicit,
                SingleAiModelStrategy.LowestCost,
                SingleAiModelStrategy.HighestPerformance,
                SingleAiModelStrategy.BestReasoning,
                SingleAiModelStrategy.BestVision,
                SingleAiModelStrategy.BestAudioIn,
                SingleAiModelStrategy.BestAudioOut,
                SingleAiModelStrategy.Embedding,
                SingleAiModelStrategy.Moderation,
            ];

            foreach (SingleAiModelStrategy strategy in expected)
            {
                Assert.IsTrue(testClass.DefaultHandlerStrategies.ContainsKey(strategy), $"Strategy {strategy} should be registered.");
            }
        }

        [TestMethod]
        public void Get_ShouldReturnStrategyDelegate()
        {
            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.LowestCost);
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void Get_ShouldThrowForMissingStrategy()
        {
            try
            {
                testClass.Get((SingleAiModelStrategy)999);
                Assert.Fail("Expected KeyNotFoundException was not thrown.");
            }
            catch (KeyNotFoundException ex)
            {
                Assert.Contains("No single model strategy handler registered for:", ex.Message);
            }
        }

        // ---------------------------------------------------------
        // Explicit Ai Model Dispatch Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void ExplicitRouting_ShouldReturnCorrectAiModel()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.Explicit,
                ExplicitModel = OpenAIModel.GPT4o,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.Explicit);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.AreEqual(OpenAIModel.GPT4o, result.Model.Name);
        }

        [TestMethod]
        public void ExplicitRouting_ShouldThrowIfNoAiModelProvided()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.Explicit,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.Explicit);

            try
            {
                handler(this.modelRegistry!, request);
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains("Explicit dispatch requires", ex.Message);
            }
        }

        // ---------------------------------------------------------
        // Lowest Cost Ai Model Dispatch Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void LowestCost_ShouldReturnAiModelWithLowestInputCost()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.LowestCost,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.LowestCost);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            AiModelDescriptor lowest = this.modelRegistry!.Values
                .OrderBy(m => m.Pricing.InputTokenCost)
                .ThenBy(m => m.Pricing.OutputTokenCost)
                .First();

            Assert.AreEqual(lowest.Name, result.Model.Name);
        }

        // ---------------------------------------------------------
        // Highest Performance Ai Model Dispatch Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void HighestPerformance_ShouldReturnHighPerformanceAiModel()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.HighestPerformance,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.HighestPerformance);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(AiModelCapability.HighPerformance, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Reasoning Ai Model Dispatch Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestReasoning_ShouldReturnReasoningAiModel()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.BestReasoning,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.BestReasoning);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(AiModelCapability.Reasoning, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Vision Ai Model Dispatch Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestVision_ShouldReturnVisionAiModel()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.BestVision,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.BestVision);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(AiModelCapability.Vision, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Audio Ai Model Dispatch Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestAudioIn_ShouldReturnAudioInAiModel()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.BestAudioIn,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.BestAudioIn);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(AiModelCapability.AudioIn, result.Model.Capabilities);
        }

        [TestMethod]
        public void BestAudioOut_ShouldReturnAudioOutAiModel()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.BestAudioOut,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.BestAudioOut);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(AiModelCapability.AudioOut, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Embedding Ai Model Dispatch Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Embedding_ShouldReturnEmbeddingAiModel()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.Embedding,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.Embedding);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(AiModelCapability.Embedding, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Moderation Ai Model Dispatch Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Moderation_ShouldReturnModerationAiModel()
        {
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = SingleAiModelStrategy.Moderation,
            };

            SingleAiModelStrategyHandler handler = testClass.Get(SingleAiModelStrategy.Moderation);
            SingleAiModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(AiModelCapability.Moderation, result.Model.Capabilities);
        }
    }
}
