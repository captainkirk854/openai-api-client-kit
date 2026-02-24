// <copyright file="AdvancedEnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Consolidation
{
    using System.Diagnostics;
    using OpenAIApiClient;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Consolidation;
    using OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring;
    using OpenAIApiClient.Models.Consolidation.Options.LLMJudge;
    using OpenAIApiClient.Models.Consolidation.Options.ResponseFusion;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Consolidation.Options;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Registries.AiModels;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdvancedEnsembleExecutor"/> class.
    /// Advanced consolidation strategies for fan-out ensemble responses.
    /// Supports: LLM Judge, Heuristic Scoring, and Response Fusion.
    /// Coordinates specialized consolidation strategy classes.
    /// </summary>
    /// <param name="client">The <see cref="ChatClient"/> instance for making API calls.</param>
    public class AdvancedEnsembleExecutor(ChatClient client)
    {
        private readonly ChatClient client = client;
        private readonly OpenAIModels modelRegistry = new();
        private readonly SingleAiModelExecutor singleModelExecutor = new(client: client);
        private readonly LLMJudge llmJudgeConsolidation = new(client: client);
        private readonly ResponseFusion responseFusionConsolidation = new(client: client);

        /// <summary>
        /// Fan-out to N models and consolidate using specified strategy.
        /// </summary>
        /// <param name="prompt">The user prompt to send to all models.</param>
        /// <param name="fanoutModels">The array of <see cref="OpenAIModel"/> values to fan-out to.</param>
        /// <param name="consolidationMode">The <see cref="ConsolidationMode"/> strategy to use.</param>
        /// <param name="judgeModel">The judge <see cref="OpenAIModel"/> for LLM-based strategies (optional).</param>
        /// <param name="cancellationToken">The cancellation token for the operation.</param>
        /// <returns>
        /// An <see cref="AdvancedConsolidatedResponse"/> containing the consolidated content,
        /// metadata, and individual fan-out <see cref="AiModelResponse"/> instances.
        /// </returns>
        public async Task<AdvancedConsolidatedResponse> FanOutAndConsolidateAdvancedAsync(string prompt,
                                                                                          OpenAIModel[] fanoutModels,
                                                                                          ConsolidationMode consolidationMode,
                                                                                          OpenAIModel? judgeModel = null,
                                                                                          CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
            {
                throw new ArgumentException("Prompt cannot be empty", nameof(prompt));
            }

            if (fanoutModels.Length == 0)
            {
                throw new ArgumentException("At least one model must be specified", nameof(fanoutModels));
            }

            Stopwatch stopwatch = Stopwatch.StartNew();

            try
            {
                // Step 1: Fan-out to all models
                Console.WriteLine($" Fanning out to {fanoutModels.Length} models...");
                List<AiModelResponse> fanoutResponses = await this.FanOutToModelsAsync(prompt, fanoutModels, cancellationToken);

                // Step 2: Consolidate based on strategy
                string consolidatedContent;
                object? consolidationMetadata = null;

                switch (consolidationMode)
                {
                    case ConsolidationMode.LLMAsJudge:
                        if (judgeModel == null)
                        {
                            throw new ArgumentException("A Judge model is required for LLM Judge consolidation!", nameof(judgeModel));
                        }

                        LLMJudgeResult judgeResult = await this.llmJudgeConsolidation.ConsolidateWithLLMJudgeAsync(prompt, fanoutResponses, judgeModel.Value, cancellationToken);
                        consolidatedContent = judgeResult.SelectedResponse;
                        consolidationMetadata = judgeResult;
                        break;

                    case ConsolidationMode.HeuristicScoring:
                        HeuristicScoringResult heuristicResult = HeuristicScoring.ConsolidateWithHeuristicScoring(prompt, fanoutResponses);
                        consolidatedContent = heuristicResult.SelectedResponse;
                        consolidationMetadata = heuristicResult;
                        break;

                    case ConsolidationMode.ResponseFusion:
                        if (judgeModel == null)
                        {
                            throw new ArgumentException("A Judge model is required for Response Fusion", nameof(judgeModel));
                        }

                        ResponseFusionResult fusionResult = await this.responseFusionConsolidation.ConsolidateWithResponseFusionAsync(prompt, fanoutResponses, judgeModel.Value, cancellationToken);
                        consolidatedContent = fusionResult.SynthesizedResponse;
                        consolidationMetadata = fusionResult;
                        break;

                    default:
                        throw new ArgumentException($"Unknown consolidation mode: {consolidationMode}");
                }

                stopwatch.Stop();

                return new AdvancedConsolidatedResponse
                {
                    UserPrompt = prompt,
                    ConsolidatedContent = consolidatedContent,
                    FanoutResponses = fanoutResponses,
                    ConsolidationMode = consolidationMode,
                    ConsolidationMetadata = consolidationMetadata,
                    TotalLatency = stopwatch.Elapsed,
                    SuccessCount = fanoutResponses.Count(r => r.IsSuccessful),
                    FailureCount = fanoutResponses.Count(r => !r.IsSuccessful),
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException($"Advanced fan-out and consolidation failed after {stopwatch.ElapsedMilliseconds}ms: {ex.Message}", ex);
            }
        }

        // ============================================================================
        // HELPER: FAN-OUT TO MODELS
        // ============================================================================

        /// <summary>
        /// Fan-out: Send the prompt to N models in parallel using <see cref="SingleAiModelExecutor"/>.
        /// </summary>
        /// <param name="prompt">The prompt to send to all models.</param>
        /// <param name="models">The <see cref="OpenAIModel"/> values to query in parallel.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// A <see cref="List{T}"/> of <see cref="AiModelResponse"/> objects from all models.
        /// </returns>
        private async Task<List<AiModelResponse>> FanOutToModelsAsync(string prompt, OpenAIModel[] models, CancellationToken cancellationToken)
        {
            List<Task<AiModelResponse>> tasks = [];

            foreach (OpenAIModel model in models)
            {
                Task<AiModelResponse> task = this.QuerySingleModelAsync(prompt: prompt, model: model, cancellationToken: cancellationToken);
                tasks.Add(task);
            }

            AiModelResponse[] results = await Task.WhenAll(tasks);
            return [.. results];
        }

        /// <summary>
        /// Queries a single model using the <see cref="SingleAiModelExecutor"/>.
        /// </summary>
        /// <param name="prompt">The prompt to send to the model.</param>
        /// <param name="model">The <see cref="OpenAIModel"/> to query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>
        /// An <see cref="AiModelResponse"/> from the requested model, including success
        /// status, raw output, and cost/latency metadata.
        /// </returns>
        private async Task<AiModelResponse> QuerySingleModelAsync(string prompt, OpenAIModel model, CancellationToken cancellationToken)
        {
            // Validate model exists in registry ..
            AiModelDescriptor modelDescriptor = this.modelRegistry.Get(model);

            // Build the request for the single model executor ..
            ChatCompletionRequest request = new ClientRequestBuilder()
                .WithModel(model)
                .AddSystemMessage("You are a helpful, accurate, and concise assistant.")
                .AddUserMessage(prompt)
                .UsingMaxTokens(2000)
                .Build();

            // Execute the request and return the response, handling any exceptions to ensure we return a well-formed AiModelResponse.
            try
            {
                AiModelResponse response = await this.singleModelExecutor.ExecuteAsync(request: request, cancelToken: cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                return new AiModelResponse
                {
                    Model = modelDescriptor,
                    RawOutput = string.Empty,
                    IsSuccessful = false,
                    ErrorMessage = ex.Message,
                    Latency = TimeSpan.Zero,
                    TotalTokens = 0,
                    EstimatedCost = 0,
                };
            }
        }
    }
}