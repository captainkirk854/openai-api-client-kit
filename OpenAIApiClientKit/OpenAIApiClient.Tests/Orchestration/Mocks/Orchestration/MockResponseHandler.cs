// <copyright file="MockResponseHandler.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks.Orchestration
{
    using OpenAIApiClient.Interfaces.Orchestration;

    public sealed class MockResponseHandler : IResponseHandler
    {
        public string LastSingleInput { get; private set; } = string.Empty;

        public IReadOnlyList<string>? LastEnsembleInput { get; private set; }

        public string HandleSingle(string modelOutput)
        {
            this.LastSingleInput = modelOutput;
            return $"HandledSingle:{modelOutput}";
        }

        public string HandleEnsemble(IReadOnlyList<string> outputs)
        {
            this.LastEnsembleInput = outputs;
            return $"HandledEnsemble:{string.Join("|", outputs)}";
        }
    }
}
