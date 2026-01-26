// <copyright file="ModelRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Individual
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries;

    public sealed class ModelRouter(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry)
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry = modelRegistry;

        /// <summary>
        /// Routes a model request based on the specified routing strategy.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A ModelRouterResult containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the request does not specify an explicit model.</exception>
        public ModelRouterResult Route(ModelRouterRequest request)
        {
            ModelRoutingStrategyHandler handler = ModelRoutingStrategyRegistry.Get(strategy: request.Strategy);
            return handler(modelRegistry: this.modelRegistry, request);
        }
    }
}