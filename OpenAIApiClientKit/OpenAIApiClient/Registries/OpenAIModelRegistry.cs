// <copyright file="OpenAIModelRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.OptimalModelSelection;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenAIModelRegistry"/> class.
    /// </summary>
    public sealed class OpenAIModelRegistry
    {
        // Dictionary to hold model descriptors ..
        private readonly Dictionary<OpenAIModels, OpenAIModelDescriptor> models;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenAIModelRegistry"/> class.
        /// </summary>
        /// <remarks>
        /// Model registry is built in two phases:
        /// 1. Initialize each model key with its descriptor "Capabilities" and "Pricing" values only.
        /// 2. Finalise registry by using dictionary key value to auto-inject the "yet-to-be-set" Model property value for each model descriptor.
        /// Registry design benefits:
        /// - No repeated Model property assignments in descriptor as dictionary key is single source of truth
        /// - No risk of mismatched keys
        /// - Adding a new model is a single line
        /// - The descriptor always knows its own model.
        /// - Resultant registry is immutable and validated.
        /// </remarks>
        public OpenAIModelRegistry()
        {
            // Start building registry dictionary by initializing each model key with its descriptor "Capabilities" and "Pricing" values only ..
            this.models = new()
            {
                // -------------------------
                // GPT‑5 Family
                // -------------------------
                [OpenAIModels.GPT5_2] = new OpenAIModelDescriptor
                {
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

                [OpenAIModels.GPT5_2_Pro] = new OpenAIModelDescriptor
                {
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

                [OpenAIModels.GPT5] = new OpenAIModelDescriptor
                {
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

                [OpenAIModels.GPT5_Mini] = new OpenAIModelDescriptor
                {
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

                [OpenAIModels.GPT5_Nano] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.GPT4_1] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.GPT4_1_Mini] = new OpenAIModelDescriptor
                {
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

                [OpenAIModels.GPT4_1_Reasoning] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.GPT4_1_Critic] = new OpenAIModelDescriptor
                {
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

                [OpenAIModels.GPT4_Turbo] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.GPT4o] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.GPT4o_Mini] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.GPT3_5_Turbo] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.TextEmbedding_3_Large] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModels.TextEmbedding_3_Small] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModels.TextEmbedding_Ada_002] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.TTS_1] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModels.TTS_1_HD] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModels.Whisper_1] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioIn,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModels.TTS1_1106] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                [OpenAIModels.TTS1HD_1106] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.DALL_E_3] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.ImageGeneration,
                    },
                    Pricing = new ModelPricing(
                    inputTokenCost: 0.00000000m,
                    outputTokenCost: 0.00000000m),
                },

                // -------------------------
                // Open‑weight Models
                // -------------------------
                [OpenAIModels.O1] = new OpenAIModelDescriptor
                {
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

                [OpenAIModels.O1_Mini] = new OpenAIModelDescriptor
                {
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
                [OpenAIModels.OmniModerationLatest] = new OpenAIModelDescriptor
                {
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
            foreach ((OpenAIModels model, OpenAIModelDescriptor descriptor) in this.models)
            {
                descriptor.Model = model;
            }
        }

        /// <summary>
        /// Gets the complete model registry dictionary ..
        /// </summary>
        public Dictionary<OpenAIModels, OpenAIModelDescriptor> Registry => this.models;

        /// <summary>
        /// Gets all registered model descriptors ..
        /// </summary>
        public IEnumerable<OpenAIModelDescriptor> All => this.models.Values;

        /// <summary>
        /// Gets the model descriptor for a specified model ..
        /// </summary>
        /// <param name="model"></param>
        /// <returns><see cref="OpenAIModelDescriptor"/>.</returns>
        public OpenAIModelDescriptor Get(OpenAIModels model) => this.models[model];
    }
}
