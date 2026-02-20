// <copyright file="MockSingleModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration;

    public sealed class MockSingleModelExecutor : ISingleModelExecutor
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
        public (ModelDescriptor model, ChatCompletionRequest request)? LastCall
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the response to be returned by the executor - only in mock for test verification purposes.
        /// </summary>
        public AIModelResponse ResponseToReturn
        {
            get;
            set;
        } = default!;

        /// <summary>
        /// Executes the model with the given prompt context.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <returns see cref="AIModelResponse">.</returns>
        public Task<AIModelResponse> ExecuteAsync(ChatCompletionRequest request, CancellationToken token)
        {
            this.WasCalled = true;
            this.LastCall = (request.ModelDescriptor, request);
            return Task.FromResult(this.ResponseToReturn);
        }
    }
}
