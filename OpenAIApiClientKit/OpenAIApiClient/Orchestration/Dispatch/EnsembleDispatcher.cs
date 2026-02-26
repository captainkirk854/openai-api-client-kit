// <copyright file="EnsembleDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Dispatch
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries.Dispatch;

    /// <summary>
    /// <see cref="EnsembleDispatcher"/> provides intentional, criteria‑based delegation to select the correct model(s) based on the provided request.
    /// </summary>
    /// <param name="modelRegistry"></param>
    public sealed class EnsembleDispatcher(IAiModelRegistry registry) : IEnsembleDispatcher
    {
        private readonly IReadOnlyDictionary<OpenAIModel, AiModelDescriptor> modelRegistry = registry.GetRegistry();

        /// <summary>
        /// Evaluates a request to select the appropriate set of model descriptor(s).
        /// </summary>
        /// <param name="request">The ensemble dispatch request containing strategy and constraints.</param>
        /// <returns see cref="EnsembleDispatchResult">Selected model descriptor(s).</returns>
        public EnsembleDispatchResult Evaluate(EnsembleDispatchRequest request)
        {
            // Validate input ..
            ArgumentNullException.ThrowIfNull(request);

            // Special case for the 'Custom' strategy ..
            if (request.Strategy == EnsembleStrategy.Custom)
            {
                return this.BuildCustomEnsemble(request: request);
            }

            // Get handler definition to use as delegate method ..
            EnsembleStrategyHandler handler = EnsembleStrategies.Get(strategy: request.Strategy);

            // Invoke the handler to get the result containing the selected models ..
            return handler(modelRegistry: this.modelRegistry);
        }

        /// <summary>
        /// Creates a custom ensemble of models that match the required capabilities specified in the request.
        /// </summary>
        /// <param name="request">The ensemble dispatch request containing the required capabilities.</param>
        /// <returns see cref="EnsembleDispatchResult">Selected model descriptor(s).</returns>
        /// <exception cref="InvalidOperationException">Thrown if no required capabilities are specified or if no models match the requested capabilities.</exception>
        private EnsembleDispatchResult BuildCustomEnsemble(EnsembleDispatchRequest request)
        {
            // If no required capabilities are specified, use the explicitly defined models (if any) or throw an exception if none are provided.
            if (request.RequiredCapabilities is null || request.RequiredCapabilities.Count == 0)
            {
                // If no explicit models are provided, throw an exception since we cannot build an ensemble without criteria or specified models.
                if (request.ExplicitModels is null || request.ExplicitModels.Count == 0)
                {
                    throw new InvalidOperationException("Custom ensemble requires at least one defined capability.");
                }

                // Otherwise, return the explicitly defined models as the ensemble result.
                return new EnsembleDispatchResult(models: [.. request.ExplicitModels.Select(model => this.modelRegistry[model])]);
            }

            List<AiModelDescriptor> models = [.. this.modelRegistry.Values
                .Where(model => request.RequiredCapabilities.All(cap => model.Capabilities.Contains(cap)))
                .OrderBy(model => model.Pricing.InputTokenCost)];

            if (models.Count == 0)
            {
                throw new InvalidOperationException("No model(s) match the requested capabilities.");
            }

            return new EnsembleDispatchResult(models: models);
        }
    }
}
