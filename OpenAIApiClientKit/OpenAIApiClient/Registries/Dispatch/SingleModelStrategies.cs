// <copyright file="SingleModelStrategies.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Dispatch
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;

    public static class SingleModelStrategies
    {
        /// <summary>
        /// Dictionary mapping routing strategies to their handler implementations.
        /// </summary>
        public static readonly IReadOnlyDictionary<SingleModelStrategy, SingleModelStrategyHandler> Strategies =
            new Dictionary<SingleModelStrategy, SingleModelStrategyHandler>
            {
                [SingleModelStrategy.Explicit] = RouteExplicit,
                [SingleModelStrategy.LowestCost] = RouteLowestCost,
                [SingleModelStrategy.HighestPerformance] = RouteHighestPerformance,
                [SingleModelStrategy.BestReasoning] = RouteBestReasoning,
                [SingleModelStrategy.BestVision] = RouteBestVision,
                [SingleModelStrategy.BestAudioIn] = RouteBestAudioIn,
                [SingleModelStrategy.BestAudioOut] = RouteBestAudioOut,
                [SingleModelStrategy.Embedding] = RouteEmbedding,
                [SingleModelStrategy.Moderation] = RouteModeration,
            };

        /// <summary>
        /// Fetches the routing strategy handler for the specified strategy.
        /// </summary>
        /// <param name="strategy"></param>
        /// <returns>ModelRoutingStrategyHandler.</returns>
        /// <exception cref="KeyNotFoundException">Thrown if the strategy has not been registered.</exception>
        public static SingleModelStrategyHandler Get(SingleModelStrategy strategy)
        {
            if (!Strategies.TryGetValue(strategy, out SingleModelStrategyHandler? handler))
            {
                throw new KeyNotFoundException($"No routing strategy registered for {strategy}");
            }

            return handler;
        }

        // -------------------------
        // Strategy Implementations
        // -------------------------

        /// <summary>
        /// Routes a request using an explicitly specified model.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The routing request containing the explicit model information.</param>
        /// <returns>A ModelRouterResult containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the request does not specify an explicit model.</exception>
        private static SingleModelDispatchResult RouteExplicit(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            if (request.ExplicitModel is null)
            {
                throw new InvalidOperationException("Explicit routing requires an explicit model.");
            }

            return new SingleModelDispatchResult(modelRegistry[request.ExplicitModel.Value]);
        }

        /// <summary>
        /// Selects the model with the lowest input and output token cost that meets the required capabilities.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The routing request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private static SingleModelDispatchResult RouteLowestCost(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            IEnumerable<ModelDescriptor> candidates = FilterByCapabilities(modelRegistry, request.RequiredCapabilities);

            ModelDescriptor descriptor = candidates
                .OrderBy(c => c.Pricing.InputTokenCost)
                .ThenBy(c => c.Pricing.OutputTokenCost)
                .First();

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Selects the model with highest performance capability from the filtered candidates based on the request.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private static SingleModelDispatchResult RouteHighestPerformance(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            IEnumerable<ModelDescriptor> candidates = FilterByCapabilities(modelRegistry, request.RequiredCapabilities);

            ModelDescriptor descriptor = candidates
                .Where(c => c.Capabilities.Contains(ModelCapability.HighPerformance)).FirstOrDefault()
                ?? candidates.First();

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Selects the best reasoning model available prioritizing high performance.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private static SingleModelDispatchResult RouteBestReasoning(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelDescriptor descriptor = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Reasoning))
                .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                .First();

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Selects the best vision-capable model available prioritizing high performance.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private static SingleModelDispatchResult RouteBestVision(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelDescriptor descriptor = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Vision))
                .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                .First();

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Selects the best audio input-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private static SingleModelDispatchResult RouteBestAudioIn(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelDescriptor descriptor = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.AudioIn))
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Selects the best audio output-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private static SingleModelDispatchResult RouteBestAudioOut(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelDescriptor descriptor = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.AudioOut))
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Selects the best embedding-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private static SingleModelDispatchResult RouteEmbedding(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelDescriptor descriptor = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Embedding))
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            return new SingleModelDispatchResult(descriptor);
        }

        /// <summary>
        /// Selects the best moderation-capable model available prioritizing lower input token cost.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private static SingleModelDispatchResult RouteModeration(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, SingleModelDispatchRequest request)
        {
            ModelDescriptor descriptor = modelRegistry.Values
                .Where(m => m.Capabilities.Contains(ModelCapability.Moderation))
                .OrderBy(m => m.Pricing.InputTokenCost)
                .First();

            return new SingleModelDispatchResult(descriptor);
        }

        // -------------------------
        // Shared Helper
        // -------------------------

        /// <summary>
        /// Helper method to filter the model descriptors in the registry based on the required capabilities.
        /// </summary>
        /// <param name="modelRegistry"></param>
        /// <param name="required"></param>
        /// <returns>List of ModelDescriptor(s) with required capabilities.</returns>
        private static IEnumerable<ModelDescriptor> FilterByCapabilities(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry, IReadOnlyCollection<ModelCapability>? required)
        {
            if (required is null || required.Count == 0)
            {
                return modelRegistry.Values;
            }

            // Filter model descriptor(s) in registry that contain all required capabilities ..
            return modelRegistry.Values.Where(md => required.All(req => md.Capabilities.Contains(req)));
        }
    }
}
