// <copyright file="ModelResponseSelector.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing
{
    using OpenAIApiClient.Models.Orchestration;
    using OpenAIApiClient.Models.Routing;

    public sealed class ModelResponseSelector
    {
        /// <summary>
        /// Selects the best OpenAI model response from a list, preferring the response with the longest output among
        /// successful responses.
        /// </summary>
        /// <param name="responses">A read-only list of OpenAI model responses to evaluate.</param>
        /// <returns>An OpenAIModelResponseCollator containing the selected best response and related metadata.</returns>
        /// <exception cref="InvalidOperationException">Thrown if all model responses in the list have failed.</exception>
        public static CollatedModelResponse SelectOptimal(IReadOnlyList<ModelResponse> responses)
        {
            // Filter to successful response(s) ..
            List<ModelResponse> successful = [.. responses.Where(r => r.IsSuccessful)];
            if (successful.Count == 0)
            {
                throw new InvalidOperationException("All model calls failed.");
            }

            // Native-style heuristic: pick the highest token count (best reasoning) ..
            ModelResponse best = successful
                                .OrderByDescending(r => r.RawOutput.Length)
                                .First();

            // Return the final collated response ..
            return new CollatedModelResponse
            {
                Content = best.RawOutput,
                Name = best.Model.Model,
                SourceResponses = responses,
                CollationStrategy = "LongestOutput",
            };
        }
    }
}
