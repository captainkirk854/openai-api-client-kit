// <copyright file="EnsembleRoutingStrategyRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Routing.Ensemble;

    /// <summary>
    /// A (extendable) registry of ensemble routing strategies mapped to their implementations.
    /// </summary>
    public static class EnsembleRoutingStrategyRegistry
    {
        /// <summary>
        /// Dictionary mapping ensemble routing strategies to their corresponding strategy implementations and ultimately their associated models.
        /// </summary>
        public static readonly IReadOnlyDictionary<EnsembleRoutingStrategy, EnsembleRoutingStrategyHandler> Strategies =
            new Dictionary<EnsembleRoutingStrategy, EnsembleRoutingStrategyHandler>
            {
                [EnsembleRoutingStrategy.Reasoning] = BuildReasoningEnsemble,
                [EnsembleRoutingStrategy.Vision] = BuildVisionEnsemble,
                [EnsembleRoutingStrategy.CostOptimized] = BuildCostOptimizedEnsemble,
            };

        /// <summary>
        /// Gets the ensemble strategy for the specified routing strategy.
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns cref="EnsembleRoutingStrategyHandler">Strategy Handler.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if strategy not registered.</exception>
        public static EnsembleRoutingStrategyHandler Get(EnsembleRoutingStrategy strategy)
        {
            if (!Strategies.TryGetValue(strategy, out EnsembleRoutingStrategyHandler? handler))
            {
                throw new KeyNotFoundException($"No ensemble strategy registered for: {strategy}");
            }

            return handler;
        }

        // -------------------------
        // Strategy Implementations
        // -------------------------

        /// <summary>
        /// Creates an ensemble of models optimized for reasoning tasks by selecting high-performance reasoning,
        /// fast chat, and cost-effective critic model descriptors from the registry.
        /// </summary>
        /// <param name="registry"></param>
        /// <returns>An EnsembleRouterResult with selected reasoning, chat, and critic model descriptors.</returns>
        private static EnsembleRouterResult BuildReasoningEnsemble(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> registry)
        {
            // Select the highest performing reasoning model ..
            ModelDescriptor reasoning = registry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Reasoning))
                .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                .First();

            // .. a fast chat model that is also not the selected reasoning model ..
            ModelDescriptor fast = registry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                .Where(m => m.Name != reasoning.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            // .. and a cost-effective critic model that is neither the reasoning nor the fast model ..
            ModelDescriptor critic = registry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Reasoning))
                .Where(m => m.Name != fast.Name && m.Name != reasoning.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            return new EnsembleRouterResult(models:
             [
                reasoning,
                fast,
                critic,
            ]);
        }

        /// <summary>
        /// Creates an ensemble of models containing the highest performing vision model descriptor and the most cost-effective chat model descriptor.
        /// model from the registry.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <returns>An EnsembleRouterResult with selected vision and chat model descriptors.</returns>
        private static EnsembleRouterResult BuildVisionEnsemble(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry)
        {
            // Select the highest performing vision model ..
            ModelDescriptor vision = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Vision))
                .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                .First();

            // .. and the most cost-effective chat model that is not the selected vision model ..
            ModelDescriptor fastText = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                .Where(m => m.Name != vision.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            return new EnsembleRouterResult(models:
            [
                vision,
                fastText,
            ]);
        }

        /// <summary>
        /// Creates an ensemble of models optimized for cost by selecting low-cost, chat-capable, and high-performance model descriptors from the registry.
        /// </summary>
        /// <returns>An EnsembleRouterResult containing the selected model descriptors.</returns>
        private static EnsembleRouterResult BuildCostOptimizedEnsemble(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry)
        {
            // Select the most cost-effective low-cost model ..
            ModelDescriptor low = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.LowCost))
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            // .. a mid-tier chat model that is not the selected low-cost model ..
            ModelDescriptor mid = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                .Where(m => m.Name != low.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            // .. and a high-performance model that is neither the low-cost nor the mid-tier chat model ..
            ModelDescriptor high = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                .Where(m => m.Name != low.Name && m.Name != mid.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            return new EnsembleRouterResult(models:
            [
                low,
                mid,
                high
            ]);
        }
    }
}