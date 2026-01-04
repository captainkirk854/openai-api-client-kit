// <copyright file="OpenAIModelResponseAggregator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.OptimalModelSelection
{
    using OpenAIApiClient.Models.OptimalModelSelection;

    public sealed class OpenAIModelResponseAggregator
    {
        public static OpenAICollatedResponse Aggregate(IReadOnlyList<OpenAIModelResponse> responses)
        {
            // Filter to successful response(s) ..
            List<OpenAIModelResponse> successful = [.. responses.Where(r => r.IsSuccessful)];
            if (successful.Count == 0)
            {
                throw new InvalidOperationException("All model calls failed.");
            }

            // Native-style heuristic: pick the highest token count (best reasoning) ..
            OpenAIModelResponse best = successful
                                .OrderByDescending(r => r.RawOutput.Length)
                                .First();

            // Return the final aggregated response ..
            return new OpenAICollatedResponse
            {
                Content = best.RawOutput,
                Name = best.Model.Model,
                SourceResponses = responses,
                AggregationStrategy = "LongestOutput",
            };
        }
    }
}
