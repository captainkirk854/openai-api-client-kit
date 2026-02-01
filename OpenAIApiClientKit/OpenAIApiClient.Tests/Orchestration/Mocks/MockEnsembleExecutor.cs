// <copyright file="MockEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Orchestration;

    public sealed class MockEnsembleExecutor : IEnsembleExecutor
    {
        /// <summary>
        /// Gets the last orchestration context passed to the executor - only in mock class for test verification purposes.
        /// </summary>
        public OrchestrationContext? LastContext
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the responses to return when ExecuteAsync is called - only in mock class for test verification purposes.
        /// </summary>
        public IReadOnlyList<ModelResponse> ResponsesToReturn
        {
            get;
            set;
        } = default!;

        public Task<IReadOnlyList<ModelResponse>> ExecuteAsync(OrchestrationContext context, CancellationToken token)
        {
            this.LastContext = context;
            return Task.FromResult(this.ResponsesToReturn);
        }
    }
}
