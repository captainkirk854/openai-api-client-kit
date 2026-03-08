// <copyright file="EnsembleStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Dispatch
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.Extensions;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// Registry for Ensemble dispatching strategies.
    /// Maps an <see cref="EnsembleStrategy"/> to its corresponding handler delegate.
    /// </summary>
    public static class EnsembleStrategies
    {
        /// <summary>
        /// An internal registry storing custom ensemble strategy handlers.
        /// </summary>
        private static readonly Dictionary<AiModelStrategy.Ensemble, EnsembleDispatchStrategyHandler> CustomHandlerStrategies = [];

        /// <summary>
        /// Registers or replaces a strategy handler for the given ensemble strategy.
        /// Allows unit test code to register fake handlers.
        /// </summary>
        /// <param name="strategy">The strategy key.</param>
        /// <param name="handler">The handler delegate to register.</param>
        public static void RegisterCustomHandler(AiModelStrategy.Ensemble strategy, EnsembleDispatchStrategyHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            // Register handler for strategy in custom registry ..
            CustomHandlerStrategies[strategy] = handler;
        }

        /// <summary>
        /// Gets the ensemble handler for the specified strategy.
        /// </summary>
        /// <param name="strategy">The ensemble strategy.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchStrategyHandler"/> that can be invoked with a collection of
        /// <see cref="AiModelPropertyRegistryModel"/> instances to produce an
        /// <see cref="EnsembleDispatchResult"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="strategy"/> is not recognized.
        /// </exception>
        public static EnsembleDispatchStrategyHandler Get(AiModelStrategy.Ensemble strategy)
        {
            return strategy switch
            {
                AiModelStrategy.Ensemble.Reasoning => ReasoningEnsemble,
                AiModelStrategy.Ensemble.Vision => VisionEnsemble,
                AiModelStrategy.Ensemble.CostOptimized => CostOptimisedEnsemble,
                _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, "Unknown ensemble strategy."),
            };
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
        private static EnsembleDispatchResult ReasoningEnsemble(IEnumerable<AiModelDescriptor> models)
        {
            // 1. Reasoning model:
            // - Has reasoning capability
            // - Among those, pick highest performance
            AiModelDescriptor? reasoning =
                models
                    .Where(model => model.HasCapability(capability: AiModelCapability.Reasoning, minScore: 4, maxScore: 5))
                    .OrderByDescending(model => model.HasCapability(capability: AiModelCapability.HighPerformance, minScore: 4, maxScore: 5))
                    .FirstOrDefault() ?? throw new InvalidOperationException("No reasoning-capable model found in the registry.");

            // 2. Fast chat model:
            //    - Has chat capability
            //    - Not the reasoning model
            //    - Lowest input token cost
            AiModelDescriptor? fast =
                models
                    .Where(model => model.HasCapability(capability: AiModelCapability.Chat, minScore: 4, maxScore: 5))
                    .Where(model => !string.Equals(model.Name, reasoning.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault() ?? throw new InvalidOperationException("No chat-capable model found in the registry.");

            // 3. Critic model:
            //    - Has critic capability
            //    - Name distinct from reasoning and fast models
            //    - Lowest input token cost
            AiModelDescriptor? critic =
                models
                    .Where(model => model.HasCapability(capability: AiModelCapability.Critic, minScore: 4, maxScore: 5))
                    .Where(model => !string.Equals(model.Name, reasoning.Name, StringComparison.Ordinal)
                                 && !string.Equals(model.Name, fast.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault() ?? throw new InvalidOperationException("No critic-capable model found in the registry.");

            AiModelDescriptor[] selected =
            [
                reasoning,
                fast,
                critic,
            ];

            return new EnsembleDispatchResult(selected);
        }

        /// <summary>
        /// Creates an ensemble of models optimized for vision tasks by selecting
        /// a high-performing vision model and a cost-effective chat model.
        /// </summary>
        /// <param name="models">The available models to select from.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchResult"/> containing the selected models.
        /// </returns>
        private static EnsembleDispatchResult VisionEnsemble(IEnumerable<AiModelDescriptor> models)
        {
            // 1. Vision model:
            //    - Has strong vision capability
            //    - Among those, pick highest performance
            AiModelDescriptor? vision =
                models
                    .Where(model => model.HasCapability(capability: AiModelCapability.Vision, minScore: 5, maxScore: 5))
                    .OrderByDescending(model => model.HasCapability(capability: AiModelCapability.HighPerformance, minScore: 5, maxScore: 5))
                    .FirstOrDefault() ?? throw new InvalidOperationException("No vision-capable model found in the registry.");

            // 2. Fast chat model:
            //    - Has chat capability
            //    - Not the vision model
            //    - Lowest input token cost
            AiModelDescriptor? fastText =
                models
                    .Where(model => model.HasCapability(capability: AiModelCapability.Chat, minScore: 4, maxScore: 5))
                    .Where(model => !string.Equals(model.Name, vision.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault() ?? throw new InvalidOperationException("No chat-capable model found in the registry.");

            AiModelDescriptor[] selected =
            [
                vision,
                fastText,
            ];

            return new EnsembleDispatchResult(selected);
        }

        /// <summary>
        /// Creates an ensemble of models optimized for cost by selecting a low-cost model,
        /// a mid-tier chat model, and a high-performance model.
        /// </summary>
        /// <param name="models">The available models to select from.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchResult"/> containing the selected models.
        /// </returns>
        private static EnsembleDispatchResult CostOptimisedEnsemble(IEnumerable<AiModelDescriptor> models)
        {
            // 1. Low-cost model:
            //    - Explicitly marked low-cost or inferred via cost-efficiency
            //    - Lowest input token cost
            AiModelDescriptor? low =
                models
                    .Where(model => model.HasCapability(capability: AiModelCapability.LowCost, minScore: 4, maxScore: 5))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault() ?? throw new InvalidOperationException("No low-cost-capable model found in the registry.");

            // 2. Mid-tier chat model:
            //    - Has chat capability
            //    - Not the low-cost model
            //    - Lowest input token cost
            AiModelDescriptor? mid =
                models
                    .Where(model => model.HasCapability(capability: AiModelCapability.Chat, minScore: 4, maxScore: 5))
                    .Where(model => !string.Equals(model.Name, low.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault() ?? throw new InvalidOperationException("No chat-capable model found in the registry.");

            // 3. High-performance model:
            //    - Has high performance capability
            //    - Distinct from low and mid models
            //    - Lowest input token cost among candidates
            AiModelDescriptor? high =
                models
                    .Where(model => model.HasCapability(capability: AiModelCapability.HighPerformance, minScore: 4, maxScore: 5))
                    .Where(model =>
                        !string.Equals(model.Name, low.Name, StringComparison.Ordinal)
                        && !string.Equals(model.Name, mid.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault() ?? throw new InvalidOperationException("No high-performance-capable model found in the registry.");

            AiModelDescriptor[] selected =
            [
                low,
                mid,
                high,
            ];

            return new EnsembleDispatchResult(selected);
        }
    }
}