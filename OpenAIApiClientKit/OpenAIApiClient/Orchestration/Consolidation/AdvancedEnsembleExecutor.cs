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
    using OpenAIApiClient.Models.Consolidation.Options.ResponseSynthesis;
    using OpenAIApiClient.Orchestration.Consolidation.Options;
    using OpenAIApiClient.Orchestration.Execution;
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
        // Specialized consolidation strategy classes that encapsulate the logic for each approach.
        private readonly OrchestratedEnsembleExecutor orchestratedEnsembleExecutor = new(client: client, responseHandler: responseHandler);
        private readonly LLMJudge llmJudgeStrategy = new(client: client);
        private readonly ResponseSynthesis responseSynthesisStrategy = new(client: client);

        /// <summary>
        /// Performs advanced consolidation of multiple model responses based on the specified consolidation mode.
        /// </summary>
        /// <param name="consolidationMode">The <see cref="ConsolidationMode"/> strategy to use.</param>
        /// <param name="prompt">The user prompt to send to all models.</param>
        /// <param name="responses">The list of <see cref="AiModelResponse"/> objects from the ensemble model calls.</param>
        /// <param name="options">Options for execution, such as chunk handling and aggregation (optional).</param>
        /// <param name="operationalModel">The judge <see cref="OpenAIModel"/> for LLM-based strategies (optional).</param>
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
        public async Task<AdvancedConsolidatedResponse> AdvancedConsolidatationAsync(ConsolidationMode consolidationMode,
                                                                                     string prompt,
                                                                                     List<AiModelResponse> responses,
                                                                                     AiCallOptions options,
                                                                                     OpenAIModel? operationalModel = null,
                                                                                     CancellationToken cancelToken = default)
        {
            // Filter out unsuccessful or blank model response(s) ..
            List<AiModelResponse> successfulResponses = [.. responses.Where(r => r.IsSuccessful && !string.IsNullOrEmpty(r.RawOutput))];
            Console.WriteLine($" Received {successfulResponses.Count} response(s) from {responses.Count} model(s)...");
            if (successfulResponses.Count == 0)
            {
                throw new InvalidOperationException("No successful model response(s) found!");
            }

            // Begin consolidation process based on consolidation mode using successful response(s) ..
            Stopwatch stopwatch = Stopwatch.StartNew();
            try
            {
                string consolidatedContent;
                object? consolidationMetadata = null;
                switch (consolidationMode)
                {
                    // LLM Judge consolidation mode uses a separate LLM to evaluate the successful model response(s) and select the best one based on the prompt ..
                    case ConsolidationMode.LLMAsJudge:
                        if (operationalModel is null)
                        {
                            throw new ArgumentException("An operational LLM is required for LLM Judge consolidation!", nameof(operationalModel));
                        }
                        LLMJudgeResult judgeResult = await this.llmJudgeStrategy.ConsolidateWithLLMJudgeAsync(prompt: prompt,
                                                                                                              responses: successfulResponses,
                                                                                                              judgeModel: operationalModel.Value,
                                                                                                              execution: options,
                                                                                                              cancellationToken: cancelToken);
                        consolidatedContent = judgeResult.SelectedResponse;
                        consolidationMetadata = judgeResult;
                        break;

                    // Heuristic Scoring consolidation mode applies a set of predefined heuristics to score and select the best response without using an additional LLM.
                    case ConsolidationMode.HeuristicScoring:
                        HeuristicScoringResult heuristicResult = HeuristicScoring.ConsolidateWithHeuristicScoring(prompt: prompt,
                                                                                                                  responses: successfulResponses);
                        consolidatedContent = heuristicResult.SelectedResponse;
                        consolidationMetadata = heuristicResult;
                        break;

                    // Response Synthesis consolidation mode uses a separate LLM to synthesise a new response that optimally blends the information from all input model response(s) ..
                    case ConsolidationMode.ResponseSynthesis:
                        if (operationalModel is null)
                        {
                            throw new ArgumentException("An operational LLM is required for Response Synthesis", nameof(operationalModel));
                        }
                        ResponseSynthesisResult synthesisResult = await this.responseSynthesisStrategy.ConsolidateWithResponseSynthesisAsync(prompt: prompt,
                                                                                                                                               responses: successfulResponses,
                                                                                                                                               synthesisModel: operationalModel.Value,
                                                                                                                                               options: options,
                                                                                                                                               cancelToken: cancelToken);
                        consolidatedContent = synthesisResult.SynthesisedResponse;
                        consolidationMetadata = synthesisResult;
                        break;

                    default:
                        throw new ArgumentException($"Unknown consolidation mode: {consolidationMode}");
                }

                stopwatch.Stop();

                // Return a comprehensive response object with all relevant data and metadata ..
                return new AdvancedConsolidatedResponse
                {
                    UserPrompt = prompt,
                    ConsolidatedContent = consolidatedContent,
                    ModelResponses = successfulResponses,
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
    }
}