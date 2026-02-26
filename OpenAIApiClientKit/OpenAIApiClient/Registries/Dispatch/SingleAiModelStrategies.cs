// <copyright file="SingleAiModelStrategies.cs" company="854 Things (tm)">
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
    /// Maps a <see cref="SingleAiModelStrategy"/> to its corresponding handler delegate.
    /// </summary>
    public static class SingleAiModelStrategies
    {
        /// <summary>
        /// Dictionary mapping single-model dispatch strategies to their specific delegate handler implementations.
        /// </summary>
        public static readonly IReadOnlyDictionary<SingleAiModelStrategy, SingleAiModelStrategyHandler> DefaultHandlerStrategies =
            new Dictionary<SingleAiModelStrategy, SingleAiModelStrategyHandler>
            {
                [SingleAiModelStrategy.Explicit] = GetExplicitModel,
                [SingleAiModelStrategy.LowestCost] = GetLowestCostModel,
                [SingleAiModelStrategy.HighestPerformance] = GetHighestPerformanceModel,
                [SingleAiModelStrategy.BestReasoning] = GetBestReasoningModel,
                [SingleAiModelStrategy.BestVision] = GetBestVisionModel,
                [SingleAiModelStrategy.BestAudioIn] = GetLowestCostAudioInModel,
                [SingleAiModelStrategy.BestAudioOut] = GetLowestCostAudioOutModel,
                [SingleAiModelStrategy.Embedding] = GetLowestCostEmbeddingModel,
                [SingleAiModelStrategy.Moderation] = GetLowestCostModerationCapableModel,
            };

        /// <summary>
        /// An internal registry storing custom single model strategy handlers.
        /// </summary>
        private static readonly Dictionary<SingleAiModelStrategy, SingleAiModelStrategyHandler> CustomHandlerStrategies = [];

        /// <summary>
        /// Registers or replaces a strategy handler for the given single‑model strategy.
        /// Allows unit test code to register fake handlers.
        /// </summary>
        /// <param name="strategy">The strategy key.</param>
        /// <param name="handler">The handler delegate to register.</param>
        public static void RegisterCustomHandler(SingleAiModelStrategy strategy, SingleAiModelStrategyHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            // Register handler for strategy in custom registry ..
            CustomHandlerStrategies[strategy] = handler;
        }

        /// <summary>
        /// Retrieves the handler for a given strategy.
        /// </summary>
        /// <param name="strategy">The strategy to retrieve.</param>
        /// <returns cref="SingleAiModelStrategyHandler">The registered handler.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if no handler is registered for the strategy.</exception>
        public static SingleAiModelStrategyHandler Get(SingleAiModelStrategy strategy)
        {
            // 1. Override and use Custom strategy handler if found for strategy ..
            if (CustomHandlerStrategies.TryGetValue(strategy, out SingleAiModelStrategyHandler? customHandler))
            {
                return customHandler;
            }

            // 2. Otherwise use Default strategy handler if found for strategy ..
            if (DefaultHandlerStrategies.TryGetValue(strategy, out SingleAiModelStrategyHandler? defaultHandler))
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
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the request does not specify an explicit model.</exception>
        private static SingleAiModelDispatchResult GetExplicitModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            if (request.ExplicitModel is null)
            {
                throw new InvalidOperationException("Explicit routing requires an explicit model.");
            }

            return new SingleAiModelDispatchResult(model: modelRegistry[request.ExplicitModel.Value]);
        }

        /// <summary>
        /// Dispatches the model with the lowest cost from the filtered candidates based on the request that meet the required capabilities.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The routing request specifying required capabilities.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult GetLowestCostModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            IEnumerable<AiModelDescriptor> candidates = FilterByCapabilities(modelRegistry, request.RequiredCapabilities);

            AiModelDescriptor descriptor = candidates
                .OrderBy(c => c.Pricing.InputTokenCost)
                .ThenBy(c => c.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("Unable to find the Lowest Cost model that matches required capability(s) in the registry.");

            return new SingleAiModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Dispatches the model with the highest performance from the filtered candidates based on the request that meet the required capabilities.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult GetHighestPerformanceModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.HighPerformance;
            IEnumerable<AiModelDescriptor> candidates = FilterByCapabilities(registry: modelRegistry, required: request.RequiredCapabilities);

            AiModelDescriptor descriptor = candidates
                .Where(c => c.Capabilities.Contains(primaryCapability))
                .OrderBy(m => m.Pricing.InputTokenCost)
                .ThenBy(m => m.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException($"Unable to find a {primaryCapability} model that matches required capability(s) in the registry.");

            return new SingleAiModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Dispatches the best reasoning-capable model available prioritizing high performance, and then lower input token cost.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult GetBestReasoningModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Reasoning;
            return Plop(registry: modelRegistry, capability: primaryCapability, sortingCapability: AiModelCapability.HighPerformance);
        }

        /// <summary>
        /// Dispatches the best vision-capable model available prioritizing high performance, and then lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult GetBestVisionModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Vision;
            return Plop(registry: modelRegistry, capability: primaryCapability, sortingCapability: AiModelCapability.HighPerformance);
        }

        /// <summary>
        /// Dispatches the audio input-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult GetLowestCostAudioInModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.AudioIn;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches the audio output-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult GetLowestCostAudioOutModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.AudioOut;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches the embedding-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult GetLowestCostEmbeddingModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Embedding;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches the moderation-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">Model registry to filter from.</param>
        /// <param name="request">Dummy model router request required for delegate pattern.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult GetLowestCostModerationCapableModel(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Moderation;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Helper method to dispatch the model with the 'Primary Lowest Overall Price (PLOP)' for the specified capability.
        /// </summary>
        /// <param name="registry">Model registry to filter from.</param>
        /// <param name="capability">Primary capability to filter model descriptor(s) by.</param>
        /// <param name="sortingCapability">Optional secondary capability to prioritize in sorting.</param>
        /// <returns>A <see cref="SingleAiModelDispatchResult"/> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if model with required capabilities not found in registry.</exception>
        private static SingleAiModelDispatchResult Plop(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> registry, AiModelCapability capability, AiModelCapability? sortingCapability = null)
        {
            // If no sorting capability is provided, use the primary capability for sorting ..
            sortingCapability ??= capability;

            AiModelDescriptor descriptor = registry.Values
                .Where(m => m.Capabilities.Contains(capability))
                .OrderByDescending(m => m.Capabilities.Contains((AiModelCapability)sortingCapability))
                .ThenBy(m => m.Pricing.InputTokenCost)
                .ThenBy(m => m.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException($"Unable to find a {capability} model in the registry.");

            return new SingleAiModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Helper method to filter the model descriptors in the registry based on the required capabilities.
        /// </summary>
        /// <param name="registry"></param>
        /// <param name="required"></param>
        /// <returns>List of ModelDescriptor(s) with required capabilities.</returns>
        private static IEnumerable<AiModelDescriptor> FilterByCapabilities(IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> registry, IReadOnlyCollection<AiModelCapability>? required)
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
