// <copyright file="OrchestratedEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Execution
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Factories;
    using OpenAIApiClient.Orchestration.Response;

    public class OrchestratedEnsembleExecutor(ChatClient client, IAiModelResponseHandler responseHandler)
    {
        // Build the orchestrator with the necessary components for dispatching and executing requests, as well as handling responses ..
        private readonly Orchestrator orchestrator = new OrchestratorBuilder()
                                                         .WithClient(client)
                                                         .WithResponseHandler(responseHandler)
                                                         .Build();

        /// <summary>
        /// Process the given prompt across multiple models in parallel using the orchestrator and return their response(s).
        /// </summary>
        /// <param name="prompt">The user prompt to send to all models.</param>
        /// <param name="models">The array of <see cref="OpenAIModel"/> values to use to evaluate prompt.</param>
        /// <param name="options">Options for execution, such as chunk handling and aggregation.</param>
        /// <param name="outputFormat">The desired output format for the model responses (e.g. text, json, etc.).</param>
        /// <param name="cancelToken">The cancellation token for the operation.</param>
        /// <returns> A <see cref="List{T}"/> of <see cref="AiModelResponse"/> objects containing the responses from each model,
        /// including metadata such as latency, token usage, and any errors.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="prompt"/> is null or whitespace,
        /// or when <paramref name="models"/> is empty.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when there is a failure during the orchestration process, such as an error from the orchestrator or a failure to consolidate responses.
        /// </exception>
        public async Task<List<AiModelResponse>> ProcessAsync(string prompt, OpenAIModel[] models, AiCallOptions options, OutputFormat outputFormat, CancellationToken cancelToken)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
            }

            if (models.Length == 0)
            {
                throw new ArgumentException("At least one model must be specified", nameof(models));
            }

            try
            {
                // Submit prompt to all models using the orchestrator and return their individual responses...
                return await this.ExecuteCustomEnsembleAsync(prompt: prompt, models: models, outputFormat: outputFormat, options: options, cancelToken: cancelToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error during ensemble execution: {ex.Message}");
                throw new InvalidOperationException("Failed to execute ensemble and consolidate responses", ex);
            }
        }

        /// <summary>
        /// Send the prompt to the specified models using the orchestrator,
        /// then load results into <see cref="AiModelResponse"/> objects for advanced consolidation.
        /// </summary>
        /// <param name="prompt">The prompt to send to all models.</param>
        /// <param name="models">The <see cref="OpenAIModel"/> values to query in parallel.</param>
        /// <param name="outputFormat">The desired output format for the responses (e.g. text, json, etc.).</param>
        /// <param name="options">The <see cref="AiCallOptions"/> for executing the requests (e.g. for streaming).</param>
        /// <param name="cancelToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="AiModelResponse"/> objects from all models.
        /// </returns>
        private async Task<List<AiModelResponse>> ExecuteCustomEnsembleAsync(string prompt, OpenAIModel[] models, OutputFormat outputFormat, AiCallOptions options, CancellationToken cancelToken)
        {
            // Define an ensemble orchestration request with explicit models or required capabilities.
            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = outputFormat,
                EnsembleRequest = new EnsembleDispatchRequest
                {
                    Strategy = EnsembleStrategy.Custom,
                    ExplicitModels = models,
                },
                CallOptions = options,
            };

            // Distribute request to all specified models in parallel using the orchestrator and get raw responses ..
            IReadOnlyList<AiModelResponse> orchestratorResponses = await this.orchestrator.ProcessAsync(request: request, cancelToken: cancelToken);

            // Map Orchestrator responses to AiModelResponse objects for advanced consolidation logic...
            List<AiModelResponse> results =
            [
                .. orchestratorResponses.Select(r => new AiModelResponse
                {
                    Model = r.Model,
                    RawOutput = r.RawOutput ?? string.Empty,
                    IsSuccessful = r.IsSuccessful,
                    ErrorMessage = r.ErrorMessage,
                    Latency = r.Latency,
                    TotalTokens = r.TotalTokens,
                    EstimatedCost = r.EstimatedCost,
                    ChunkCount = r.ChunkCount,
                }),
            ];

            return results;
        }
    }
}