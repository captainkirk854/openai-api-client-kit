// <copyright file="EnsembleRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Ensemble
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// EnsembleRouter routes ensemble requests to the appropriate strategy based on the provided context.
    /// </summary>
    /// <param name="modelRegistry"></param>
    public sealed class EnsembleRouter(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry)
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry = modelRegistry;

        /// <summary>
        /// Routes an ensemble context to the appropriate strategy and returns related model(s) as part of EnsembleRouterResult.
        /// </summary>
        /// <param name="context"></param>
        /// <returns cref ="EnsembleRouterResult"> containing the resolved model descriptor(s).</returns>
        public EnsembleRouterResult Route(EnsembleContext context)
        {
            // Special case for custom strategy ..
            if (context.Strategy == EnsembleRoutingStrategy.Custom)
            {
                return this.BuildCustomEnsemble(context);
            }

            // Get the strategy from the registry and invoke it ..
            EnsembleRoutingStrategyHandler strategy = EnsembleRoutingStrategyRegistry.Get(strategy: context.Strategy);
            return strategy(modelRegistry: this.modelRegistry);
        }

        /// <summary>
        /// Creates a custom ensemble of models that match the required capabilities specified in the request.
        /// </summary>
        /// <param name="request">The ensemble router request containing required capabilities and model count.</param>
        /// <returns>An EnsembleRouterResult containing the selected models.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no required capabilities are specified or if no models match the requested capabilities.</exception>
        private EnsembleRouterResult BuildCustomEnsemble(EnsembleContext request)
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
                throw new InvalidOperationException("No model(s) match the requested capabilities.");
            }

            return new EnsembleRouterResult(models: models);
        }
    }
}
