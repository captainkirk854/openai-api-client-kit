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

            // Read prompt from console ..
            Console.Write("Enter your prompt for OpenAI: ");
            string userPrompt = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(userPrompt))
            {
                Console.WriteLine("Prompt cannot be empty. Please try again.");
                return;
            }

            // Determine whether to use streaming or non-streaming mode ..
            Console.Write("Use streaming mode? (y/n) (n): ");
            string streamingChoice = Console.ReadLine() ?? "n";
            bool isStreaming = streamingChoice.Equals(value: "y", comparisonType: StringComparison.OrdinalIgnoreCase);

            // Create OpenAI Chat client instance ..
            ChatClient client = new(apiKey: apiKey);

            // Initialise a cancellation token with a timeout ..
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(TimeoutInSeconds));

            // Build request payload ..
            ChatCompletionRequest request = new ClientRequestBuilder().WithModel(input: OpenAIModels.GPT4o_Mini)
                                                                      .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
                                                                      .AddUserMessage(input: userPrompt)
                                                                      .EnableStreaming(input: isStreaming)
                                                                      .WithTemperature(input: 1.0)
                                                                      .WithMaxTokens(input: 1000)
                                                                      .WithTopP(input: 0.5)
                                                                      .WithPresencePenalty(input: 2.0)
                                                                      .WithFrequencyPenalty(input: 2.0)
                                                                      .Build();

            // Clear console for better readability ..
            Console.Clear();

            // Display the prompt ..
            Console.WriteLine($"User prompt: {userPrompt}");

            // If not streaming ..
            if (!isStreaming)
            {
                // Perform Non-streaming call ..
                Console.WriteLine("Non-Streaming ..");
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
                Console.WriteLine("Streaming ..");
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

            // Final newline for better console readability ..
            Console.WriteLine();
            Console.WriteLine("Press Enter to quit ..");
        }
    }
}