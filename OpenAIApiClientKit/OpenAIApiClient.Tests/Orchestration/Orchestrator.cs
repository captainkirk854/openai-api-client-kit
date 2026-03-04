// <copyright file="Orchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;
    using OpenAIApiClient.Tests.Orchestration.Mocks;
    using testClass = OpenAIApiClient.Orchestration.Orchestrator;

    /// <summary>
    /// Tests for the <see cref="Orchestrator"/> class.
    /// </summary>
    [TestClass]
    public class Orchestrator
    {
        private MockSingleAiModelDispatcher mockSingleModelDispatcher = null!;
        private MockEnsembleDispatcher mockEnsembleDispatcher = null!;
        private MockSingleAiModelExecutor mockSingleExecutor = null!;
        private MockEnsembleExecutor mockEnsembleExecutor = null!;
        private MockAiModelResponseHandler mockModelResponseHandler = null!;
        private testClass orchestrator = null!;

        private AiModelDescriptor modelA = null!;
        private AiModelDescriptor modelB = null!;

        [TestInitialize]
        public void Setup()
        {
            this.modelA = CreateModelDescriptor(OpenAIModel.GPT4o);
            this.modelB = CreateModelDescriptor(OpenAIModel.GPT4o_Mini);

            this.mockSingleModelDispatcher = new MockSingleAiModelDispatcher();
            this.mockEnsembleDispatcher = new MockEnsembleDispatcher();
            this.mockSingleExecutor = new MockSingleAiModelExecutor();
            this.mockEnsembleExecutor = new MockEnsembleExecutor();
            this.mockModelResponseHandler = new MockAiModelResponseHandler();

            this.orchestrator = new testClass(requestBuilderFactory: () => new ChatClientRequestBuilder(),
                                              singleModelDispatcher: this.mockSingleModelDispatcher,
                                              ensembleDispatcher: this.mockEnsembleDispatcher,
                                              singleModelExecutor: this.mockSingleExecutor,
                                              ensembleExecutor: this.mockEnsembleExecutor,
                                              responseHandler: this.mockModelResponseHandler);
        }

        // ------------------------------------------------------------
        // SINGLE AI MODEL TESTS
        // ------------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_UsesSingleAiModelDispatcher_WhenUseEnsembleFalse()
        {
            // Arrange
            this.mockSingleModelDispatcher.ReturnedModel = this.modelA;
            this.mockSingleExecutor.ResponseToReturn = new AiModelResponse { Model = this.modelA, RawOutput = "ok", IsSuccessful = true };
            this.mockModelResponseHandler.ResponsesToReturn = [this.mockSingleExecutor.ResponseToReturn];
            string prompt = "Hello";

            OrchestrationRequest request = new()
            {
                UseEnsemble = false,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest { Strategy = SingleAiModelStrategy.BestReasoning },
            };

            // Act
            IReadOnlyList<AiModelResponse> result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.HasCount(1, result);
            Assert.AreEqual(this.modelA, result[0].Model);

            Assert.IsNotNull(this.mockSingleModelDispatcher.LastRequest);
            Assert.IsNull(this.mockEnsembleDispatcher.LastRequest);

            Assert.IsNotNull(this.mockSingleExecutor.LastCall);
            Assert.Contains(m => m.Content == prompt, this.mockSingleExecutor.LastCall.Value.request.Messages);
        }

        [TestMethod]
        public async Task ProcessAsync_SingleAiModel_InvokesResponseHandler()
        {
            // Arrange
            this.mockSingleModelDispatcher.ReturnedModel = this.modelA;

            AiModelResponse modelResponse = new()
            {
                Model = this.modelA,
                RawOutput = "done",
                IsSuccessful = true,
            };

            this.mockSingleExecutor.ResponseToReturn = modelResponse;
            this.mockModelResponseHandler.ResponsesToReturn = [modelResponse];

            OrchestrationRequest request = new()
            {
                UseEnsemble = false,
                Prompt = "Test",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest
                                     {
                                        Strategy = SingleAiModelStrategy.LowestCost,
                                     },
            };

            // Act
            IReadOnlyList<AiModelResponse> result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(this.mockModelResponseHandler.ResponsesToReturn, result);
            Assert.AreEqual(modelResponse, this.mockModelResponseHandler.LastResponses![0]);
        }

        // ------------------------------------------------------------
        // ENSEMBLE TESTS
        // ------------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_UsesEnsembleDispatcher_WhenUseEnsembleTrue()
        {
            // Arrange
            string prompt = "Explain";
            this.mockEnsembleDispatcher.ReturnedModels = [this.modelA, this.modelB];

            AiModelResponse[] responses =
            [
                new AiModelResponse {
                                    Model = this.modelA,
                                    RawOutput = "A",
                                    IsSuccessful = true,
                                  },
                new AiModelResponse {
                                    Model = this.modelB,
                                    RawOutput = "B",
                                    IsSuccessful = true,
                                  },
            ];

            this.mockEnsembleExecutor.ResponsesToReturn = responses;
            this.mockModelResponseHandler.ResponsesToReturn = responses;

            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest { Strategy = EnsembleStrategy.Reasoning },
            };

            // Act
            IReadOnlyList<AiModelResponse> result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.HasCount(2, result);
            Assert.AreEqual(this.modelA, result[0].Model);
            Assert.AreEqual(this.modelB, result[1].Model);

            Assert.IsNotNull(this.mockEnsembleDispatcher.LastRequest);
            Assert.IsNull(this.mockSingleModelDispatcher.LastRequest);

            Assert.IsNotNull(this.mockEnsembleExecutor.LastContext);
            Assert.AreEqual(prompt, this.mockEnsembleExecutor.LastContext!.Prompt);
        }

        [TestMethod]
        public async Task ProcessAsync_Ensemble_InvokesResponseHandler()
        {
            // Arrange
            string prompt = "Explain";
            this.mockEnsembleDispatcher.ReturnedModels = [this.modelA, this.modelB];

            AiModelResponse[] responses =
            [
                new AiModelResponse {
                                    Model = this.modelA,
                                    RawOutput = "A",
                                    IsSuccessful = true,
                                  },
                new AiModelResponse {
                                    Model = this.modelB,
                                    RawOutput = "B",
                                    IsSuccessful = true,
                                  },
            ];

            this.mockEnsembleExecutor.ResponsesToReturn = responses;
            this.mockModelResponseHandler.ResponsesToReturn = responses;

            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest { Strategy = EnsembleStrategy.Reasoning },
            };

            // Act
            IReadOnlyList<AiModelResponse> result = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.AreEqual(this.mockModelResponseHandler.ResponsesToReturn, result);
            Assert.AreEqual(responses[0], this.mockModelResponseHandler.LastResponses![0]);
            Assert.AreEqual(responses[1], this.mockModelResponseHandler.LastResponses![1]);
        }

        [TestMethod]
        public async Task ProcessAsync_DoesNotLeakMessages_AcrossConsecutiveCalls()
        {
            // Arrange: two separate requests with distinct prompts
            this.mockSingleModelDispatcher.ReturnedModel = this.modelA;
            AiModelResponse response = new() { Model = this.modelA, RawOutput = "ok", IsSuccessful = true };
            this.mockSingleExecutor.ResponseToReturn = response;
            this.mockModelResponseHandler.ResponsesToReturn = [response];

            OrchestrationRequest firstRequest = new()
            {
                UseEnsemble = false,
                Prompt = "First prompt",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest { Strategy = SingleAiModelStrategy.BestReasoning },
            };

            OrchestrationRequest secondRequest = new()
            {
                UseEnsemble = false,
                Prompt = "Second prompt",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest { Strategy = SingleAiModelStrategy.BestReasoning },
            };

            // Act
            _ = await this.orchestrator.ProcessAsync(firstRequest, CancellationToken.None);
            _ = await this.orchestrator.ProcessAsync(secondRequest, CancellationToken.None);

            // Assert: the second request should only contain its own prompt, not the first prompt
            Assert.IsNotNull(this.mockSingleExecutor.LastCall);
            IReadOnlyList<OpenAIApiClient.Models.Chat.Common.ChatMessage> messages = this.mockSingleExecutor.LastCall.Value.request.Messages;
            Assert.Contains(m => m.Content == "Second prompt", messages, "Second request should contain its own prompt.");
            Assert.DoesNotContain(m => m.Content == "First prompt", messages, "Second request must not contain the first prompt.");
        }

        // ------------------------------------------------------------
        // CALL OPTIONS PASSTHROUGH TESTS
        // ------------------------------------------------------------
        [TestMethod]
        public async Task ProcessAsync_SingleAiModel_PassesDefaultCallOptionsToExecutor()
        {
            // Arrange
            this.mockSingleModelDispatcher.ReturnedModel = this.modelA;
            AiModelResponse response = new() { Model = this.modelA, RawOutput = "ok", IsSuccessful = true };
            this.mockSingleExecutor.ResponseToReturn = response;
            this.mockModelResponseHandler.ResponsesToReturn = [response];

            AiCallOptions callOptions = new() { Mode = AiCallMode.NonStreaming };

            OrchestrationRequest request = new()
            {
                UseEnsemble = false,
                Prompt = "Hello",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest { Strategy = SingleAiModelStrategy.BestReasoning },
                CallOptions = callOptions,
            };

            // Act
            _ = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(this.mockSingleExecutor.LastOptions);
            Assert.AreEqual(AiCallMode.NonStreaming, this.mockSingleExecutor.LastOptions.Mode);
            Assert.AreSame(callOptions, this.mockSingleExecutor.LastOptions);
        }

        [TestMethod]
        public async Task ProcessAsync_SingleAiModel_PassesBufferedStreamingCallOptionsToExecutor()
        {
            // Arrange
            this.mockSingleModelDispatcher.ReturnedModel = this.modelA;
            AiModelResponse response = new() { Model = this.modelA, RawOutput = "ok", IsSuccessful = true };
            this.mockSingleExecutor.ResponseToReturn = response;
            this.mockModelResponseHandler.ResponsesToReturn = [response];

            AiCallOptions callOptions = new() { Mode = AiCallMode.BufferedStreaming };

            OrchestrationRequest request = new()
            {
                UseEnsemble = false,
                Prompt = "Hello",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest { Strategy = SingleAiModelStrategy.BestReasoning },
                CallOptions = callOptions,
            };

            // Act
            _ = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(this.mockSingleExecutor.LastOptions);
            Assert.AreEqual(AiCallMode.BufferedStreaming, this.mockSingleExecutor.LastOptions.Mode);
        }

        [TestMethod]
        public async Task ProcessAsync_SingleAiModel_PassesPushStreamingCallOptionsToExecutor()
        {
            // Arrange
            this.mockSingleModelDispatcher.ReturnedModel = this.modelA;
            AiModelResponse response = new() { Model = this.modelA, RawOutput = "ok", IsSuccessful = true };
            this.mockSingleExecutor.ResponseToReturn = response;
            this.mockModelResponseHandler.ResponsesToReturn = [response];

            AiCallOptions callOptions = new()
            {
                Mode = AiCallMode.PushStreaming,
                OnChunkDeltaContentToken = (_, _) => Task.CompletedTask,
            };

            OrchestrationRequest request = new()
            {
                UseEnsemble = false,
                Prompt = "Hello",
                OutputFormat = OutputFormat.PlainText,
                SingleModelRequest = new SingleAiModelDispatchRequest { Strategy = SingleAiModelStrategy.BestReasoning },
                CallOptions = callOptions,
            };

            // Act
            _ = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(this.mockSingleExecutor.LastOptions);
            Assert.AreEqual(AiCallMode.PushStreaming, this.mockSingleExecutor.LastOptions.Mode);
            Assert.IsNotNull(this.mockSingleExecutor.LastOptions.OnChunkDeltaContentToken);
        }

        [TestMethod]
        public async Task ProcessAsync_Ensemble_PassesDefaultCallOptionsToExecutor()
        {
            // Arrange
            this.mockEnsembleDispatcher.ReturnedModels = [this.modelA, this.modelB];

            AiModelResponse[] responses =
            [
                new AiModelResponse { Model = this.modelA, RawOutput = "A", IsSuccessful = true },
                new AiModelResponse { Model = this.modelB, RawOutput = "B", IsSuccessful = true },
            ];

            this.mockEnsembleExecutor.ResponsesToReturn = responses;
            this.mockModelResponseHandler.ResponsesToReturn = responses;

            AiCallOptions callOptions = new() { Mode = AiCallMode.NonStreaming };

            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = "Explain",
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest { Strategy = EnsembleStrategy.Reasoning },
                CallOptions = callOptions,
            };

            // Act
            _ = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(this.mockEnsembleExecutor.LastOptions);
            Assert.AreEqual(AiCallMode.NonStreaming, this.mockEnsembleExecutor.LastOptions.Mode);
            Assert.AreSame(callOptions, this.mockEnsembleExecutor.LastOptions);
        }

        [TestMethod]
        public async Task ProcessAsync_Ensemble_PassesBufferedStreamingCallOptionsToExecutor()
        {
            // Arrange
            this.mockEnsembleDispatcher.ReturnedModels = [this.modelA, this.modelB];

            AiModelResponse[] responses =
            [
                new AiModelResponse { Model = this.modelA, RawOutput = "A", IsSuccessful = true },
                new AiModelResponse { Model = this.modelB, RawOutput = "B", IsSuccessful = true },
            ];

            this.mockEnsembleExecutor.ResponsesToReturn = responses;
            this.mockModelResponseHandler.ResponsesToReturn = responses;

            AiCallOptions callOptions = new() { Mode = AiCallMode.BufferedStreaming };

            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = "Explain",
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest { Strategy = EnsembleStrategy.Reasoning },
                CallOptions = callOptions,
            };

            // Act
            _ = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(this.mockEnsembleExecutor.LastOptions);
            Assert.AreEqual(AiCallMode.BufferedStreaming, this.mockEnsembleExecutor.LastOptions.Mode);
        }

        [TestMethod]
        public async Task ProcessAsync_Ensemble_PassesPushStreamingCallOptionsToExecutor()
        {
            // Arrange
            this.mockEnsembleDispatcher.ReturnedModels = [this.modelA, this.modelB];

            AiModelResponse[] responses =
            [
                new AiModelResponse { Model = this.modelA, RawOutput = "A", IsSuccessful = true },
                new AiModelResponse { Model = this.modelB, RawOutput = "B", IsSuccessful = true },
            ];

            this.mockEnsembleExecutor.ResponsesToReturn = responses;
            this.mockModelResponseHandler.ResponsesToReturn = responses;

            AiCallOptions callOptions = new()
            {
                Mode = AiCallMode.PushStreaming,
                OnChunkDeltaContentToken = (_, _) => Task.CompletedTask,
            };

            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = "Explain",
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest { Strategy = EnsembleStrategy.Reasoning },
                CallOptions = callOptions,
            };

            // Act
            _ = await this.orchestrator.ProcessAsync(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(this.mockEnsembleExecutor.LastOptions);
            Assert.AreEqual(AiCallMode.PushStreaming, this.mockEnsembleExecutor.LastOptions.Mode);
            Assert.IsNotNull(this.mockEnsembleExecutor.LastOptions.OnChunkDeltaContentToken);
        }

        /// <summary>
        /// Creates a ModelDescriptor instance for the specified model name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="AiModelDescriptor">.</returns>
        private static AiModelDescriptor CreateModelDescriptor(OpenAIModel name)
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
