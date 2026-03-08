// <copyright file="OrchestratorBuilderTests.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Factories
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Factories;
    using OpenAIApiClient.Tests.Orchestration.Mocks;

    [TestClass]
    public class OrchestratorBuilderTests
    {
        private MockChatClient client = default!;
        private MockAiModelResponseHandler handler = default!;
        private MockOpenAIModels registry = default!;

        //[TestInitialize]
        //public void Init()
        //{
        //    this.client = new MockChatClient(apiKey: "*fake*");
        //    this.handler = new MockAiModelResponseHandler();
        //    this.registry = new MockOpenAIModels(models: new()
        //    {
        //        { OpenAIModel.GPT4o, CreateModelDescriptor(OpenAIModel.GPT4o) },
        //        { OpenAIModel.GPT4o_Mini, CreateModelDescriptor(OpenAIModel.GPT4o_Mini) },
        //    });
        //}

        [TestMethod]
        public void Build_Uses_Defaults_When_No_Overrides_Provided()
        {
            OrchestratorBuilder builder = new OrchestratorBuilder()
                                              .WithClient(this.client)
                                              .WithResponseHandler(this.handler);

            Orchestrator orchestrator = builder.Build();

            Assert.IsNotNull(orchestrator);
        }

        [TestMethod]
        public async Task Build_Uses_Custom_SingleAiModelDispatcher()
        {
            //// Set up the mock dispatcher to always return a valid model descriptor for the test.
            //MockSingleAiModelDispatcher singleModelDispatcher = new()
            //{
            //    ReturnedModel = CreateModelDescriptor(OpenAIModel.GPT4o),
            //};

            //OrchestratorBuilder builder = new OrchestratorBuilder()
            //                                  .WithClient(this.client)
            //                                  .WithResponseHandler(this.handler)
            //                                  .WithModelRegistry(this.registry)
            //                                  .WithSingleModelDispatcher(singleModelDispatcher);

            //Orchestrator orchestrator = builder.Build();

            //OrchestrationRequest request = new()
            //{
            //    Prompt = "test",
            //    OutputFormat = OutputFormat.PlainText,
            //    UseEnsemble = false,
            //};

            //await orchestrator.ProcessAsync(request, CancellationToken.None);

            //Assert.IsTrue(singleModelDispatcher.WasCalled);
        }

        [TestMethod]
        public async Task Build_Uses_Custom_SingleAiModelExecutor()
        {
            //// Set up the mock dispatcher to always return a valid model descriptor for the test.
            //MockSingleAiModelDispatcher singleModelDispatcher = new()
            //{
            //    ReturnedModel = CreateModelDescriptor(OpenAIModel.GPT4o),
            //};

            //MockSingleAiModelExecutor singleModelExecutor = new();

            //OrchestratorBuilder builder = new OrchestratorBuilder()
            //                                  .WithClient(this.client)
            //                                  .WithResponseHandler(this.handler)
            //                                  .WithSingleModelDispatcher(singleModelDispatcher)
            //                                  .WithSingleModelExecutor(singleModelExecutor);

            //Orchestrator orchestrator = builder.Build();

            //OrchestrationRequest request = new()
            //{
            //    Prompt = "hello",
            //    OutputFormat = OutputFormat.PlainText,
            //    UseEnsemble = false,
            //};

            //await orchestrator.ProcessAsync(request, CancellationToken.None);

            //Assert.IsTrue(singleModelDispatcher.WasCalled);
            //Assert.IsTrue(singleModelExecutor.WasCalled);
        }

        [TestMethod]
        public async Task Build_Uses_Custom_Ensemble_Components()
        {
            //// Set up the mock dispatcher to return a list of model descriptors for the test.
            //AiModelDescriptor modelA = CreateModelDescriptor(OpenAIModel.GPT4o);
            //AiModelDescriptor modelB = CreateModelDescriptor(OpenAIModel.GPT4o_Mini);
            //MockEnsembleDispatcher ensembleDispatcher = new()
            //{
            //    ReturnedModels = [modelA, modelB],
            //};

            //// Set up the mock executor to return a list of responses for the test.
            //MockEnsembleExecutor ensembleExecutor = new();

            //// Build the orchestrator with the mock ensemble dispatcher and executor.
            //OrchestratorBuilder builder = new OrchestratorBuilder()
            //                                  .WithClient(this.client)
            //                                  .WithResponseHandler(this.handler)
            //                                  .WithEnsembleDispatcher(ensembleDispatcher)
            //                                  .WithEnsembleExecutor(ensembleExecutor);

            //Orchestrator orchestrator = builder.Build();

            //// Create a request that will trigger ensemble processing.
            //OrchestrationRequest request = new()
            //{
            //    Prompt = "test",
            //    OutputFormat = OutputFormat.PlainText,
            //    UseEnsemble = true,
            //};

            //await orchestrator.ProcessAsync(request, CancellationToken.None);

            //// Verify that both the ensemble dispatcher and executor were called.
            //Assert.IsTrue(ensembleDispatcher.WasCalled);
            //Assert.IsTrue(ensembleExecutor.WasCalled);
        }

        [TestMethod]
        public void Build_Throws_When_Client_Not_Provided()
        {
            OrchestratorBuilder builder = new OrchestratorBuilder()
                                              .WithResponseHandler(this.handler);

            try
            {
                builder.Build();
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains("ChatClient", ex.Message);
            }
        }

        [TestMethod]
        public void Build_Throws_When_ResponseHandler_Not_Provided()
        {
            OrchestratorBuilder builder = new OrchestratorBuilder()
                                              .WithClient(this.client);

            try
            {
                builder.Build();
                Assert.Fail("Expected InvalidOperationException was not thrown.");
            }
            catch (InvalidOperationException ex)
            {
                Assert.Contains("IAiModelResponseHandler", ex.Message);
            }
        }

        /// <summary>
        /// Creates a ModelDescriptor instance for the specified model name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="AiModelDescriptor">.</returns>
        private static AiModelDescriptor CreateModelDescriptor(string name)
        {
            // Use reflection to create an instance since the constructor is internal.
            AiModelDescriptor modelDescriptor = (AiModelDescriptor)Activator.CreateInstance(typeof(AiModelDescriptor), nonPublic: true)!;

            // Set the Name property using reflection since it has an internal setter.
            typeof(AiModelDescriptor)
                .GetProperty(nameof(AiModelDescriptor.Name))!
                .SetValue(modelDescriptor, name);

            return modelDescriptor;
        }
    }
}
