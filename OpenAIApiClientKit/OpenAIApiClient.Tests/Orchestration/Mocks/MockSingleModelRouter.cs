// <copyright file="MockSingleModelRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;

    public sealed class MockSingleModelRouter : ISingleModelDispatcher
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
        public SingleModelDispatchRequest? LastRequest
        {
            get;
            private set;
        }

        /// <summary>
        /// Routes the request to a model.
        /// </summary>
        /// <param name="request"></param>
        /// <returns see cref="SingleModelDispatchResult">.</returns>
        public SingleModelDispatchResult Evaluate(SingleModelDispatchRequest request)
        {
            this.LastRequest = request;
            return new SingleModelDispatchResult(this.ReturnedModel);
        }
    }
}
