// <copyright file="AdvancedEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Consolidation
{
    using System.Diagnostics;
    using OpenAIApiClient;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Models.Consolidation;
    using OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring;
    using OpenAIApiClient.Models.Consolidation.Options.LLMJudge;
    using OpenAIApiClient.Models.Consolidation.Options.ResponseFusion;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Consolidation.Options;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Factories;
    using OpenAIApiClient.Orchestration.Response;

    /// <summary>
    /// Advanced consolidation strategies for fan-out ensemble responses.
    /// Supports: LLM Judge, Heuristic Scoring, and Response Fusion.
    /// Coordinates specialized consolidation strategy classes.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AdvancedEnsembleExecutor"/> class.
    /// </remarks>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AdvancedEnsembleExecutor"/> class
    /// using the default orchestration pipeline created by <see cref="OrchestratorBuilder"/>.
    /// </remarks>
    /// <param name="client">The <see cref="ChatClient"/> used for API calls.</param>
    /// <param name="responseHandler">
    /// The <see cref="IAiModelResponseHandler"/> used by the orchestrator to post‑process responses.
    /// </param>
    public class AdvancedEnsembleExecutor(ChatClient client, IAiModelResponseHandler responseHandler)
    {
        // Use the Orchestrator for fan-out to leverage its parallel dispatch and response handling capabilities.
        private readonly Orchestrator orchestrator = new OrchestratorBuilder()
                                                         .WithClient(client)
                                                         .WithResponseHandler(responseHandler)
                                                         .Build();

        // Specialized consolidation strategy classes that encapsulate the logic for each approach.
        private readonly LLMJudge llmJudgeStrategy = new(client: client);
        private readonly ResponseFusion responseFusionStrategy = new(client: client);

        /// <summary>
        /// Fan-out to N models and consolidate using specified strategy.
        /// </summary>
        /// <param name="prompt">The user prompt to send to all models.</param>
        /// <param name="models">The array of <see cref="OpenAIModel"/> values to use to evaluate prompt.</param>
        /// <param name="consolidationMode">The <see cref="ConsolidationMode"/> strategy to use.</param>
        /// <param name="judgeModel">The judge <see cref="OpenAIModel"/> for LLM-based strategies (optional).</param>
        /// <param name="cancelToken">The cancellation token for the operation.</param>
        /// <returns>
        /// An <see cref="AdvancedConsolidatedResponse"/> containing the consolidated content,
        /// metadata, and individual model response(s) <see cref="AiModelResponse"/> instances.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="prompt"/> is null or whitespace,
        /// or when <paramref name="models"/> is empty,
        /// or when a judge model is required but not supplied.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when advanced fan-out and consolidation fails.
        /// </exception>
        public async Task<AdvancedConsolidatedResponse> FanOutAndConsolidateAdvancedAsync(string prompt,
                                                                                          OpenAIModel[] models,
                                                                                          ConsolidationMode consolidationMode,
                                                                                          OpenAIModel? judgeModel = null,
                                                                                          CancellationToken cancelToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
            }

            if (models.Length == 0)
            {
                throw new ArgumentException("At least one model must be specified", nameof(models));
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                // Step 1: Submit prompt to all models using the orchestrator.
                Console.WriteLine($" Using {models.Length} models...");
                List<AiModelResponse> responses = await this.ExecuteEnsembleAsync(prompt, models, cancelToken);

                // Step 2: Consolidate based on strategy.
                string consolidatedContent;
                object? consolidationMetadata = null;

                switch (consolidationMode)
                {
                    case ConsolidationMode.LLMAsJudge:
                        if (judgeModel is null)
                        {
                            throw new ArgumentException("A Judge model is required for LLM Judge consolidation!", nameof(judgeModel));
                        }
                        LLMJudgeResult judgeResult = await this.llmJudgeStrategy.ConsolidateWithLLMJudgeAsync(prompt, responses, judgeModel.Value, cancelToken);
                        consolidatedContent = judgeResult.SelectedResponse;
                        consolidationMetadata = judgeResult;
                        break;

                    case ConsolidationMode.HeuristicScoring:
                        HeuristicScoringResult heuristicResult = HeuristicScoring.ConsolidateWithHeuristicScoring(prompt, responses);
                        consolidatedContent = heuristicResult.SelectedResponse;
                        consolidationMetadata = heuristicResult;
                        break;

                    case ConsolidationMode.ResponseFusion:
                        if (judgeModel is null)
                        {
                            throw new ArgumentException("A Judge model is required for Response Fusion", nameof(judgeModel));
                        }
                        ResponseFusionResult fusionResult = await this.responseFusionStrategy.ConsolidateWithResponseFusionAsync(prompt, responses, judgeModel.Value, cancelToken);
                        consolidatedContent = fusionResult.SynthesizedResponse;
                        consolidationMetadata = fusionResult;
                        break;

                    default:
                        throw new ArgumentException($"Unknown consolidation mode: {consolidationMode}");
                }

                stopwatch.Stop();

                // Step 3: Return a comprehensive response object with all relevant data and metadata.
                return new AdvancedConsolidatedResponse
                {
                    UserPrompt = prompt,
                    ConsolidatedContent = consolidatedContent,
                    FanoutResponses = responses,
                    ConsolidationMode = consolidationMode,
                    ConsolidationMetadata = consolidationMetadata,
                    TotalLatency = stopwatch.Elapsed,
                    SuccessCount = responses.Count(r => r.IsSuccessful),
                    FailureCount = responses.Count(r => !r.IsSuccessful),
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException($"Advanced consolidation failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Fan-out: Send the prompt to the specified models using the orchestrator,
        /// then map <see cref="ModelResponse"/> results into <see cref="AiModelResponse"/>
        /// objects for advanced consolidation.
        /// </summary>
        /// <param name="prompt">The prompt to send to all models.</param>
        /// <param name="models">The <see cref="OpenAIModel"/> values to query in parallel.</param>
        /// <param name="cancelToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="AiModelResponse"/> objects from all models.
        /// </returns>
        private async Task<List<AiModelResponse>> ExecuteEnsembleAsync(string prompt, OpenAIModel[] models, CancellationToken cancelToken)
        {
            // Define an ensemble orchestration request with explicit models or required capabilities.
            OrchestrationRequest request = new()
            {
                UseEnsemble = true,
                Prompt = prompt,
                OutputFormat = OutputFormat.PlainText,
                EnsembleRequest = new EnsembleDispatchRequest
                {
                    Strategy = EnsembleStrategy.Custom,
                    ExplicitModels = models,
                },
            };

            // Distribute request to all specified models in parallel using the orchestrator.
            IReadOnlyList<AiModelResponse> orchestratorResponses = await this.orchestrator.ProcessAsync(request: request, cancelToken: cancelToken);

            // Map Orchestrator's ModelResponse objects to AiModelResponse for consolidation strategies.
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