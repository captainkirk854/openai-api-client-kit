// <copyright file="OpenAIModelRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.OptimalModelSelection;

    public sealed class OpenAIModelRegistry
    {
        // Dictionary to hold model descriptors ..
        private readonly Dictionary<OpenAIModels, OpenAIModelDescriptor> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIModelRegistry"/> class.
        /// </summary>
        public OpenAIModelRegistry()
        {
            // Initialize the model descriptors ..
            this.models = new()
            {
                [OpenAIModels.GPT4_1] = new OpenAIModelDescriptor
                {
                    Name = OpenAIModels.GPT4_1,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Reasoning,
                    },
                },
                [OpenAIModels.GPT4_1_Mini] = new OpenAIModelDescriptor
                {
                    Name = OpenAIModels.GPT4_1_Mini,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.FastInference,
                        ModelCapability.LowCost,
                    },
                },
                [OpenAIModels.GPT4_1_Critic] = new OpenAIModelDescriptor
                {
                    Name = OpenAIModels.GPT4_1_Critic,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Critic,
                    },
                },
                [OpenAIModels.GPT4o_Mini] = new OpenAIModelDescriptor
                {
                    Name = OpenAIModels.GPT4o_Mini,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.FastInference,
                        ModelCapability.LowCost,
                    },
                },
            };
        }

        /// <summary>
        /// Gets all registered model descriptors ..
        /// </summary>
        public IEnumerable<OpenAIModelDescriptor> All => this.models.Values;

        /// <summary>
        /// Gets the model descriptor for the specified model ..
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="OpenAIModelDescriptor"/>.</returns>
        public OpenAIModelDescriptor Get(OpenAIModels model) => this.models[model];
    }
}
