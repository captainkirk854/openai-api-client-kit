// <copyright file="ChatCompletionRequest.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Request
{
    using System.Text.Json.Serialization;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Common;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries.Models;

    public class ChatCompletionRequest
    {
        /// <summary>
        /// Gets or sets the model to use for generating the chat completion (e.g., "gpt-4o").
        /// </summary>
        [JsonPropertyName("model")]
        required public string Model
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the ordered list of messages that make up the conversation.
        /// </summary>
        [JsonPropertyName("messages")]
        required public List<ChatMessage> Messages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Presence penalty parameter. Penalizes new tokens based on whether they appear in the text so far.
        /// </summary>
        [JsonPropertyName("presence_penalty")]
        public double? PresencePenalty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Frequency penalty parameter. Penalizes new tokens based on their existing frequency in the text so far.
        /// </summary>
        [JsonPropertyName("frequency_penalty")]
        public double? FrequencyPenalty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Tops sampling parameter. Controls diversity via nucleus sampling.
        /// </summary>
        [JsonPropertyName("top_p")]
        public double? TopP
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Controls randomness. Higher values produce more creative output.
        /// </summary>
        /// <remarks>
        /// Valid values range from 0 to 2. Higher values (1.2 - 2.0) produce more random and creative
        /// responses, while lower values (0 - 0.3) make the output more focused and deterministic.
        /// Values above 2.0 are not permitted by the OpenAI API.
        /// </remarks>
        [JsonPropertyName("temperature")]
        public double? Temperature
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the maximum number of tokens the model is allowed to generate.
        /// </summary>
        [JsonPropertyName("max_completion_tokens")]
        public int? MaxTokens
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether to stream the response back in chunks.
        /// </summary>
        [JsonPropertyName("stream")]
        public bool? Stream
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the optional list of tools (functions) the model may call.
        /// </summary>
        [JsonPropertyName("tools")]
        public List<ChatToolDefinition>? Tools
        {
            get;
            set;
        }

        /// <summary>
        /// Gets OpenAIModel from model string (not to be deserialized).
        /// </summary>
        [JsonIgnore]
        public OpenAIModel OpenAIModel
        {
            get
            {
                return OpenAIModelApis.FromApiString(apiModelId: this.Model);
            }
        }

        /// <summary>
        /// Gets ModelDescriptor for OpenAI model (not to be deserialized).
        /// </summary>
        [JsonIgnore]
        public ModelDescriptor ModelDescriptor
        {
            get
            {
                OpenAIModels models = new();
                return models.Get(model: this.OpenAIModel);
            }
        }
    }
}
