// <copyright file="SingleModelStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries.Dispatch
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;
    using testClass = OpenAIApiClient.Registries.Dispatch.SingleModelStrategies;

    /// <summary>
    /// Tests for the <see cref="SingleModelStrategies"/> class.
    /// </summary>
    [TestClass]
    public class SingleModelStrategies
    {
        private IReadOnlyDictionary<OpenAIModel, ModelDescriptor>? modelRegistry;

        [TestInitialize]
        public void Setup()
        {
            this.modelRegistry = new OpenAIApiClient.Registries.OpenAIModels().Registry;
        }

        // ---------------------------------------------------------
        // Registry Structure Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Registry_ShouldContainAllExpectedStrategies()
        {
            SingleModelStrategy[] expected =
            [
                SingleModelStrategy.Explicit,
                SingleModelStrategy.LowestCost,
                SingleModelStrategy.HighestPerformance,
                SingleModelStrategy.BestReasoning,
                SingleModelStrategy.BestVision,
                SingleModelStrategy.BestAudioIn,
                SingleModelStrategy.BestAudioOut,
                SingleModelStrategy.Embedding,
                SingleModelStrategy.Moderation,
            ];

            foreach (SingleModelStrategy strategy in expected)
            {
                Assert.IsTrue(testClass.DefaultHandlerStrategies.ContainsKey(strategy), $"Strategy {strategy} should be registered.");
            }
        }

        [TestMethod]
        public void Get_ShouldReturnStrategyDelegate()
        {
            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.LowestCost);
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void Get_ShouldThrowForMissingStrategy()
        {
            try
            {
                testClass.Get((SingleModelStrategy)999);
                Assert.Fail("Expected KeyNotFoundException was not thrown.");
            }
            catch (KeyNotFoundException ex)
            {
                Assert.Contains("No single model strategy handler registered for:", ex.Message);
            }
        }

        // ---------------------------------------------------------
        // Explicit Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void ExplicitRouting_ShouldReturnCorrectModel()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.Explicit,
                ExplicitModel = OpenAIModel.GPT4o,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.Explicit);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.AreEqual(OpenAIModel.GPT4o, result.Model.Name);
        }

        [TestMethod]
        public void ExplicitRouting_ShouldThrowIfNoModelProvided()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.Explicit,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.Explicit);

            try
            {
                handler(this.modelRegistry!, request);
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains("Explicit routing requires", ex.Message);
            }
        }

        // ---------------------------------------------------------
        // Lowest Cost Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void LowestCost_ShouldReturnModelWithLowestInputCost()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.LowestCost,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.LowestCost);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            ModelDescriptor lowest = this.modelRegistry!.Values
                .OrderBy(m => m.Pricing.InputTokenCost)
                .ThenBy(m => m.Pricing.OutputTokenCost)
                .First();

            Assert.AreEqual(lowest.Name, result.Model.Name);
        }

        // ---------------------------------------------------------
        // Highest Performance Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void HighestPerformance_ShouldReturnHighPerformanceModel()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.HighestPerformance,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.HighestPerformance);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.HighPerformance, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Reasoning Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestReasoning_ShouldReturnReasoningModel()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.BestReasoning,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.BestReasoning);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Reasoning, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Vision Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestVision_ShouldReturnVisionModel()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.BestVision,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.BestVision);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Vision, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Audio Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestAudioIn_ShouldReturnAudioInModel()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.BestAudioIn,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.BestAudioIn);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.AudioIn, result.Model.Capabilities);
        }

        [TestMethod]
        public void BestAudioOut_ShouldReturnAudioOutModel()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.BestAudioOut,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.BestAudioOut);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.AudioOut, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Embedding Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Embedding_ShouldReturnEmbeddingModel()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.Embedding,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.Embedding);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Embedding, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Moderation Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Moderation_ShouldReturnModerationModel()
        {
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.Moderation,
            };

            SingleModelStrategyHandler handler = testClass.Get(SingleModelStrategy.Moderation);
            SingleModelDispatchResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Moderation, result.Model.Capabilities);
        }
    }
}
