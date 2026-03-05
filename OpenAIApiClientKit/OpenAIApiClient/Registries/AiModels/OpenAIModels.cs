// <copyright file="OpenAIModels.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// Provides a registry of available OpenAI models and their descriptors, including capabilities and pricing information.
    /// </summary>
    /// <remarks>Model registry is built in two phases:
    ///
    /// 1. Initialize each model key with its descriptor "Capabilities" and "Pricing" values only.
    /// 2. Finalise registry by using dictionary key value to auto-inject the "yet-to-be-set" Model property value for each model descriptor.
    ///
    /// Registry design benefits:
    /// - No repeated Model property assignments in descriptor as dictionary key is single source of truth
    /// - No risk of mismatched keys
    /// - Adding a new model is a single line
    /// - The descriptor always knows its own model.
    /// - Resultant registry is immutable and validated.
    ///
    /// Pricing values are placeholders and should be updated with actual costs as per OpenAI's pricing documentation.
    /// The costs are represented in USD per 1 token (e.g. an input token cost set to: 0.0000015m = $0.0000015 ), so they may need to be
    /// adjusted based on the specific pricing structure provided by OpenAI:
    /// https://openai.com/api/pricing/
    /// https://platform.openai.com/docs/pricing
    /// https://pricepertoken.com/pricing-page/provider/openai
    ///
    /// This registry can be easily extended to include additional models as they are released by OpenAI.
    /// </remarks>
    public sealed class OpenAIModels : IAiModelRegistry
    {
        // Dictionary to hold model descriptors ..
        private readonly Dictionary<OpenAIModel, AiModelDescriptor> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIModels"/> class.
        /// </summary>
        /// <remarks>
        /// Model registry is built in two phases:
        ///   1. Initialize each model key with its descriptor "Capabilities" and "Pricing" values only.
        ///   2. Finalise registry by using dictionary key value to auto-inject the "yet-to-be-set" Model property value for each model descriptor.
        /// Registry design benefits:
        ///   - No repeated Model property assignments in descriptor as dictionary key is single source of truth
        ///   - No risk of mismatched keys
        ///   - Adding a new model is a single line
        ///   - The descriptor always knows its own model.
        ///   - Resultant registry is immutable and validated.
        /// Pricing values are placeholders and should be updated with actual costs as per OpenAI's pricing documentation. The costs are represented in
        /// USD per 1 token (e.g. an input token cost set to: 0.0000015m = $0.0000015 ), so they may need to be adjusted based on the specific pricing
        /// structure provided by OpenAI (https://openai.com/api/pricing/, https://platform.openai.com/docs/pricing, https://pricepertoken.com/pricing-page/provider/openai)
        /// This registry can be easily extended to include additional models as they are released by OpenAI.
        /// </remarks>
        public OpenAIModels()
        {
            // Start building registry dictionary by initializing each model key with its descriptor "Capabilities" and "Pricing" values only ..
            this.models = new()
            {
                // -------------------------
                // GPT‑5 Family
                // -------------------------
                [OpenAIModel.GPT5_2] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.Reasoning,
                        AiModelCapability.HighPerformance,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000175m,
                    outputTokenCost: 0.000014m),
                },

                [OpenAIModel.GPT5_2_Pro] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.Reasoning,
                        AiModelCapability.HighPerformance,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.000021m,
                    outputTokenCost: 0.000168m),
                },

                [OpenAIModel.GPT5] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.HighPerformance,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000125m,
                    outputTokenCost: 0.00001m),
                },

                [OpenAIModel.GPT5_Mini] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.LowCost,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000025m,
                    outputTokenCost: 0.000002m),
                },

                [OpenAIModel.GPT5_Nano] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.LowCost,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000005m,
                    outputTokenCost: 0.0000004m),
                },

                // -------------------------
                // GPT‑4 Family
                // -------------------------
                // GPT‑4.1 — Standard Reasoning Model
                [OpenAIModel.GPT4_1] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT41,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.Reasoning,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00001m,
                    outputTokenCost: 0.00003m,
                    cachedInputTokenCost: 0.000002m),
                },

                // GPT‑4.1 Mini — Fast, Low‑Cost Model
                [OpenAIModel.GPT4_1_Mini] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT41,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.LowCost,
                        AiModelCapability.FastInference,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.0000015m,
                    outputTokenCost: 0.000002m),
                },

                [OpenAIModel.GPT4_Turbo] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT4,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.HighPerformance,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // GPT‑4o Family
                // -------------------------
                [OpenAIModel.GPT4o] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT41,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.Vision,
                        AiModelCapability.AudioIn,
                        AiModelCapability.AudioOut,
                        AiModelCapability.HighPerformance,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // GPT‑4o Mini — NEW
                [OpenAIModel.GPT4o_Mini] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT4o,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.Vision,
                        AiModelCapability.LowCost,
                        AiModelCapability.FastInference,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000015m,
                    outputTokenCost: 0.00000060m),
                },

                // -------------------------
                // GPT‑3.5 Family
                // -------------------------
                [OpenAIModel.GPT3_5_Turbo] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT35,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.LowCost,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Embedding Models
                // -------------------------
                [OpenAIModel.TextEmbedding_3_Large] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Embedding,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Embedding,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TextEmbedding_3_Small] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Embedding,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Embedding,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TextEmbedding_Ada_002] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT3,
                    Domain = ModelDomain.Embedding,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Embedding,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Audio / TTS / Whisper
                // -------------------------
                [OpenAIModel.TTS_1] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.AudioOut,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TTS_1_HD] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.AudioOut,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.Whisper_1] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.AudioIn,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TTS1_1106] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.AudioOut,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TTS1HD_1106] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.AudioOut,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Image Models
                // -------------------------
                [OpenAIModel.DALL_E_3] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Image,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.ImageGeneration,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Open‑Weight Models
                // -------------------------
                [OpenAIModel.O1] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.OpenWeight,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.O1_Mini] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Text,
                        AiModelCapability.Chat,
                        AiModelCapability.OpenWeight,
                        AiModelCapability.LowCost,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.O3] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Reasoning,
                        AiModelCapability.HighPerformance,
                        AiModelCapability.Critic,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m,
                    reasoningTokenCost: 0.00000000m),
                },

                [OpenAIModel.O3_Mini] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Reasoning,
                        AiModelCapability.HighPerformance,
                        AiModelCapability.Critic,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m,
                    reasoningTokenCost: 0.00000000m),
                },

                [OpenAIModel.O4_Mini] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Reasoning,
                        AiModelCapability.HighPerformance,
                        AiModelCapability.LowCost,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m,
                    reasoningTokenCost: 0.00000000m),
                },

                // -------------------------
                // Moderation
                // -------------------------
                [OpenAIModel.OmniModerationLatest] = new AiModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT4o,
                    Domain = ModelDomain.Moderation,
                    Capabilities = new HashSet<AiModelCapability>
                    {
                        AiModelCapability.Moderation,
                    },
                    Pricing = new AiModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },
            };

            // Finalise registry, by using dictionary key value to auto-inject the "yet-to-be-set" Model property value for each model descriptor ..
            foreach ((OpenAIModel model, AiModelDescriptor descriptor) in this.models)
            {
                descriptor.Name = model;
            }
        }

        /// <summary>
        /// Gets the complete model registry dictionary ..
        /// </summary>
        /// <returns see cref="Dictionary(OpenAIModel, AiModelDescriptor)">.</returns>
        public Dictionary<OpenAIModel, AiModelDescriptor> GetRegistry() => this.models;

        /// <summary>
        /// Gets all registered model descriptors ..
        /// </summary>
        /// <returns see cref="IEnumerable(AiModelDescriptor)">.</returns>
        public IEnumerable<AiModelDescriptor> GetAll() => this.models.Values;

        /// <summary>
        /// Gets the model descriptor for a specified model by its unique name, or null if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="AiModelDescriptor">.</returns>
        public AiModelDescriptor? GetByName(string name) => this.models.Values.Where(m => m.Name.ToApiString() == name).FirstOrDefault();

        /// <summary>
        /// Gets the model descriptor for a specified model ..
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="AiModelDescriptor"/>.</returns>
        public AiModelDescriptor Get(OpenAIModel model) => this.models[model];

        /// <summary>
        /// Returns all models that satisfy a capability predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns see cref="IEnumerable(AiModelDescriptor)">.</returns>
        public IEnumerable<AiModelDescriptor> Find(Func<AiModelDescriptor, bool> predicate)
        {
            return this.models.Values.Where(predicate);
        }
    }
}
