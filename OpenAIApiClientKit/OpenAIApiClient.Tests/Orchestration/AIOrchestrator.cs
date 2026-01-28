// <copyright file="AIOrchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Registries;
    using OpenAIApiClient.Routing.Ensemble;
    using OpenAIApiClient.Routing.SingleModel;
    using OpenAIApiClient.Tests.Orchestration.Mocks;
    using testClass = OpenAIApiClient.Orchestration.AIOrchestrator;

    /// <summary>
    /// Tests for the AIOrchestrator class.
    /// </summary>
    [TestClass]
    public class AIOrchestrator
    {
        private SingleModelRouter? singleRouter;
        private EnsembleRouter? ensembleRouter;
        private MockModelExecutor? modelExecutor;
        private MockEnsembleExecutor? ensembleExecutor;
        private MockResponseHandler? responseHandler;
        private testClass? orchestrator;

        [TestInitialize]
        public void Setup()
        {
            Dictionary<OpenAIModel, OpenAIApiClient.Models.Registries.ModelDescriptor> registry = new OpenAIModelRegistry().Registry;

            this.singleRouter = new SingleModelRouter(registry);
            this.ensembleRouter = new EnsembleRouter(registry);

            this.modelExecutor = new MockModelExecutor();
            this.ensembleExecutor = new MockEnsembleExecutor();
            this.responseHandler = new MockResponseHandler();

            this.orchestrator = new testClass(
                this.singleRouter,
                this.ensembleRouter,
                this.modelExecutor,
                this.ensembleExecutor,
                this.responseHandler);
        }

        // ---------------------------------------------------------
        // Single Model Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_ShouldUseSingleModelRouter_WhenUseEnsembleIsFalse()
        {
            OrchestrationContext context = new()
            {
                UseEnsemble = false,
                Prompt = "Hello world",
                SingleModelContext = new SingleModelContext
                {
                    Strategy = ModelRoutingStrategy.BestReasoning,
                },
            };

            string result = await this.orchestrator!.ProcessAsync(context: context);

            Assert.StartsWith("HandledSingle:", result);
            Assert.HasCount(1, this.modelExecutor!.Calls);
            Assert.AreEqual("Hello world", this.modelExecutor.Calls[0].prompt);
            Assert.IsNull(this.responseHandler!.LastEnsembleInput);
        }

        [TestMethod]
        public async Task ProcessAsync_ShouldReturnHandledSingleResponse()
        {
            OrchestrationContext context = new()
            {
                UseEnsemble = false,
                Prompt = "Test prompt",
                SingleModelContext = new SingleModelContext
                {
                    Strategy = ModelRoutingStrategy.LowestCost,
                },
            };

            string result = await this.orchestrator!.ProcessAsync(context: context);

            Assert.AreEqual($"HandledSingle:{this.responseHandler!.LastSingleInput}", result);
        }

        // ---------------------------------------------------------
        // Ensemble Routing Tests
        // ---------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_ShouldUseEnsembleRouter_WhenUseEnsembleIsTrue()
        {
            OrchestrationContext context = new()
            {
                UseEnsemble = true,
                Prompt = "Explain gravity",
                EnsembleContext = new EnsembleContext
                {
                    Strategy = EnsembleRoutingStrategy.Reasoning,
                },
            };

            string result = await this.orchestrator!.ProcessAsync(context: context);

            Assert.StartsWith("HandledEnsemble:", result);
            Assert.HasCount(1, this.ensembleExecutor!.Calls);
            Assert.AreEqual("Explain gravity", this.ensembleExecutor!.Calls[0].prompt);
            Assert.HasCount(3, this.ensembleExecutor!.Calls[0].models); // Reasoning ensemble size
        }

        [TestMethod]
        public async Task ProcessAsync_ShouldReturnHandledEnsembleResponse()
        {
            OrchestrationContext context = new()
            {
                UseEnsemble = true,
                Prompt = "Combine outputs",
                EnsembleContext = new EnsembleContext
                {
                    Strategy = EnsembleRoutingStrategy.Vision,
                },
            };

            string result = await this.orchestrator!.ProcessAsync(context: context);

            Assert.AreEqual($"HandledEnsemble:{string.Join("|", this.responseHandler!.LastEnsembleInput!)}", result);
        }

        // ---------------------------------------------------------
        // Routing Behavior Tests
        // ---------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_ShouldNotCallSingleExecutor_WhenUsingEnsemble()
        {
            OrchestrationContext context = new()
            {
                UseEnsemble = true,
                Prompt = "Test",
                EnsembleContext = new EnsembleContext
                {
                    Strategy = EnsembleRoutingStrategy.CostOptimized,
                },
            };

            _ = await this.orchestrator!.ProcessAsync(context: context);

            Assert.IsEmpty(this.modelExecutor!.Calls);
            Assert.HasCount(1, this.ensembleExecutor!.Calls);
        }

        [TestMethod]
        public async Task ProcessAsync_ShouldNotCallEnsembleExecutor_WhenUsingSingleModel()
        {
            OrchestrationContext context = new()
            {
                UseEnsemble = false,
                Prompt = "Test",
                SingleModelContext = new SingleModelContext
                {
                    Strategy = ModelRoutingStrategy.HighestPerformance,
                },
            };

            await this.orchestrator!.ProcessAsync(context: context);

            Assert.HasCount(1, this.modelExecutor!.Calls);
            Assert.IsEmpty(this.ensembleExecutor!.Calls);
        }
    }
}
