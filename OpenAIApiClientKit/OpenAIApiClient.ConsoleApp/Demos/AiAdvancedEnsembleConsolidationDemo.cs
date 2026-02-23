// <copyright file="AiAdvancedEnsembleConsolidationDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Consolidation;
    using OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring;
    using OpenAIApiClient.Models.Consolidation.Options.LLMJudge;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Consolidation;

    public static class AiAdvancedEnsembleConsolidationDemo
    {
        public static async Task GetBestModelResponseAsync(ChatClient client, string prompt, CancellationTokenSource cts)
        {
            AdvancedEnsembleConsolidation consolidation = new(client);

            OpenAIModel[] fanoutModels =
            [
                OpenAIModel.GPT5_2,
                OpenAIModel.GPT4o,
                OpenAIModel.GPT4_1_Mini,
                OpenAIModel.O4_Mini,
            ];

            OpenAIModel judgeModel = OpenAIModel.GPT5;
            OpenAIModel fusionModel = OpenAIModel.O4_Mini;

            // ========================================
            // OPTION 1: LLM AS JUDGE
            // ========================================
            Console.WriteLine("\nOPTION 1: LLM AS JUDGE\n");
            try
            {
                AdvancedConsolidatedResponse llmJudgeResponse = await consolidation.FanOutAndConsolidateAdvancedAsync(prompt: prompt,
                                                                                                                      fanoutModels: fanoutModels,
                                                                                                                      consolidationMode: ConsolidationMode.LLMAsJudge,
                                                                                                                      judgeModel: judgeModel,
                                                                                                                      cancellationToken: cts.Token);

                Console.WriteLine("Fan-out Responses:");
                foreach (AiModelResponse resp in llmJudgeResponse.FanoutResponses)
                {
                    Console.WriteLine($"  {resp.Model.Name}: {resp.RawOutput[..80]}...");
                }

                Console.WriteLine("\nJudge Decision:");
                if (llmJudgeResponse.ConsolidationMetadata is LLMJudgeResult judgeResult)
                {
                    Console.WriteLine($"  Selected: Response #{judgeResult.SelectedIndex + 1}");
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

            // ========================================
            // OPTION 2: HEURISTIC SCORING
            // ========================================
            Console.WriteLine("\n\nOPTION 2: HEURISTIC SCORING\n");
            try
            {
                AdvancedConsolidatedResponse heuristicResponse = await consolidation.FanOutAndConsolidateAdvancedAsync(prompt: prompt,
                                                                                                                       fanoutModels: fanoutModels,
                                                                                                                       consolidationMode: ConsolidationMode.HeuristicScoring,
                                                                                                                       cancellationToken: cts.Token);

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

            // ========================================
            // OPTION 3: RESPONSE FUSION
            // ========================================
            Console.WriteLine("\n\nOPTION 3: RESPONSE FUSION\n");
            try
            {
                AdvancedConsolidatedResponse fusionResponse = await consolidation.FanOutAndConsolidateAdvancedAsync(prompt: prompt,
                                                                                                                    fanoutModels: fanoutModels,
                                                                                                                    consolidationMode: ConsolidationMode.ResponseFusion,
                                                                                                                    judgeModel: fusionModel,
                                                                                                                    cancellationToken: cts.Token);

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