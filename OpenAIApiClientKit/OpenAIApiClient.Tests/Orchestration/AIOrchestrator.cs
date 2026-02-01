// <copyright file="AIOrchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.OrchestrationNEW04;
    using OpenAIApiClient.Tests.Orchestration.Mocks.OrchestrationNEW04;
    using testClass = OpenAIApiClient.OrchestrationNEW04.AIOrchestrator;

    /// <summary>
    /// Tests for the AIOrchestrator class.
    /// </summary>
    [TestClass]
    public class AIOrchestrator
    {
        private MockSingleModelRouter singleRouter = null!;
        private MockEnsembleRouter ensembleRouter = null!;
        private MockSingleModelExecutor singleExecutor = null!;
        private MockEnsembleExecutor ensembleExecutor = null!;
        private MockResponseHandler responseHandler = null!;
        private ClientRequestBuilder requestBuilder = null!;
        private OpenAIApiClient.OrchestrationNEW04.AIOrchestrator orchestrator = null!;

        private ModelDescriptor modelA = null!;
        private ModelDescriptor modelB = null!;

        [TestInitialize]
        public void Setup()
        {
            this.modelA = CreateModelDescriptor(OpenAIModel.GPT4o);
            this.modelB = CreateModelDescriptor(OpenAIModel.GPT4o_Mini);

            this.singleRouter = new MockSingleModelRouter();
            this.ensembleRouter = new MockEnsembleRouter();
            this.singleExecutor = new MockSingleModelExecutor();
            this.ensembleExecutor = new MockEnsembleExecutor();
            this.responseHandler = new MockResponseHandler();
            this.requestBuilder = new ClientRequestBuilder();

            this.orchestrator = new testClass(
                this.singleRouter,
                this.ensembleRouter,
                this.singleExecutor,
                this.ensembleExecutor,
                this.requestBuilder,
                this.responseHandler);
        }

        // ------------------------------------------------------------
        // SINGLE MODEL TESTS
        // ------------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_UsesSingleModelRouter_WhenUseEnsembleFalse()
        {
            this.singleRouter.ReturnedModel = this.modelA;
            this.singleExecutor.ResponseToReturn = new ModelResponse { Model = this.modelA, RawOutput = "ok", IsSuccessful = true };
            this.responseHandler.ResponsesToReturn = [this.singleExecutor.ResponseToReturn];

            var request = new OrchestrationRequest
            {
                UseEnsemble = false,
                Prompt = "Hello",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleModelRouterRequest { Strategy = ModelRoutingStrategy.BestReasoning },
            };

            var result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            Assert.HasCount(1, result);
            Assert.AreEqual(this.modelA, result[0].Model);

            Assert.IsNotNull(this.singleRouter.LastRequest);
            Assert.IsNull(this.ensembleRouter.LastRequest);

            Assert.IsNotNull(this.singleExecutor.LastCall);
            Assert.AreEqual("Hello", this.singleExecutor.LastCall.Value.context.Prompt);
        }

        [TestMethod]
        public async Task ProcessAsync_SingleModel_InvokesResponseHandler()
        {
            this.singleRouter.ReturnedModel = this.modelA;

            var modelResponse = new ModelResponse { Model = this.modelA, RawOutput = "done", IsSuccessful = true };
            this.singleExecutor.ResponseToReturn = modelResponse;

            this.responseHandler.ResponsesToReturn = [modelResponse];

            var request = new OrchestrationRequest
            {
                UseEnsemble = false,
                Prompt = "Test",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleModelRouterRequest { Strategy = ModelRoutingStrategy.LowestCost },
            };

            var result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            Assert.AreEqual(this.responseHandler.ResponsesToReturn, result);
            Assert.AreEqual(modelResponse, this.responseHandler.LastResponses![0]);
        }

        // ------------------------------------------------------------
        // ENSEMBLE TESTS
        // ------------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_UsesEnsembleRouter_WhenUseEnsembleTrue()
        {
            this.ensembleRouter.ReturnedModels = [this.modelA, this.modelB];

            var responses = new[]
            {
                new ModelResponse { Model = this.modelA, RawOutput = "A", IsSuccessful = true },
                new ModelResponse { Model = this.modelB, RawOutput = "B", IsSuccessful = true },
            };

            this.ensembleExecutor.ResponsesToReturn = responses;
            this.responseHandler.ResponsesToReturn = responses;

            var request = new OrchestrationRequest
            {
                UseEnsemble = true,
                Prompt = "Explain",
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleRouterRequest { Strategy = EnsembleRoutingStrategy.Reasoning },
            };

            var result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            Assert.HasCount(2, result);
            Assert.AreEqual(this.modelA, result[0].Model);
            Assert.AreEqual(this.modelB, result[1].Model);

            Assert.IsNotNull(this.ensembleRouter.LastRequest);
            Assert.IsNull(this.singleRouter.LastRequest);

            Assert.IsNotNull(this.ensembleExecutor.LastContext);
            Assert.AreEqual("Explain", this.ensembleExecutor.LastContext!.Prompt);
        }

        [TestMethod]
        public async Task ProcessAsync_Ensemble_InvokesResponseHandler()
        {
            this.ensembleRouter.ReturnedModels = [this.modelA, this.modelB];

            var responses = new[]
            {
                new ModelResponse { Model = this.modelA, RawOutput = "A", IsSuccessful = true },
                new ModelResponse { Model = this.modelB, RawOutput = "B", IsSuccessful = true },
            };

            this.ensembleExecutor.ResponsesToReturn = responses;
            this.responseHandler.ResponsesToReturn = responses;

            var request = new OrchestrationRequest
            {
                UseEnsemble = true,
                Prompt = "Explain",
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleRouterRequest { Strategy = EnsembleRoutingStrategy.Reasoning },
            };

            var result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            Assert.AreEqual(this.responseHandler.ResponsesToReturn, result);
            Assert.AreEqual(responses[0], this.responseHandler.LastResponses![0]);
            Assert.AreEqual(responses[1], this.responseHandler.LastResponses![1]);
        }

        /// <summary>
        /// Creates a ModelDescriptor instance for the specified model name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="ModelDescriptor">.</returns>
        private static ModelDescriptor CreateModelDescriptor(OpenAIModel name)
        {
            var descriptor = (ModelDescriptor)Activator.CreateInstance(typeof(ModelDescriptor), nonPublic: true)!;

            typeof(ModelDescriptor)
                .GetProperty(nameof(ModelDescriptor.Name))!
                .SetValue(descriptor, name);

            return descriptor;
        }
    }
}
