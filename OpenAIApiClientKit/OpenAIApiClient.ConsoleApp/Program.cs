// <copyright file="Program.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp
{
    using OpenAIApiClient.Demos;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
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
            ChatClient client = new(apiKey: apiKey, maxRetries: 5, baseDelayMs: 1000);

            // Initialise a cancellation token with a timeout ..
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(TimeoutInSeconds));

            // Run Model-Selection Demo first ..
            Console.WriteLine("Run Simple Model Selection Demo? (y/n) (n)");
            bool runOptimalModelSelectionDemo = Console.ReadLine()?.Trim().Equals(value: "y", comparisonType: StringComparison.OrdinalIgnoreCase) ?? false;
            if (runOptimalModelSelectionDemo)
            {
                string prompt = "explain the theory of relativity in simple terms.";
                Console.Clear();
                Console.WriteLine("Running Simple Optimal Model Selection Demo ..");
                Console.WriteLine();
                Console.WriteLine($"Using Prompt: {prompt}");
                await SimpleOpenAIModelSelectionDemo.RunAsync(client: client, prompt: prompt, cts: cts);
                Console.WriteLine("Press Enter to continue..");
                Console.ReadLine();
            }
            else
            {
                // Run regular demo starting with whether to use streaming or non-streaming mode ..
                Console.Write("Use streaming mode? (y/n) (n): ");
                string streamingChoice = Console.ReadLine() ?? "n";
                bool isStreaming = streamingChoice.Equals(value: "y", comparisonType: StringComparison.OrdinalIgnoreCase);

                // Clear console for better readability ..
                Console.Clear();

                // Loop to allow multiple prompts ..
                while (true)
                {
                    // Reset cancellation token ..
                    cts.TryReset();

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

                    // Ask to force JSON mode ..
                    Console.Write("Enable forced JSON output? (y/n) (n): ");
                    string jsonChoice = Console.ReadLine() == string.Empty ? "n" : "y";
                    bool isJson = jsonChoice.Equals(value: "y", comparisonType: StringComparison.OrdinalIgnoreCase);

                    // Process user prompt with additional options ..
                    await ProcessUserPromptAsync(client: client, isStreaming: isStreaming, userPrompt: userPrompt, isDeterministic: isDeterministic, isJson: isJson, cts: cts);

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
        /// <param name="isDeterministic">true to use deterministic output parameters for the chat completion request; otherwise, false to use
        /// non-deterministic parameters.</param>
        /// <param name="isJson">true to enable JSON mode for the chat completion request; otherwise, false.</param>
        /// <param name="cts">A CancellationTokenSource used to observe cancellation requests for the operation. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task ProcessUserPromptAsync(ChatClient client, bool isStreaming, string userPrompt, bool isDeterministic, bool isJson, CancellationTokenSource cts)
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
            ChatCompletionRequest request = new ClientRequestBuilder().WithModel(input: OpenAIModels.GPT4o_Mini)
                                                                      .AddSystemMessage(input: "You are a helpful assistant that answers concisely.")
                                                                      .AddUserMessage(input: userPrompt)
                                                                      .UsingMaxTokens(input: 1000)
                                                                      .EnableStreaming(input: isStreaming)
                                                                      .WithTemperature(input: temperature)
                                                                      .WithTopP(input: topP)
                                                                      .WithPresencePenalty(input: presencePenalty)
                                                                      .WithFrequencyPenalty(input: frequencyPenalty)
                                                                      .ForceJsonOutput(input: isJson)
                                                                      .Build();

            // If not streaming ..
            if (!isStreaming)
            {
                // Perform Non-streaming Call ..
                Console.WriteLine("Waiting for Non-Streaming Response ..");
                try
                {
                    string? content = await ChatClientHelpers.GetChatCompletionNonStreamingMessageContentAsync(client: client, request: request, cancelTokenSource: cts);
                    Console.WriteLine();
                    Console.WriteLine(content);
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
                    Console.WriteLine();
                    Console.WriteLine(content);
                    Console.WriteLine($"(Total Chunk(s) received: {chunkCount})");
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
    }
}