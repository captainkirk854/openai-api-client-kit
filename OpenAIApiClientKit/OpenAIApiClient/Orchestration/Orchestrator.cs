// <copyright file="Orchestrator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Responsible for orchestrating model requests, dispatching them to the appropriate models,
    /// executing the requests, and handling the responses.
    /// </summary>
    /// <param name="requestBuilderFactory">Factory that produces a fresh client request builder for each request.</param>
    /// <param name="singleModelDispatcher">Single model router.</param>
    /// <param name="ensembleDispatcher">Ensemble router.</param>
    /// <param name="singleModelExecutor">Single model executor.</param>
    /// <param name="ensembleExecutor">Ensemble model executor.</param>
    /// <param name="responseHandler">Response handler.</param>
    public sealed class Orchestrator(Func<ChatClientRequestBuilder> requestBuilderFactory,
                                     ISingleAiModelDispatcher singleModelDispatcher,
                                     IEnsembleDispatcher ensembleDispatcher,
                                     ISingleAiModelExecutor singleModelExecutor,
                                     IEnsembleExecutor ensembleExecutor,
                                     IAiModelResponseHandler responseHandler)
    {
        private readonly Func<ChatClientRequestBuilder> requestBuilderFactory = requestBuilderFactory;
        private readonly ISingleAiModelDispatcher singleModelDispatcher = singleModelDispatcher;
        private readonly IEnsembleDispatcher ensembleDispatcher = ensembleDispatcher;
        private readonly ISingleAiModelExecutor singleModelExecutor = singleModelExecutor;
        private readonly IEnsembleExecutor ensembleExecutor = ensembleExecutor;
        private readonly IAiModelResponseHandler responseHandler = responseHandler;

        /// <summary>
        /// Process the orchestration request and return raw model responses.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancelToken"></param>
        /// <returns>IReadOnlyList&lt;ModelResponse&gt;.</returns>
        public async Task<IReadOnlyList<AiModelResponse>> ProcessAsync(OrchestrationRequest request, CancellationToken cancelToken)
        {
            // Create a fresh, clean builder instance insulated to this request to prevent state leaking across calls ..
            ChatClientRequestBuilder requestBuilder = this.requestBuilderFactory();

            // Append prompt and output format from orchestration request to chat request builder ..
            requestBuilder.SetPromptAndFormat(prompt: request.Prompt, format: request.OutputFormat);

            // Determine execution context based on request type ..
            IExecutionContext executionContext = request.UseEnsemble
                ? new EnsembleExecutionContext(prompt: request.Prompt,
                                               outputFormat: request.OutputFormat,
                                               models: this.ensembleDispatcher.Evaluate(request.EnsembleRequest!).Models)
                : new SingleAiModelExecutionContext(prompt: request.Prompt,
                                                    outputFormat: request.OutputFormat,
                                                    model: this.singleModelDispatcher.Evaluate(request.SingleModelRequest!).Model);

            // Check if execution context contains multiple models (ensemble) or single model ..
            bool isEnsemble = executionContext.Models.Count > 1;

            // Execute the request based on the determined execution context and handle the responses accordingly ..
            if (isEnsemble)
            {
                IReadOnlyList<AiModelResponse> responses = await this.ensembleExecutor.ExecuteAsync(requestBuilder: requestBuilder, context: executionContext, execution: request.CallOptions, cancelToken: cancelToken);
                return this.responseHandler.HandleResponses(modelResponses: responses);
            }
            else
            {
                // In single model scenario, there is implicitly one model and so we set that model on the request builder before execution ..
                AiModelDescriptor model = executionContext.Models[0];
                ChatCompletionRequest chatRequest = requestBuilder.WithModel(input: model.Name).Build();

                // Execute the request and handle the response ..
                AiModelResponse response = await this.singleModelExecutor.ExecuteAsync(request: chatRequest, options: request.CallOptions, cancelToken: cancelToken);
                return this.responseHandler.HandleResponses(modelResponses: AiModelResponse.WrapSingleResponseAsList(modelResponse: response));
            }
        }
    }
}
