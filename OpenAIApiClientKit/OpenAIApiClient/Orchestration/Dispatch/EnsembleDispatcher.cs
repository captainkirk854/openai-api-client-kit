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
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.Dispatch;

    /// <summary>
    /// Dispatches ensemble strategies over the available AI models.
    /// </summary>
    public sealed class EnsembleDispatcher : IEnsembleDispatcher
    {
        private readonly IAiModelRegistryNEW modelRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsembleDispatcher"/> class.
        /// </summary>
        /// <param name="modelRegistry">
        /// The model registry to use when resolving models for ensemble dispatch.
        /// </param>
        public EnsembleDispatcher(IAiModelRegistryNEW modelRegistry)
        {
            ArgumentNullException.ThrowIfNull(modelRegistry);
            this.modelRegistry = modelRegistry;
        }

        /// <summary>
        /// Evaluates a request to select the appropriate set of model descriptor(s).
        /// </summary>
        /// <param name="request">The ensemble dispatch request containing strategy and constraints.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchResultNEW"/> that describes the selected model(s) for the request.
        /// </returns>
        public EnsembleDispatchResultNEW Evaluate(EnsembleDispatchRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.Strategy == EnsembleStrategy.Custom)
            {
                return this.BuildCustomEnsemble(request);
            }

            EnsembleStrategyHandlerNEW handler = EnsembleStrategies.GetNew(request.Strategy);

            IReadOnlyCollection<AiModelPropertyRegistryModel> availableModels = this.modelRegistry.GetAll();

            return handler(availableModels);
        }

        /// <summary>
        /// Creates a custom ensemble of models that match the required capabilities specified in the request.
        /// </summary>
        /// <param name="request">The ensemble dispatch request containing the required capabilities.</param>
        /// <returns>
        /// An <see cref="EnsembleDispatchResultNEW"/> that describes the selected model(s) for the custom ensemble.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if no required capabilities are specified and no explicit models are provided,
        /// or if no models match the requested capabilities.
        /// </exception>
        private EnsembleDispatchResultNEW BuildCustomEnsemble(EnsembleDispatchRequest request)
        {
            IReadOnlyCollection<AiModelPropertyRegistryModel> allModels = this.modelRegistry.GetAll();

            bool hasRequiredCapabilities = request.RequiredCapabilities is not null && request.RequiredCapabilities.Count > 0;

            bool hasExplicitModels = request.ExplicitModels is not null && request.ExplicitModels.Count > 0;

            if (!hasRequiredCapabilities)
            {
                if (!hasExplicitModels)
                {
                    throw new InvalidOperationException("Custom ensemble requires at least one defined capability or explicit model.");
                }

                List<AiModelPropertyRegistryModel> explicitModels = [];

                if(request.ExplicitModels is null)
                {
                    throw new InvalidOperationException("Unexpected null value for explicit models.");
                }
                foreach (string modelName in request.ExplicitModels)
                {
                    AiModelPropertyRegistryModel? resolved = this.modelRegistry.TryGetByName(modelName);

                    if (resolved is null)
                    {
                        throw new InvalidOperationException(
                            $"The explicit model '{modelName}' could not be resolved using the configured model registry.");
                    }

                    explicitModels.Add(resolved);
                }

                return new EnsembleDispatchResultNEW(explicitModels);
            }

            List<AiModelPropertyRegistryModel> matchingModels =
                allModels
                    .Where(model => request.RequiredCapabilities.All(capability => SupportsCapability(model, capability)))
                    .OrderBy(model => model.Pricing.InputTokenCost)
                    .ToList();

            if (matchingModels.Count == 0)
            {
                throw new InvalidOperationException("No model(s) match the requested capabilities.");
            }

            return new EnsembleDispatchResultNEW(matchingModels);
        }

        /// <summary>
        /// Determines whether the specified model supports the given capability.
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

                case AiModelCapability.Critic:
                    {
                        return model.Capabilities.Advanced.Critic > 0;
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