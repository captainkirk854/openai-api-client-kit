// <copyright file="EnsembleRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Ensemble
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries;

    public sealed class EnsembleRouter(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry)
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry = modelRegistry;

        /// <summary>
        /// Routes the ensemble request to the appropriate strategy and returns the result.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>EnsembleRouterResult.</returns>
        public EnsembleRouterResult Route(EnsembleRouterRequest request)
        {
            if (request.Strategy == EnsembleRoutingStrategy.Custom)
            {
                return this.BuildCustomEnsemble(request);
            }

            // Get the strategy from the registry and invoke it ..
            EnsembleRoutingStrategyHandler strategy = EnsembleRoutingStrategyRegistry.Get(strategy: request.Strategy);
            return strategy(modelRegistry: this.modelRegistry);
        }

        /// <summary>
        /// Creates a custom ensemble of models that match the required capabilities specified in the request.
        /// </summary>
        /// <param name="request">The ensemble router request containing required capabilities and model count.</param>
        /// <returns>An EnsembleRouterResult containing the selected models.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no required capabilities are specified or if no models match the requested capabilities.</exception>
        private EnsembleRouterResult BuildCustomEnsemble(EnsembleRouterRequest request)
        {
            if (request.RequiredCapabilities is null || request.RequiredCapabilities.Count == 0)
            {
                throw new InvalidOperationException("Custom ensemble requires capabilities.");
            }

            int count = request.ModelCount ?? 1;

            List<ModelDescriptor> models = [.. this.modelRegistry.Values
                                                   .Where(m => request.RequiredCapabilities.All(c => m.Capabilities.Contains(c)))
                                                   .OrderBy(m => m.Pricing.InputTokenCost)
                                                   .Take(count)];

            if (models.Count == 0)
            {
                throw new InvalidOperationException("No models match the requested capabilities.");
            }

            return new EnsembleRouterResult(models: models);
        }
    }
}
