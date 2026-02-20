// <copyright file="StubResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Orchestration;

    public sealed class StubResponseHandler : IResponseHandler
    {
        public string HandleSingle(AIModelResponse response)
        {
            return response.RawOutput;
        }

        public string HandleEnsemble(IReadOnlyList<AIModelResponse> responses)
        {
            return string.Join(Environment.NewLine, responses.Select(r => r.RawOutput));
        }

        public IReadOnlyList<AIModelResponse> HandleResponses(IReadOnlyList<AIModelResponse> modelResponses)
        {
            return modelResponses;
        }
    }
}
