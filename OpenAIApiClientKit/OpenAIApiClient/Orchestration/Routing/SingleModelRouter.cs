// <copyright file="SingleModelRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Routing
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Orchestration.Routing;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries.Routing;

    /// <summary>
    /// SingleRouter routes single model requests to the appropriate strategy based on the provided context.
    /// </summary>
    /// <param name="modelRegistry"></param>
    public sealed class SingleModelRouter(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry) : ISingleModelRouter
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry = modelRegistry;

        /// <summary>
        /// Routes a model context to the appropriate strategy and returns related model as part of SingleRouterResult.
        /// </summary>
        /// <param name="request"></param>
        /// <returns cref ="SingleModelRouterResult"> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the context does not specify an explicit model.</exception>
        public SingleModelRouterResult Route(SingleModelRouterRequest request)
        {
            // Define the single model context from the request ..
            SingleModelRouterRequest context = new()
            {
                ExplicitModel = request.ExplicitModel,
                RequiredCapabilities = request.RequiredCapabilities,
                Strategy = request.Strategy,
            };

            // Get the actual strategy handler definition to use as delegate ..
            SingleModelRoutingStrategyHandler handler = SingleModelStrategies.Get(strategy: context.Strategy);

            // Invoke the handler to get the strategy ..
            return handler(modelRegistry: this.modelRegistry, request: context);
        }
    }
}