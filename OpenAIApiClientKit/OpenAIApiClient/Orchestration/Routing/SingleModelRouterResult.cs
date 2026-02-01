// <copyright file="SingleModelRouterResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Routing
{
    using OpenAIApiClient.Models.Registries;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleModelRouterResult"/> class with the specified model descriptor.
    /// </summary>
    /// <param name="model">The model descriptor to associate with the result.</param>
    public sealed class SingleModelRouterResult(ModelDescriptor model)
    {
        /// <summary>
        /// Gets the model descriptor associated with this instance.
        /// </summary>
        public ModelDescriptor Model
        {
            get;
            init;
        } = model;
    }
}
