// <copyright file="MultiModelOrchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.OptimalModelSelection
{
    using OpenAIApiClient.Models.OptimalModelSelection;

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
        /// <param name="context">Prompt context.</param>
        /// <param name="cancelToken">Cancellation token.</param>
        /// <returns><see cref="OpenAICollatedResponse"/>.</returns>
        public async Task<OpenAICollatedResponse> ExecuteAsync(PromptContext context, CancellationToken cancelToken)
        {
            // Route to appropriate models based on the prompt context ..
            IReadOnlyList<OpenAIModelDescriptor> models = this.router.Route(context);

            // Execute all routed models in parallel ..
            List<Task<OpenAIModelResponse>> tasks = [.. models.Select(m => this.executor.ExecuteAsync(model: m, context: context, cancelToken: cancelToken))];

            // Await all model responses ..
            OpenAIModelResponse[] responses = await Task.WhenAll(tasks);

            // Aggregate all model responses into a final response ..
            return OpenAIModelResponseAggregator.Aggregate(responses: responses);
        }
    }
}
