// <copyright file="ModelRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Individual
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;

    public sealed class ModelRouter(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> registry)
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> registry = registry;

        /// <summary>
        /// Routes a model request based on the specified routing strategy.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A ModelRouterResult containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the request does not specify an explicit model.</exception>
        public ModelRouterResult Route(ModelRouterRequest request)
        {
            // Dispatch to appropriate strategy implementation ..
            return request.Strategy switch
            {
                ModelRoutingStrategy.Explicit => this.RouteExplicit(request),
                ModelRoutingStrategy.LowestCost => this.RouteLowestCost(request),
                ModelRoutingStrategy.HighestPerformance => this.RouteHighestPerformance(request),
                ModelRoutingStrategy.BestReasoning => this.RouteByCapability(ModelCapability.Reasoning),
                ModelRoutingStrategy.BestVision => this.RouteByCapability(ModelCapability.Vision),
                ModelRoutingStrategy.BestAudioIn => this.RouteByCapability(ModelCapability.AudioIn),
                ModelRoutingStrategy.BestAudioOut => this.RouteByCapability(ModelCapability.AudioOut),
                ModelRoutingStrategy.Embedding => this.RouteByCapability(ModelCapability.Embedding),
                ModelRoutingStrategy.Moderation => this.RouteByCapability(ModelCapability.Moderation),

                _ => throw new InvalidOperationException("Unknown routing strategy.")
            };
        }

        // -------------------------
        // Strategy Implementations
        // -------------------------

        /// <summary>
        /// Routes a request using an explicitly specified model.
        /// </summary>
        /// <param name="request">The routing request containing the explicit model information.</param>
        /// <returns>A ModelRouterResult containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the request does not specify an explicit model.</exception>
        private ModelRouterResult RouteExplicit(ModelRouterRequest request)
        {
            if (request.ExplicitModel is null)
            {
                throw new InvalidOperationException("Explicit routing requires an explicit model.");
            }

            ModelDescriptor descriptor = this.registry[request.ExplicitModel.Value];
            return new ModelRouterResult(descriptor);
        }

        /// <summary>
        /// Selects the model with the lowest input and output token cost that meets the required capabilities.
        /// </summary>
        /// <param name="request">The routing request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private ModelRouterResult RouteLowestCost(ModelRouterRequest request)
        {
            IEnumerable<ModelDescriptor> candidates = this.FilterByCapabilities(request.RequiredCapabilities);

            ModelDescriptor descriptor = candidates
                                         .OrderBy(c => c.Pricing.InputTokenCost)
                                         .ThenBy(c => c.Pricing.OutputTokenCost)
                                         .First();

            return new ModelRouterResult(descriptor);
        }

        /// <summary>
        /// Selects the model with highest performance capability from the filtered candidates based on the request.
        /// </summary>
        /// <param name="request">The model router request specifying required capabilities.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private ModelRouterResult RouteHighestPerformance(ModelRouterRequest request)
        {
            IEnumerable<ModelDescriptor> candidates = this.FilterByCapabilities(request.RequiredCapabilities);

            ModelDescriptor descriptor = candidates
                                         .Where(c => c.Capabilities.Contains(ModelCapability.HighPerformance)).FirstOrDefault()
                                         ?? candidates.First();

            return new ModelRouterResult(descriptor);
        }

        /// <summary>
        /// Selects the model that supports the specified capability, prioritizing high performance and
        /// lower input token cost.
        /// </summary>
        /// <param name="capability">The required model capability to route by.</param>
        /// <returns>A ModelRouterResult containing the selected model descriptor.</returns>
        private ModelRouterResult RouteByCapability(ModelCapability capability)
        {
            ModelDescriptor descriptor = this.registry.Values
                                             .Where(d => d.Capabilities.Contains(capability))
                                             .OrderByDescending(d => d.Capabilities.Contains(ModelCapability.HighPerformance))
                                             .ThenBy(d => d.Pricing.InputTokenCost)
                                             .First();

            return new ModelRouterResult(descriptor);
        }

        /// <summary>
        /// Helper method to filter the model descriptors in the registry based on the required capabilities.
        /// </summary>
        /// <param name="required"></param>
        /// <returns>List of ModelDescriptor(s) with required capabilities.</returns>
        private IEnumerable<ModelDescriptor> FilterByCapabilities(IReadOnlyCollection<ModelCapability>? required)
        {
            if (required is null || required.Count == 0)
            {
                return this.registry.Values;
            }

            // Filter model descriptor(s) in registry that contain all required capabilities ..
            return this.registry.Values
                                .Where(md => required.All(rc => md.Capabilities.Contains(rc)));
        }
    }
}