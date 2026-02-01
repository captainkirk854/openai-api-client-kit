// <copyright file="MockEnsembleRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration.Routing;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Routing;

    public sealed class MockEnsembleRouter : IEnsembleRouter
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
        public EnsembleRouterRequest? LastRequest
        {
            get;
            private set;
        }

        /// <summary>
        /// Routes the request and returns the result.
        /// </summary>
        /// <param name="request"></param>
        /// <returns see cref="EnsembleRouterResult">.</returns>
        public EnsembleRouterResult Route(EnsembleRouterRequest request)
        {
            this.LastRequest = request;
            return new EnsembleRouterResult(this.ReturnedModels);
        }
    }
}
