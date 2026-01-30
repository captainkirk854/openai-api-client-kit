// <copyright file="AIOrchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.OrchestrationNEW03
{
    using OpenAIApiClient.Routing.Ensemble;
    using OpenAIApiClient.Routing.SingleModel;

    public sealed class AIOrchestrator(SingleModelRouter singleModelRouter, EnsembleRouter ensembleRouter, ISingleModelExecutor singleModelExecutor, IEnsembleExecutor ensembleExecutor, IResponseHandler responseHandler)
    {
        private readonly SingleModelRouter singleModelRouter = singleModelRouter;
        private readonly EnsembleRouter ensembleRouter = ensembleRouter;
        private readonly ISingleModelExecutor singleModelExecutor = singleModelExecutor;
        private readonly IEnsembleExecutor ensembleExecutor = ensembleExecutor;
        private readonly IResponseHandler responseHandler = responseHandler;

        public async Task<string> ProcessAsync(OrchestrationContext request, CancellationToken cancelToken)
        {
            IExecutionContext context;

            if (request.UseEnsemble)
            {
                EnsembleRouterResult ensemble = this.ensembleRouter.Route(request.EnsembleContext!);
                context = new EnsembleExecutionContext(prompt: request.Prompt, outputFormat: request.OutputFormat, models: ensemble.Models);
            }
            else
            {
                SingleModelRouterResult single = this.singleModelRouter.Route(request.SingleModelContext!);
                context = new SingleModelExecutionContext(prompt: request.Prompt, outputFormat: request.OutputFormat, model: single.Model);
            }

            if (context.Models.Count == 1)
            {
                Models.Registries.ModelDescriptor model = context.Models[0];
                PromptContext promptContext = context.BuildPromptContext();

                ModelResponse response = await this.singleModelExecutor.ExecuteAsync(model: model, promptContext: promptContext, cancelToken: cancelToken);
                return this.responseHandler.HandleSingle(modelResponse: response);
            }
            else
            {
                IReadOnlyList<ModelResponse> responses = await this.ensembleExecutor.ExecuteAsync(context: context, cancelToken: cancelToken);
                return this.responseHandler.HandleEnsemble(responses);
            }
        }
    }
}
