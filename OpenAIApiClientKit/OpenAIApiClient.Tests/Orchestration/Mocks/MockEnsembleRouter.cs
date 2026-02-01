// <copyright file="MockEnsembleRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;

    public sealed class MockEnsembleRouter : IEnsembleDispatcher
    {
        /// <summary>
        /// Gets or sets the model descriptors to be returned by the router - only in mock for test verification purposes.
        /// </summary>
        public IReadOnlyList<ModelDescriptor> ReturnedModels
        {
            get;
            set;
        } = default!;

        /// <summary>
        /// Gets the last request received by the router  - only in mock for test verification purposes.
        /// </summary>
        public EnsembleDispatchRequest? LastRequest
        {
            get;
            private set;
        }

        /// <summary>
        /// Routes the request and returns the result.
        /// </summary>
        /// <param name="request"></param>
        /// <returns see cref="EnsembleDispatchResult">.</returns>
        public EnsembleDispatchResult Evaluate(EnsembleDispatchRequest request)
        {
            this.LastRequest = request;
            return new EnsembleDispatchResult(this.ReturnedModels);
        }
    }
}
