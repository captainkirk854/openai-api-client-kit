// <copyright file="MockSingleModelExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks.OrchestrationNEW04
{
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.OrchestrationNEW04;

    public sealed class MockSingleModelExecutor : ISingleModelExecutor
    {
        /// <summary>
        /// Gets the last call made to the executor.
        /// </summary>
        public (ModelDescriptor model, PromptContext context)? LastCall
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the response to be returned by the executor.
        /// </summary>
        public ModelResponse ResponseToReturn
        {
            get;
            set;
        } = default!;

        /// <summary>
        /// Executes the model with the given prompt context.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns see cref="ModelResponse">.</returns>
        public Task<ModelResponse> ExecuteAsync(ModelDescriptor model, PromptContext context, CancellationToken token)
        {
            this.LastCall = (model, context);
            return Task.FromResult(this.ResponseToReturn);
        }
    }
}
