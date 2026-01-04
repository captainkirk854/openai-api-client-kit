// <copyright file="MultiModelOrchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OptimalSelection
{
    public sealed class MultiModelOrchestrator(OpenAIModelRouter router, OpenAIModelExecutor executor)
    {
        // <summary>
        // Orchestrates the execution of multiple OpenAI models based on routing decisions.
        // </summary>
        private readonly OpenAIModelRouter router = router;
        private readonly OpenAIModelExecutor executor = executor;

        /// <summary>
        /// Executes the orchestrator logic: routes the prompt to appropriate models, executes them, and aggregates the responses.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="token"></param>
        /// <returns><see cref="FinalResponse"/>.</returns>
        public async Task<FinalResponse> ExecuteAsync(PromptContext context, CancellationToken token)
        {
            // Route to appropriate models based on the prompt context ..
            IReadOnlyList<ModelDescriptor> models = this.router.Route(context);

            // Execute all routed models in parallel ..
            List<Task<ModelResponse>> tasks = [.. models.Select(m => this.executor.ExecuteAsync(m, context, token))];

            // Await all model responses ..
            ModelResponse[] responses = await Task.WhenAll(tasks);

            // Aggregate all model responses into a final response ..
            return OpenAIResponseAggregator.Aggregate(responses);
        }
    }
}
