// <copyright file="SingleAiModelStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Dispatch
{
    using System.Linq;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
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
        private static readonly Dictionary<SingleAiModelStrategy, SingleAiModelStrategyHandler> CustomHandlerStrategies = new Dictionary<SingleAiModelStrategy, SingleAiModelStrategyHandler>();

        /// <summary>
        /// Registers or replaces a strategy handler for the given single-model strategy.
        /// </summary>
        /// <param name="strategy">The strategy key.</param>
        /// <param name="handler">The handler delegate to register.</param>
        public static void RegisterCustomHandler(
            SingleAiModelStrategy strategy,
            SingleAiModelStrategyHandler handler)
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
        public static SingleAiModelStrategyHandler Get(SingleAiModelStrategy strategy)
        {
            if (CustomHandlerStrategies.TryGetValue(strategy, out SingleAiModelStrategyHandler customHandler))
            {
                return customHandler;
            }

            if (DefaultHandlerStrategies.TryGetValue(strategy, out SingleAiModelStrategyHandler defaultHandler))
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
        private static SingleAiModelDispatchResult GetExplicitModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            if (request.ExplicitModel is null)
            {
                throw new InvalidOperationException("Explicit dispatch requires an explicit model.");
            }

            AiModelPropertyRegistryModel model = modelRegistry[request.ExplicitModel];

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
        private static SingleAiModelDispatchResult GetLowestCostModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            IEnumerable<AiModelPropertyRegistryModel> candidates = FilterByCapabilities(
                modelRegistry,
                request.RequiredCapabilities);

            AiModelPropertyRegistryModel descriptor = candidates
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
        private static SingleAiModelDispatchResult GetHighestPerformanceModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.HighPerformance;

            IEnumerable<AiModelPropertyRegistryModel> candidates = FilterByCapabilities(
                modelRegistry,
                request.RequiredCapabilities);

            AiModelPropertyRegistryModel descriptor = candidates
                .Where(candidate => SupportsCapability(candidate, primaryCapability))
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
        private static SingleAiModelDispatchResult GetBestReasoningModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Reasoning;

            return Plop(
                modelRegistry,
                primaryCapability,
                AiModelCapability.HighPerformance);
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
        private static SingleAiModelDispatchResult GetBestVisionModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Vision;

            return Plop(
                modelRegistry,
                primaryCapability,
                AiModelCapability.HighPerformance);
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
        private static SingleAiModelDispatchResult GetLowestCostAudioInModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.AudioIn;

            return Plop(
                modelRegistry,
                primaryCapability);
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
        private static SingleAiModelDispatchResult GetLowestCostAudioOutModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.AudioOut;

            return Plop(
                modelRegistry,
                primaryCapability);
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
        private static SingleAiModelDispatchResult GetLowestCostEmbeddingModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Embedding;

            return Plop(
                modelRegistry,
                primaryCapability);
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
        private static SingleAiModelDispatchResult GetLowestCostModerationCapableModel(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> modelRegistry,
            SingleAiModelDispatchRequest request)
        {
            AiModelCapability primaryCapability = AiModelCapability.Moderation;

            return Plop(
                modelRegistry,
                primaryCapability);
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
        private static SingleAiModelDispatchResult Plop(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> registry,
            AiModelCapability capability,
            AiModelCapability? sortingCapability = null)
        {
            sortingCapability ??= capability;

            AiModelPropertyRegistryModel descriptor = registry.Values
                .Where(model => SupportsCapability(model, capability))
                .OrderByDescending(model => sortingCapability.HasValue && SupportsCapability(model, sortingCapability.Value))
                .ThenBy(model => model.Pricing.InputTokenCost)
                .ThenBy(model => model.Pricing.OutputTokenCost)
                .FirstOrDefault() ?? throw new InvalidOperationException($"Unable to find a {capability} model in the registry.");

            return new SingleAiModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Helper method to filter the models in the registry based on the required capabilities.
        /// </summary>
        /// <param name="registry">The model registry keyed by upper-cased model name.</param>
        /// <param name="required">The required capabilities that models must support.</param>
        /// <returns>
        /// A sequence of <see cref="AiModelPropertyRegistryModel"/> instances that have all required capabilities.
        /// </returns>
        private static IEnumerable<AiModelPropertyRegistryModel> FilterByCapabilities(
            IReadOnlyDictionary<string, AiModelPropertyRegistryModel> registry,
            IReadOnlyCollection<AiModelCapability>? required)
        {
            if (required is null || required.Count == 0)
            {
                return registry.Values;
            }

            return registry.Values.Where(model => required.All(capability => SupportsCapability(model, capability)));
        }

        /// <summary>
        /// Determines whether a model supports a given capability using its leaf capability properties.
        /// </summary>
        /// <param name="model">The model to evaluate.</param>
        /// <param name="capability">The capability to check.</param>
        /// <returns>
        /// <see langword="true"/> if the model supports the specified capability; otherwise, <see langword="false"/>.
        /// </returns>
        private static bool SupportsCapability(
            AiModelPropertyRegistryModel model,
            AiModelCapability capability)
        {
            switch (capability)
            {
                case AiModelCapability.Chat:
                    {
                        return model.Capabilities.Core.Chat > 0;
                    }

                case AiModelCapability.Reasoning:
                    {
                        return model.Capabilities.Core.Reasoning > 0;
                    }

                case AiModelCapability.Embedding:
                    {
                        return model.Capabilities.Advanced.Embedding > 0;
                    }

                case AiModelCapability.Vision:
                    {
                        return model.Capabilities.Core.Vision > 0;
                    }

                case AiModelCapability.AudioIn:
                    {
                        return model.Capabilities.Core.AudioIn > 0;
                    }

                case AiModelCapability.AudioOut:
                    {
                        return model.Capabilities.Core.AudioOut > 0;
                    }

                case AiModelCapability.Moderation:
                    {
                        return model.Capabilities.Operational.Moderation > 0;
                    }

                case AiModelCapability.HighPerformance:
                    {
                        return model.Capabilities.Performance.HighPerformance > 0;
                    }

                case AiModelCapability.LowCost:
                    {
                        return model.Capabilities.Operational.LowCost > 0;
                    }

                default:
                    {
                        return false;
                    }
            }
        }
    }
}
