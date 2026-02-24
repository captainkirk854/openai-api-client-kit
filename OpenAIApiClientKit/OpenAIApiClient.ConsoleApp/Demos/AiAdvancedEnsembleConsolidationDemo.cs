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
    using OpenAIApiClient.Orchestration.Response;

    public static class AiAdvancedEnsembleConsolidationDemo
    {
        public static async Task GetBestModelResponseAsync(ChatClient client, string prompt, CancellationTokenSource cts)
        {
            // This demo showcases three advanced consolidation strategies for ensemble LLM responses:
            DefaultAiModelResponseHandler responseHandler = new();
            AdvancedEnsembleExecutor executor = new(client: client, responseHandler: responseHandler);

            // We will fan-out the same prompt to multiple models and then consolidate their responses using different strategies:
            OpenAIModel[] fanoutModels =
            [
                OpenAIModel.GPT5_2,
                OpenAIModel.GPT4o,
                OpenAIModel.GPT4_1_Mini,
                OpenAIModel.O4_Mini,
            ];

            // For the LLM-as-Judge and Response Fusion strategies, we need to specify a model that will act as the judge and fusion engine:
            OpenAIModel judgeModel = OpenAIModel.GPT5;
            OpenAIModel fusionModel = OpenAIModel.O4_Mini;

            // OPTION 1: LLM AS JUDGE
            Console.WriteLine("\nOPTION 1: LLM AS JUDGE\n");
            try
            {
                AdvancedConsolidatedResponse llmJudgeResponse = await executor.FanOutAndConsolidateAdvancedAsync(prompt: prompt,
                                                                                                                 models: fanoutModels,
                                                                                                                 consolidationMode: ConsolidationMode.LLMAsJudge,
                                                                                                                 judgeModel: judgeModel,
                                                                                                                 cancelToken: cts.Token);

                Console.WriteLine("Fan-out Responses:");
                foreach (AiModelResponse resp in llmJudgeResponse.FanoutResponses)
                {
                    Console.WriteLine($"  {resp.Model.Name}: {resp.RawOutput[..80]}...");
                }

                Console.WriteLine("\nJudge Decision:");
                if (llmJudgeResponse.ConsolidationMetadata is LLMJudgeResult judgeResult)
                {
                    Console.WriteLine($"  Selected: Response #{judgeResult.SelectedIndex}");
                    Console.WriteLine($"  Reasoning: {judgeResult.JudgeReasoning}");
                    Console.WriteLine($"  Scores: Correctness={judgeResult.JudgeScores.GetValueOrDefault("correctness")}, " + $"Completeness={judgeResult.JudgeScores.GetValueOrDefault("completeness")}");
                }

                Console.WriteLine("\nFinal Answer:");
                Console.WriteLine(llmJudgeResponse.ConsolidatedContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // OPTION 2: HEURISTIC SCORING
            Console.WriteLine("\n\nOPTION 2: HEURISTIC SCORING\n");
            try
            {
                AdvancedConsolidatedResponse heuristicResponse = await executor.FanOutAndConsolidateAdvancedAsync(prompt: prompt,
                                                                                                                  models: fanoutModels,
                                                                                                                  consolidationMode: ConsolidationMode.HeuristicScoring,
                                                                                                                  cancelToken: cts.Token);

                if (heuristicResponse.ConsolidationMetadata is HeuristicScoringResult heuristicResult)
                {
                    Console.WriteLine("Heuristic Scores:");
                    foreach ((string modelName, ScoredResponseDetail detail) in heuristicResult.ScoredResponses)
                    {
                        Console.WriteLine($"  {modelName}: {detail.TotalScore} points");
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

            // OPTION 3: RESPONSE FUSION
            Console.WriteLine("\n\nOPTION 3: RESPONSE FUSION\n");
            try
            {
                AdvancedConsolidatedResponse fusionResponse = await executor.FanOutAndConsolidateAdvancedAsync(prompt: prompt,
                                                                                                               models: fanoutModels,
                                                                                                               consolidationMode: ConsolidationMode.ResponseFusion,
                                                                                                               judgeModel: fusionModel,
                                                                                                               cancelToken: cts.Token);

                Console.WriteLine("Synthesizing all responses...");
                Console.WriteLine("\nSynthesized Answer:");
                Console.WriteLine(fusionResponse.ConsolidatedContent);

                Console.WriteLine($"\n  Total latency: {fusionResponse.TotalLatency.TotalMilliseconds:F2}ms");
                Console.WriteLine($" Success rate: {fusionResponse.SuccessCount}/{fusionResponse.SuccessCount + fusionResponse.FailureCount}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error: {ex.Message}");
            }
        }
    }
}