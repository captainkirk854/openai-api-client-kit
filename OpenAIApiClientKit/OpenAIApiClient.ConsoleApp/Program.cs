// <copyright file="Program.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp
{
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Models.Chat.Request;
    using OpenAIApiClient.Models.Chat.Response.Completion;
    using OpenAIApiClient.Models.Chat.Response.Streaming;

    public class Program
    {
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
            string promptInput = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(promptInput))
            {
                Console.WriteLine("Prompt cannot be empty. Please try again.");
                return;
            }

            // Initialize OpenAI client ..
            OpenAIChatClient client = new(apiKey: apiKey);

            // Set a cancellation token with a timeout ..
            using CancellationTokenSource cancellation = new(TimeSpan.FromSeconds(30));

            // Build the request payload ..
            ChatCompletionRequest request = OpenAIPayloadHelper.BuildChatCompletionRequestObject(model: OpenAIModels.GPT4o_Mini, userPrompt: promptInput, stream: false);

            // Perform Non-streaming call ..
            Console.WriteLine("\nOpenAI non-streaming response:");
            try
            {
                ChatCompletionResponse? reply = await client.CreateChatCompletionAsync(request: request, cancellationToken: cancellation.Token);
                Console.WriteLine(reply?.Choices[0].Message.Content);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The request was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            // Perform Streaming call
            Console.WriteLine("\nOpenAI streaming reply:");
            int chunkIndex = 0;
            string replyStreamed = string.Empty;

            // Build the request payload ..
            request = OpenAIPayloadHelper.BuildChatCompletionRequestObject(model: OpenAIModels.GPT4o_Mini, userPrompt: promptInput, stream: true, temperature: 0.75);

            // Perform Streaming call ..
            try
            {
                // Stream the response chunks ..
                await foreach (ChatCompletionChunk chunk in client.CreateChatCompletionStreamAsync(request: request, cancellationToken: cancellation.Token))
                {
                    chunkIndex++;

                    // Extract delta content from the chunk ..
                    ChatDelta chunkDelta = chunk.Choices[0].Delta;
                    if (!string.IsNullOrEmpty(chunkDelta.Content))
                    {
                        Console.WriteLine($"Chunk index: [{chunkIndex}]; Chunk value: {chunkDelta.Content}");
                    }

                    replyStreamed += chunkDelta.Content;
                }

                Console.WriteLine();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("The streaming request was cancelled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nFull streamed reply:");
            Console.WriteLine(replyStreamed);
        }
    }
}