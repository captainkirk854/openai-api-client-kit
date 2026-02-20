// <copyright file="OpenAIModels.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.Models
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Registries;

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
    public sealed class OpenAIModels : IAIModelRegistry
    {
        // Dictionary to hold model descriptors ..
        private readonly Dictionary<OpenAIModel, ModelDescriptor> models;

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
                [OpenAIModel.GPT5_2] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.HighPerformance,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.GPT5_2_Pro] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.HighPerformance,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.GPT5] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.HighPerformance,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.GPT5_Mini] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.GPT5_Nano] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT5,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // GPT‑4 Family
                // -------------------------
                // GPT‑4.1 — Standard Reasoning Model
                [OpenAIModel.GPT4_1] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT41,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00001m,
                    outputTokenCost: 0.00003m,
                    cachedInputTokenCost: 0.000002m),
                },

                // GPT‑4.1 Mini — Fast, Low‑Cost Model
                [OpenAIModel.GPT4_1_Mini] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT41,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                        ModelCapability.FastInference,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.0000015m,
                    outputTokenCost: 0.000002m),
                },

                [OpenAIModel.GPT4_1_Reasoning] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT41,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.HighPerformance,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m,
                    reasoningTokenCost: 0.00000000m),
                },

                // GPT‑4.1 Critic — Evaluation / Critique Model
                [OpenAIModel.GPT4_1_Critic] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT41,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.Critic,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00001m,
                    outputTokenCost: 0.00003m),
                },

                [OpenAIModel.GPT4_Turbo] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT4,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.HighPerformance,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // GPT‑4o Family
                // -------------------------
                [OpenAIModel.GPT4o] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT41,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Vision,
                        ModelCapability.AudioIn,
                        ModelCapability.AudioOut,
                        ModelCapability.HighPerformance,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // GPT‑4o Mini — NEW
                [OpenAIModel.GPT4o_Mini] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT4o,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Vision,
                        ModelCapability.LowCost,
                        ModelCapability.FastInference,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000015m,
                    outputTokenCost: 0.00000060m),
                },

                // -------------------------
                // GPT‑3.5 Family
                // -------------------------
                [OpenAIModel.GPT3_5_Turbo] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT35,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Embedding Models
                // -------------------------
                [OpenAIModel.TextEmbedding_3_Large] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Embedding,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TextEmbedding_3_Small] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Embedding,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TextEmbedding_Ada_002] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT3,
                    Domain = ModelDomain.Embedding,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Audio / TTS / Whisper
                // -------------------------
                [OpenAIModel.TTS_1] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TTS_1_HD] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.Whisper_1] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioIn,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TTS1_1106] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.TTS1HD_1106] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Audio,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Image Models
                // -------------------------
                [OpenAIModel.DALL_E_3] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Image,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.ImageGeneration,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Open‑Weight Models
                // -------------------------
                [OpenAIModel.O1] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.OpenWeight,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModel.O1_Mini] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.Other,
                    Domain = ModelDomain.Chat,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.OpenWeight,
                        ModelCapability.LowCost,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Moderation
                // -------------------------
                [OpenAIModel.OmniModerationLatest] = new ModelDescriptor
                {
                    Generation = OpenAIModelGeneration.GPT4o,
                    Domain = ModelDomain.Moderation,
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Moderation,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },
            };

            // Finalise registry, by using dictionary key value to auto-inject the "yet-to-be-set" Model property value for each model descriptor ..
            foreach ((OpenAIModel model, ModelDescriptor descriptor) in this.models)
            {
                descriptor.Name = model;
            }
        }

        /// <summary>
        /// Gets the complete model registry dictionary ..
        /// </summary>
        /// <returns see cref="Dictionary(OpenAIModel, ModelDescriptor)">.</returns>
        public Dictionary<OpenAIModel, ModelDescriptor> GetRegistry() => this.models;

        /// <summary>
        /// Gets all registered model descriptors ..
        /// </summary>
        /// <returns see cref="IEnumerable(ModelDescriptor)">.</returns>
        public IEnumerable<ModelDescriptor> GetAll() => this.models.Values;

        /// <summary>
        /// Gets the model descriptor for a specified model by its unique name, or null if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="ModelDescriptor">.</returns>
        public ModelDescriptor? GetByName(string name) => this.models.Values.Where(m => m.Name.ToApiString() == name).FirstOrDefault();

        /// <summary>
        /// Gets the model descriptor for a specified model ..
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="ModelDescriptor"/>.</returns>
        public ModelDescriptor Get(OpenAIModel model) => this.models[model];

        /// <summary>
        /// Returns all models that satisfy a capability predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns see cref="IEnumerable(ModelDescriptor)">.</returns>
        public IEnumerable<ModelDescriptor> Find(Func<ModelDescriptor, bool> predicate)
        {
            return this.models.Values.Where(predicate);
        }
    }
}
