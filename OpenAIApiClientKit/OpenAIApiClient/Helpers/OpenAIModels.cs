// <copyright file="OpenAIModels.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers
{
    /// <summary>
    /// List of OpenAI models.
    /// list is based on the latest OpenAI documentation as of December 2025.
    /// </summary>
    public enum OpenAIModels
    {
        // GPT-5 Family
        GPT5_2,
        GPT5_2_Pro,
        GPT5,
        GPT5_Mini,
        GPT5_Nano,

        // GPT-4 Family
        GPT4_1,
        GPT4o,        // Multimodal model (text + vision)
        GPT4o_Mini,   // Optimized GPT-4 variant
        GPT4_Turbo,   // Optimized GPT-4 variant

        // GPT-3.5 Family
        GPT3_5_Turbo,

        // Embedding Models
        TextEmbedding_3_Large,
        TextEmbedding_3_Small,

        // Audio Models
        TTS_1,        // Text-to-Speech
        TTS_1_HD,
        Whisper_1,    // Speech-to-Text

        // Image Models
        DALL_E_3,

        // Open-weight Models
        O1,
        O1_Mini,
    }
}
