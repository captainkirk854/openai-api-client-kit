// <copyright file="MockResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Orchestration;

    public sealed class MockResponseHandler : IResponseHandler
    {
        /// <summary>
        /// Gets the last responses passed to the handler - only in mock for test verification purposes.
        /// </summary>
        public IReadOnlyList<AIModelResponse>? LastResponses
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the responses to return when handling responses - only in mock for test verification purposes.
        /// </summary>
        public IReadOnlyList<AIModelResponse> ResponsesToReturn
        {
            get;
            set;
        } = default!;

        /// <summary>
        /// Gets the last single response passed to the handler.
        /// </summary>
        /// <param name="modelResponses"></param>
        /// <returns see cref="IReadOnlyList{ModelResponse}">.</returns>
        public IReadOnlyList<AIModelResponse> HandleResponses(IReadOnlyList<AIModelResponse> modelResponses)
        {
            this.LastResponses = modelResponses;
            return this.ResponsesToReturn;
        }

        /// <inheritdoc/>
        string IResponseHandler.HandleEnsemble(IReadOnlyList<AIModelResponse> modelResponses)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        string IResponseHandler.HandleSingle(AIModelResponse modelResponse)
        {
            throw new NotImplementedException();
        }
    }
}
