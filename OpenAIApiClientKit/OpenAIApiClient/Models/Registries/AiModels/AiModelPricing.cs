// <copyright file="AiModelPricing.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.AiModels
{
    public sealed class AiModelPricing
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AiModelPricing"/> class.
        /// </summary>
        /// <param name="cachedInputTokenCost">Cost per single cached input token.</param>
        /// <param name="inputTokenCost">Cost per single input token (non-cached).</param>
        /// <param name="outputTokenCost">Cost per single output token.</param>
        /// <param name="reasoningTokenCost">Cost per single reasoning token (for reasoning models) (optional).</param>
        /// <param name="toolUseTokenCost">Cost per single tool-use token (optional).</param>
        public AiModelPricing(decimal inputTokenCost, decimal outputTokenCost, decimal cachedInputTokenCost = 0m, decimal? reasoningTokenCost = null, decimal? toolUseTokenCost = null)
        {
            // Validate parameters ..
            ArgumentOutOfRangeException.ThrowIfNegative(inputTokenCost);
            ArgumentOutOfRangeException.ThrowIfNegative(outputTokenCost);
            ArgumentOutOfRangeException.ThrowIfNegative(cachedInputTokenCost);

            // Assign to properties ..
            this.InputTokenCost = inputTokenCost;
            this.OutputTokenCost = outputTokenCost;
            this.CachedInputTokenCost = cachedInputTokenCost;
            this.ReasoningTokenCost = reasoningTokenCost;
            this.ToolUseTokenCost = toolUseTokenCost;
        }

        /// <summary>
        /// Gets cost per single input token (non-cached).
        /// </summary>
        public decimal InputTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets cost per single cached input token.
        /// </summary>
        public decimal CachedInputTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets cost per single output token.
        /// </summary>
        public decimal OutputTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets cost per single reasoning token (for reasoning models) (optional).
        /// </summary>
        public decimal? ReasoningTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets cost per single tool-use token (optional).
        /// </summary>
        public decimal? ToolUseTokenCost
        {
            get;
            init;
        }
    }
}
