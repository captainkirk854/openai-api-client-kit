// <copyright file="EnsembleDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Dispatch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.Extensions;
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.Dispatch;

    /// <summary>
    /// Dispatches ensemble strategies over the available AI models.
    /// </summary>
    public sealed class EnsembleDispatcher : IEnsembleDispatcher
    {
        private readonly IAiModelRegistry modelRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsembleDispatcher"/> class.
        /// </summary>
        /// <param name="modelRegistry">
        /// The model registry to use when resolving models for ensemble dispatch.
        /// </param>
        public EnsembleDispatcher(IAiModelRegistry modelRegistry)
        {
            ArgumentNullException.ThrowIfNull(modelRegistry);
            this.modelRegistry = modelRegistry;
        }

        /// <summary>
        /// Evaluates a request to select the appropriate set of model descriptor(s).
        /// </summary>
        /// <param name="request">The ensemble dispatch request containing strategy and constraints.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchResult"/> that describes the selected model(s) for the request.
        /// </returns>
        public EnsembleDispatchResult Evaluate(EnsembleDispatchRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.Strategy == AiModelStrategy.Ensemble.Custom)
            {
                return this.BuildCustomEnsemble(request);
            }

            EnsembleDispatchStrategyHandler handler = EnsembleStrategies.Get(request.Strategy);

            IReadOnlyCollection<AiModelDescriptor> availableModels = this.modelRegistry.GetAll();

            return handler(availableModels);
        }

        /// <summary>
        /// Creates a custom ensemble of models that match the required capabilities specified in the request.
        /// </summary>
        /// <param name="request">The ensemble dispatch request containing the required capabilities.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchResult"/> that describes the selected model(s) for the custom ensemble.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no required capabilities are specified and no explicit models are provided,
        /// or if no models match the requested capabilities.
        /// </exception>
        private EnsembleDispatchResult BuildCustomEnsemble(EnsembleDispatchRequest request)
        {
            // For a custom ensemble, we require at least one capability or explicit model to be specified. We will then resolve the models based on the provided information.
            IReadOnlyCollection<AiModelDescriptor> allModels = this.modelRegistry.GetAll();

            // First check if the request has any required capabilities or explicit models defined. If not, we cannot proceed with building a custom ensemble.
            bool hasRequiredCapabilities = request.RequiredCapabilities is not null && request.RequiredCapabilities.Count > 0;
            bool hasExplicitModels = request.ExplicitModels is not null && request.ExplicitModels.Count > 0;
            if (!hasRequiredCapabilities)
            {
                if (!hasExplicitModels)
                {
                    throw new InvalidOperationException("Custom ensemble requires at least one defined capability or explicit model.");
                }

                // If there are no required capabilities, but explicit models are provided, we will attempt to resolve those models directly from the model registry. If any of the explicit models cannot be resolved, we will throw an exception.
                List<AiModelDescriptor> explicitModels = [];
                if(request.ExplicitModels is null)
                {
                    throw new InvalidOperationException("Unexpected null value for explicit models.");
                }

                // Resolve each explicit model name to its corresponding descriptor in the model registry.
                foreach (string modelName in request.ExplicitModels)
                {
                    AiModelDescriptor? resolved = this.modelRegistry.TryGetByName(modelName) ?? throw new InvalidOperationException($"The explicit model '{modelName}' could not be resolved using the configured model registry.");
                    explicitModels.Add(resolved);
                }

                return new EnsembleDispatchResult(models: explicitModels);
            }

            // If we have required capabilities defined, we will filter the available models to find those that match all of the requested capabilities. We will then order the matching models by their input token cost (ascending) and return them in the result. If no models match the requested capabilities, we will throw an exception.
            List<AiModelDescriptor> matchingModels;
            if (request.RequiredCapabilities is null)
            {
                throw new InvalidOperationException("Unexpected null value for required capabilities.");
            }

            // Filter the available models to find those that support all of the required capabilities specified in the request. We will use the SupportsCapability helper method to check if each model supports each required capability.
            matchingModels = [.. allModels
                    .Where(model => request.RequiredCapabilities.All(capability => model.HasCapability(capability: capability)))
                    .OrderBy(model => model.Pricing.InputTokenCost)];

            if (matchingModels.Count == 0)
            {
                throw new InvalidOperationException("No model(s) match the requested capabilities.");
            }

            return new EnsembleDispatchResult(models: matchingModels);
        }
    }
}