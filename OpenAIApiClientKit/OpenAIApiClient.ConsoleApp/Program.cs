// <copyright file="Program.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp
{
    using OpenAIApiClient.Helpers;

    public class Program
    {
        public static async Task Main()
        {
            // Read API key from environment variable (e.g. setx OPENAI_API_KEY "your_api_key_here") in Windows Cmd) ..
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
            OpenAIChatClient client = new(apiKey: apiKey, model: OpenAIModels.GPT4o_Mini);

            // Non-streaming call ..
            Console.WriteLine("\nOpenAI non-streaming response:");
            string reply = await client.CreateChatCompletionAsync(prompt: promptInput);
            Console.WriteLine(reply);

            // Streaming call
            Console.WriteLine("\nOpenAI streaming reply:");
            int chunkIndex = 0;
            string replyStreamed = string.Empty;
            await foreach (string chunk in client.CallOpenAIStreamResponseAsync(prompt: promptInput))
            {
                chunkIndex++;
                Console.WriteLine($"Chunk index: [{chunkIndex}]; Chunk value: {chunk}");
                replyStreamed += chunk;
            }

            Console.WriteLine("\nFull streamed reply:");
            Console.WriteLine(replyStreamed);

            Console.WriteLine();
        }
    }
}