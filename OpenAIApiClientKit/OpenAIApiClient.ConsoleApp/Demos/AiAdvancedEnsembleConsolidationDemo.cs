// <copyright file="AiAdvancedEnsembleConsolidationDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.Extensions;
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
        /// This method showcases three advanced ensemble response consolidation strategies and
        /// outputs results to the console for demonstration purposes.
        /// </remarks>
        /// <param name="client">The chat client used to interact with language models.</param>
        /// <param name="prompt">The prompt to dispatch to the selected models.</param>
        /// <param name="useStreaming">Indicates whether to use streaming mode for responses.</param>
        /// <param name="useReasoningModels">Indicates whether to use reasoning-optimized models for dispatch.</param>
        /// <param name="cts">A cancellation token source to observe while awaiting the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task GetAdvancedResponsesAsync(ChatClient client, string prompt, bool useStreaming, bool useReasoningModels, CancellationTokenSource cts)
        {
            // This demo showcases three advanced consolidation strategies for ensemble LLM responses ..
            DefaultAiModelResponseHandler responseHandler = new();
            OrchestratedEnsembleExecutor orchestratedExecutor = new(client: client, responseHandler: responseHandler);
            AdvancedEnsembleExecutor advancedExecutor = new(client: client, responseHandler: responseHandler);

            // Define AI call options for this demo; can be customized per call if needed. Note: the callbacks only work when using AiCallMode modes: BufferedStreaming or PushStreaming
            AiCallMode callMode = useStreaming ? AiCallMode.BufferedStreaming : AiCallMode.NonStreaming;
            AiCallOptions options = ConfigureCallOptions(callMode: callMode, isChunkContentCallback: true, isChunkCallback: false);

            // Initialise a list of model(s) to dispatch the prompt to; can be any combination of models based on caller's needs and preferences ..
            OpenAIModel[] dispatchModels;
            if (useReasoningModels)
            {
                dispatchModels = EnsembleStrategy.Reasoning.GetOpenAIModels();
            }
            else
            {
                // Provide a custom list of models to dispatch to; for best results, include a mix of models with strong reasoning capabilities as well as some cost effective models for diversity (e.g. GPT5_2, GPT4o, GPT4_1_Mini, O4_Mini, etc ..)
                dispatchModels =
                [
                    OpenAIModel.GPT5_2,
                    OpenAIModel.GPT4o,
                    OpenAIModel.GPT4_1_Mini,
                    OpenAIModel.O4_Mini,
                ];
            }

            // Specify the judge models to use for LLM-as-judge and response fusion strategies; models should have good reasoning capabilities for best results (e.g. GPT5 or GPT5_2)
            OpenAIModel judgeModel = OpenAIModel.GPT5;
            OpenAIModel synthesisModel = OpenAIModel.GPT5_2;

            // Dispatch the prompt to the selected model(s) and get their response(s); these will be used as input for the different consolidation strategies below.#
            // Note: in a real application, you might want to implement some retry logic here to handle potential API errors or timeouts when calling multiple models, especially if using a large ensemble.
            Console.WriteLine($" Dispatching to {dispatchModels.Length} model(s)...");
            List<AiModelResponse> responses = await orchestratedExecutor.ProcessAsync(prompt: prompt, models: dispatchModels, options: options, outputFormat: OutputFormat.PlainText, cancelToken: cts.Token);

            // Option: LLM AS JUDGE
            Console.WriteLine();
            Console.WriteLine("Option: LLM AS JUDGE\n");
            try
            {
                AdvancedConsolidatedResponse llmJudgeResponse = await advancedExecutor.AdvancedConsolidatationAsync(prompt: prompt,
                                                                                                                    responses: responses,
                                                                                                                    consolidationMode: ConsolidationMode.LLMAsJudge,
                                                                                                                    options: options,
                                                                                                                    operationalModel: judgeModel,
                                                                                                                    cancelToken: cts.Token);

                Console.WriteLine(" Dispatched Model Response(s):");
                foreach (AiModelResponse resp in llmJudgeResponse.ModelResponses)
                {
                    Console.WriteLine($"  [{resp.Model.Name}]: {resp.RawOutput[..80]}...");
                }

                Console.WriteLine($" Response selected by Judge Model [{judgeModel}]:");
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
                AdvancedConsolidatedResponse heuristicResponse = await advancedExecutor.AdvancedConsolidatationAsync(prompt: prompt,
                                                                                                                     responses: responses,
                                                                                                                     consolidationMode: ConsolidationMode.HeuristicScoring,
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
                AdvancedConsolidatedResponse synthesisResponse = await advancedExecutor.AdvancedConsolidatationAsync(prompt: prompt,
                                                                                                                     responses: responses,
                                                                                                                     consolidationMode: ConsolidationMode.ResponseSynthesis,
                                                                                                                     options: options,
                                                                                                                     operationalModel: synthesisModel,
                                                                                                                     cancelToken: cts.Token);

                Console.WriteLine("Synthesised Answer:");
                Console.WriteLine(synthesisResponse.ConsolidatedContent);

                Console.WriteLine($" Total latency: {synthesisResponse.TotalLatency.TotalMilliseconds:F2}ms");
                Console.WriteLine($" Success rate: {synthesisResponse.SuccessCount}/{synthesisResponse.SuccessCount + synthesisResponse.FailureCount}");
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
            if(isChunkContentCallback == false && isChunkCallback == false)
            {
                return new AiCallOptions
                {
                    Mode = callMode,
                };
            }
            else
            {
                if(isChunkContentCallback == true && isChunkCallback == false)
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