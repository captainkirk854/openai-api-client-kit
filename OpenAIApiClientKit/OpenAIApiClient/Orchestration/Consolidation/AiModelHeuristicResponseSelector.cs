// <copyright file="AiModelHeuristicResponseSelector.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Consolidation
{
    using OpenAIApiClient.Models.Consolidation.Options.HeuristicScoring;
    using OpenAIApiClient.Orchestration.Consolidation.Options;
    using OpenAIApiClient.Orchestration.Response;
    using OpenAIApiClient.Registries.AiModels;

    /// <summary>
    /// Provides helper methods for selecting the optimal AI model response
    /// from a set of candidates.
    /// </summary>
    public sealed class AiModelHeuristicResponseSelector
    {
        /// <summary>
        /// Gets the best OpenAI model response from a list, using the <see cref="HeuristicScoring"/> strategy.
        /// </summary>
        /// <param name="prompt">The original user prompt used to generate the responses.</param>
        /// <param name="responses">A read-only list of OpenAI model responses to evaluate.</param>
        /// <returns>
        /// An <see cref="AiModelResponseCollator"/> containing the selected best response
        /// and related metadata.
        /// </returns>
        /// <exception cref="InvalidOperationException">Thrown if there are no successful model responses to score.</exception>
        public static AiModelResponseCollator GetBestHeuristicResponse(string prompt, IReadOnlyList<AiModelResponse> responses)
        {
            // Delegate scoring to the shared HeuristicScoring implementation.
            HeuristicScoringResult scoringResult = HeuristicScoring.ConsolidateWithHeuristicScoring(prompt, [.. responses]);

            // Log the scoring details for debugging and transparency ..
            foreach ((string modelName, ScoredResponseDetail detail) in scoringResult.ScoredResponses)
            {
                Console.WriteLine($"  {modelName}: {detail.TotalScore} points");
                foreach ((string factor, int score) in detail.ScoreBreakdown)
                {
                    Console.WriteLine($"    - {factor}: {score}");
                }
            }
            Console.WriteLine($"\nWinner: {scoringResult.SelectedModel}");

            // HeuristicScoringResult.SelectedModel is the model's API name string.
            string selectedModelName = scoringResult.SelectedModel;

            // Try to find the corresponding AiModelResponse by model name.
            AiModelResponse? best = responses.FirstOrDefault(r => r.Model.Name.ToApiString().Equals(selectedModelName, StringComparison.OrdinalIgnoreCase));

            if (best is null)
            {
                // Fallback: use the selected index if name-based lookup fails.
                int idx = scoringResult.SelectedModelIndex;
                if (idx < 0 || idx >= responses.Count)
                {
                    throw new InvalidOperationException("HeuristicScoring returned an invalid SelectedModelIndex.");
                }

                best = responses[idx];
            }

            // Return the final collated response ..
            return new AiModelResponseCollator
            {
                Content = best.RawOutput,
                Name = best.Model.Name,
                SourceResponses = responses,
                CollationStrategy = "HeuristicScoring",
            };
        }
    }
}