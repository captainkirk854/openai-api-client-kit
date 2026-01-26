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
        /// <returns>EnsembleStrategy definition.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if strategy not registered.</exception>
        public static EnsembleRoutingStrategyHandler Get(EnsembleRoutingStrategy strategy)
        {
            if (!Strategies.TryGetValue(strategy, out EnsembleRoutingStrategyHandler? strat))
            {
                throw new KeyNotFoundException($"No ensemble strategy registered for {strategy}");
            }

            return strat;
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
            ModelDescriptor reasoning = registry.Values
                                        .Where(m => m.Capabilities.Contains(ModelCapability.Reasoning))
                                        .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                                        .First();

            ModelDescriptor fast = registry.Values
                                   .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                                   .OrderBy(m => m.Pricing.InputTokenCost)
                                   .First();

            ModelDescriptor critic = registry.Values
                                     .Where(m => m.Capabilities.Contains(ModelCapability.Reasoning))
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
            ModelDescriptor vision = modelRegistry.Values
                                     .Where(m => m.Capabilities.Contains(ModelCapability.Vision))
                                     .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                                     .First();

            ModelDescriptor fastText = modelRegistry.Values
                                       .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
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
            ModelDescriptor low = modelRegistry.Values
                                  .Where(m => m.Capabilities.Contains(ModelCapability.LowCost))
                                  .OrderBy(m => m.Pricing.InputTokenCost)
                                  .First();

            ModelDescriptor mid = modelRegistry.Values
                                  .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                                  .OrderBy(m => m.Pricing.InputTokenCost)
                                  .Skip(1)
                                  .FirstOrDefault() ?? low;

            ModelDescriptor high = modelRegistry.Values
                                   .Where(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
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
