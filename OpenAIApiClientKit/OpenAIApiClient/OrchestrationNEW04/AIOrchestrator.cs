// <copyright file="AIOrchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW04
{
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.Ensemble;
    using OpenAIApiClient.Routing.SingleModel;

    public sealed class AIOrchestrator(SingleModelRouter singleRouter, EnsembleRouter ensembleRouter, ModelExecutor executor, IEnsembleExecutor ensembleExecutor, IResponseHandler responseHandler, ClientRequestBuilder requestBuilder)
    {
        private readonly SingleModelRouter singleRouter = singleRouter;
        private readonly EnsembleRouter ensembleRouter = ensembleRouter;
        private readonly ModelExecutor executor = executor;
        private readonly IEnsembleExecutor ensembleExecutor = ensembleExecutor;
        private readonly IResponseHandler responseHandler = responseHandler;
        private readonly ClientRequestBuilder requestBuilder = requestBuilder;

        public async Task<string> ProcessAsync(OrchestrationRequest request, CancellationToken cancelToken)
        {
            IExecutionContext execContext =
                request.UseEnsemble
                    ? new EnsembleExecutionContext(prompt: request.Prompt, outputFormat: request.OutputFormat, models: this.ensembleRouter.Route(request: request.EnsembleRequest!).Models)
                    : new SingleModelExecutionContext(prompt: request.Prompt, outputFormat: request.OutputFormat, model: this.singleRouter.Route(request: request.SingleModelRequest!).Model);

            OrchestrationContext context = new(prompt: request.Prompt, outputFormat: request.OutputFormat, executionContext: execContext, builder: this.requestBuilder);

            if (!context.IsEnsemble)
            {
                ModelDescriptor model = context.ExecutionContext.Models[0];
                ModelResponse response = await this.executor.ExecuteAsync(model, context.PromptContext, cancelToken);
                return this.responseHandler.HandleSingle(response);
            }
            else
            {
                IReadOnlyList<ModelResponse> responses = await this.ensembleExecutor.ExecuteAsync(context, cancelToken);
                return this.responseHandler.HandleEnsemble(responses);
            }
        }
    }
}
