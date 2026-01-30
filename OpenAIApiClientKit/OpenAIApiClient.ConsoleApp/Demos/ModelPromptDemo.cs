// <copyright file="ModelPromptDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using OpenAIApiClient.Interfaces.Validators;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Registries;

    /// <summary>
    /// Console App Demo to demonstrate implementation example for processing user prompts with various models.
    /// </summary>
    public static class ModelPromptDemo
    {
        /// <summary>
        /// Processes a user prompt by sending it to the chat client and displaying the response, supporting both streaming and non-streaming modes.
        /// </summary>
        /// <remarks>
        /// If the operation is cancelled via the provided CancellationTokenSource, a cancellation message is displayed. The method writes the user prompt
        /// and the response to the console. Deterministic or non-deterministic parameters are applied to the chat completion request based on the value
        /// of: isDeterminismEnabled.
        /// </remarks>
        /// <param name="client">The chat client used to send the prompt and receive the response.</param>
        /// <param name="isStreaming">true to enable streaming responses; otherwise, false for non-streaming responses.</param>
        /// <param name="userPrompt">The user input prompt to be sent to the chat client. Cannot be null.</param>
        /// <param name="isDeterministic">true to use deterministic output parameters for the chat completion request; otherwise, false to use non-deterministic parameters.</param>
        /// <param name="outputFormat">The desired output format for the chat completion response.</param>
        /// <param name="cts">A CancellationTokenSource used to observe cancellation requests for the operation. Cannot be null.</param>
        /// <param name="model">The OpenAI model to use for the chat completion request. Defaults to GPT4o_Mini.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task ProcessUserPromptAsync(ChatClient client, bool isStreaming, string userPrompt, bool isDeterministic, OutputFormat outputFormat, CancellationTokenSource cts, OpenAIModel model = OpenAIModel.GPT4o_Mini)
        {
            // Set deterministic output parameters ..
            double temperature = 0.0;
            double topP = 0.0;
            double presencePenalty = -2.0;
            double frequencyPenalty = 0.0;
            if (!isDeterministic)
            {
                temperature = 2.0;
                topP = 0.99; // Note: using 1.0 can lead to corrupted output in some cases ..
                presencePenalty = 2.0;
                frequencyPenalty = 2.0;
            }

            // Build request payload ..
            ChatCompletionRequest request = new ClientRequestBuilder()
                .WithModel(input: model)
                .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
                .AddUserMessage(input: userPrompt)
                .UsingMaxTokens(input: 1000)
                .EnableStreaming(input: isStreaming)
                .WithTemperature(input: temperature)
                .WithTopP(input: topP)
                .WithPresencePenalty(input: presencePenalty)
                .WithFrequencyPenalty(input: frequencyPenalty)
                .SetOutputFormat(input: outputFormat)
                .Build();

            Console.WriteLine($"Using Model: {model}");

            // If not streaming ..
            if (!isStreaming)
            {
                // Perform Non-streaming Call ..
                Console.WriteLine("Waiting for Non-Streaming Response ..");
                try
                {
                    // Get response content ..
                    string? content = await ChatClientHelpers.GetChatCompletionNonStreamingMessageContentAsync(client: client, request: request, cancelTokenSource: cts);

                    // Validate response format ..
                    if(!content.IsValidFormat(outputFormat: outputFormat))
                    {
                        return;
                    }
                    else
                    {
                        // Output response content ..
                        Console.WriteLine();
                        Console.WriteLine(content);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Non-Streaming Request was cancelled.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                // Perform Streaming Call ..
                Console.WriteLine("Waiting for Streaming Response ..");
                try
                {
                    (string?, int) response = await ChatClientHelpers.GetChatCompletionStreamingMessageContentAsync(client: client, request: request, cancelTokenSource: cts);
                    string content = response.Item1 ?? string.Empty;
                    int chunkCount = response.Item2;

                    // Validate response format ..
                    if (!content.IsValidFormat(outputFormat: outputFormat))
                    {
                        return;
                    }
                    else
                    {
                        // Output response content ..
                        Console.WriteLine();
                        Console.WriteLine(content);
                        Console.WriteLine($"(Total Chunk(s) received: {chunkCount})");
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("Streaming request was cancelled.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Helper method to validate if the content matches the expected output format.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="outputFormat"></param>
        private static bool IsValidFormat(this string? content, OutputFormat outputFormat)
        {
            IOutputFormatValidator validator = OutputFormatRegistry.Prompts[outputFormat].Validator;
            if (!validator.IsValidFormat(content: content ?? string.Empty, out string? error))
            {
                Console.WriteLine();
                Console.WriteLine($"Warning: The response format is invalid based on the {error}.");
                return false;
            }

            return true;
        }
    }
}
