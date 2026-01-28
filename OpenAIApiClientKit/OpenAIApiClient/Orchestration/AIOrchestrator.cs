// <copyright file="AIOrchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Routing.Ensemble;
    using OpenAIApiClient.Routing.SingleModel;

    public sealed class AIOrchestrator(SingleModelRouter singleRouter, EnsembleRouter ensembleRouter, IModelExecutor executor, IEnsembleExecutor ensembleExecutor, IResponseHandler responseHandler)
    {
        private readonly SingleModelRouter singleRouter = singleRouter;
        private readonly EnsembleRouter ensembleRouter = ensembleRouter;
        private readonly IModelExecutor executor = executor;
        private readonly IEnsembleExecutor ensembleExecutor = ensembleExecutor;
        private readonly IResponseHandler responseHandler = responseHandler;

        public async Task<string> ProcessAsync(OrchestrationContext context)
        {
            if (context.UseEnsemble)
            {
                EnsembleRouterResult ensemble = this.ensembleRouter.Route(context: context.EnsembleContext!);
                IReadOnlyList<string> outputs = await this.ensembleExecutor.ExecuteAsync(models: ensemble.Models, prompt: context.Prompt);
                return this.responseHandler.HandleEnsemble(outputs);
            }
            else
            {
                SingleModelRouterResult single = this.singleRouter.Route(context: context.SingleModelContext!);
                string output = await this.executor.ExecuteAsync(model: single.Model, context.Prompt);
                return this.responseHandler.HandleSingle(output);
            }
        }
    }
}