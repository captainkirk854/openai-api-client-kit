// <copyright file="SingleModelStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries.Routing
{
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Enums.Routing;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Routing;
    using testClass = OpenAIApiClient.Registries.Routing.SingleModelStrategies;

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
                Assert.IsTrue(testClass.Strategies.ContainsKey(strategy), $"Strategy {strategy} should be registered.");
            }
        }

        [TestMethod]
        public void Get_ShouldReturnStrategyDelegate()
        {
            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.LowestCost);
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
                Assert.Contains("No routing strategy registered", ex.Message);
            }
        }

        // ---------------------------------------------------------
        // Explicit Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void ExplicitRouting_ShouldReturnCorrectModel()
        {
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.Explicit,
                ExplicitModel = OpenAIModel.GPT4o,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.Explicit);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.AreEqual(OpenAIModel.GPT4o, result.Model.Name);
        }

        [TestMethod]
        public void ExplicitRouting_ShouldThrowIfNoModelProvided()
        {
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.Explicit,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.Explicit);

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
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.LowestCost,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.LowestCost);
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
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.HighestPerformance,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.HighestPerformance);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.HighPerformance, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Reasoning Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestReasoning_ShouldReturnReasoningModel()
        {
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.BestReasoning,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.BestReasoning);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Reasoning, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Vision Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestVision_ShouldReturnVisionModel()
        {
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.BestVision,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.BestVision);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Vision, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Audio Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void BestAudioIn_ShouldReturnAudioInModel()
        {
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.BestAudioIn,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.BestAudioIn);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.AudioIn, result.Model.Capabilities);
        }

        [TestMethod]
        public void BestAudioOut_ShouldReturnAudioOutModel()
        {
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.BestAudioOut,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.BestAudioOut);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.AudioOut, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Embedding Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Embedding_ShouldReturnEmbeddingModel()
        {
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.Embedding,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.Embedding);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Embedding, result.Model.Capabilities);
        }

        // ---------------------------------------------------------
        // Moderation Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public void Moderation_ShouldReturnModerationModel()
        {
            SingleModelRouterRequest request = new()
            {
                Strategy = SingleModelStrategy.Moderation,
            };

            SingleModelRoutingStrategyHandler handler = testClass.Get(SingleModelStrategy.Moderation);
            SingleModelRouterResult result = handler(this.modelRegistry!, request);

            Assert.Contains(ModelCapability.Moderation, result.Model.Capabilities);
        }
    }
}
