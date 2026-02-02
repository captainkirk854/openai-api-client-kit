// <copyright file="EnsembleDispatcherTests.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Dispatch
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.Dispatch;

    [TestClass]
    public sealed class EnsembleDispatcherTests
    {
        [TestMethod]
        public void Evaluate_Throws_WhenRequestIsNull()
        {
            // Arrange
            Dictionary<OpenAIModel, ModelDescriptor> registry = [];
            EnsembleDispatcher dispatcher = new(registry);

            // Act & Assert
            try
            {
                dispatcher.Evaluate(null!);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                // success
            }
        }

        [TestMethod]
        public void Evaluate_UsesStrategyHandler_ForCustomStrategy()
        {
            // Arrange
            ModelDescriptor model = CreateDescriptor(model: OpenAIModel.GPT4o, cost: 1.0m, capabilities: [ModelCapability.Reasoning]);

            // Create a registry with a single model ..
            Dictionary<OpenAIModel, ModelDescriptor> registry = new()
            {
                { OpenAIModel.GPT4o, model },
            };

            // Register a custom handler for the Reasoning strategy ..
            EnsembleDispatchResult expected = new(models: [model]);
            EnsembleStrategies.Register(strategy: EnsembleStrategy.Reasoning, handler: _ => expected); // EnsembleStrategies.Register(strategy: EnsembleStrategy.Reasoning, handler: (IReadOnlyDictionary<OpenAIModel, ModelDescriptor> _) => expected);

            // Create the dispatcher ..
            EnsembleDispatcher dispatcher = new(registry);

            // Define the request ..
            EnsembleDispatchRequest request = new()
            {
                Strategy = EnsembleStrategy.Custom,
                RequiredCapabilities = [ModelCapability.Reasoning],
                ExplicitModels = [OpenAIModel.GPT4o],
            };

            // Act
            EnsembleDispatchResult result = dispatcher.Evaluate(request);

            // Assert
            Assert.HasCount(1, result.Models);
            Assert.AreEqual(OpenAIModel.GPT4o, result.Models[0].Name);
        }

        [TestMethod]
        public void Evaluate_CustomStrategy_ReturnsModelsMatchingCapabilities()
        {
            // Arrange

            // Create model descriptors ..
            ModelDescriptor m1 = CreateDescriptor(OpenAIModel.GPT4o, 1.0m, [ModelCapability.Reasoning, ModelCapability.Vision]);
            ModelDescriptor m2 = CreateDescriptor(OpenAIModel.GPT4o_Mini, 0.5m, [ModelCapability.Reasoning]);
            ModelDescriptor m3 = CreateDescriptor(OpenAIModel.GPT4_Turbo, 2.0m, [ModelCapability.Vision]);

            // Create a registry with the models ..
            Dictionary<OpenAIModel, ModelDescriptor> registry = new()
            {
                { OpenAIModel.GPT4o, m1 },
                { OpenAIModel.GPT4o_Mini, m2 },
                { OpenAIModel.GPT4_Turbo, m3 },
            };

            // Create the dispatcher ..
            EnsembleDispatcher dispatcher = new(registry);

            // Define the request ..
            EnsembleDispatchRequest request = new()
            {
                Strategy = EnsembleStrategy.Custom,
                RequiredCapabilities = [ModelCapability.Reasoning],
            };

            // Act
            EnsembleDispatchResult result = dispatcher.Evaluate(request);

            // Assert
            Assert.HasCount(2, result.Models);
            Assert.IsTrue(result.Models.Any(m => m.Name == m1.Name), "Expected model m1 was not found in the result set.");
            Assert.IsTrue(result.Models.Any(m => m.Name == m2.Name), "Expected model m2 was not found in the result set.");
            Assert.AreEqual(m2, result.Models[0]);
        }

        [TestMethod]
        public void Evaluate_CustomStrategy_Throws_WhenNoCapabilitiesProvided()
        {
            // Arrange
            Dictionary<OpenAIModel, ModelDescriptor> registry = [];
            EnsembleDispatcher dispatcher = new(registry);

            EnsembleDispatchRequest request = new()
            {
                Strategy = EnsembleStrategy.Custom,
                RequiredCapabilities = [],
            };

            // Act & Assert
            try
            {
                dispatcher.Evaluate(request);
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException)
            {
                // success
            }
        }

        [TestMethod]
        public void Evaluate_CustomStrategy_Throws_WhenNoModelsMatch()
        {
            // Arrange
            ModelDescriptor m1 = CreateDescriptor(OpenAIModel.GPT4o, 1.0m, [ModelCapability.Reasoning]);

            Dictionary<OpenAIModel, ModelDescriptor> registry = new()
            {
                { OpenAIModel.GPT4o, m1 },
            };

            // Create the dispatcher ..
            EnsembleDispatcher dispatcher = new(registry);

            // Define the request ..
            EnsembleDispatchRequest request = new()
            {
                Strategy = EnsembleStrategy.Custom,
                RequiredCapabilities = [ModelCapability.Vision],
            };

            // Act & Assert
            try
            {
                dispatcher.Evaluate(request);
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException)
            {
                // success
            }
        }

        /// <summary>
        /// Creates a ModelDescriptor for the specified model, cost, and capabilities.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="cost"></param>
        /// <param name="capabilities"></param>
        /// <returns see cref="ModelDescriptor">.</returns>
        private static ModelDescriptor CreateDescriptor(OpenAIModel model, decimal cost, IEnumerable<ModelCapability> capabilities)
        {
            // Create the descriptor ..
            ModelDescriptor descriptor = new()
            {
                Capabilities = new HashSet<ModelCapability>(capabilities),
                Pricing = new ModelPricing(cost, cost),
                Domain = ModelDomain.Other,
                Generation = OpenAIModelGeneration.Other,
            };

            // and set the descriptor's internal property "Name" via reflection
            typeof(ModelDescriptor)
                .GetProperty(name: nameof(ModelDescriptor.Name))!
                .SetValue(obj: descriptor, value: model);

            return descriptor;
        }
    }
}
