// <copyright file="ChatUsage.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.Chat.Response.Completion
{
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Registries.AiModels;
    using testClass = OpenAIApiClient.Models.Chat.Response.Completion.ChatUsage;

    [TestClass]
    public class ChatUsage
    {
        [TestMethod]
        public void CalculateCost_WithPerTokenPricing_ComputesExpectedCost()
        {
            // Arrange
            testClass usage = new()
            {
                PromptTokens = 100,
                CompletionTokens = 50,
                ReasoningTokens = 20,
                PromptTokensDetails = new PromptTokensDetails
                {
                    CachedTokens = 30,
                },
            };

            // Per-token pricing
            AiModelPricing pricing = new (inputTokenCost: 0.001m,   // $0.001 per input token
                                          outputTokenCost: 0.002m,  // $0.002 per output token
                                          cachedInputTokenCost: 0.0005m,
                                          reasoningTokenCost: 0.01m);

            // Manually calculate expected values:
            // non‑cached input tokens = PromptTokens - CachedTokens = 100 - 30 = 70
            // inputCost   = 70  * 0.001  = 0.070
            // cachedCost  = 30  * 0.0005 = 0.015
            // outputCost  = 50  * 0.002  = 0.100
            // reasoning   = 20  * 0.01   = 0.200
            // total       = 0.070 + 0.015 + 0.100 + 0.200 = 0.385
            decimal expected = 0.385m;

            // Act
            decimal? actual = usage.CalculateCost(pricing);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculateCost_WithPerMillionPricing_ComputesExpectedCost()
        {
            // Arrange
            testClass usage = new()
            {
                PromptTokens = 10_000,
                CompletionTokens = 5_000,
                ReasoningTokens = 0,
                PromptTokensDetails = new PromptTokensDetails
                {
                    CachedTokens = 2_000,
                },
            };

            // Per‑1M‑token pricing: ModelPricing will convert to per‑token internally
            decimal inputCostPerMillion = 1.00m;   // $1.00 per 1M non‑cached input tokens
            decimal outputCostPerMillion = 2.00m;  // $2.00 per 1M output tokens
            decimal cachedInputCostPerMillion = 0.50m;

            AiModelPricing pricing = new(inputTokenCost: 0m,
                                         outputTokenCost: 0m,
                                         cachedInputTokenCost: 0m,
                                         reasoningTokenCost: null,
                                         toolUseTokenCost: null,
                                         inputCost: inputCostPerMillion,
                                         outputCost: outputCostPerMillion,
                                         cachedInputCost: cachedInputCostPerMillion);

            // Derived per‑token costs:
            decimal inputTokenCost = inputCostPerMillion / 1_000_000m;
            decimal outputTokenCost = outputCostPerMillion / 1_000_000m;
            decimal cachedInputTokenCost = cachedInputCostPerMillion / 1_000_000m;

            // non‑cached input = 10_000 - 2_000 = 8_000
            // inputCost  = 8_000  * (1 / 1_000_000)   = 8_000 / 1_000_000
            // cachedCost = 2_000  * (0.5 / 1_000_000) = 1_000 / 1_000_000
            // outputCost = 5_000  * (2 / 1_000_000)   = 10_000 / 1_000_000
            // total      = (8_000 + 1_000 + 10_000) / 1_000_000 = 19_000 / 1_000_000 = 0.019
            decimal expected =
                (8_000m * inputTokenCost) +
                (2_000m * cachedInputTokenCost) +
                (5_000m * outputTokenCost);

            // Sanity: expected should be 0.019m
            Assert.AreEqual(0.019m, expected);

            // Act
            decimal? actual = usage.CalculateCost(pricing);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculateCost_WithNullReasoningTokens_AndNoReasoningPrice_IgnoresReasoning()
        {
            // Arrange
            testClass usage = new()
            {
                PromptTokens = 100,
                CompletionTokens = 50,
                ReasoningTokens = null,
                PromptTokensDetails = new PromptTokensDetails
                {
                    CachedTokens = 0,
                },
            };

            AiModelPricing pricing = new(inputTokenCost: 0.001m,
                                         outputTokenCost: 0.002m,
                                         cachedInputTokenCost: 0m,
                                         reasoningTokenCost: null);

            // expected = (100 * 0.001) + (0 * 0) + (50 * 0.002) + 0
            decimal expected = 0.100m + 0.100m; // 0.200

            // Act
            decimal? actual = usage.CalculateCost(pricing);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CalculateCost_WithZeroTokens_ReturnsZero()
        {
            // Arrange
            testClass usage = new()
            {
                PromptTokens = 0,
                CompletionTokens = 0,
                ReasoningTokens = 0,
                PromptTokensDetails = new PromptTokensDetails
                {
                    CachedTokens = 0,
                },
            };

            AiModelPricing pricing = new(inputTokenCost: 0.001m,
                                         outputTokenCost: 0.002m,
                                         cachedInputTokenCost: 0.0005m,
                                         reasoningTokenCost: 0.01m);

            // Act
            decimal? actual = usage.CalculateCost(pricing);

            // Assert
            Assert.AreEqual(0m, actual);
        }
    }
}
