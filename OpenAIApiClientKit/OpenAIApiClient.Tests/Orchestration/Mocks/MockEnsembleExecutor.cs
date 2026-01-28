// <copyright file="MockEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Models.Registries;

    public sealed class MockEnsembleExecutor : IEnsembleExecutor
    {
        /// <summary>
        /// Gets the calls made to the executor.
        /// </summary>
        public List<(IReadOnlyList<ModelDescriptor> models, string prompt)> Calls
        {
            get;
        } = [];

        /// <summary>
        /// Executes the ensemble of models with the given prompt.
        /// </summary>
        /// <param name="models"></param>
        /// <param name="prompt"></param>
        /// <returns>string collection.</returns>
        public Task<IReadOnlyList<string>> ExecuteAsync(IReadOnlyList<ModelDescriptor> models, string prompt)
        {
            // Record the call.
            this.Calls.Add((models, prompt));

            // Return mock outputs.
            List<string> outputs = [.. models.Select(m => $"Ensemble:{m.Name}:{prompt}")];
            return Task.FromResult((IReadOnlyList<string>)outputs);
        }
    }
}
