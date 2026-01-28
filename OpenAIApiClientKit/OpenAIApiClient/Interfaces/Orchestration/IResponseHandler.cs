// <copyright file="IResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Interfaces.Orchestration
{
    public interface IResponseHandler
    {
        string HandleSingle(string modelOutput);

        string HandleEnsemble(IReadOnlyList<string> outputs);
    }
}