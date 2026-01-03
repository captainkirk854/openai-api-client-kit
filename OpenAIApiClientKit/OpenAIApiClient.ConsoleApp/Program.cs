// <copyright file="Program.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Models.Chat.Request;

    public class Program
    {
        private const int TimeoutInSeconds = 30;

        public static async Task Main()
        {
            // Read API key from pre-defined environment variable (e.g. In Windows Cmd, use: setx OPENAI_API_KEY "your_api_key_here") ..
            string? apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                Console.WriteLine("Please set the OPENAI_API_KEY environment variable and try again.");
                return;
            }

            // Create OpenAI Chat client instance ..
            ChatClient client = new(apiKey: apiKey);

            // Determine whether to use streaming or non-streaming mode ..
            Console.Write("Use streaming mode? (y/n) (n): ");
            string streamingChoice = Console.ReadLine() ?? "n";
            bool isStreaming = streamingChoice.Equals(value: "y", comparisonType: StringComparison.OrdinalIgnoreCase);

            // Clear console for better readability ..
            Console.Clear();

            // Loop to allow multiple prompts ..
            while (true)
            {
                // Initialise a cancellation token with a timeout ..
                using CancellationTokenSource cts = new(TimeSpan.FromSeconds(TimeoutInSeconds));

                // Read prompt from console ..
                Console.Write("Enter your prompt for OpenAI: ");
                string userPrompt = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(userPrompt))
                {
                    Console.WriteLine("Prompt cannot be empty. Please try again.");
                    return;
                }

                // Ask if creativity settings should be enabled ..
                Console.Write("Enable creativity settings? (y/n) (n): ");
                string creativityChoice = Console.ReadLine() == string.Empty ? "n" : "y";
                bool isDeterministic = creativityChoice.Equals(value: "n", comparisonType: StringComparison.OrdinalIgnoreCase);

                // Process user prompt ..
                await ProcessUserPromptAsync(client: client, isStreaming: isStreaming, userPrompt: userPrompt, isDeterminismEnabled: isDeterministic, cts: cts);

                // Ask if the user wants to enter another prompt ..
                Console.WriteLine();
                Console.Write("Do you want to enter another prompt? (y/n) (n): ");
                string continueChoice = Console.ReadLine() ?? "n";
                if (!continueChoice.Equals(value: "y", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            // Final newline for better console readability ..
            Console.WriteLine();
            Console.WriteLine("Exiting application. Goodbye!");
        }

        /// <summary>
        /// Processes a user prompt by sending it to the chat client and displaying the response, supporting both streaming and non-streaming modes.
        /// </summary>
        /// <remarks>
        /// If the operation is cancelled via the provided CancellationTokenSource, a
        /// cancellation message is displayed. The method writes the user prompt and the response to the console.
        /// Deterministic or non-deterministic parameters are applied to the chat completion request based on the value yof isDeterminismEnabled.
        /// </remarks>
        /// <param name="client">The chat client used to send the prompt and receive the response.</param>
        /// <param name="isStreaming">true to enable streaming responses; otherwise, false for non-streaming responses.</param>
        /// <param name="userPrompt">The user input prompt to be sent to the chat client. Cannot be null.</param>
        /// <param name="isDeterminismEnabled">true to use deterministic output parameters for the chat completion request; otherwise, false to use
        /// non-deterministic parameters.</param>
        /// <param name="cts">A CancellationTokenSource used to observe cancellation requests for the operation. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task ProcessUserPromptAsync(ChatClient client, bool isStreaming, string userPrompt, bool isDeterminismEnabled, CancellationTokenSource cts)
        {
            // Set deterministic output parameters ..
            double temperature = 0.0;
            double topP = 0.0;
            double presencePenalty = -2.0;
            double frequencyPenalty = 0.0;
            if (!isDeterminismEnabled)
            {
                temperature = 2.0;
                topP = 1.0;
                presencePenalty = 2.0;
                frequencyPenalty = 2.0;
            }

            // Build request payload ..
            ChatCompletionRequest request = new ClientRequestBuilder().WithModel(input: OpenAIModels.GPT4o_Mini)
                                                                      .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
                                                                      .AddUserMessage(input: userPrompt)
                                                                      .UsingMaxTokens(input: 1000)
                                                                      .EnableStreaming(input: isStreaming)
                                                                      .WithTemperature(input: temperature)
                                                                      .WithTopP(input: topP)
                                                                      .WithPresencePenalty(input: presencePenalty)
                                                                      .WithFrequencyPenalty(input: frequencyPenalty)
                                                                      .Build();

            // Display the prompt ..
            Console.WriteLine($"User prompt: {userPrompt}");

            // If not streaming ..
            if (!isStreaming)
            {
                // Perform Non-streaming call ..
                Console.Write("Non-Streaming ");
                try
                {
                    string? content = await ChatClientHelpers.GetChatCompletionNonStreamingMessageContentAsync(client: client, request: request, cancelTokenSource: cts);
                    Console.WriteLine("Response:");
                    Console.WriteLine(content);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("The request was cancelled.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                // Perform Streaming call ..
                Console.Write("Streaming ");
                try
                {
                    // Stream the response chunk(s) ..
                    string? content = await ChatClientHelpers.GetChatCompletionStreamingMessageContentAsync(client: client, request: request, cancelTokenSource: cts);
                    Console.WriteLine("Response:");
                    Console.WriteLine(content);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("The streaming request was cancelled.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}