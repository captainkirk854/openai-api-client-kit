// <copyright file="Orchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Execution;

    /// <summary>
    /// Responsible for orchestrating model requests, routing them to the appropriate models,
    /// executing the requests, and handling the responses.
    /// </summary>
    /// <param name="requestBuilder">Client request builder.</param>
    /// <param name="singleModelDispatcher">Single model router.</param>
    /// <param name="ensembleDispatcher">Ensemble router.</param>
    /// <param name="singleModelExecutor">Single model executor.</param>
    /// <param name="ensembleExecutor">Ensemble model executor.</param>
    /// <param name="responseHandler">Response handler.</param>
    public sealed class Orchestrator(ClientRequestBuilder requestBuilder,
                                     ISingleModelDispatcher singleModelDispatcher,
                                     IEnsembleDispatcher ensembleDispatcher,
                                     ISingleModelExecutor singleModelExecutor,
                                     IEnsembleExecutor ensembleExecutor,
                                     IResponseHandler responseHandler)
    {
        private readonly ClientRequestBuilder requestBuilder = requestBuilder;
        private readonly ISingleModelDispatcher singleModelRouter = singleModelDispatcher;
        private readonly IEnsembleDispatcher ensembleRouter = ensembleDispatcher;
        private readonly ISingleModelExecutor singleModelExecutor = singleModelExecutor;
        private readonly IEnsembleExecutor ensembleExecutor = ensembleExecutor;
        private readonly IResponseHandler responseHandler = responseHandler;

        /// <summary>
        /// Process the orchestration request and return raw model responses.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancelToken"></param>
        /// <returns>IReadOnlyList&lt;ModelResponse&gt;.</returns>
        public async Task<IReadOnlyList<ModelResponse>> ProcessAsync(OrchestrationRequest request, CancellationToken cancelToken)
        {
            // Append prompt and output format from orchestration request to chat request builder ..
            this.requestBuilder.SetPromptAndFormat(prompt: request.Prompt, format: request.OutputFormat);

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

            // Do we actually have more than one model to be used?
            bool isEnsemble = executionContext.Models.Count > 1;

            // Control Execution based on execution context (ensemble/single) ..
            if (isEnsemble)
            {
                IReadOnlyList<ModelResponse> responses = await this.ensembleExecutor.ExecuteAsync(requestBuilder: this.requestBuilder, context: executionContext, cancelToken: cancelToken);
                return this.responseHandler.HandleResponses(modelResponses: responses);
            }
            else
            {
                ModelDescriptor model = executionContext.Models[0];
                ChatCompletionRequest chatRequest = this.requestBuilder.WithModel(input: model.Name).Build();
                ModelResponse response = await this.singleModelExecutor.ExecuteAsync(request: chatRequest, cancelToken: cancelToken);
                return this.responseHandler.HandleResponses(modelResponses: ModelResponse.WrapSingleResponseAsList(modelResponse: response));
            }
        }
    }
}
