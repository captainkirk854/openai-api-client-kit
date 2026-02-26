// <copyright file="SingleAiModelDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Dispatch
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.AiModels;
    using OpenAIApiClient.Registries.Dispatch;
    using OpenAIApiClient.Tests.Orchestration.Mocks;
    using testClass = OpenAIApiClient.Orchestration.Dispatch.SingleAiModelDispatcher;

    [TestClass]
    public sealed class SingleAiModelDispatcher
    {
        [TestInitialize]
        public void Init()
        {
            // Important: Cleanup of custom handler registries on every test method start ..
            SingleAiModelStrategies.ClearCustomHandlers();
            EnsembleStrategies.ClearCustomHandlers();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Important: Cleanup of custom handler registries on every test method end ..
            SingleAiModelStrategies.ClearCustomHandlers();
            EnsembleStrategies.ClearCustomHandlers();
        }

        [TestMethod]
        public void Evaluate_Throws_WhenRequestIsNull()
        {
            // Arrange
            IAiModelRegistry registry = new OpenAIModels();
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
            IReadOnlyList<AiModelCapability> capabilities = [AiModelCapability.Reasoning];
            SingleAiModelStrategy strategy = SingleAiModelStrategy.BestReasoning;

            // Get the mock registry, the model descriptor and the expected result as a tuple ..
            (IAiModelRegistry aiModels, AiModelDescriptor model, SingleAiModelDispatchResult expected) = Cook(aiModel: aiModelToUse, capabilities: capabilities);

            // Register a custom strategy handler
            SingleAiModelStrategies.RegisterCustomHandler(strategy: strategy, handler: (_, _) => expected);

            // Create the dispatcher ..
            testClass dispatcher = new(registry: aiModels);

            // Define the request for the strategy to test, without any explicit model or capabilities, to ensure the strategy handler is the one doing the work ..
            SingleAiModelDispatchRequest request = new()
            {
                Strategy = strategy,
            };

            // Act
            SingleAiModelDispatchResult result = dispatcher.Evaluate(request: request);

            // Assert
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void Evaluate_PassesCorrectContextToHandler()
        {
            // Arrange
            OpenAIModel aiModelToUse = OpenAIModel.GPT4o;
            IReadOnlyList<AiModelCapability> capabilities = [AiModelCapability.Chat];
            SingleAiModelStrategy strategy = SingleAiModelStrategy.LowestCost;

            // Get the mock registry, the model descriptor and the expected result as a tuple ..
            (IAiModelRegistry aiModels, AiModelDescriptor model, _) = Cook(aiModel: aiModelToUse, capabilities: capabilities);

            // Initialise ..
            SingleAiModelDispatchRequest? capturedContext = null;

            // Register a custom strategy handler
            SingleAiModelStrategies.RegisterCustomHandler(strategy: strategy, handler:
                (_, req) =>
                {
                    capturedContext = req;
                    return new SingleAiModelDispatchResult(model: model);
                });

            // Create the dispatcher ..
            testClass dispatcher = new(registry: aiModels);

            // Define the request for the strategy to test, with explicit model and capabilities, to ensure the strategy handler receives them in the context ..
            SingleAiModelDispatchRequest request = new()
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
            Assert.IsTrue(capturedContext.RequiredCapabilities!.Contains(AiModelCapability.Chat));
        }

        [TestMethod]
        public void Evaluate_Throws_WhenStrategyNotRegistered()
        {
            // Arrange
            OpenAIModel aiModelToUse = OpenAIModel.GPT4o;
            IReadOnlyList<AiModelCapability> capabilities = [AiModelCapability.Reasoning];

            // Get the mock registry, the model descriptor and the expected result as a tuple ..
            (IAiModelRegistry aiModels, _, _) = Cook(aiModel: aiModelToUse, capabilities: capabilities);

            // Create the dispatcher ..
            testClass dispatcher = new(aiModels);

            SingleAiModelDispatchRequest request = new()
            {
                Strategy = (SingleAiModelStrategy)999, // use an undefined and unregistered strategy
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
            IReadOnlyList<AiModelCapability> capabilities = [AiModelCapability.Chat];
            SingleAiModelStrategy strategy = SingleAiModelStrategy.HighestPerformance;

            // Get the mock registry, the model descriptor and the expected result as a tuple ..
            (IAiModelRegistry aiModels, AiModelDescriptor model, SingleAiModelDispatchResult expected) = Cook(aiModel: aiModelToUse, capabilities: capabilities);

            // Register a custom strategy handler
            SingleAiModelStrategies.RegisterCustomHandler(strategy: strategy, handler: (_, _) => expected); // lambda expression is implicitly type: (_, _) => expected

            // Create the dispatcher ..
            testClass dispatcher = new(registry: aiModels);

            SingleAiModelDispatchRequest request = new()
            {
                Strategy = strategy,
            };

            // Act
            SingleAiModelDispatchResult result = dispatcher.Evaluate(request: request);

            // Assert
            Assert.AreSame(expected, result);
        }

        /// <summary>
        /// Gets the mock registry, the model descriptor and the expected dispatch result as a tuple for the specified model and capabilities.
        /// </summary>
        /// <param name="aiModel"></param>
        /// <param name="capabilities"></param>
        /// <returns see cref="(IAiModelRegistry, AiModelDescriptor, SingleAiModelDispatchResult)">.</returns>
        private static (IAiModelRegistry, AiModelDescriptor, SingleAiModelDispatchResult) Cook(OpenAIModel aiModel, IReadOnlyList<AiModelCapability> capabilities)
        {
            // Create the test model descriptor ..
            AiModelDescriptor m1 = CreateDescriptor(aiModel: aiModel, cost: 1.0m, capabilities: capabilities);

            // Define an internal registry with one, single entry for the test model ..
            Dictionary<OpenAIModel, AiModelDescriptor> internalRegistry = new()
            {
                { aiModel, m1 },
            };

            // Define the mock registry that returns the internal registry ..
            MockOpenAIModels mockRegistry = new(models: internalRegistry);

            // Define the expected result ..
            SingleAiModelDispatchResult expected = new(model: m1);

            // Return the mock registry, the model descriptor and the expected dispatch result as a tuple ..
            return (mockRegistry, m1, expected);
        }

        /// <summary>
        /// Creates a ModelDescriptor for the specified model, cost, and capabilities.
        /// </summary>
        /// <param name="aiModel"></param>
        /// <param name="cost"></param>
        /// <param name="capabilities"></param>
        /// <returns see cref="AiModelDescriptor">.</returns>
        private static AiModelDescriptor CreateDescriptor(OpenAIModel aiModel, decimal cost, IEnumerable<AiModelCapability> capabilities)
        {
            // Create the descriptor ..
            AiModelDescriptor descriptor = new()
            {
                Capabilities = new HashSet<AiModelCapability>(capabilities),
                Pricing = new ModelPricing(cost, cost),
                Domain = ModelDomain.Other,
                Generation = OpenAIModelGeneration.Other,
            };

            // and set the descriptor's internal property "Name" via reflection
            typeof(AiModelDescriptor)
                .GetProperty(name: nameof(AiModelDescriptor.Name))!
                .SetValue(obj: descriptor, value: aiModel);

            return descriptor;
        }
    }
}