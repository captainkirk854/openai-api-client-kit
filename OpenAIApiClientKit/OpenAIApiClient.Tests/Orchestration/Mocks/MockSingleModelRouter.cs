// <copyright file="MockSingleModelRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration.Routing;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Routing;

    public sealed class MockSingleModelRouter : ISingleModelRouter
    {
        /// <summary>
        /// Gets or sets the model descriptor to be returned by the router - only in mock for test verification purposes.
        /// </summary>
        public ModelDescriptor ReturnedModel
        {
            get;
            set;
        } = default!;

        /// <summary>
        /// Gets the last request received by the router - only in mock for test verification purposes.
        /// </summary>
        public SingleModelRouterRequest? LastRequest
        {
            get;
            private set;
        }

        /// <summary>
        /// Routes the request to a model.
        /// </summary>
        /// <param name="request"></param>
        /// <returns see cref="SingleModelRouterResult">.</returns>
        public SingleModelRouterResult Route(SingleModelRouterRequest request)
        {
            this.LastRequest = request;
            return new SingleModelRouterResult(this.ReturnedModel);
        }
    }
}
