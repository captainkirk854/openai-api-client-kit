// <copyright file="SingleAiModelStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Dispatch
{
    using System.Linq;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.Extensions;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;

    /// <summary>
    /// Provides strategy-based selection of single AI models from the model registry.
    /// </summary>
    public static class SingleAiModelStrategies
    {
        /// <summary>
        /// Dictionary mapping single-model dispatch strategies to their specific delegate handler implementations.
        /// </summary>
        public static readonly IReadOnlyDictionary<AiModelStrategy.SingleAiModel, SingleAiModelStrategyHandler> DefaultHandlerStrategies =
            new Dictionary<AiModelStrategy.SingleAiModel, SingleAiModelStrategyHandler>
            {
                [AiModelStrategy.SingleAiModel.Explicit] = GetExplicitModel,
                [AiModelStrategy.SingleAiModel.LowestCost] = GetLowestCostModel,
                [AiModelStrategy.SingleAiModel.HighestPerformance] = GetHighestPerformanceModel,
                [AiModelStrategy.SingleAiModel.BestReasoning] = GetBestReasoningModel,
                [AiModelStrategy.SingleAiModel.BestVision] = GetBestVisionModel,
                [AiModelStrategy.SingleAiModel.BestAudioIn] = GetLowestCostAudioInModel,
                [AiModelStrategy.SingleAiModel.BestAudioOut] = GetLowestCostAudioOutModel,
                [AiModelStrategy.SingleAiModel.Embedding] = GetLowestCostEmbeddingModel,
                [AiModelStrategy.SingleAiModel.Moderation] = GetLowestCostModerationCapableModel,
            };

        /// <summary>
        /// An internal registry storing custom single model strategy handlers.
        /// </summary>
        private static readonly Dictionary<AiModelStrategy.SingleAiModel, SingleAiModelStrategyHandler> CustomHandlerStrategies = [];

        /// <summary>
        /// Registers or replaces a strategy handler for the given single-model strategy.
        /// </summary>
        /// <param name="strategy">The strategy key.</param>
        /// <param name="handler">The handler delegate to register.</param>
        public static void RegisterCustomHandler(AiModelStrategy.SingleAiModel strategy, SingleAiModelStrategyHandler handler)
        {
            ArgumentNullException.ThrowIfNull(handler);

            CustomHandlerStrategies[strategy] = handler;
        }

        /// <summary>
        /// Retrieves the handler for a given strategy.
        /// </summary>
        /// <param name="strategy">The strategy to retrieve.</param>
        /// <returns>
        /// A <see cref="SingleAiModelStrategyHandler"/> representing the registered handler for the specified strategy.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Thrown if no handler is registered for the specified <paramref name="strategy"/>.
        /// </exception>
        public static SingleAiModelStrategyHandler? Get(AiModelStrategy.SingleAiModel strategy)
        {
            if (CustomHandlerStrategies.TryGetValue(strategy, out SingleAiModelStrategyHandler? customHandler))
            {
                return customHandler;
            }

            if (DefaultHandlerStrategies.TryGetValue(strategy, out SingleAiModelStrategyHandler? defaultHandler))
            {
                return defaultHandler;
            }

            throw new KeyNotFoundException($"No single model strategy handler registered for: {strategy}");
        }

        /// <summary>
        /// Clears all custom strategy overrides.
        /// Useful for unit tests to ensure isolation between test cases.
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
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The dispatch request containing the explicit model identifier.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the request does not specify an explicit model.
        /// </exception>
        private static SingleAiModelDispatchResult GetExplicitModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            if (request.ExplicitModel is null)
            {
                throw new InvalidOperationException("Explicit dispatch requires an explicit model.");
            }

            AiModelDescriptor model = modelRegistry[request.ExplicitModel];

            return new SingleAiModelDispatchResult(model);
        }

        /// <summary>
        /// Dispatches the model with the lowest cost from the filtered candidates that meet the required capabilities.
        /// </summary>
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The routing request specifying required capabilities.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a model with the required capabilities cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult GetLowestCostModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            IEnumerable<AiModelDescriptor> candidates = FilterByCapabilities(registry: modelRegistry, required: request.RequiredCapabilities);

            AiModelDescriptor descriptor = candidates
                .OrderBy(candidate => candidate.Pricing.InputTokenCost)
                .ThenBy(candidate => candidate.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException("Unable to find the Lowest Cost model that matches required capability(s) in the registry.");

            return new SingleAiModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Dispatches the model with the highest performance from the filtered candidates that meet the required capabilities.
        /// </summary>
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a model with the required capabilities cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult GetHighestPerformanceModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.HighPerformance;

            IEnumerable<AiModelDescriptor> candidates = FilterByCapabilities(registry: modelRegistry, required: request.RequiredCapabilities);

            AiModelDescriptor descriptor = candidates
                .Where(candidate => candidate.HasCapability(capability: primaryCapability, minScore: 4, maxScore: 5))
                .OrderBy(model => model.Pricing.InputTokenCost)
                .ThenBy(model => model.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException($"Unable to find a {primaryCapability} model that matches required capability(s) in the registry.");

            return new SingleAiModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Dispatches the best reasoning-capable model available, prioritizing high performance and then lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The model router request; only the strategy is used.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a reasoning-capable model cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult GetBestReasoningModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Reasoning;

            return Plop(registry: modelRegistry, capability: primaryCapability, sortingCapability: AiModelCapability.HighPerformance);
        }

        /// <summary>
        /// Dispatches the best vision-capable model available, prioritizing high performance and then lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The model router request; only the strategy is used.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a vision-capable model cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult GetBestVisionModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Vision;

            return Plop(registry: modelRegistry, capability: primaryCapability, sortingCapability: AiModelCapability.HighPerformance);
        }

        /// <summary>
        /// Dispatches the audio input-capable model available, prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The model router request; only the strategy is used.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if an audio input-capable model cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult GetLowestCostAudioInModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.AudioIn;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches the audio output-capable model available, prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The model router request; only the strategy is used.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if an audio output-capable model cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult GetLowestCostAudioOutModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.AudioOut;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches an embedding-capable model, prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The model router request; only the strategy is used.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if an embedding-capable model cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult GetLowestCostEmbeddingModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Embedding;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Dispatches a moderation-capable model, prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry">The model registry keyed by upper-cased model name.</param>
        /// <param name="request">The model router request; only the strategy is used.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a moderation-capable model cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult GetLowestCostModerationCapableModel(IReadOnlyDictionary<string, AiModelDescriptor> modelRegistry, SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Moderation;
            return Plop(registry: modelRegistry, capability: primaryCapability);
        }

        /// <summary>
        /// Helper method to dispatch the model with the "Primary Lowest Overall Price (PLOP)" for the specified capability.
        /// </summary>
        /// <param name="registry">The model registry to filter from.</param>
        /// <param name="capability">The primary capability to filter models by.</param>
        /// <param name="sortingCapability">An optional secondary capability to prioritize when sorting.</param>
        /// <returns>
        /// A <see cref="SingleAiModelDispatchResult"/> containing the resolved model.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if a model with the required capabilities cannot be found in the registry.
        /// </exception>
        private static SingleAiModelDispatchResult Plop(IReadOnlyDictionary<string, AiModelDescriptor> registry, AiModelCapability capability, AiModelCapability? sortingCapability = null)
        {
            sortingCapability ??= capability;

            AiModelDescriptor descriptor = registry.Values
                .Where(model => model.HasCapability(capability: capability, minScore: 4, maxScore: 5))
                .OrderByDescending(model => sortingCapability.HasValue && model.HasCapability(capability: sortingCapability.Value, minScore: 3, maxScore: 5))
                .ThenBy(model => model.Pricing.InputTokenCost)
                .ThenBy(model => model.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException($"Unable to find a {capability} model in the registry.");

            return new SingleAiModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Helper method to filter the models in the registry based on the required capabilities.
        /// </summary>
        /// <param name="registry">The model registry to filter from.</param>
        /// <param name="required">The required capabilities to filter by.</param>
        /// <returns>
        /// A sequence of <see cref="AiModelDescriptor"/> instances that have all required capabilities.
        /// </returns>
        private static IEnumerable<AiModelDescriptor> FilterByCapabilities(IReadOnlyDictionary<string, AiModelDescriptor> registry, IReadOnlyCollection<AiModelCapability>? required)
        {
            if (required is null || required.Count == 0)
            {
                return registry.Values;
            }

            return registry.Values.Where(model => required.All(capability => model.HasCapability(capability: capability, minScore: 3, maxScore: 5)));
        }
    }
}
