// <copyright file="EnsembleDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Dispatch
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.Dispatch;
    using OpenAIApiClient.Tests.Orchestration.Mocks;
    using testClass = OpenAIApiClient.Orchestration.Dispatch.EnsembleDispatcher;

    [TestClass]
    public sealed class EnsembleDispatcher
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
            Dictionary<string, AiModelPropertyRegistryModel> internalRegistry = [];
            IAiModelRegistryNEW registry = new MockOpenAIModels(models: internalRegistry);
            testClass dispatcher = new(modelRegistry: registry);

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
        public void Evaluate_UsesStrategyHandler_ForCustomStrategy()
        {
            //// Arrange
            //OpenAIModel modelToUse = OpenAIModel.GPT4o;
            //IReadOnlyList<AiModelCapability> capabilities = [AiModelCapability.Reasoning];

            //// Get the mock registry, the model descriptor and the expected result as a tuple ..
            //(IAiModelRegistryNEW aiModels, AiModelDescriptor _, EnsembleDispatchResult expected) = Cook(aiModel: modelToUse, capabilities: capabilities);

            //// Register a custom strategy handler
            //EnsembleStrategies.RegisterCustomHandler(strategy: EnsembleStrategy.Reasoning, handler: _ => expected); // EnsembleStrategies.Register(strategy: EnsembleStrategy.Reasoning, handler: (IReadOnlyDictionary<OpenAIModel, ModelDescriptor> _) => expected);

            //// Create the dispatcher ..
            //testClass dispatcher = new(modelRegistry: aiModels);

            //// Define the request ..
            //EnsembleDispatchRequest request = new()
            //{
            //    Strategy = EnsembleStrategy.Custom,
            //    RequiredCapabilities = capabilities,
            //    ExplicitModels = [modelToUse],
            //};

            //// Act
            //EnsembleDispatchResult result = dispatcher.Evaluate(request: request);

            //// Assert
            //Assert.HasCount(1, result.Models);
            //Assert.AreSame(expected.Models[0], result.Models[0]);
        }

        [TestMethod]
        public void Evaluate_CustomStrategy_ReturnsModelsMatchingCapabilities()
        {
            //// Arrange
            //OpenAIModel aiModel1 = OpenAIModel.GPT4o;
            //OpenAIModel aiModel2 = OpenAIModel.GPT4o_Mini;
            //OpenAIModel aiModel3 = OpenAIModel.GPT4_Turbo;

            //// Create model descriptors ..
            //AiModelDescriptor m1 = CreateDescriptor(aiModel1, 1.0m, [AiModelCapability.Reasoning, AiModelCapability.Vision]);
            //AiModelDescriptor m2 = CreateDescriptor(aiModel2, 0.5m, [AiModelCapability.Reasoning]);
            //AiModelDescriptor m3 = CreateDescriptor(aiModel3, 2.0m, [AiModelCapability.Vision]);

            //// Create a registry with the models ..
            //Dictionary<OpenAIModel, AiModelDescriptor> internalRegistry = new()
            //{
            //    { aiModel1, m1 },
            //    { aiModel2, m2 },
            //    { aiModel3, m3 },
            //};

            //// Define a mock registry that returns the internal registry ..
            //MockOpenAIModels aiModels = new(models: internalRegistry);

            //// Create the dispatcher ..
            //testClass dispatcher = new(aiModels);

            //// Define the request ..
            //EnsembleDispatchRequest request = new()
            //{
            //    Strategy = EnsembleStrategy.Custom,
            //    RequiredCapabilities = [AiModelCapability.Reasoning],
            //};

            //// Act
            //EnsembleDispatchResult result = dispatcher.Evaluate(request: request);

            //// Assert
            //Assert.HasCount(2, result.Models);
            //Assert.Contains(m => m.Name == m1.Name, result.Models, "Expected model m1 was not found in the result set.");
            //Assert.Contains(m => m.Name == m2.Name, result.Models, "Expected model m2 was not found in the result set.");
            //Assert.AreEqual(m2, result.Models[0]);
        }

        [TestMethod]
        public void Evaluate_CustomStrategy_Throws_WhenNoCapabilitiesProvided()
        {
            //// Arrange
            //Dictionary<OpenAIModel, AiModelDescriptor> registry = [];
            //MockOpenAIModels aiModels = new(models: registry);

            //testClass dispatcher = new(registry: aiModels);

            //EnsembleDispatchRequest request = new()
            //{
            //    Strategy = EnsembleStrategy.Custom,
            //    RequiredCapabilities = [],
            //};

            //// Act & Assert
            //try
            //{
            //    dispatcher.Evaluate(request: request);
            //    Assert.Fail("Expected InvalidOperationException was not thrown.");
            //}
            //catch (InvalidOperationException)
            //{
            //    // success
            //}
        }

        [TestMethod]
        public void Evaluate_CustomStrategy_Throws_WhenNoModelsMatch()
        {
            //// Arrange
            //OpenAIModel modelToUse = OpenAIModel.GPT4o;
            //IReadOnlyList<AiModelCapability> capabilities = [AiModelCapability.Reasoning];

            //// Get the mock registry, the model descriptor and the expected result as a tuple ..
            //(IAiModelRegistryNEW aiModels, AiModelDescriptor _, EnsembleDispatchResult _) = Cook(aiModel: modelToUse, capabilities: capabilities);

            //// Create the dispatcher ..
            //testClass dispatcher = new(registry: aiModels);

            //// Define the request ..
            //EnsembleDispatchRequest request = new()
            //{
            //    Strategy = EnsembleStrategy.Custom,
            //    RequiredCapabilities = [AiModelCapability.Vision],
            //};

            //// Act & Assert
            //try
            //{
            //    dispatcher.Evaluate(request: request);
            //    Assert.Fail("Expected InvalidOperationException was not thrown.");
            //}
            //catch (InvalidOperationException)
            //{
            //    // success
            //}
        }

        /// <summary>
        /// Gets the mock registry, the model descriptor and the expected dispatch result as a tuple for the specified model and capabilities.
        /// </summary>
        /// <param name="aiModel"></param>
        /// <param name="capabilities"></param>
        /// <returns see cref="(IAiModelRegistryNEW, AiModelPropertyRegistryModel, EnsembleDispatchResult)">.</returns>
        //private static (IAiModelRegistryNEW, AiModelDescriptor, EnsembleDispatchResult) Cook(OpenAIModel aiModel, IReadOnlyList<AiModelCapability> capabilities)
        //{
        //    //// Create the test model descriptor ..
        //    //AiModelDescriptor m1 = CreateDescriptor(aiModel: aiModel, cost: 1.0m, capabilities: capabilities);

        //    //// Define an internal registry with one, single entry for the test model ..
        //    //Dictionary<OpenAIModel, AiModelDescriptor> internalRegistry = new()
        //    //{
        //    //    { aiModel, m1 },
        //    //};

        //    //// Define the mock registry that returns the internal registry ..
        //    //MockOpenAIModels mockRegistry = new(models: internalRegistry);

        //    //// Register a custom handler for the Reasoning strategy ..
        //    //EnsembleDispatchResult expected = new(models: [m1]);

        //    //// Return the mock registry, the model descriptor and the expected dispatch result as a tuple ..
        //    //return (mockRegistry, m1, expected);
        //}

        /// <summary>
        /// Creates a ModelDescriptor for the specified model, cost, and capabilities.
        /// </summary>
        /// <param name="aiModel"></param>
        /// <param name="cost"></param>
        /// <param name="capabilities"></param>
        /// <returns see cref="AiModelDescriptor">.</returns>
        //private static AiModelPropertyRegistryModel CreateDescriptor(string aiModel, decimal cost, IEnumerable<AiModelCapability> capabilities)
        //{
        //    // Create the descriptor ..
        //    AiModelPropertyRegistryModel descriptor = new()
        //    {
        //        Capabilities = new HashSet<AiModelCapability>(capabilities),
        //        Pricing = new AiModelPricing(cost, cost),
        //        Domain = ModelDomain.Other,
        //        Generation = OpenAIModelGeneration.Other,
        //    };

        //    // and set the descriptor's internal property "Name" via reflection
        //    typeof(AiModelPropertyRegistryModel)
        //        .GetProperty(name: nameof(AiModelPropertyRegistryModel.Name))!
        //        .SetValue(obj: descriptor, value: aiModel);

        //    return descriptor;
        //}
    }
}
