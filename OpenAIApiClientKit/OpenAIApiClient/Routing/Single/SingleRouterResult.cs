// <copyright file="SingleRouterResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Single
{
    using OpenAIApiClient.Models.Registries;

    /// <summary>
    /// Initializes a new instance of the <see cref="SingleRouterResult"/> class with the specified model descriptor.
    /// </summary>
    /// <param name="model">The model descriptor to associate with the result.</param>
    public sealed class SingleRouterResult(ModelDescriptor model)
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
