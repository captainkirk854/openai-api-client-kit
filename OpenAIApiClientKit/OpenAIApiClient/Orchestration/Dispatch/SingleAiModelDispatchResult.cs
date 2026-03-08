// <copyright file="SingleAiModelDispatchResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Dispatch
{
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleAiModelDispatchResult"/> class with the specified model descriptor.
    /// </summary>
    /// <param name="model">The model descriptor to associate with the result.</param>
    public sealed class SingleAiModelDispatchResult(AiModelDescriptor model)
    {
        /// <summary>
        /// Gets the model descriptor associated with this instance.
        /// </summary>
        public AiModelDescriptor Model
        {
            get;
            init;
        } = model;
    }
}
