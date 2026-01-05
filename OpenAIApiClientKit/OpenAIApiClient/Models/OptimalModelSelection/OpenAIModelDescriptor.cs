// <copyright file="OpenAIModelDescriptor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.OptimalModelSelection
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Models.Chat.Response.Completion;

    /// <summary>
    /// Model descriptor for OpenAI models.
    /// </summary>
    /// <remarks>
    ///  - Model is: 'internal set' so that only the registry builder can create valid instances.
    ///  - Capabilities is immutable from the outside.
    /// </remarks>
    public sealed class OpenAIModelDescriptor
    {
        /// <summary>
        /// Gets the model.
        /// </summary>
        public OpenAIModels Model
        {
            get;
            internal set;
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
        /// Gets the pricing information for the model.
        /// </summary>
        public ModelPricing Pricing
        {
            get;
            init;
        } = new ModelPricing(0, 0);

        /// <summary>
        /// Overrides ToString to provide a string representation of the model descriptor.
        /// </summary>
        /// <returns>A string representation of the model descriptor.</returns>
        public override string ToString() => this.Model.ToApiString();
    }
}