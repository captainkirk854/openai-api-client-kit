// <copyright file="ModelRouterResult.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Individual
{
    using OpenAIApiClient.Models.Registries;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelRouterResult"/> class with the specified model descriptor.
    /// </summary>
    /// <param name="descriptor">The model descriptor to associate with the result.</param>
    public sealed class ModelRouterResult(ModelDescriptor descriptor)
    {
        /// <summary>
        /// Gets the model descriptor associated with this instance.
        /// </summary>
        public ModelDescriptor Descriptor
        {
            get;
            init;
        } = descriptor;
    }
}
