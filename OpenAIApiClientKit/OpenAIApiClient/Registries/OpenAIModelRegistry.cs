// <copyright file="OpenAIModelRegistry.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries
{
    using OpenAIApiClient.Enums;
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
        /// Registry design benefits:
        /// - No repeated Model property assignments in descriptor as dictionary key is single source of truth
        /// - No risk of mismatched keys
        /// - Adding a new model is a single line
        /// - The descriptor always knows its own model.
        /// - Resultant registry is immutable and validated.
        /// </remarks>
        public OpenAIModelRegistry()
        {
            // Start building registry dictionary by initializing each model key with its descriptor "Capabilities" value only ..
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
                },

                [OpenAIModels.GPT5] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.HighPerformance,
                    },
                },

                [OpenAIModels.GPT5_Mini] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                    },
                },

                [OpenAIModels.GPT5_Nano] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                    },
                },

                // -------------------------
                // GPT‑4 Family
                // -------------------------
                [OpenAIModels.GPT4_1] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                    },
                },

                [OpenAIModels.GPT4_1_Mini] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.LowCost,
                        ModelCapability.FastInference,
                    },
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
                },

                [OpenAIModels.GPT4_1_Critic] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.Reasoning,
                        ModelCapability.Critic,
                    },
                },

                [OpenAIModels.GPT4_Turbo] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Text,
                        ModelCapability.Chat,
                        ModelCapability.HighPerformance,
                    },
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
                },

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
                },

                [OpenAIModels.TextEmbedding_3_Small] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                },

                [OpenAIModels.TextEmbedding3Small] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                },

                [OpenAIModels.TextEmbedding3Large] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
                },

                [OpenAIModels.TextEmbeddingAda002] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.Embedding,
                    },
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
                },

                [OpenAIModels.TTS_1_HD] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.Whisper_1] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioIn,
                    },
                },

                [OpenAIModels.Whisper1] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioIn,
                    },
                },

                [OpenAIModels.TTS1] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.TTS1HD] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.TTS1_1106] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
                },

                [OpenAIModels.TTS1HD_1106] = new OpenAIModelDescriptor
                {
                    Capabilities = new HashSet<ModelCapability>
                    {
                        ModelCapability.AudioOut,
                    },
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
                },
            };

            // Finalise the registry, by using dictionary key value to auto-inject the "yet-to-be-set" Model property value for each model descriptor ..
            foreach ((OpenAIModels model, OpenAIModelDescriptor descriptor) in this.models)
            {
                descriptor.Model = model;
            }
        }

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
