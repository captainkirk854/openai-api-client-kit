// <copyright file="MockAiModelResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Orchestration;

    public sealed class MockAiModelResponseHandler : IAiModelResponseHandler
    {
        /// <summary>
        /// Gets the last responses passed to the handler - only in mock for test verification purposes.
        /// </summary>
        public IReadOnlyList<AiModelResponse>? LastResponses
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the responses to return when handling responses - only in mock for test verification purposes.
        /// </summary>
        public IReadOnlyList<AiModelResponse> ResponsesToReturn
        {
            get;
            set;
        } = default!;

        /// <summary>
        /// Gets the last single response passed to the handler.
        /// </summary>
        /// <param name="modelResponses"></param>
        /// <returns see cref="IReadOnlyList{ModelResponse}">.</returns>
        public IReadOnlyList<AiModelResponse> HandleResponses(IReadOnlyList<AiModelResponse> modelResponses)
        {
            this.LastResponses = modelResponses;
            return this.ResponsesToReturn;
        }

        /// <inheritdoc/>
        string IAiModelResponseHandler.HandleEnsemble(IReadOnlyList<AiModelResponse> modelResponses)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        string IAiModelResponseHandler.HandleSingle(AiModelResponse modelResponse)
        {
            throw new NotImplementedException();
        }
    }
}
