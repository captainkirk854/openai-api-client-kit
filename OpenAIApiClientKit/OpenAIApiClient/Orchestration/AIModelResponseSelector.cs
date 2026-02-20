// <copyright file="AIModelResponseSelector.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    public sealed class AIModelResponseSelector
    {
        /// <summary>
        /// Gets the best OpenAI model response from a list, based on the heuristic: response with the longest output = best response.
        /// </summary>
        /// <param name="responses">A read-only list of OpenAI model responses to evaluate.</param>
        /// <returns>An OpenAIModelResponseCollator containing the selected best response and related metadata.</returns>
        /// <exception cref="InvalidOperationException">Thrown if all model responses in the list have failed.</exception>
        public static AIModelResponseCollator GetOptimal(IReadOnlyList<AIModelResponse> responses)
        {
            // Filter to successful response(s) ..
            List<AIModelResponse> successful = [.. responses.Where(r => r.IsSuccessful)];
            if (successful.Count == 0)
            {
                throw new InvalidOperationException("All model calls failed.");
            }

            // Native-style heuristic: pick the highest token count (best reasoning) ..
            AIModelResponse best = successful
                .OrderByDescending(r => r.RawOutput.Length)
                .First();

            // Return the final collated response ..
            return new AIModelResponseCollator
            {
                Content = best.RawOutput,
                Name = best.Model.Name,
                SourceResponses = responses,
                CollationStrategy = "LongestOutput",
            };
        }
    }
}
