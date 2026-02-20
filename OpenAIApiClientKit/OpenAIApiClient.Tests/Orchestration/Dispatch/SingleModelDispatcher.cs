// <copyright file="SingleModelDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Dispatch
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.Dispatch;
    using OpenAIApiClient.Registries.Models;
    using OpenAIApiClient.Tests.Orchestration.Mocks;
    using testClass = OpenAIApiClient.Orchestration.Dispatch.SingleModelDispatcher;

    [TestClass]
    public sealed class SingleModelDispatcher
    {
        [TestInitialize]
        public void Init()
        {
            // Important: Cleanup of custom handler registries on every test method start ..
            SingleModelStrategies.ClearCustomHandlers();
            EnsembleStrategies.ClearCustomHandlers();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Important: Cleanup of custom handler registries on every test method end ..
            SingleModelStrategies.ClearCustomHandlers();
            EnsembleStrategies.ClearCustomHandlers();
        }

        [TestMethod]
        public void Evaluate_Throws_WhenRequestIsNull()
        {
            // Arrange
            IAIModelRegistry registry = new OpenAIModels();
            testClass dispatcher = new(registry: registry);

            // Act & Assert
            try
            {
                dispatcher.Evaluate(request: null!);
                Assert.Fail("Expected ArgumentNullException was not thrown.");
            }
            catch (ArgumentNullException)
            {
                // success
            }
        }

        [TestMethod]
        public void Evaluate_UsesRegisteredStrategyHandler()
        {
            // Arrange
            OpenAIModel aiModelToUse = OpenAIModel.GPT4o;
            IReadOnlyList<ModelCapability> capabilities = [ModelCapability.Reasoning];
            SingleModelStrategy strategy = SingleModelStrategy.BestReasoning;

            // Get the mock registry, the model descriptor and the expected result as a tuple ..
            (IAIModelRegistry aiModels, ModelDescriptor model, SingleModelDispatchResult expected) = Cook(aiModel: aiModelToUse, capabilities: capabilities);

            // Register a custom strategy handler
            SingleModelStrategies.RegisterCustomHandler(strategy: strategy, handler: (_, _) => expected);

            // Create the dispatcher ..
            testClass dispatcher = new(registry: aiModels);

            // Define the request for the strategy to test, without any explicit model or capabilities, to ensure the strategy handler is the one doing the work ..
            SingleModelDispatchRequest request = new()
            {
                Strategy = strategy,
            };

            // Act
            SingleModelDispatchResult result = dispatcher.Evaluate(request: request);

            // Assert
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void Evaluate_PassesCorrectContextToHandler()
        {
            // Arrange
            OpenAIModel aiModelToUse = OpenAIModel.GPT4o;
            IReadOnlyList<ModelCapability> capabilities = [ModelCapability.Chat];
            SingleModelStrategy strategy = SingleModelStrategy.LowestCost;

            // Get the mock registry, the model descriptor and the expected result as a tuple ..
            (IAIModelRegistry aiModels, ModelDescriptor model, _) = Cook(aiModel: aiModelToUse, capabilities: capabilities);

            // Initialise ..
            SingleModelDispatchRequest? capturedContext = null;

            // Register a custom strategy handler
            SingleModelStrategies.RegisterCustomHandler(strategy: strategy, handler:
                (_, req) =>
                {
                    capturedContext = req;
                    return new SingleModelDispatchResult(model: model);
                });

            // Create the dispatcher ..
            testClass dispatcher = new(registry: aiModels);

            // Define the request for the strategy to test, with explicit model and capabilities, to ensure the strategy handler receives them in the context ..
            SingleModelDispatchRequest request = new()
            {
                Strategy = strategy,
                ExplicitModel = aiModelToUse,
                RequiredCapabilities = capabilities,
            };

            // Act
            dispatcher.Evaluate(request: request);

            // Assert
            Assert.IsNotNull(capturedContext);
            Assert.AreEqual(aiModelToUse, capturedContext!.ExplicitModel);
            Assert.AreEqual(strategy, capturedContext.Strategy);
            Assert.IsTrue(capturedContext.RequiredCapabilities!.Contains(ModelCapability.Chat));
        }

        [TestMethod]
        public void Evaluate_Throws_WhenStrategyNotRegistered()
        {
            // Arrange
            OpenAIModel aiModelToUse = OpenAIModel.GPT4o;
            IReadOnlyList<ModelCapability> capabilities = [ModelCapability.Reasoning];

            // Get the mock registry, the model descriptor and the expected result as a tuple ..
            (IAIModelRegistry aiModels, _, _) = Cook(aiModel: aiModelToUse, capabilities: capabilities);

            // Create the dispatcher ..
            testClass dispatcher = new(aiModels);

            SingleModelDispatchRequest request = new()
            {
                Strategy = (SingleModelStrategy)999, // use an undefined and unregistered strategy
            };

            // Act and assert ..
            try
            {
                dispatcher.Evaluate(request: request);
                Assert.Fail("Expected KeyNotFoundException was not thrown.");
            }
            catch (KeyNotFoundException)
            {
                // success
            }
        }

        [TestMethod]
        public void Evaluate_ReturnsHandlerResult()
        {
            // Arrange
            OpenAIModel aiModelToUse = OpenAIModel.GPT4o_Mini;
            IReadOnlyList<ModelCapability> capabilities = [ModelCapability.Chat];
            SingleModelStrategy strategy = SingleModelStrategy.HighestPerformance;

            // Get the mock registry, the model descriptor and the expected result as a tuple ..
            (IAIModelRegistry aiModels, ModelDescriptor model, SingleModelDispatchResult expected) = Cook(aiModel: aiModelToUse, capabilities: capabilities);

            // Register a custom strategy handler
            SingleModelStrategies.RegisterCustomHandler(strategy: strategy, handler: (_, _) => expected); // lambda expression is implicitly type: (_, _) => expected

            // Create the dispatcher ..
            testClass dispatcher = new(registry: aiModels);

            SingleModelDispatchRequest request = new()
            {
                Strategy = strategy,
            };

            // Act
            SingleModelDispatchResult result = dispatcher.Evaluate(request: request);

            // Assert
            Assert.AreSame(expected, result);
        }

        /// <summary>
        /// Gets the mock registry, the model descriptor and the expected dispatch result as a tuple for the specified model and capabilities.
        /// </summary>
        /// <param name="aiModel"></param>
        /// <param name="capabilities"></param>
        /// <returns see cref="(IAIModelRegistry, ModelDescriptor, SingleModelDispatchResult)">.</returns>
        private static (IAIModelRegistry, ModelDescriptor, SingleModelDispatchResult) Cook(OpenAIModel aiModel, IReadOnlyList<ModelCapability> capabilities)
        {
            // Create the test model descriptor ..
            ModelDescriptor m1 = CreateDescriptor(aiModel: aiModel, cost: 1.0m, capabilities: capabilities);

            // Define an internal registry with one, single entry for the test model ..
            Dictionary<OpenAIModel, ModelDescriptor> internalRegistry = new()
            {
                { aiModel, m1 },
            };

            // Define the mock registry that returns the internal registry ..
            MockOpenAIModels mockRegistry = new(models: internalRegistry);

            // Define the expected result ..
            SingleModelDispatchResult expected = new(model: m1);

            // Return the mock registry, the model descriptor and the expected dispatch result as a tuple ..
            return (mockRegistry, m1, expected);
        }

        /// <summary>
        /// Creates a ModelDescriptor for the specified model, cost, and capabilities.
        /// </summary>
        /// <param name="aiModel"></param>
        /// <param name="cost"></param>
        /// <param name="capabilities"></param>
        /// <returns see cref="ModelDescriptor">.</returns>
        private static ModelDescriptor CreateDescriptor(OpenAIModel aiModel, decimal cost, IEnumerable<ModelCapability> capabilities)
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
                .SetValue(obj: descriptor, value: aiModel);

            return descriptor;
        }
    }
}