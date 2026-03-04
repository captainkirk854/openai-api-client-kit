// <copyright file="MockSingleAiModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;

    public sealed class MockSingleAiModelExecutor : ISingleAiModelExecutor
    {
        /// <summary>
        /// Gets a value indicating whether the executor was called - only in mock for test verification purposes.
        /// </summary>
        public bool WasCalled
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last call made to the executor - only in mock for test verification purposes.
        /// </summary>
        public (AiModelDescriptor model, ChatCompletionRequest request)? LastCall
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last <see cref="AiCallOptions"/> passed to the executor - only in mock for test verification purposes.
        /// </summary>
        public AiCallOptions? LastOptions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the response to be returned by the executor - only in mock for test verification purposes.
        /// </summary>
        public AiModelResponse ResponseToReturn
        {
            get;
            set;
        } = default!;

        /// <summary>
        /// Executes the model with the given prompt context.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="execution"></param>
        /// <param name="token"></param>
        /// <returns see cref="AiModelResponse">.</returns>
        public Task<AiModelResponse> ExecuteAsync(ChatCompletionRequest request, AiCallOptions execution, CancellationToken token)
        {
            this.WasCalled = true;
            this.LastCall = (request.ModelDescriptor, request);
            this.LastOptions = execution;
            return Task.FromResult(this.ResponseToReturn);
        }
    }
}
