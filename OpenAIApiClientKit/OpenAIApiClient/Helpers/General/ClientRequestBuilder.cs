// <copyright file="ClientRequestBuilder.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Helpers.General
{
    using System.Collections.Generic;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Chat.Common;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// Provides a fluent builder for constructing chat completion requests with configurable model, message sequence,
    /// tool calls, and generation parameters.
    /// </summary>
    /// <remarks>
    /// Use <see cref="ClientRequestBuilder"/> to incrementally configure all aspects of a
    /// chat completion request for OpenAI-compatible APIs. The builder supports adding system, user, assistant, tool,
    /// and developer messages, as well as setting advanced generation parameters such as temperature, top-p, presence
    /// penalty, and frequency penalty. Tool calls can be included to enable function calling capabilities. After
    /// configuration, call <c>Build()</c> to produce a <see cref="ChatCompletionRequest"/> instance ready for
    /// submission. The builder is designed for method chaining and does not modify the returned request after
    /// <c>Build()</c> is called.
    /// Sample usage:
    ///     <see cref="ChatCompletionRequest"/> request = new <see cref="ClientRequestBuilder"/>().WithModel(input: OpenAIModels.GPT4o_Mini)
    ///                                                                                           .AddDeveloperMessage(input: "Always answer concisely.")
    ///                                                                                           .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
    ///                                                                                           .AddUserMessage(input: userPrompt)
    ///                                                                                           .EnableStreaming(input: isStreaming)
    ///                                                                                           .WithTemperature(input: 1.0)
    ///                                                                                           .WithMaxTokens(input: 100)
    ///                                                                                           .WithTopP(input: 0.5)
    ///                                                                                           .WithPresencePenalty(input: 2.0)
    ///                                                                                           .WithFrequencyPenalty(input: 2.0)
    ///                                                                                           .ForceJsonOutput(input: false)
    ///                                                                                           .AddToolCall(id: "weather-1", name: "getWeather", args: new Dictionary string, object
    ///                                                                                            {
    ///                                                                                               { "city", "London" },
    ///                                                                                            })
    ///                                                                                           .Build();.
    /// </remarks>
    public class ClientRequestBuilder
    {
        private readonly List<ChatMessage> messages = [];
        private readonly List<ChatMessage> developerMessages = []; // Optional developer messages (for Messages API compatibility)
        private readonly List<ToolCall> toolCalls = []; // Optional tool calls

        private string model = string.Empty;

        // Optional parameters ..
        private int? maxTokens;
        private bool stream;
        private double? frequencyPenalty;
        private double? presencePenalty;
        private double? temperature;
        private double? topP;
        private bool jsonModeEnabled;

        /// <summary>
        /// Define the OpenAI model to use.
        /// </summary>
        /// <param name="input">The OpenAI model to be used for the request.</param>
        /// <returns>The current instance of <see cref="ClientRequestBuilder"/> with the updated Model setting.</returns>
        public ClientRequestBuilder WithModel(OpenAIModels input)
        {
            this.model = input.ToApiString();
            return this;
        }

        /// <summary>
        /// Add message for system role.
        /// </summary>
        /// <param name="input">The system input prompt to include in the chat message sequence.</param>
        /// <returns>The current instance of <see cref="ClientRequestBuilder"/> with the updated System Message setting.</returns>
        public ClientRequestBuilder AddSystemMessage(string input)
        {
            this.messages.Add(new ChatMessage { RoleAsEnum = OpenAIRole.System, Content = input });
            return this;
        }

        /// <summary>
        /// Add message for user role.
        /// </summary>
        /// <param name="input">The user input prompt to include in the chat message sequence.</param>
        /// <returns>The current instance of <see cref="ClientRequestBuilder"/> with the updated User Message setting.</returns>
        public ClientRequestBuilder AddUserMessage(string input)
        {
            this.messages.Add(new ChatMessage { RoleAsEnum = OpenAIRole.User, Content = input });
            return this;
        }

        /// <summary>
        /// Add message for assistant role.
        /// </summary>
        /// <param name="input">The assistant input prompt to include in the chat message sequence.</param>
        /// <returns>The current instance of <see cref="ClientRequestBuilder"/> with the updated Assistant Message setting.</returns>
        public ClientRequestBuilder AddAssistantMessage(string input)
        {
            this.messages.Add(new ChatMessage { RoleAsEnum = OpenAIRole.Assistant, Content = input });
            return this;
        }

        /// <summary>
        /// Add message for tool role.
        /// </summary>
        /// <param name="input">The tool input prompt to include in the chat message sequence.</param>
        /// <returns>The current instance of <see cref="ClientRequestBuilder"/> with the updated Tool Message setting.</returns>
        public ClientRequestBuilder AddToolMessage(string input)
        {
            this.messages.Add(new ChatMessage { RoleAsEnum = OpenAIRole.Tool, Content = input });
            return this;
        }

        /// <summary>
        /// Adds a developer message with the specified content to the chat completion request.
        /// </summary>
        /// <remarks>
        /// This method enables fluent configuration by returning the same builder instance.
        /// Developer messages are typically used to provide system-level instructions or context to the AI model.
        /// </remarks>
        /// <param name="input">The developer input prompt to include in the chat message sequence.</param>
        /// <returns>The current <see cref="ClientRequestBuilder"/> instance with the developer message added.</returns>
        public ClientRequestBuilder AddDeveloperMessage(string input)
        {
            this.developerMessages.Add(new ChatMessage { RoleAsEnum = OpenAIRole.Developer, Content = input });
            return this;
        }

        // -----------------------------
        // Optional Parameters
        // -----------------------------

        /// <summary>
        /// Enables or disables streaming responses for the chat completion request.
        /// </summary>
        /// <param name="input">Set to <see langword="true"/> to enable streaming; otherwise, <see langword="false"/>.</param>
        /// <returns>The current instance of <see cref="ClientRequestBuilder"/> with the updated streaming setting.</returns>
        public ClientRequestBuilder EnableStreaming(bool input)
        {
            this.stream = input;
            return this;
        }

        /// <summary>
        /// Sets the temperature parameter for the chat completion request, controlling the randomness of generated responses.
        /// </summary>
        /// <remarks>
        /// The temperature parameter influences the variability of the model's output.
        /// Must be a value between 0.0 and 2.0.
        /// Higher values produce more diverse and creative outputs; lower values result in more focused and deterministic responses.
        /// </remarks>
        /// <param name="input">A value between 0.0 and 2.0 that determines the sampling temperature.</param>
        /// <returns>The current instance of <see cref="ClientRequestBuilder"/> with the updated temperature setting.</returns>
        public ClientRequestBuilder WithTemperature(double input)
        {
            // Check within valid range: 0 - 2.0 ..
            input = input < 0 ? 0 : input > 2.0 ? 2.0 : input;

            // Set temperature ..
            this.temperature = input;
            return this;
        }

        /// <summary>
        /// Sets the maximum number of tokens allowed in the chat completion response.
        /// </summary>
        /// <remarks>
        /// Use this method to limit the length of the generated response. Setting a lower value may result in shorter outputs.
        /// Must be a positive integer.
        /// </remarks>
        /// <param name="input">The maximum number of tokens to generate in the response. Default is 1000.</param>
        /// <returns>The current <see cref="ClientRequestBuilder"/> instance with the updated maximum token setting.</returns>
        public ClientRequestBuilder UsingMaxTokens(int input = 1000)
        {
            // Ensure positive integer ..
            input = input < 1 ? 1 : Math.Abs(input); // Ensure at least 1 token ..

            // Set max tokens ..
            this.maxTokens = input;
            return this;
        }

        /// <summary>
        /// Sets the nucleus sampling parameter (top-p) for the chat completion request and returns the updated builder instance.
        /// </summary>
        /// <remarks>
        /// Nucleus sampling controls the diversity of generated responses by restricting the model to tokens whose cumulative probability
        /// does not exceed the specified threshold. Adjusting this value can affect the creativity and randomness of the output.
        /// Must be a value between 0.0 and 1.0, where lower values limit the model to more likely tokens and higher values allow for more diverse outputs.
        /// </remarks>
        /// <param name="input">The probability threshold for nucleus sampling.</param>
        /// <returns>The current <see cref="ClientRequestBuilder"/> instance with the updated top-p value.</returns>
        public ClientRequestBuilder WithTopP(double input)
        {
            // Check within valid range: 0.0 and 1.0 ..
            input = input < 0.0 ? 0.0 : input > 1.0 ? 1.0 : input;

            this.topP = input;
            return this;
        }

        /// <summary>
        /// Sets the presence penalty value for the chat completion request and returns the updated builder instance.
        /// </summary>
        /// <remarks>
        /// The presence penalty influences the likelihood that the model will introduce new topics or deviate from prior context.
        /// Adjust this value to control the balance between repetition and novelty in generated responses.
        /// Higher values encourage the model to generate more novel content, while lower values may lead to more repetitive outputs.
        /// Values range from -2.0 to 2.0.
        /// </remarks>
        /// <param name="input">The presence penalty to apply. Higher values encourage the model to generate more novel responses. </param>
        /// <returns>The current <see cref="ClientRequestBuilder"/> instance with the updated presence penalty.</returns>
        public ClientRequestBuilder WithPresencePenalty(double input)
        {
            // Check within valid range: -2.0 - 2.0 ..
            input = input < -2.0 ? -2.0 : input > 2.0 ? 2.0 : input;

            // Set presence penalty ..
            this.presencePenalty = input;
            return this;
        }

        /// <summary>
        /// Sets the frequency penalty value to be applied to the chat completion request and returns the updated builder instance.
        /// </summary>
        /// <remarks>
        /// Use frequency penalty to control repetition in generated responses.
        /// Setting a higher penalty encourages more diverse output by penalizing repeated tokens.
        /// Values range from 0.0 (no penalty) to 2.0.
        /// </remarks>
        /// <param name="input">The frequency penalty to apply. Higher values decrease the likelihood of repeated tokens in the generated output.</param>
        /// <returns>The current <see cref="ClientRequestBuilder"/> instance with the specified frequency penalty
        /// applied.</returns>
        public ClientRequestBuilder WithFrequencyPenalty(double input)
        {
            // Check within valid range: 0.0 - 2.0 ..
            input = input < 0.0 ? 0.0 : input > 2.0 ? 2.0 : input;

            // Set frequency penalty ..
            this.frequencyPenalty = input;
            return this;
        }

        /// <summary>
        /// Force Output to be in JSON for the chat completion request.
        /// </summary>
        /// <param name="input">Set to <see langword="true"/> to force JSON output; otherwise, <see langword="false"/>.</param>
        /// <returns>The current <see cref="ClientRequestBuilder"/> instance with JSON mode enabled.</returns>
        public ClientRequestBuilder ForceJsonOutput(bool input)
        {
            this.jsonModeEnabled = input;
            return this;
        }

        // -----------------------------
        // Tool Calls
        // -----------------------------

        /// <summary>
        /// Adds a tool call to the chat completion request with the specified identifier, name, and arguments.
        /// </summary>
        /// <remarks>
        /// Use this method to include a tool call as part of the chat completion request. Multiple tool calls can be added by invoking this method
        /// multiple times. All parameters must be provided to ensure the tool call is correctly constructed.
        /// </remarks>
        /// <param name="id">The unique identifier for the tool call. Cannot be null or empty.</param>
        /// <param name="name">The name of the tool to invoke. Cannot be null or empty.</param>
        /// <param name="args">A dictionary containing the arguments to pass to the tool. Cannot be null.</param>
        /// <returns>The current <see cref="ClientRequestBuilder"/> instance, enabling method chaining.</returns>
        public ClientRequestBuilder AddToolCall(string id, string name, Dictionary<string, object> args)
        {
            this.toolCalls.Add(new ToolCall
            {
                Id = id,
                Name = name,
                Arguments = args,
            });

            return this;
        }

        /// <summary>
        /// Constructs a new instance of <see cref="ChatCompletionRequest"/> using the current builder settings.
        /// </summary>
        /// <remarks>
        /// Call this method after setting all desired parameters on the builder. The returned request can be used to initiate a chat completion operation.
        /// Subsequent changes to the builder do not affect the returned request.
        /// </remarks>
        /// <returns>A <see cref="ChatCompletionRequest"/> object populated with the configured model, messages, and parameter values.</returns>
        public ChatCompletionRequest Build()
        {
            // Combine all messages: developer messages first, then standard messages, then tool calls if any (as assistant messages) ..
            List<ChatMessage> allMessages =
            [

                // Developer messages first (if any) ..
                .. this.developerMessages,

                // Standard messages ..
                .. this.messages,
            ];

            // If JSON mode is enabled, add system message to enforce JSON-only responses ..
            if (this.jsonModeEnabled)
            {
                allMessages.Add(new ChatMessage
                                {
                                    RoleAsEnum = OpenAIRole.System,
                                    Content =
                                    "You MUST respond with a valid JSON object only. " +
                                    "No explanations, no prose, no markdown. " +
                                    "Return strictly valid JSON that matches the expected schema.",
                                });
            }

            // Tool Calls become Assistant Messages with tool_call content ..
            foreach (ToolCall tool in this.toolCalls)
            {
                allMessages.Add(new ChatMessage { RoleAsEnum = OpenAIRole.Assistant, Content = string.Empty, ToolCall = tool });
            }

            // Create and return the ChatCompletionRequest ..
            return new ChatCompletionRequest
            {
                Model = this.model,
                Messages = allMessages,
                Temperature = this.temperature,
                MaxTokens = this.maxTokens,
                TopP = this.topP,
                PresencePenalty = this.presencePenalty,
                FrequencyPenalty = this.frequencyPenalty,
                Stream = this.stream,
            };
        }
    }
}
