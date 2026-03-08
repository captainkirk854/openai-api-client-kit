// <copyright file="AiModelPricing.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.AiModels
{
    /// <summary>
    /// Represents pricing information in U.S.Dollar$ for AI model token usage, supporting
    /// per-token and per-1,000,000-token cost schemes.
    /// </summary>
    /// <remarks>
    /// Exactly one pricing scheme must be used: either all per-token costs or all per-1M-token
    /// costs. Mixing the two schemes is not allowed.
    /// </remarks>
    public sealed class AiModelPricing
    {
        private const decimal OneMillionTokens = 1_000_000m;

        /// <summary>
        /// Initializes a new instance of the <see cref="AiModelPricing"/> class with specified costs in U.S.Dollar$.
        /// </summary>
        /// <param name="inputTokenCost">Cost per single input token (non-cached).</param>
        /// <param name="outputTokenCost">Cost per single output token.</param>
        /// <param name="cachedInputTokenCost">Cost per single cached input token.</param>
        /// <param name="reasoningTokenCost">Cost per single reasoning token (optional).</param>
        /// <param name="toolUseTokenCost">Cost per single tool-use token (optional).</param>
        /// <param name="inputCost">Cost per 1,000,000 input tokens (non-cached) (optional).</param>
        /// <param name="outputCost">Cost per 1,000,000 output tokens (optional).</param>
        /// <param name="cachedInputCost">Cost per 1,000,000 cached input tokens (optional).</param>
        /// <remarks>
        /// Exactly one pricing scheme must be used:
        /// - Either all per-token costs (inputTokenCost/outputTokenCost[/cachedInputTokenCost])
        /// - Or all per-1M-token costs (inputCost/outputCost[/cachedInputCost])
        /// Mixing the two schemes is not allowed.
        /// </remarks>
        public AiModelPricing(decimal inputTokenCost,
                              decimal outputTokenCost,
                              decimal cachedInputTokenCost = 0m,
                              decimal? reasoningTokenCost = null,
                              decimal? toolUseTokenCost = null,
                              decimal? inputCost = null,
                              decimal? outputCost = null,
                              decimal? cachedInputCost = null)
        {
            // Basic non‑negativity validation on all provided values
            ArgumentOutOfRangeException.ThrowIfNegative(inputTokenCost);
            ArgumentOutOfRangeException.ThrowIfNegative(outputTokenCost);
            ArgumentOutOfRangeException.ThrowIfNegative(cachedInputTokenCost);

            // Optional values are only validated if provided (non-null) ..
            if (reasoningTokenCost.HasValue)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(reasoningTokenCost.Value);
            }

            if (toolUseTokenCost.HasValue)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(toolUseTokenCost.Value);
            }

            if (inputCost.HasValue)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(inputCost.Value);
            }

            if (outputCost.HasValue)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(outputCost.Value);
            }

            if (cachedInputCost.HasValue)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(cachedInputCost.Value);
            }

            // Determine which pricing scheme(s) are being used ..
            bool hasPerTokenScheme = inputTokenCost > 0m || outputTokenCost > 0m || cachedInputTokenCost > 0m;
            bool hasPerMillionScheme = inputCost.HasValue || outputCost.HasValue || cachedInputCost.HasValue;

            // Enforce "only one scheme" rule
            if (hasPerTokenScheme && hasPerMillionScheme)
            {
                throw new ArgumentException("AiModelPricing must use either per-token costs OR per-1M-token costs, but not both.");
            }

            // If per‑1M‑token pricing is provided, derive per‑token pricing
            if (!hasPerTokenScheme && hasPerMillionScheme)
            {
                inputTokenCost = inputCost.GetValueOrDefault() / OneMillionTokens;
                outputTokenCost = outputCost.GetValueOrDefault() / OneMillionTokens;
                cachedInputTokenCost = cachedInputCost.GetValueOrDefault() / OneMillionTokens;
            }
            else if (hasPerTokenScheme && !hasPerMillionScheme)
            {
                // If per‑token pricing is provided, derive per‑1M‑token pricing
                inputCost = inputTokenCost * OneMillionTokens;
                outputCost = outputTokenCost * OneMillionTokens;
                cachedInputCost = cachedInputTokenCost * OneMillionTokens;
            }
            else if (!hasPerTokenScheme && !hasPerMillionScheme)
            {
                // If neither scheme is provided, default to zero-cost per-token scheme
                inputTokenCost = 0m;
                outputTokenCost = 0m;
                cachedInputTokenCost = 0m;
            }

            // Assign to properties
            this.InputTokenCost = inputTokenCost;
            this.OutputTokenCost = outputTokenCost;
            this.CachedInputTokenCost = cachedInputTokenCost;
            this.ReasoningTokenCost = reasoningTokenCost;
            this.ToolUseTokenCost = toolUseTokenCost;
            this.InputCost = inputCost;
            this.OutputCost = outputCost;
            this.CachedInputCost = cachedInputCost;
        }

        /// <summary>
        /// Gets $cost per single input token (non-cached).
        /// </summary>
        public decimal InputTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets $cost per single output token.
        /// </summary>
        public decimal OutputTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets $cost per single cached input token.
        /// </summary>
        public decimal CachedInputTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets $cost per single reasoning token (for reasoning models) (optional).
        /// </summary>
        public decimal? ReasoningTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets $cost per single tool-use token (optional).
        /// </summary>
        public decimal? ToolUseTokenCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets $cost per 1,000,000 input tokens (non-cached), if specified.
        /// </summary>
        public decimal? InputCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets $cost per 1,000,000 output tokens, if specified.
        /// </summary>
        public decimal? OutputCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets $cost per 1,000,000 cached input tokens, if specified.
        /// </summary>
        public decimal? CachedInputCost
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the total $cost per single token, summing all applicable per-token costs (input, output, cached input, reasoning, tool-use).
        /// </summary>
        public decimal TotalCostInTokens
        {
            get
            {
                decimal total = 0m;
                total += this.InputTokenCost;
                total += this.OutputTokenCost;
                total += this.CachedInputTokenCost;
                if (this.ReasoningTokenCost.HasValue)
                {
                    total += this.ReasoningTokenCost.Value;
                }
                if (this.ToolUseTokenCost.HasValue)
                {
                    total += this.ToolUseTokenCost.Value;
                }
                return total;
            }
        }

        /// <summary>
        /// Gets the total $cost per 1,000,000 tokens, calculated by multiplying the total per-token cost by 1,000,000.
        /// </summary>
        public decimal TotalCostPerMillionTokens
        {
            get
            {
                return this.TotalCostInTokens * OneMillionTokens;
            }
        }
    }
}
