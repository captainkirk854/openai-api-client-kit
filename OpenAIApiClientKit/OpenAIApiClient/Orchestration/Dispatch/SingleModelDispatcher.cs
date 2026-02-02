// <copyright file="SingleModelDispatcher.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Dispatch
{
    using OpenAIApiClient.Delegates;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries.Dispatch;

    /// <summary>
    /// <see cref="SingleModelDispatcher"/> provides intentional, criteria‑based delegation to select the correct model based on the provided request.
    /// </summary>
    /// <param name="modelRegistry"></param>
    public sealed class SingleModelDispatcher(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry) : ISingleModelDispatcher
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> modelRegistry = modelRegistry;

        /// <summary>
        /// Evaluates request to select an appropriate model descriptor.
        /// </summary>
        /// <param name="request">The single model dispatch request containing strategy and constraints.</param>
        /// <returns see cref="SingleModelDispatchResult">Selected model descriptor.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the context does not specify an explicit model.</exception>
        public SingleModelDispatchResult Evaluate(SingleModelDispatchRequest request)
        {
            // Validate input ..
            ArgumentNullException.ThrowIfNull(request);

            // Get the actual strategy handler definition to use as delegate ..
            SingleModelStrategyHandler handler = SingleModelStrategies.Get(strategy: request.Strategy);

            // Invoke the handler to get the result containing the selected model ..
            return handler(modelRegistry: this.modelRegistry, request: request);
        }
    }
}