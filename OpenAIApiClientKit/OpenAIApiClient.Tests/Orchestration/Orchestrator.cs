// <copyright file="Orchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Tests.Orchestration.Mocks;
    using testClass = OpenAIApiClient.Orchestration.Orchestrator;

    /// <summary>
    /// Tests for the <see cref="Orchestrator"/> class.
    /// </summary>
    [TestClass]
    public class Orchestrator
    {
        private MockSingleModelRouter mockSingleModelRouter = null!;
        private MockEnsembleRouter mockEnsembleRouter = null!;
        private MockSingleModelExecutor mockSingleExecutor = null!;
        private MockEnsembleExecutor mockEnsembleExecutor = null!;
        private MockResponseHandler mockResponseHandler = null!;
        private ClientRequestBuilder requestBuilder = null!;
        private testClass orchestrator = null!;

        private ModelDescriptor modelA = null!;
        private ModelDescriptor modelB = null!;

        [TestInitialize]
        public void Setup()
        {
            this.modelA = CreateModelDescriptor(OpenAIModel.GPT4o);
            this.modelB = CreateModelDescriptor(OpenAIModel.GPT4o_Mini);

            this.mockSingleModelRouter = new MockSingleModelRouter();
            this.mockEnsembleRouter = new MockEnsembleRouter();
            this.mockSingleExecutor = new MockSingleModelExecutor();
            this.mockEnsembleExecutor = new MockEnsembleExecutor();
            this.mockResponseHandler = new MockResponseHandler();
            this.requestBuilder = new ClientRequestBuilder();

            this.orchestrator = new testClass(singleModelDispatcher: this.mockSingleModelRouter,
                                              ensembleDispatcher: this.mockEnsembleRouter,
                                              singleModelExecutor: this.mockSingleExecutor,
                                              ensembleExecutor: this.mockEnsembleExecutor,
                                              requestBuilder: this.requestBuilder,
                                              responseHandler: this.mockResponseHandler);
        }

        // ------------------------------------------------------------
        // SINGLE MODEL TESTS
        // ------------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_UsesSingleModelRouter_WhenUseEnsembleFalse()
        {
            // Arrange
            this.mockSingleModelRouter.ReturnedModel = this.modelA;
            this.mockSingleExecutor.ResponseToReturn = new ModelResponse { Model = this.modelA, RawOutput = "ok", IsSuccessful = true };
            this.mockResponseHandler.ResponsesToReturn = [this.mockSingleExecutor.ResponseToReturn];

            OrchestrationRequest request = new()
            {
                UseEnsemble = false,
                Prompt = "Hello",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleModelDispatchRequest { Strategy = SingleModelStrategy.BestReasoning },
            };

            // Act
            IReadOnlyList<ModelResponse> result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.HasCount(1, result);
            Assert.AreEqual(this.modelA, result[0].Model);

            Assert.IsNotNull(this.mockSingleModelRouter.LastRequest);
            Assert.IsNull(this.mockEnsembleRouter.LastRequest);

            Assert.IsNotNull(this.mockSingleExecutor.LastCall);
            Assert.AreEqual("Hello", this.mockSingleExecutor.LastCall.Value.context.Prompt);
        }

        [TestMethod]
        public async Task ProcessAsync_SingleModel_InvokesResponseHandler()
        {
            // Arrange
            this.mockSingleModelRouter.ReturnedModel = this.modelA;

            ModelResponse modelResponse = new()
            {
                Model = this.modelA,
                RawOutput = "done",
                IsSuccessful = true,
            };

            this.mockSingleExecutor.ResponseToReturn = modelResponse;
            this.mockResponseHandler.ResponsesToReturn = [modelResponse];

            OrchestrationRequest request = new()
            {
                UseEnsemble = false,
                Prompt = "Test",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleModelDispatchRequest
                                     {
                                        Strategy = SingleModelStrategy.LowestCost,
                                     },
            };

            // Act
            IReadOnlyList<ModelResponse> result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(this.mockResponseHandler.ResponsesToReturn, result);
            Assert.AreEqual(modelResponse, this.mockResponseHandler.LastResponses![0]);
        }

        // ------------------------------------------------------------
        // ENSEMBLE TESTS
        // ------------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_UsesEnsembleRouter_WhenUseEnsembleTrue()
        {
            // Arrange
            string prompt = "Explain";
            this.mockEnsembleRouter.ReturnedModels = [this.modelA, this.modelB];

            ModelResponse[] responses =
            [
                new ModelResponse {
                                    Model = this.modelA,
                                    RawOutput = "A",
                                    IsSuccessful = true,
                                  },
                new ModelResponse {
                                    Model = this.modelB,
                                    RawOutput = "B",
                                    IsSuccessful = true,
                                  },
            ];

            this.mockEnsembleExecutor.ResponsesToReturn = responses;
            this.mockResponseHandler.ResponsesToReturn = responses;

            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest { Strategy = EnsembleStrategy.Reasoning },
            };

            // Act
            IReadOnlyList<ModelResponse> result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.HasCount(2, result);
            Assert.AreEqual(this.modelA, result[0].Model);
            Assert.AreEqual(this.modelB, result[1].Model);

            Assert.IsNotNull(this.mockEnsembleRouter.LastRequest);
            Assert.IsNull(this.mockSingleModelRouter.LastRequest);

            Assert.IsNotNull(this.mockEnsembleExecutor.LastContext);
            Assert.AreEqual(prompt, this.mockEnsembleExecutor.LastContext!.Prompt);
        }

        [TestMethod]
        public async Task ProcessAsync_Ensemble_InvokesResponseHandler()
        {
            // Arrange
            string prompt = "Explain";
            this.mockEnsembleRouter.ReturnedModels = [this.modelA, this.modelB];

            ModelResponse[] responses =
            [
                new ModelResponse {
                                    Model = this.modelA,
                                    RawOutput = "A",
                                    IsSuccessful = true,
                                  },
                new ModelResponse {
                                    Model = this.modelB,
                                    RawOutput = "B",
                                    IsSuccessful = true,
                                  },
            ];

            this.mockEnsembleExecutor.ResponsesToReturn = responses;
            this.mockResponseHandler.ResponsesToReturn = responses;

            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest { Strategy = EnsembleStrategy.Reasoning },
            };

            // Act
            IReadOnlyList<ModelResponse> result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(this.mockResponseHandler.ResponsesToReturn, result);
            Assert.AreEqual(responses[0], this.mockResponseHandler.LastResponses![0]);
            Assert.AreEqual(responses[1], this.mockResponseHandler.LastResponses![1]);
        }

        /// <summary>
        /// Creates a ModelDescriptor instance for the specified model name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="ModelDescriptor">.</returns>
        private static ModelDescriptor CreateModelDescriptor(OpenAIModel name)
        {
            // Use reflection to create an instance since the constructor is internal.
            ModelDescriptor modelDescriptor = (ModelDescriptor)Activator.CreateInstance(typeof(ModelDescriptor), nonPublic: true)!;

            // Set the Name property using reflection since it has an internal setter.
            typeof(ModelDescriptor)
                .GetProperty(nameof(ModelDescriptor.Name))!
                .SetValue(modelDescriptor, name);

            return modelDescriptor;
        }
    }
}
