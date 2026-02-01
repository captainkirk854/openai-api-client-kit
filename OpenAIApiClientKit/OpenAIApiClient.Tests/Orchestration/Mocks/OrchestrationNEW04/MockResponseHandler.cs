// <copyright file="MockResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks.OrchestrationNEW04
{
    using OpenAIApiClient.OrchestrationNEW04;

    public sealed class MockResponseHandler : IResponseHandler
    {
        /// <summary>
        /// Gets the last responses passed to the handler.
        /// </summary>
        public IReadOnlyList<ModelResponse>? LastResponses
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the responses to return when handling responses.
        /// </summary>
        public IReadOnlyList<ModelResponse> ResponsesToReturn
        {
            get;
            set;
        } = default!;

        /// <summary>
        /// Gets the last single response passed to the handler.
        /// </summary>
        /// <param name="modelResponses"></param>
        /// <returns see cref="IReadOnlyList{ModelResponse}">.</returns>
        public IReadOnlyList<ModelResponse> HandleResponses(IReadOnlyList<ModelResponse> modelResponses)
        {
            this.LastResponses = modelResponses;
            return this.ResponsesToReturn;
        }

        /// <inheritdoc/>
        string IResponseHandler.HandleEnsemble(IReadOnlyList<ModelResponse> modelResponses)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        string IResponseHandler.HandleSingle(ModelResponse modelResponse)
        {
            throw new NotImplementedException();
        }
    }
}
