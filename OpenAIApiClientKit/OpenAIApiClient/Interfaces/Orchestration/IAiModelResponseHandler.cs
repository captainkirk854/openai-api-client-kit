// <copyright file="IAiModelResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration
{
    using OpenAIApiClient.Orchestration.Response;

    public interface IAiModelResponseHandler
    {
        string HandleSingle(AiModelResponse modelResponse);

        string HandleEnsemble(IReadOnlyList<AiModelResponse> modelResponses);

        IReadOnlyList<AiModelResponse> HandleResponses(IReadOnlyList<AiModelResponse> modelResponses);
    }
}