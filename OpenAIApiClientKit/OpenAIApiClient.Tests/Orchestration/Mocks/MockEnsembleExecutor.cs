// <copyright file="MockEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;

    public sealed class MockEnsembleExecutor : IEnsembleExecutor
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
        /// Gets the last execution context passed to the executor - only in mock class for test verification purposes.
        /// </summary>
        public IExecutionContext? LastContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the responses to return when ExecuteAsync is called - only in mock class for test verification purposes.
        /// </summary>
        public IReadOnlyList<AiModelResponse> ResponsesToReturn
        {
            get;
            set;
        } = default!;

        public Task<IReadOnlyList<AiModelResponse>> ExecuteAsync(ClientRequestBuilder requestBuilder, IExecutionContext context, CancellationToken cancelToken)
        {
            this.WasCalled = true;
            this.LastContext = context;
            return Task.FromResult(this.ResponsesToReturn);
        }
    }
}
