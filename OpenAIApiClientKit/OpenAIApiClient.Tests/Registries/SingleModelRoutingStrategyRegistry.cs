// <copyright file="SingleModelRoutingStrategyRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.SingleModel;
    using testClass = OpenAIApiClient.Registries.SingleModelRoutingStrategyRegistry;

    [TestClass]
    public class SingleModelRoutingStrategyRegistry
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
            ModelRoutingStrategy[] expected =
            [
                ModelRoutingStrategy.Explicit,
                ModelRoutingStrategy.LowestCost,
                ModelRoutingStrategy.HighestPerformance,
                ModelRoutingStrategy.BestReasoning,
                ModelRoutingStrategy.BestVision,
                ModelRoutingStrategy.BestAudioIn,
                ModelRoutingStrategy.BestAudioOut,
                ModelRoutingStrategy.Embedding,
                ModelRoutingStrategy.Moderation,
            ];

            foreach (ModelRoutingStrategy strategy in expected)
            {
                Assert.IsTrue(testClass.Strategies.ContainsKey(strategy), $"Strategy {strategy} should be registered.");
            }
        }

        [TestMethod]
        public void Get_ShouldReturnStrategyDelegate()
        {
            Delegates.SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.LowestCost);
            Assert.IsNotNull(handler);
        }

        [TestMethod]
        public void Get_ShouldThrowForMissingStrategy()
        {
            try
            {
                testClass.Get((ModelRoutingStrategy)999);
                Assert.Fail("Expected KeyNotFoundException was not thrown.");
            }
            catch (KeyNotFoundException ex)
            {
                Assert.Contains("No routing strategy registered", ex.Message);
            }
        }

        // ---------------------------------------------------------
        // Explicit Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void ExplicitRouting_ShouldReturnCorrectModel()
        {
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.Explicit,
                ExplicitModel = OpenAIModel.GPT4o,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.Explicit);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.AreEqual(OpenAIModel.GPT4o, result.Model.Name);
        }

        [TestMethod]
        public void ExplicitRouting_ShouldThrowIfNoModelProvided()
        {
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.Explicit,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.Explicit);

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
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.LowestCost,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.LowestCost);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

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
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.HighestPerformance,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.HighestPerformance);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.HighPerformance, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Reasoning Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestReasoning_ShouldReturnReasoningModel()
        {
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.BestReasoning,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.BestReasoning);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Reasoning, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Vision Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestVision_ShouldReturnVisionModel()
        {
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.BestVision,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.BestVision);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Vision, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Audio Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestAudioIn_ShouldReturnAudioInModel()
        {
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.BestAudioIn,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.BestAudioIn);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.AudioIn, result.Model.Capabilities);
        }

        [TestMethod]
        public void BestAudioOut_ShouldReturnAudioOutModel()
        {
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.BestAudioOut,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.BestAudioOut);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.AudioOut, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Embedding Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Embedding_ShouldReturnEmbeddingModel()
        {
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.Embedding,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.Embedding);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Embedding, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Moderation Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Moderation_ShouldReturnModerationModel()
        {
            SingleModelContext request = new()
            {
                Strategy = ModelRoutingStrategy.Moderation,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.Moderation);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Moderation, result.Model.Capabilities);
        }
    }
}
