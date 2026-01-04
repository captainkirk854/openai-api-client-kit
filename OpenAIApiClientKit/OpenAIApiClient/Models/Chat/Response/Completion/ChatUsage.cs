// <copyright file="ChatUsage.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Response.Completion
{
    using System.Text.Json.Serialization;

    public class ChatUsage
    {
        /// <summary>
        /// Gets or sets the number of tokens used in the prompt.
        /// </summary>
        [JsonPropertyName("prompt_tokens")]
        public int? PromptTokens
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of tokens generated in the completion.
        /// </summary>
        [JsonPropertyName("completion_tokens")]
        public int? CompletionTokens
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Number of reasoning tokens consumed by the request.
        /// </summary>
        [JsonPropertyName("reasoning_tokens")]
        public int? ReasoningTokens
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the Total number of tokens consumed by the request.
        /// </summary>
        [JsonPropertyName("total_tokens")]
        public int? TotalTokens
        {
            get
            {
                return (this.PromptTokens ?? 0) + (this.CompletionTokens ?? 0) + (this.ReasoningTokens ?? 0);
            }
        }

        /// <summary>
        /// Gets or sets the details about prompt tokens.
        /// </summary>
        [JsonPropertyName("prompt_tokens_details")]
        public PromptTokensDetails? PromptTokensDetails
        {
            get;
            set;
        }

        /// <summary>
        /// Calculates the cost of the request based on the provided model pricing.
        /// </summary>
        /// <param name="pricing"></param>
        /// <returns>see <see cref="decimal?"/>.</returns>
        public decimal? CalculateCost(ModelPricing pricing)
        {
            // Set cost components ..
            decimal? inputCost = (this.PromptTokens - this.PromptTokensDetails!.CachedTokens) * pricing.InputTokenCost;
            decimal? cachedCost = this.PromptTokensDetails.CachedTokens * pricing.CachedInputTokenCost;
            decimal? outputCost = this.CompletionTokens * pricing.OutputTokenCost;
            decimal? reasoningCost = (this.ReasoningTokens ?? 0m) * (pricing.ReasoningTokenCost ?? 0m);

            // Return total cost ..
            return inputCost + cachedCost + outputCost + reasoningCost;
        }
    }
}
