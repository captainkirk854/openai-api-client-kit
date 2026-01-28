// <copyright file="MockModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Models.Registries;

    public sealed class MockModelExecutor : IModelExecutor
    {
        /// <summary>
        /// Gets the calls made to the executor.
        /// </summary>
        public List<(ModelDescriptor model, string prompt)> Calls
        {
            get;
        } = [];

        /// <summary>
        /// Executes the model with the given prompt.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="prompt"></param>
        /// <returns>string.</returns>
        public Task<string> ExecuteAsync(ModelDescriptor model, string prompt)
        {
            // Record the call.
            this.Calls.Add((model, prompt));

            // Return a mock response.
            return Task.FromResult($"Single:{model.Name}:{prompt}");
        }
    }
}
