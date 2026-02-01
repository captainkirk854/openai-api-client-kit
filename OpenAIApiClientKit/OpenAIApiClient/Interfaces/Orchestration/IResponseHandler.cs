// <copyright file="IResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration
{
    using OpenAIApiClient.Orchestration;

    public interface IResponseHandler
    {
        string HandleSingle(ModelResponse modelResponse);

        string HandleEnsemble(IReadOnlyList<ModelResponse> modelResponses);

        IReadOnlyList<ModelResponse> HandleResponses(IReadOnlyList<ModelResponse> modelResponses);
    }
}