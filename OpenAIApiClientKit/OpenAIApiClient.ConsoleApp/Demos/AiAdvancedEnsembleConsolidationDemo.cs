// <copyright file="AiAdvancedEnsembleConsolidationDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Consolidation;
    using OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring;
    using OpenAIApiClient.Models.Consolidation.Options.LLMJudge;
    using OpenAIApiClient.Orchestration.Consolidation;
    using OpenAIApiClient.Orchestration.Execution;
    using OpenAIApiClient.Orchestration.Response;

    public static class AiAdvancedEnsembleConsolidationDemo
    {
        /// <summary>
        /// Demonstrates advanced strategies for consolidating ensemble LLM responses using LLM-as-judge,
        /// Heuristic Scoring, and Response Synthesis approaches.
        /// </summary>
        /// <remarks>
        /// This method showcases three advanced ensemble LLM response consolidation strategies and
        /// outputs results to the console for demonstration purposes.
        /// </remarks>
        /// <param name="client">The chat client used to interact with language models.</param>
        /// <param name="prompt">The prompt to dispatch to the selected models.</param>
        /// <param name="workers">An array of models to which the prompt will be dispatched for response generation.</param>
        /// <param name="judge">The model to use for the LLM-as-judge consolidation strategy; should have strong reasoning capabilities for best results.</param>
        /// <param name="synthesiser">The model to use for the Response Synthesis consolidation strategy; should have strong reasoning and synthesis capabilities for best results.</param>
        /// <param name="callMode">The mode to use for the AI calls (e.g., streaming vs non-streaming); affects how responses are received and processed.</param>
        /// <param name="cts">A cancellation token source to observe while awaiting the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task GetAdvancedResponsesAsync(ChatClient client,
                                                           string prompt,
                                                           string[] workers,
                                                           string judge,
                                                           string synthesiser,
                                                           AiCallMode callMode,
                                                           CancellationTokenSource cts)
        {
            // Create response handler and executors for orchestrating calls and consolidations ..
            DefaultAiModelResponseHandler responseHandler = new();
            OrchestratedEnsembleExecutor orchestratedExecutor = new(client: client, responseHandler: responseHandler);
            AdvancedEnsembleExecutor advancedExecutor = new(client: client);

            // Define AI call options; can be customized per call if needed. Note: the callbacks only work when using AiCallMode modes: BufferedStreaming or PushStreaming ..
            AiCallOptions options = ConfigureCallOptions(callMode: callMode, isChunkContentCallback: true, isChunkCallback: false);

            // Dispatch the prompt to the selected model(s) and get their response(s); these will be used as input for the different consolidation strategies below.
            // Note: The Chat Client has built-in retry logic to handle potential API errors or timeouts when calling multiple models, especially if using a large ensemble.
            Console.WriteLine($" Dispatching to {workers.Length} model(s)...");
            List<AiModelResponse> responses = await orchestratedExecutor.ProcessAsync(prompt: prompt,
                                                                                      models: workers,
                                                                                      options: options,
                                                                                      outputFormat: OutputFormat.PlainText,
                                                                                      cancelToken: cts.Token);

            // Option: LLM AS JUDGE
            Console.WriteLine();
            Console.WriteLine("Option: LLM AS JUDGE\n");
            try
            {
                AdvancedConsolidatedResponse llmJudgeResponse = await advancedExecutor.AdvancedConsolidationAsync(consolidationMode: AiModelConsolidationMode.LLMAsJudge,
                                                                                                                  prompt: prompt,
                                                                                                                  responses: responses,
                                                                                                                  options: options,
                                                                                                                  operatorModel: judge,
                                                                                                                  cancelToken: cts.Token);

                Console.WriteLine(" Dispatched Model Response(s):");
                int sampleOutputLength = 120;
                foreach (AiModelResponse resp in llmJudgeResponse.ModelResponses)
                {
                    string raw = resp.RawOutput ?? string.Empty;
                    int previewLength = Math.Min(raw.Length, sampleOutputLength);
                    Console.WriteLine($"  [{resp.Model.Name}]: {raw[..previewLength]}...");
                }

                Console.WriteLine($" Response selected by Judge Model:");
                if (llmJudgeResponse.ConsolidationMetadata is LLMJudgeResult judgeResult)
                {
                    Console.WriteLine($"  Selected: Response #{judgeResult.SelectedIndex}");
                    Console.WriteLine($"  Reasoning: {judgeResult.JudgeReasoning}");
                    Console.WriteLine($"  Scores: Correctness={judgeResult.JudgeScores.GetValueOrDefault("correctness")}," +
                                      $"Completeness={judgeResult.JudgeScores.GetValueOrDefault("completeness")}," +
                                      $"Alignment={judgeResult.JudgeScores.GetValueOrDefault("alignment")}," +
                                      $"Clarity={judgeResult.JudgeScores.GetValueOrDefault("clarity")}");
                }

                Console.WriteLine("Final Answer:");
                Console.WriteLine(llmJudgeResponse.ConsolidatedContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Option: HEURISTIC SCORING
            Console.WriteLine();
            Console.WriteLine("Option: HEURISTIC SCORING\n");
            try
            {
                AdvancedConsolidatedResponse heuristicResponse = await advancedExecutor.AdvancedConsolidationAsync(consolidationMode: AiModelConsolidationMode.HeuristicScoring,
                                                                                                                   prompt: prompt,
                                                                                                                   responses: responses,
                                                                                                                   options: options,
                                                                                                                   cancelToken: cts.Token);

                if (heuristicResponse.ConsolidationMetadata is HeuristicScoringResult heuristicResult)
                {
                    Console.WriteLine("Heuristic Scores:");
                    foreach ((string modelName, ScoredResponseDetail detail) in heuristicResult.ScoredResponses)
                    {
                        Console.WriteLine($"  {modelName}: {detail.TotalScore} point(s)");
                        foreach ((string factor, int score) in detail.ScoreBreakdown)
                        {
                            Console.WriteLine($"    - {factor}: {score}");
                        }
                    }

                    Console.WriteLine($"\nWinner: {heuristicResult.SelectedModel}");
                }

                Console.WriteLine("\n Final Answer:");
                Console.WriteLine(heuristicResponse.ConsolidatedContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Option: RESPONSE SYNTHESIS
            Console.WriteLine();
            Console.WriteLine("Option: RESPONSE SYNTHESIS\n");
            try
            {
                Console.WriteLine($"Using input response(s) to synthesise ..");
                AdvancedConsolidatedResponse synthesisResponse = await advancedExecutor.AdvancedConsolidationAsync(consolidationMode: AiModelConsolidationMode.ResponseSynthesis,
                                                                                                                   prompt: prompt,
                                                                                                                   responses: responses,
                                                                                                                   options: options,
                                                                                                                   operatorModel: synthesiser,
                                                                                                                   cancelToken: cts.Token);

                Console.WriteLine("Synthesised Answer:");
                Console.WriteLine(synthesisResponse.ConsolidatedContent);

                Console.WriteLine("\nSynthesis Details:");
                Console.WriteLine($" Total latency: {synthesisResponse.TotalLatency.TotalMilliseconds:F2}ms");
                Console.WriteLine($" Success rate: {synthesisResponse.SuccessCount}/{synthesisResponse.SuccessCount + synthesisResponse.FailureCount}");
                Console.WriteLine($" Model(s) used for synthesis: {string.Join(", ", synthesisResponse.ModelResponses.Select(r => r.Model.Name))}");
                Console.WriteLine($" Synthesis model: {synthesiser}");
                Console.WriteLine($" Synthesis mode: {synthesisResponse.ConsolidationMode}");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates and configures an AiCallOptions instance based on the specified call mode and callback flags.
        /// </summary>
        /// <param name="callMode">The mode to use for the AI call.</param>
        /// <param name="isChunkContentCallback">Indicates whether to set a callback for chunk delta content tokens.</param>
        /// <param name="isChunkCallback">Indicates whether to set a callback for full chunk objects.</param>
        /// <returns>A configured <see cref="AiCallOptions"/> instance with the appropriate callbacks and settings based on the provided parameters.</returns>
        private static AiCallOptions ConfigureCallOptions(AiCallMode callMode = AiCallMode.NonStreaming, bool isChunkContentCallback = false, bool isChunkCallback = false)
        {
            // If neither callback is enabled, return basic options with just the call mode set; otherwise, configure callbacks as needed based on the flags provided.
            if (isChunkContentCallback == false && isChunkCallback == false)
            {
                return new AiCallOptions
                {
                    Mode = callMode,
                };
            }
            else
            {
                // If only the chunk content callback is enabled, set it up and return options; otherwise, configure both callbacks based on the flags provided.
                if (isChunkContentCallback == true && isChunkCallback == false)
                {
                    return new AiCallOptions
                    {
                        Mode = callMode,

                        // Define callback to receive chunk delta content tokens and chunks as they arrive from the API and print to console; can be used for live streaming scenarios or to build custom progress indicators, etc ..
                        OnChunkDeltaContentToken = async (model, chunkDeltaContent) =>
                        {
                            Console.WriteLine($"chunk content: {chunkDeltaContent}");
                            await Task.Yield(); // keep it async
                        },
                        AggregateChunkContent = true,
                    };
                }

                // If only the chunk callback is enabled, set it up and return options; otherwise, configure both callbacks based on the flags provided.
                return new AiCallOptions
                {
                    Mode = callMode,

                    // Define callback to receive chunk delta content tokens and chunks as they arrive from the API and print to console; can be used for live streaming scenarios or to build custom progress indicators, etc ..
                    OnChunkDeltaContentToken = isChunkContentCallback ? async (model, chunkDeltaContent) =>
                    {
                        Console.WriteLine($"chunk content: {chunkDeltaContent}");
                        await Task.Yield(); // keep it async
                    } : null,

                    // Define callback to receive the full chunk object as it arrives from the API ..
                    OnChunk = isChunkCallback ? async (model, chunk, chunkIndex) =>
                    {
                        Console.WriteLine($"Index: {chunkIndex}  Id: {chunk.Id}");
                        await Task.Yield(); // keep it async
                    } : null,
                    AggregateChunkContent = true,
                };
            }
        }
    }
}