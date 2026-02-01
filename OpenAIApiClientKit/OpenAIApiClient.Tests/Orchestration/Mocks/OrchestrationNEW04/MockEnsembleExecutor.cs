// <copyright file="MockEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks.OrchestrationNEW04
{
    using OpenAIApiClient.OrchestrationNEW04;

    public sealed class MockEnsembleExecutor : IEnsembleExecutor
    {
        public OrchestrationContext? LastContext
        {
            get;
            private set;
        }

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
