// <copyright file="SingleModelRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.SingleModel
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// SingleRouter routes single model requests to the appropriate strategy based on the provided context.
    /// </summary>
    /// <param name="modelRegistry"></param>
    public sealed class SingleModelRouter(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry)
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry = modelRegistry;

        /// <summary>
        /// Routes a model context to the appropriate strategy and returns related model as part of SingleRouterResult.
        /// </summary>
        /// <param name="context"></param>
        /// <returns cref ="SingleModelRouterResult"> containing the resolved model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the context does not specify an explicit model.</exception>
        public SingleModelRouterResult Route(SingleModelContext context)
        {
            // Get the strategy from the registry and invoke it ..
            SingleModelRoutingStrategyHandler handler = SingleModelRoutingStrategyRegistry.Get(strategy: context.Strategy);
            return handler(modelRegistry: this.modelRegistry, context);
        }
    }
}