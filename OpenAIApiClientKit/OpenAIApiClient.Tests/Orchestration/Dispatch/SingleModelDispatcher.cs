// <copyright file="SingleModelDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Dispatch
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.Dispatch;
    using testClass = OpenAIApiClient.Orchestration.Dispatch.SingleModelDispatcher;

    [TestClass]
    public sealed class SingleModelDispatcher
    {
        [TestInitialize]
        public void Init()
        {
            // Important cleanup of custom handler registries on every test method start ..
            SingleModelStrategies.ClearCustomHandlers();
            EnsembleStrategies.ClearCustomHandlers();
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Important cleanup of custom handler registries on every test method end ..
            SingleModelStrategies.ClearCustomHandlers();
            EnsembleStrategies.ClearCustomHandlers();
        }

        [TestMethod]
        public void Evaluate_Throws_WhenRequestIsNull()
        {
            // Arrange
            Dictionary<OpenAIModel, ModelDescriptor> registry = [];
            testClass dispatcher = new(registry);

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
        public void Evaluate_UsesRegisteredStrategyHandler()
        {
            // Arrange
            OpenAIModel modelToUse = OpenAIModel.GPT4o;
            IReadOnlyList<ModelCapability> capabilities = [ModelCapability.Reasoning];

            // Create a model descriptor for testing
            ModelDescriptor model = CreateDescriptor(model: modelToUse, cost: 1.0m, capabilities: capabilities);

            // Create a registry with the model
            Dictionary<OpenAIModel, ModelDescriptor> registry = new()
            {
                { modelToUse, model },
            };

            // Create the expected result
            SingleModelDispatchResult expected = new(model);

            // Register a custom strategy handler
            SingleModelStrategies.RegisterCustomHandler(strategy: SingleModelStrategy.BestReasoning, handler: (_, _) => expected);

            // Create the dispatcher ..
            testClass dispatcher = new(registry);

            // Define the request ..
            SingleModelDispatchRequest request = new()
            {
                Strategy = SingleModelStrategy.BestReasoning,
            };

            // Act
            SingleModelDispatchResult result = dispatcher.Evaluate(request);

            // Assert
            Assert.AreSame(expected, result);
        }

        [TestMethod]
        public void Evaluate_PassesCorrectContextToHandler()
        {
            // Arrange
            OpenAIModel modelToUse = OpenAIModel.GPT4o;
            IReadOnlyList<ModelCapability> capabilities = [ModelCapability.Chat];
            SingleModelStrategy strategy = SingleModelStrategy.LowestCost;

            // Create a model descriptor for testing
            ModelDescriptor model = CreateDescriptor(modelToUse, 1.0m, capabilities);

            // Create a registry with the model
            Dictionary<OpenAIModel, ModelDescriptor> registry = new()
            {
                { modelToUse, model },
            };

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
            testClass dispatcher = new(registry);

            SingleModelDispatchRequest request = new()
            {
                Strategy = strategy,
                ExplicitModel = modelToUse,
                RequiredCapabilities = capabilities,
            };

            // Act
            dispatcher.Evaluate(request);

            // Assert
            Assert.IsNotNull(capturedContext);
            Assert.AreEqual(modelToUse, capturedContext!.ExplicitModel);
            Assert.AreEqual(strategy, capturedContext.Strategy);
            Assert.IsTrue(capturedContext.RequiredCapabilities!.Contains(ModelCapability.Chat));
        }

        [TestMethod]
        public void Evaluate_Throws_WhenStrategyNotRegistered()
        {
            // Arrange
            OpenAIModel modelToUse = OpenAIModel.GPT4o;
            IReadOnlyList<ModelCapability> capabilities = [ModelCapability.Reasoning];

            // Create a model descriptor for testing
            ModelDescriptor model = CreateDescriptor(modelToUse, 1.0m, capabilities);

            // Create a registry with the model
            Dictionary<OpenAIModel, ModelDescriptor> registry = new()
            {
                { modelToUse, model },
            };

            // Create the dispatcher ..
            testClass dispatcher = new(registry);

            SingleModelDispatchRequest request = new()
            {
                Strategy = (SingleModelStrategy)999, // use an undefined and unregistered strategy
            };

            // Act and assert ..
            try
            {
                dispatcher.Evaluate(request);
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
            OpenAIModel modelToUse = OpenAIModel.GPT4o_Mini;
            IReadOnlyList<ModelCapability> capabilities = [ModelCapability.Chat];
            SingleModelStrategy strategy = SingleModelStrategy.HighestPerformance;

            // Create a model descriptor for testing
            ModelDescriptor model = CreateDescriptor(modelToUse, 1.0m, capabilities);

            // Create a registry with the model
            Dictionary<OpenAIModel, ModelDescriptor> registry = new()
            {
                { modelToUse, model },
            };

            SingleModelDispatchResult expected = new(model);

            // Register a custom strategy handler
            SingleModelStrategies.RegisterCustomHandler(strategy: strategy, handler: (_, _) => expected); // lambda expression is implicitly type: (_, _) => expected

            // Create the dispatcher ..
            testClass dispatcher = new(registry);

            SingleModelDispatchRequest request = new()
            {
                Strategy = strategy,
            };

            // Act
            SingleModelDispatchResult result = dispatcher.Evaluate(request);

            // Assert
            Assert.AreSame(expected, result);
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