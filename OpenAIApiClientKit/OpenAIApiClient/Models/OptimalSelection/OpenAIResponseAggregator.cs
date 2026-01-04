// <copyright file="OpenAIResponseAggregator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OptimalSelection
{
    public sealed class OpenAIResponseAggregator
    {
        public static FinalResponse Aggregate(IReadOnlyList<ModelResponse> responses)
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

            // Return the final aggregated response ..
            return new FinalResponse
            {
                Content = best.RawOutput,
                Name = best.Model.Name,
                SourceResponses = responses,
                AggregationStrategy = "LongestOutput",
            };
        }
    }
}
