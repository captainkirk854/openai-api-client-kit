// <copyright file="EnsembleStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Dispatch
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// Registry for Ensemble dispatching strategies.
    /// Maps an <see cref="EnsembleStrategy"/> to its corresponding handler delegate.
    /// </summary>
    public static class EnsembleStrategies
    {
        ///// <summary>
        ///// Dictionary mapping ensemble-model dispatch strategies to their specific delegate handler implementations.
        ///// </summary>
        //public static readonly IReadOnlyDictionary<EnsembleStrategy, EnsembleStrategyHandler> DefaultHandlerStrategies =
        //    new Dictionary<EnsembleStrategy, EnsembleStrategyHandler>
        //    {
        //        [EnsembleStrategy.Reasoning] = BuildReasoningEnsemble,
        //        [EnsembleStrategy.Vision] = BuildVisionEnsemble,
        //        [EnsembleStrategy.CostOptimized] = BuildCostOptimisedEnsemble,
        //    };

        /// <summary>
        /// An internal registry storing custom ensemble strategy handlers.
        /// </summary>
        private static readonly Dictionary<EnsembleStrategy, EnsembleStrategyHandlerNEW> CustomHandlerStrategies = [];

        /// <summary>
        /// Registers or replaces a strategy handler for the given ensemble strategy.
        /// Allows unit test code to register fake handlers.
        /// </summary>
        /// <param name="strategy">The strategy key.</param>
        /// <param name="handler">The handler delegate to register.</param>
        public static void RegisterCustomHandler(EnsembleStrategy strategy, EnsembleStrategyHandlerNEW handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            // Register handler for strategy in custom registry ..
            CustomHandlerStrategies[strategy] = handler;
        }

        ///// <summary>
        ///// Retrieves the handler for a given strategy.
        ///// </summary>
        ///// <param name="strategy">The strategy to retrieve.</param>
        ///// <returns cref="EnsembleStrategyHandler">The registered handler.</returns>
        ///// <exception cref="KeyNotFoundException">Thrown if no handler is registered for the strategy.</exception>
        //public static EnsembleStrategyHandler Get(EnsembleStrategy strategy)
        //{
        //    // 1. Override and use Custom strategy handler if found for strategy ..
        //    if (CustomHandlerStrategies.TryGetValue(strategy, out EnsembleStrategyHandler? customHandler))
        //    {
        //        return customHandler;
        //    }

        //    // 2. Otherwise use Default strategy handler if found for strategy ..
        //    if (DefaultHandlerStrategies.TryGetValue(strategy, out EnsembleStrategyHandler? defaultHandler))
        //    {
        //        return defaultHandler;
        //    }

        //    throw new KeyNotFoundException($"No ensemble strategy handler registered for: {strategy}");
        //}


        // --------------------------------------------------------------------
        // NEW enum-free, AiModelPropertyRegistryModel-based strategies
        // --------------------------------------------------------------------

        /// <summary>
        /// Gets the NEW ensemble handler for the specified strategy.
        /// </summary>
        /// <param name="strategy">The ensemble strategy.</param>
        /// <returns>
        /// An <see cref="EnsembleStrategyHandlerNEW"/> that can be invoked with a collection of
        /// <see cref="AiModelPropertyRegistryModel"/> instances to produce an
        /// <see cref="EnsembleDispatchResultNEW"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when the specified <paramref name="strategy"/> is not recognized.
        /// </exception>
        public static EnsembleStrategyHandlerNEW GetNew(EnsembleStrategy strategy)
        {
            return strategy switch
            {
                /// NOTE THE SCORE IN THE NEW STRATEGIES SHOULD BE ACCORDINGLY ASSIGNED - > 0 is not good enough!!!!!
                EnsembleStrategy.ReasoningNEW => ReasoningEnsembleNew,
                EnsembleStrategy.VisionNEW => VisionEnsembleNew,      // to be ported later
                EnsembleStrategy.CostOptimizedNEW => CostOptimisedEnsembleNew, //
                EnsembleStrategy.NoneNEW => NoEnsemble,
                EnsembleStrategy.TwoModelNEW => TwoModelStrategy,
                EnsembleStrategy.ThreeModelNEW => ThreeModelStrategy,
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

        ///// <summary>
        ///// Creates an ensemble of models optimized for reasoning tasks by selecting high-performance reasoning,
        ///// fast chat, and cost-effective critic model descriptors from the registry.
        ///// </summary>
        ///// <param name="registry"></param>
        ///// <returns cref="EnsembleDispatchResult"> with selected reasoning, chat, and critic model descriptors.</returns>
        //private static EnsembleDispatchResult BuildReasoningEnsemble(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> registry)
        //{
        //    // Select the highest performing reasoning model ..
        //    AiModelDescriptor? reasoning = registry.Values
        //        .Where(m => m.Capabilities.Contains(AiModelCapability.Reasoning))
        //        .OrderByDescending(m => m.Capabilities.Contains(AiModelCapability.HighPerformance))
        //        .FirstOrDefault() ?? throw new InvalidOperationException("No Reasoning-capable model found in the registry.");

        //    // .. a fast chat model that is also not the selected reasoning model ..
        //    AiModelDescriptor? fast = registry.Values
        //        .Where(m => m.Capabilities.Contains(AiModelCapability.Chat))
        //        .Where(m => m.Name != reasoning.Name)
        //        .OrderBy(m => m.Pricing.InputTokenCost)
        //        .FirstOrDefault() ?? throw new InvalidOperationException("No Chat-capable model found in the registry.");

        //    // .. and a cost-effective critic model that is neither the reasoning nor the fast model ..
        //    AiModelDescriptor? critic = registry.Values
        //        .Where(m => m.Capabilities.Contains(AiModelCapability.Critic))
        //        .Where(m => m.Name != fast.Name && m.Name != reasoning.Name)
        //        .OrderBy(m => m.Pricing.InputTokenCost)
        //        .FirstOrDefault() ?? throw new InvalidOperationException("No Critic-capable model found in the registry.");

        //    return new EnsembleDispatchResult(models:
        //     [
        //        reasoning,
        //        fast,
        //        critic,
        //    ]);
        //}


        private static EnsembleDispatchResultNEW ReasoningEnsembleNew(
            IEnumerable<AiModelPropertyRegistryModel> availableModels)
        {
            // 1. Reasoning model:
            // - Has reasoning capability
            // - Among those, pick highest performance
            AiModelPropertyRegistryModel? reasoning =
                availableModels
                    .Where(model => model.Capabilities.Core.Reasoning > 0)
                    .OrderByDescending(model => model.Capabilities.Performance.HighPerformance)
                    .FirstOrDefault();

            if (reasoning is null)
            {
                throw new InvalidOperationException("No reasoning-capable model found in the registry.");
            }

            // 2. Fast chat model:
            //    - Has chat capability
            //    - Not the reasoning model
            //    - Lowest input token cost
            AiModelPropertyRegistryModel? fast =
                availableModels
                    .Where(model => model.Capabilities.Core.Chat > 0)
                    .Where(model => !string.Equals(model.Name, reasoning.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault();

            if (fast is null)
            {
                throw new InvalidOperationException("No chat-capable model found in the registry.");
            }

            // 3. Critic model:
            //    - Has critic capability
            //    - Name distinct from reasoning and fast models
            //    - Lowest input token cost
            AiModelPropertyRegistryModel? critic =
                availableModels
                    .Where(model => model.Capabilities.Advanced.Critic > 0)
                    .Where(model => !string.Equals(model.Name, reasoning.Name, StringComparison.Ordinal)
                                 && !string.Equals(model.Name, fast.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault();

            if (critic is null)
            {
                throw new InvalidOperationException("No critic-capable model found in the registry.");
            }

            AiModelPropertyRegistryModel[] selected =
            [
                reasoning,
                fast,
                critic,
            ];

            return new EnsembleDispatchResultNEW(selected);
        }




        ///// <summary>
        ///// Creates an ensemble of models containing the highest performing vision model descriptor and the most cost-effective chat model descriptor.
        ///// model from the registry.
        ///// </summary>
        ///// <param name="modelRegistry"></param>
        ///// <returns cref="EnsembleDispatchResult"> with selected vision and chat model descriptors.</returns>
        //private static EnsembleDispatchResult BuildVisionEnsemble(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry)
        //{
        //    // Select the highest performing vision model ..
        //    AiModelDescriptor? vision = modelRegistry.Values
        //        .Where(m => m.Capabilities.Contains(AiModelCapability.Vision))
        //        .OrderByDescending(m => m.Capabilities.Contains(AiModelCapability.HighPerformance))
        //        .FirstOrDefault() ?? throw new InvalidOperationException("No Vision-capable model found in the registry.");

        //    // .. and the most cost-effective chat model that is not the selected vision model ..
        //    AiModelDescriptor? fastText = modelRegistry.Values
        //        .Where(m => m.Capabilities.Contains(AiModelCapability.Chat))
        //        .Where(m => m.Name != vision.Name)
        //        .OrderBy(m => m.Pricing.InputTokenCost)
        //        .FirstOrDefault() ?? throw new InvalidOperationException("No Chat-capable model found in the registry.");

        //    return new EnsembleDispatchResult(models:
        //    [
        //        vision,
        //        fastText,
        //    ]);
        //}


        /// <summary>
        /// Creates an ensemble of models optimized for vision tasks by selecting
        /// a high-performing vision model and a cost-effective chat model.
        /// </summary>
        /// <param name="availableModels">The available models to select from.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchResultNEW"/> containing the selected models.
        /// </returns>
        private static EnsembleDispatchResultNEW VisionEnsembleNew(
            IEnumerable<AiModelPropertyRegistryModel> availableModels)
        {
            // 1. Vision model:
            //    - Has strong vision capability
            //    - Among those, pick highest performance
            AiModelPropertyRegistryModel? vision =
                availableModels
                    .Where(model => model.Capabilities.Core.Vision > 0)
                    .OrderByDescending(model => model.Capabilities.Performance.HighPerformance)
                    .FirstOrDefault();

            if (vision is null)
            {
                throw new InvalidOperationException("No vision-capable model found in the registry.");
            }

            // 2. Fast chat model:
            //    - Has chat capability
            //    - Not the vision model
            //    - Lowest input token cost
            AiModelPropertyRegistryModel? fastText =
                availableModels
                    .Where(model => model.Capabilities.Core.Chat > 0)
                    .Where(model => !string.Equals(model.Name, vision.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault();

            if (fastText is null)
            {
                throw new InvalidOperationException("No chat-capable model found in the registry.");
            }

            AiModelPropertyRegistryModel[] selected =
            [
                vision,
                fastText,
            ];

            return new EnsembleDispatchResultNEW(selected);
        }



        ///// <summary>
        ///// Creates an ensemble of models optimized for cost by selecting low-cost, chat-capable, and high-performance model descriptors from the registry.
        ///// </summary>
        ///// <returns cref="EnsembleDispatchResult"> with selected cost-optimised, chat-capable, and high-performance model descriptors.</returns>
        //private static EnsembleDispatchResult BuildCostOptimisedEnsemble(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry)
        //{
        //    // Select the most cost-effective low-cost model ..
        //    AiModelDescriptor? low = modelRegistry.Values
        //        .Where(m => m.Capabilities.Contains(AiModelCapability.LowCost))
        //        .OrderBy(m => m.Pricing.InputTokenCost)
        //        .FirstOrDefault() ?? throw new InvalidOperationException("No LowCost-capable model found in the registry.");

        //    // .. a mid-tier chat model that is not the selected low-cost model ..
        //    AiModelDescriptor? mid = modelRegistry.Values
        //        .Where(m => m.Capabilities.Contains(AiModelCapability.Chat))
        //        .Where(m => m.Name != low.Name)
        //        .OrderBy(m => m.Pricing.InputTokenCost)
        //        .FirstOrDefault() ?? throw new InvalidOperationException("No Chat-capable model found in the registry.");

        //    // .. and a high-performance model that is neither the low-cost nor the mid-tier chat model ..
        //    AiModelDescriptor? high = modelRegistry.Values
        //        .Where(m => m.Capabilities.Contains(AiModelCapability.HighPerformance))
        //        .Where(m => m.Name != low.Name && m.Name != mid.Name)
        //        .OrderBy(m => m.Pricing.InputTokenCost)
        //        .FirstOrDefault() ?? throw new InvalidOperationException("No HighPerformance-capable model found in the registry.");

        //    return new EnsembleDispatchResult(models:
        //    [
        //        low,
        //        mid,
        //        high
        //    ]);
        //}


        /// <summary>
        /// Creates an ensemble of models optimized for cost by selecting a low-cost model,
        /// a mid-tier chat model, and a high-performance model.
        /// </summary>
        /// <param name="availableModels">The available models to select from.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchResultNEW"/> containing the selected models.
        /// </returns>
        private static EnsembleDispatchResultNEW CostOptimisedEnsembleNew(
            IEnumerable<AiModelPropertyRegistryModel> availableModels)
        {
            // 1. Low-cost model:
            //    - Explicitly marked low-cost or inferred via cost-efficiency
            //    - Lowest input token cost
            AiModelPropertyRegistryModel? low =
                availableModels
                    .Where(model => model.Capabilities.Operational.LowCost > 0)
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault();

            if (low is null)
            {
                throw new InvalidOperationException("No low-cost-capable model found in the registry.");
            }

            // 2. Mid-tier chat model:
            //    - Has chat capability
            //    - Not the low-cost model
            //    - Lowest input token cost
            AiModelPropertyRegistryModel? mid =
                availableModels
                    .Where(model => model.Capabilities.Core.Chat > 0)
                    .Where(model => !string.Equals(model.Name, low.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault();

            if (mid is null)
            {
                throw new InvalidOperationException("No chat-capable model found in the registry.");
            }

            // 3. High-performance model:
            //    - Has high performance capability
            //    - Distinct from low and mid models
            //    - Lowest input token cost among candidates
            AiModelPropertyRegistryModel? high =
                availableModels
                    .Where(model => model.Capabilities.Performance.HighPerformance > 0)
                    .Where(model =>
                        !string.Equals(model.Name, low.Name, StringComparison.Ordinal)
                        && !string.Equals(model.Name, mid.Name, StringComparison.Ordinal))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .FirstOrDefault();

            if (high is null)
            {
                throw new InvalidOperationException("No high-performance-capable model found in the registry.");
            }

            AiModelPropertyRegistryModel[] selected =
            [
                low,
                mid,
                high,
            ];

            return new EnsembleDispatchResultNEW(selected);
        }




        // Example strategy implementations (you’ll replace with your real logic)
        private static EnsembleDispatchResultNEW NoEnsemble(IEnumerable<AiModelPropertyRegistryModel> availableModels)
        {
            // For example, pick a single “default” chat model
            AiModelPropertyRegistryModel? model =
                availableModels.FirstOrDefault(m => m.Tags.Contains("default-chat"))
                ?? availableModels.FirstOrDefault();

            AiModelPropertyRegistryModel[] selected =
                model is null ? Array.Empty<AiModelPropertyRegistryModel>() : new[] { model };

            return new EnsembleDispatchResultNEW(selected);
        }

        private static EnsembleDispatchResultNEW TwoModelStrategy(IEnumerable<AiModelPropertyRegistryModel> availableModels)
        {
            AiModelPropertyRegistryModel[] selected =
                availableModels
                    .Where(m => m.Tags.Contains("chat"))
                    .Take(2)
                    .ToArray();

            return new EnsembleDispatchResultNEW(selected);
        }

        private static EnsembleDispatchResultNEW ThreeModelStrategy(IEnumerable<AiModelPropertyRegistryModel> availableModels)
        {
            AiModelPropertyRegistryModel[] selected =
                availableModels
                    .Where(m => m.Tags.Contains("chat"))
                    .Take(3)
                    .ToArray();

            return new EnsembleDispatchResultNEW(selected);
        }
    }
}