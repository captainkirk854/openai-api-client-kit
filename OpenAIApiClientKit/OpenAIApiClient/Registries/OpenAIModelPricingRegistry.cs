// <copyright file="OpenAIModelPricingRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries
{
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Response.Completion;

    /// <summary>
    /// Registry of pricing information for OpenAI models.
    /// </summary>
    public static class OpenAIModelPricingRegistry
    {
        /// <summary>
        /// Dictionary mapping OpenAI models to their pricing details.
        /// <see cref="ModelPricing"/> for details on pricing structure."/>.
        /// </summary>
        public static readonly IReadOnlyDictionary<OpenAIModels, ModelPricing> Pricing =
            new Dictionary<OpenAIModels, ModelPricing>
            {
                // GPT‑4.1 — Standard Reasoning Model
                [OpenAIModels.GPT4_1] = new ModelPricing(
                    inputTokenCost: 0.00001m,
                    outputTokenCost: 0.00003m,
                    cachedInputTokenCost: 0.000002m),

                // GPT‑4.1 Mini — Fast, Low‑Cost Model
                [OpenAIModels.GPT4_1_Mini] = new ModelPricing(
                    inputTokenCost: 0.0000015m,
                    outputTokenCost: 0.000002m),

                // GPT‑4.1 Critic — Evaluation / Critique Model
                [OpenAIModels.GPT4_1_Critic] = new ModelPricing(
                    inputTokenCost: 0.00001m,
                    outputTokenCost: 0.00003m),

                // GPT‑4o Mini — NEW
                [OpenAIModels.GPT4o_Mini] = new ModelPricing(
                    inputTokenCost: 0.00000015m,
                    outputTokenCost: 0.00000060m),
            };
    }
}
