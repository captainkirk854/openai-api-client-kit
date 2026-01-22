// <copyright file="ModelsOrchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.Orchestration
{
    using OpenAIApiClient.Models.Selection;

    public sealed class ModelsOrchestrator(ModelRouter router, ModelExecutor executor)
    {
        // <summary>
        // Orchestrates the execution of multiple OpenAI models based on routing decisions.
        // </summary>
        private readonly ModelRouter router = router;
        private readonly ModelExecutor executor = executor;

        /// <summary>
        /// Executes the orchestrator logic: routes the prompt to appropriate models, executes them, and aggregates the responses.
        /// </summary>
        /// <param name="context">Prompt context.</param>
        /// <param name="cancelToken">Cancellation token.</param>
        /// <returns><see cref="CollatedModelResponse"/>.</returns>
        public async Task<CollatedModelResponse> ExecuteAsync(PromptContext context, CancellationToken cancelToken)
        {
            // Route to appropriate models based on the prompt context ..
            IReadOnlyList<ModelDescriptor> models = this.router.Route(context);

            // Execute all routed models in parallel ..
            List<Task<ModelResponse>> tasks = [.. models.Select(m => this.executor.ExecuteAsync(model: m, context: context, cancelToken: cancelToken))];

            // Await all model responses ..
            ModelResponse[] responses = await Task.WhenAll(tasks);

            // Aggregate all model responses into a final response ..
            return Selection.ModelResponseSelector.SelectBest(responses: responses);
        }
    }
}
