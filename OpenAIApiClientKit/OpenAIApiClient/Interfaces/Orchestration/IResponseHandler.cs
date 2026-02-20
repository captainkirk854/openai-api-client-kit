// <copyright file="IResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration
{
    using OpenAIApiClient.Orchestration;

    public interface IResponseHandler
    {
        string HandleSingle(AIModelResponse modelResponse);

        string HandleEnsemble(IReadOnlyList<AIModelResponse> modelResponses);

        IReadOnlyList<AIModelResponse> HandleResponses(IReadOnlyList<AIModelResponse> modelResponses);
    }
}