// <copyright file="Orchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Execution;

    /// <summary>
    /// Responsible for orchestrating model requests, routing them to the appropriate models,
    /// executing the requests, and handling the responses.
    /// </summary>
    /// <param name="singleModelDispatcher">Single model router.</param>
    /// <param name="ensembleDispatcher">Ensemble router.</param>
    /// <param name="singleModelExecutor">Single model executor.</param>
    /// <param name="ensembleExecutor">Ensemble model executor.</param>
    /// <param name="requestBuilder">Client request builder.</param>
    /// <param name="responseHandler">Response handler.</param>
    public sealed class Orchestrator(ISingleModelDispatcher singleModelDispatcher, IEnsembleDispatcher ensembleDispatcher, ISingleModelExecutor singleModelExecutor, IEnsembleExecutor ensembleExecutor, ClientRequestBuilder requestBuilder, IResponseHandler responseHandler)
    {
        private readonly ISingleModelDispatcher singleModelRouter = singleModelDispatcher;
        private readonly IEnsembleDispatcher ensembleRouter = ensembleDispatcher;
        private readonly ISingleModelExecutor singleModelExecutor = singleModelExecutor;
        private readonly IEnsembleExecutor ensembleExecutor = ensembleExecutor;
        private readonly ClientRequestBuilder requestBuilder = requestBuilder;
        private readonly IResponseHandler responseHandler = responseHandler;

        /// <summary>
        /// Process the orchestration request and return raw model responses.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancelToken"></param>
        /// <returns>IReadOnlyList&lt;ModelResponse&gt;.</returns>
        public async Task<IReadOnlyList<ModelResponse>> ProcessAsync(OrchestrationRequest request, CancellationToken cancelToken)
        {
            // Determine execution context based on request type ..
            IExecutionContext executionContext;
            if (request.UseEnsemble)
            {
                EnsembleDispatchResult routerResult = this.ensembleRouter.Evaluate(request: request.EnsembleRequest!);
                executionContext = new EnsembleExecutionContext(prompt: request.Prompt, outputFormat: request.OutputFormat, models: routerResult.Models);
            }
            else
            {
                SingleModelDispatchResult routerResult = this.singleModelRouter.Evaluate(request: request.SingleModelRequest!);
                executionContext = new SingleModelExecutionContext(prompt: request.Prompt, outputFormat: request.OutputFormat, model: routerResult.Model);
            }

            // Create orchestration context ..
            OrchestrationContext context = new(prompt: request.Prompt, outputFormat: request.OutputFormat, executionContext: executionContext, builder: this.requestBuilder);

            // Execute based on context type ..
            if (context.IsEnsemble)
            {
                IReadOnlyList<ModelResponse> responses = await this.ensembleExecutor.ExecuteAsync(context: context, cancelToken: cancelToken);
                return this.responseHandler.HandleResponses(modelResponses: responses);
            }
            else
            {
                ModelDescriptor model = context.ExecutionContext.Models[0];
                ModelResponse response = await this.singleModelExecutor.ExecuteAsync(model: model, context: context.PromptContext, cancelToken: cancelToken);
                return this.responseHandler.HandleResponses(modelResponses: ModelResponse.WrapSingleResponseAsList(modelResponse: response));
            }
        }
    }
}
