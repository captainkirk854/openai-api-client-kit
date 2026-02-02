// <copyright file="SingleModelStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Dispatch
{
    using System.Linq;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// Registry for single‑model dispatching strategies.
    /// Maps a <see cref="SingleModelStrategy"/> to its corresponding handler delegate.
    /// </summary>
    public static class SingleModelStrategies
    {
        /// <summary>
        /// Dictionary mapping single-model dispatch strategies to their specific delegate handler implementations.
        /// </summary>
        public static readonly IReadOnlyDictionary<SingleModelStrategy, SingleModelStrategyHandler> DefaultHandlerStrategies =
            new Dictionary<SingleModelStrategy, SingleModelStrategyHandler>
            {
                [SingleModelStrategy.Explicit] = GetExplicitModel,
                [SingleModelStrategy.LowestCost] = GetLowestCostModel,
                [SingleModelStrategy.HighestPerformance] = GetHighestPerformanceModel,
                [SingleModelStrategy.BestReasoning] = GetBestReasoningModel,
                [SingleModelStrategy.BestVision] = GetBestVisionModel,
                [SingleModelStrategy.BestAudioIn] = GetLowestCostAudioInModel,
                [SingleModelStrategy.BestAudioOut] = GetLowestCostAudioOutModel,
                [SingleModelStrategy.Embedding] = GetLowestCostEmbeddingModel,
                [SingleModelStrategy.Moderation] = GetLowestCostModerationCapableModel,
            };

        /// <summary>
        /// An internal registry storing custom single model strategy handlers.
        /// </summary>
        private static readonly Dictionary<SingleModelStrategy, SingleModelStrategyHandler> CustomHandlerStrategies = [];

        /// <summary>
        /// Registers or replaces a strategy handler for the given single‑model strategy.
        /// Allows unit test code to register fake handlers.
        /// </summary>
        /// <param name="strategy">The strategy key.</param>
        /// <param name="handler">The handler delegate to register.</param>
        public static void RegisterCustomHandler(SingleModelStrategy strategy, SingleModelStrategyHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            // Register handler for strategy in custom registry ..
            CustomHandlerStrategies[strategy] = handler;
        }

        /// <summary>
        /// Retrieves the handler for a given strategy.
        /// </summary>
        /// <param name="strategy">The strategy to retrieve.</param>
        /// <returns cref="SingleModelStrategyHandler">The registered handler.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no handler is registered for the strategy.</exception>
        public static SingleModelStrategyHandler Get(SingleModelStrategy strategy)
        {
            // 1. Override and use Custom strategy handler if found for strategy ..
            if (CustomHandlerStrategies.TryGetValue(strategy, out SingleModelStrategyHandler? customHandler))
            {
                return customHandler;
            }

            // 2. Otherwise use Default strategy handler if found for strategy ..
            if (DefaultHandlerStrategies.TryGetValue(strategy, out SingleModelStrategyHandler? defaultHandler))
            {
                return defaultHandler;
            }

            throw new KeyNotFoundException($"No single model strategy handler registered for: {strategy}");
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
        /// Dispatches the explicitly specified model from the request.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The dispatch request containing the explicit model descriptor.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the request does not specify an explicit model.</exception>
        private static SingleModelDispatchResult GetExplicitModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            if (request.ExplicitModel is null)
            {
                throw new InvalidOperationException("Explicit routing requires an explicit model.");
            }

            return new SingleModelDispatchResult(model: modelRegistry[request.ExplicitModel.Value]);
        }

        /// <summary>
        /// Dispatches the model with the lowest cost from the filtered candidates based on the request that meet the required capabilities.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The routing request specifying required capabilities.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult GetLowestCostModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            IEnumerable<ModelDescriptor> candidates = FilterByCapabilities(modelRegistry, request.RequiredCapabilities);

            ModelDescriptor descriptor = candidates
                .OrderBy(c => c.Pricing.InputTokenCost)
                .ThenBy(c => c.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("Unable to find the Lowest Cost model that matches required capability(s) in the registry.");

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Dispatches the model with the highest performance from the filtered candidates based on the request that meet the required capabilities.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult GetHighestPerformanceModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelCapability primaryCapability = ModelCapability.HighPerformance;
            IEnumerable<ModelDescriptor> candidates = FilterByCapabilities(registry: modelRegistry, required: request.RequiredCapabilities);

            ModelDescriptor descriptor = candidates
                .Where(c => c.Capabilities.Contains(primaryCapability))
                .OrderBy(m => m.Pricing.InputTokenCost)
                .ThenBy(m => m.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException($"Unable to find a {primaryCapability} model that matches required capability(s) in the registry.");

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Dispatches the best reasoning-capable model available prioritizing high performance, and then lower input token cost.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult GetBestReasoningModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelCapability primaryCapability = ModelCapability.Reasoning;
            return Plop(registry: modelRegistry, capability: primaryCapability, sortingCapability: ModelCapability.HighPerformance);
        }

        /// <summary>
        /// Dispatches the best vision-capable model available prioritizing high performance, and then lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult GetBestVisionModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelCapability primaryCapability = ModelCapability.Vision;
            return Plop(registry: modelRegistry, capability: primaryCapability, sortingCapability: ModelCapability.HighPerformance);
        }

        /// <summary>
        /// Dispatches the audio input-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult GetLowestCostAudioInModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelCapability primaryCapability = ModelCapability.AudioIn;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches the audio output-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult GetLowestCostAudioOutModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelCapability primaryCapability = ModelCapability.AudioOut;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches the embedding-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult GetLowestCostEmbeddingModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelCapability primaryCapability = ModelCapability.Embedding;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches the moderation-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult GetLowestCostModerationCapableModel(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelCapability primaryCapability = ModelCapability.Moderation;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Helper method to dispatch the model with the 'Primary Lowest Overall Price (PLOP)' for the specified capability.
        /// </summary>
        /// <param name="registry">Model registry to filter from.</param>
        /// <param name="capability">Primary capability to filter model descriptor(s) by.</param>
        /// <param name="sortingCapability">Optional secondary capability to prioritize in sorting.</param>
        /// <returns>A <see cref="SingleModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleModelDispatchResult Plop(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> registry, ModelCapability capability, ModelCapability? sortingCapability = null)
        {
            // If no sorting capability is provided, use the primary capability for sorting ..
            sortingCapability ??= capability;

            ModelDescriptor descriptor = registry.Values
                .Where(m => m.Capabilities.Contains(capability))
                .OrderByDescending(m => m.Capabilities.Contains((ModelCapability)sortingCapability))
                .ThenBy(m => m.Pricing.InputTokenCost)
                .ThenBy(m => m.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException($"Unable to find a {capability} model in the registry.");

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Helper method to filter the model descriptors in the registry based on the required capabilities.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="required"></param>
        /// <returns>List of ModelDescriptor(s) with required capabilities.</returns>
        private static IEnumerable<ModelDescriptor> FilterByCapabilities(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> registry, IReadOnlyCollection<ModelCapability>? required)
        {
            if (required is null || required.Count == 0)
            {
                return registry.Values;
            }

            // Filter model descriptor(s) in registry that contain all required capabilities ..
            return registry.Values.Where(md => required.All(req => md.Capabilities.Contains(req)));
        }
    }
}
