// <copyright file="ModelDescriptor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// Model descriptor for OpenAI models.
    /// </summary>
    /// <remarks>
    ///  - Model is: 'internal set' so that only the registry builder can create valid instances.
    ///  - Capabilities is immutable from the outside.
    /// </remarks>
    public sealed class ModelDescriptor
    {
        /// <summary>
        /// Gets the model.
        /// </summary>
        public OpenAIModel Model
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the generation of the model.
        /// </summary>
        public OpenAIModelGeneration Generation
        {
            get;
            init;
        } = OpenAIModelGeneration.Other;

        /// <summary>
        /// Gets the domain of the model.
        /// </summary>
        public ModelDomain Domain
        {
            get;
            init;
        } = ModelDomain.Other;

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
        /// Overrides ToString() to provide a string representation of the OpenAIModels enum.
        /// </summary>
        /// <returns>String representation of the OpenAIModels enum.</returns>
        public override string ToString() => this.Model.ToApiString();
    }
}