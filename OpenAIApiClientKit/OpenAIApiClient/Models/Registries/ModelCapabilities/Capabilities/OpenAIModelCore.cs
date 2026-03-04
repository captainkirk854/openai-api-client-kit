// <copyright file="OpenAIModelCore.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Registries.ModelCapabilities.Capabilities
{
    /// <summary>
    /// Represents the core capability scores of an OpenAI model, including reasoning, text generation, chat, vision,
    /// and audio processing features.
    /// </summary>
    public sealed class OpenAIModelCore
    {
        /// <summary>
        /// Gets the reasoning capability of the model, which indicates its ability to perform logical reasoning and problem-solving tasks. A higher value suggests that the model is more proficient in understanding complex concepts and making inferences based on provided information.
        /// </summary>
        public int Reasoning
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the text generation capability of the model, which reflects its ability to produce coherent and contextually relevant text based on input prompts. A higher value indicates that the model can generate more fluent and meaningful responses, making it suitable for tasks such as content creation, conversation, and storytelling.
        /// </summary>
        public int Text
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the chat capability of the model, which measures its proficiency in engaging in interactive conversations. A higher value suggests that the model can maintain context, understand user intent, and provide more relevant and coherent responses in a conversational setting.
        /// </summary>
        public int Chat
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the vision capability of the model, which indicates its ability to process and understand visual information. A higher value suggests that the model can analyze images, recognize objects, and interpret visual data more effectively, making it suitable for tasks such as image captioning, object detection, and visual question answering.
        /// </summary>
        public int Vision
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the audio input capability of the model, which reflects its ability to process and understand audio data. A higher value indicates that the model can analyze and interpret audio signals more effectively, making it suitable for tasks such as speech recognition, audio classification, and sound analysis.
        /// </summary>
        public int AudioIn
        {
            get;
            init;
        }

        /// <summary>
        /// Gets the audio output capability of the model, which measures its proficiency in generating and producing audio content. A higher value suggests that the model can create more natural and coherent audio outputs, making it suitable for tasks such as text-to-speech synthesis, music generation, and audio-based storytelling.
        /// </summary>
        public int AudioOut
        {
            get;
            init;
        }
    }
}
