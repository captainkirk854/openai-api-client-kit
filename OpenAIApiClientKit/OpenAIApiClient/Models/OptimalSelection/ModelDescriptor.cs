// <copyright file="ModelDescriptor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OptimalSelection
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers;

    public sealed class ModelDescriptor
    {
        /// <summary>
        /// Gets the name of the model.
        /// </summary>
        public OpenAIModels Name
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the capabilities of the model.
        /// </summary>
        public IReadOnlySet<ModelCapability> Capabilities
        {
            get;
            init;
        } = new HashSet<ModelCapability>();

        /// <summary>
        /// Overrides ToString to provide a string representation of the model descriptor.
        /// </summary>
        /// <returns>A string representation of the model descriptor.</returns>
        public override string ToString() => this.Name.ToApiString();
    }
}