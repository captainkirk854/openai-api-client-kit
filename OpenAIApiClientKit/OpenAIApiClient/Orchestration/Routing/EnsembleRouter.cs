// <copyright file="EnsembleRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Routing
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Enums.Routing;
    using OpenAIApiClient.Interfaces.Orchestration.Routing;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries.Routing;

    /// <summary>
    /// EnsembleRouter routes ensemble requests to the appropriate strategy based on the provided context.
    /// </summary>
    /// <param name="modelRegistry"></param>
    public sealed class EnsembleRouter(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry) : IEnsembleRouter
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry = modelRegistry;

        /// <summary>
        /// Routes an ensemble request to the appropriate set of models
        /// based on the ensemble routing strategy.
        /// </summary>
        /// <param name="request">The ensemble router request containing the routing strategy.</param>
        /// <returns see cref="EnsembleRouterResult"> containing the selected models.</returns>
        public EnsembleRouterResult Route(EnsembleRouterRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            // Special case for the 'Custom' strategy ..
            if (request.Strategy == EnsembleStrategy.Custom)
            {
                return this.BuildCustomEnsemble(request: request);
            }

            // Get the actual strategy handler definition to use as delegate ..
            EnsembleRoutingStrategyHandler handler = EnsembleStrategies.Get(strategy: request.Strategy);

            // Invoke the handler to get the strategy ..
            return handler(modelRegistry: this.modelRegistry);
        }

        /// <summary>
        /// Creates a custom ensemble of models that match the required capabilities specified in the request.
        /// </summary>
        /// <param name="request">The ensemble router request containing the required capabilities.</param>
        /// <returns>An EnsembleRouterResult containing the selected models.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no required capabilities are specified or if no models match the requested capabilities.</exception>
        private EnsembleRouterResult BuildCustomEnsemble(EnsembleRouterRequest request)
        {
            if (request.RequiredCapabilities is null || request.RequiredCapabilities.Count == 0)
            {
                throw new InvalidOperationException("Custom ensemble requires at least one defined capability.");
            }

            List<ModelDescriptor> models = [.. this.modelRegistry.Values
                .Where(model => request.RequiredCapabilities.All(cap => model.Capabilities.Contains(cap)))
                .OrderBy(model => model.Pricing.InputTokenCost)];

            if (models.Count == 0)
            {
                throw new InvalidOperationException("No model(s) match the requested capabilities.");
            }

            return new EnsembleRouterResult(models: models);
        }
    }
}
