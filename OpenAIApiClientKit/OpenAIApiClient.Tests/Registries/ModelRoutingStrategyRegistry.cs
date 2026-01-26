// <copyright file="ModelRoutingStrategyRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.Individual;
    using testClass = OpenAIApiClient.Registries.ModelRoutingStrategyRegistry;

    [TestClass]
    public class ModelRoutingStrategyRegistry
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
            Delegates.ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.LowestCost);
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
                Assert.Contains(ex.Message, "No routing strategy registered");
            }
        }

        // ---------------------------------------------------------
        // Explicit Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void ExplicitRouting_ShouldReturnCorrectModel()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.Explicit,
                ExplicitModel = OpenAIModel.GPT4o,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.Explicit);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.AreEqual(OpenAIModel.GPT4o, result.Descriptor.Model);
        }

        [TestMethod]
        public void ExplicitRouting_ShouldThrowIfNoModelProvided()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.Explicit,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.Explicit);

            try
            {
                handler(this.modelRegistry!, request);
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains(ex.Message, "Explicit routing requires");
            }
        }

        // ---------------------------------------------------------
        // Lowest Cost Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void LowestCost_ShouldReturnModelWithLowestInputCost()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.LowestCost,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.LowestCost);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            ModelDescriptor lowest = this.modelRegistry!.Values
                .OrderBy(m => m.Pricing.InputTokenCost)
                .ThenBy(m => m.Pricing.OutputTokenCost)
                .First();

            Assert.AreEqual(lowest.Model, result.Descriptor.Model);
        }

        // ---------------------------------------------------------
        // Highest Performance Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void HighestPerformance_ShouldReturnHighPerformanceModel()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.HighestPerformance,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.HighestPerformance);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.HighPerformance, result.Descriptor.Capabilities);
        }

        // ---------------------------------------------------------
        // Reasoning Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestReasoning_ShouldReturnReasoningModel()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.BestReasoning,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.BestReasoning);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Reasoning, result.Descriptor.Capabilities);
        }

        // ---------------------------------------------------------
        // Vision Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestVision_ShouldReturnVisionModel()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.BestVision,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.BestVision);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Vision, result.Descriptor.Capabilities);
        }

        // ---------------------------------------------------------
        // Audio Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestAudioIn_ShouldReturnAudioInModel()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.BestAudioIn,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.BestAudioIn);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.AudioIn, result.Descriptor.Capabilities);
        }

        [TestMethod]
        public void BestAudioOut_ShouldReturnAudioOutModel()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.BestAudioOut,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.BestAudioOut);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.AudioOut, result.Descriptor.Capabilities);
        }

        // ---------------------------------------------------------
        // Embedding Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Embedding_ShouldReturnEmbeddingModel()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.Embedding,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.Embedding);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Embedding, result.Descriptor.Capabilities);
        }

        // ---------------------------------------------------------
        // Moderation Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Moderation_ShouldReturnModerationModel()
        {
            ModelRouterRequest request = new()
            {
                Strategy = ModelRoutingStrategy.Moderation,
            };

            ModelRoutingStrategyHandler handler = testClass.Get(ModelRoutingStrategy.Moderation);
            ModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Moderation, result.Descriptor.Capabilities);
        }
    }
}
