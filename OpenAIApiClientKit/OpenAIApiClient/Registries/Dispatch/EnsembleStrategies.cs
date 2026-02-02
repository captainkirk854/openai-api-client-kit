// <copyright file="EnsembleStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Dispatch
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// Registry for Ensemble dispatching strategies.
    /// Maps an <see cref="EnsembleStrategy"/> to its corresponding handler delegate.
    /// </summary>
    public static class EnsembleStrategies
    {
        /// <summary>
        /// Dictionary mapping ensemble-model dispatch strategies to their specific delegate handler implementations.
        /// </summary>
        public static readonly IReadOnlyDictionary<EnsembleStrategy, EnsembleStrategyHandler> DefaultHandlerStrategies =
            new Dictionary<EnsembleStrategy, EnsembleStrategyHandler>
            {
                [EnsembleStrategy.Reasoning] = BuildReasoningEnsemble,
                [EnsembleStrategy.Vision] = BuildVisionEnsemble,
                [EnsembleStrategy.CostOptimized] = BuildCostOptimisedEnsemble,
            };

        /// <summary>
        /// An internal registry storing custom ensemble strategy handlers.
        /// </summary>
        private static readonly Dictionary<EnsembleStrategy, EnsembleStrategyHandler> CustomHandlerStrategies = [];

        /// <summary>
        /// Registers or replaces a strategy handler for the given ensemble strategy.
        /// Allows unit test code to register fake handlers.
        /// </summary>
        /// <param name="strategy">The strategy key.</param>
        /// <param name="handler">The handler delegate to register.</param>
        public static void RegisterCustomHandler(EnsembleStrategy strategy, EnsembleStrategyHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            // Register handler for strategy in custom registry ..
            CustomHandlerStrategies[strategy] = handler;
        }

        /// <summary>
        /// Retrieves the handler for a given strategy.
        /// </summary>
        /// <param name="strategy">The strategy to retrieve.</param>
        /// <returns cref="EnsembleStrategyHandler">The registered handler.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no handler is registered for the strategy.</exception>
        public static EnsembleStrategyHandler Get(EnsembleStrategy strategy)
        {
            // 1. Override and use Custom strategy handler if found for strategy ..
            if (CustomHandlerStrategies.TryGetValue(strategy, out EnsembleStrategyHandler? customHandler))
            {
                return customHandler;
            }

            // 2. Otherwise use Default strategy handler if found for strategy ..
            if (DefaultHandlerStrategies.TryGetValue(strategy, out EnsembleStrategyHandler? defaultHandler))
            {
                return defaultHandler;
            }

            throw new KeyNotFoundException($"No ensemble strategy handler registered for: {strategy}");
        }

        /// <summary>
        /// Clears all custom strategy overrides.
        /// Useful for unit tests to ensure isolation.
        /// </summary>
        public static void ClearCustomHandlers()
        {
            CustomHandlerStrategies.Clear();
        }

        // -------------------------
        // Strategy Implementations
        // -------------------------

        /// <summary>
        /// Creates an ensemble of models optimized for reasoning tasks by selecting high-performance reasoning,
        /// fast chat, and cost-effective critic model descriptors from the registry.
        /// </summary>
        /// <param name="registry"></param>
        /// <returns cref="EnsembleDispatchResult"> with selected reasoning, chat, and critic model descriptors.</returns>
        private static EnsembleDispatchResult BuildReasoningEnsemble(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> registry)
        {
            // Select the highest performing reasoning model ..
            ModelDescriptor? reasoning = registry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Reasoning))
                .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                .FirstOrDefault() ?? throw new InvalidOperationException("No Reasoning-capable model found in the registry.");

            // .. a fast chat model that is also not the selected reasoning model ..
            ModelDescriptor? fast = registry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                .Where(m => m.Name != reasoning.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("No Chat-capable model found in the registry.");

            // .. and a cost-effective critic model that is neither the reasoning nor the fast model ..
            ModelDescriptor? critic = registry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Critic))
                .Where(m => m.Name != fast.Name && m.Name != reasoning.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("No Critic-capable model found in the registry.");

            return new EnsembleDispatchResult(models:
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
        /// <returns cref="EnsembleDispatchResult"> with selected vision and chat model descriptors.</returns>
        private static EnsembleDispatchResult BuildVisionEnsemble(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry)
        {
            // Select the highest performing vision model ..
            ModelDescriptor? vision = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Vision))
                .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                .FirstOrDefault() ?? throw new InvalidOperationException("No Vision-capable model found in the registry.");

            // .. and the most cost-effective chat model that is not the selected vision model ..
            ModelDescriptor? fastText = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                .Where(m => m.Name != vision.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("No Chat-capable model found in the registry.");

            return new EnsembleDispatchResult(models:
            [
                vision,
                fastText,
            ]);
        }

        /// <summary>
        /// Creates an ensemble of models optimized for cost by selecting low-cost, chat-capable, and high-performance model descriptors from the registry.
        /// </summary>
        /// <returns cref="EnsembleDispatchResult"> with selected cost-optimised, chat-capable, and high-performance model descriptors.</returns>
        private static EnsembleDispatchResult BuildCostOptimisedEnsemble(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry)
        {
            // Select the most cost-effective low-cost model ..
            ModelDescriptor? low = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.LowCost))
                .OrderBy(m => m.Pricing.InputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("No LowCost-capable model found in the registry.");

            // .. a mid-tier chat model that is not the selected low-cost model ..
            ModelDescriptor? mid = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                .Where(m => m.Name != low.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("No Chat-capable model found in the registry.");

            // .. and a high-performance model that is neither the low-cost nor the mid-tier chat model ..
            ModelDescriptor? high = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                .Where(m => m.Name != low.Name && m.Name != mid.Name)
                .OrderBy(m => m.Pricing.InputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("No HighPerformance-capable model found in the registry.");

            return new EnsembleDispatchResult(models:
            [
                low,
                mid,
                high
            ]);
        }
    }
}