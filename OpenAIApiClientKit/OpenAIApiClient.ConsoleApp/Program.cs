// <copyright file="Program.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp
{
    using OpenAIApiClient.ConsoleApp.Demos;
    using OpenAIApiClient.Enums;

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

            // Run Best Model-Selection Demo first ..
            bool runBestModelSelectionDemo = SetBooleanPrompt(message: "Run Best Model Response Demo?", setTrue: 'y', setFalse: 'n');
            if (runBestModelSelectionDemo)
            {
                string prompt = "explain the theory of relativity in simple terms.";

                Console.Clear();
                Console.WriteLine("Running Demo ..");
                Console.WriteLine();
                Console.WriteLine($"Using Prompt: {prompt}");
                Console.WriteLine();

                // Get best model for the prompt ..
                await BestModelResponseDemo.GetBestModelResponseAsync(client: client, prompt: prompt, cts: cts);

                Console.WriteLine("Press Enter to continue..");
                Console.ReadLine();
            }
            else
            {
                // Run regular demo starting with whether to use streaming or non-streaming modes ..
                bool isStreaming = SetBooleanPrompt(message: "Use streaming mode?", setTrue: 'y', setFalse: 'n');

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
                    bool isDeterministic = SetBooleanPrompt(message: "Enable creativity settings?", setTrue: 'n', setFalse: 'y', setDefault: 'n');

                    // Prompt for output format ..
                    Console.WriteLine("Select output format:");
                    Console.WriteLine("1. PlainText");
                    Console.WriteLine("2. Markdown");
                    Console.WriteLine("3. Html");
                    Console.WriteLine("4. Json");
                    Console.WriteLine("5. Xml");
                    Console.WriteLine("6. Yaml");
                    Console.WriteLine("7. Sql");
                    Console.WriteLine("8. Tabular");
                    Console.WriteLine("Press Enter to select default (PlainText).");
                    Console.Write("Enter choice (1-8) [default is 1]: ");

                    // Read format choice ..
                    string? formatInput = Console.ReadLine();
                    OutputFormat outputFormatChoice = formatInput switch
                    {
                        "1" => OutputFormat.PlainText,
                        "2" => OutputFormat.Markdown,
                        "3" => OutputFormat.Html,
                        "4" => OutputFormat.Json,
                        "5" => OutputFormat.Xml,
                        "6" => OutputFormat.Yaml,
                        "7" => OutputFormat.Sql,
                        "8" => OutputFormat.Table,
                        _ => OutputFormat.PlainText,
                    };
                    Console.WriteLine($"Selected output format: {outputFormatChoice}");
                    Console.WriteLine();

                    // Process user prompt with additional options ..
                    await ModelPromptDemo.ProcessUserPromptAsync(client: client, isStreaming: isStreaming, userPrompt: userPrompt, isDeterministic: isDeterministic, outputFormat: outputFormatChoice, cts: cts);

                    // Ask if the user wants to enter another prompt ..
                    Console.WriteLine();
                    bool continuePrompting = SetBooleanPrompt(message: "Do you want to enter another prompt?", setTrue: 'y', setFalse: 'n');
                    if (!continuePrompting)
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
        /// Boolean Console Prompt helper method.
        /// </summary>
        /// <param name="message">Prompt message.</param>
        /// <param name="setTrue">Positive response.</param>
        /// <param name="setFalse">Negative response.</param>
        /// <param name="setDefault">Default response.</param>
        /// <returns><see cref="bool"/>.</returns>
        public static bool SetBooleanPrompt(string message, char setTrue, char setFalse, char setDefault = 'n')
        {
            // Prompt user ..
            Console.Write($"{message} ({setTrue}/{setFalse}) ({setDefault}) ");

            // Read input and determine choice ..
            string? input = Console.ReadLine();
            string choice = string.IsNullOrEmpty(input) ? setDefault.ToString() : input;

            // Return true for positive choice, false otherwise ..
            return choice.Equals(value: setTrue.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase);
        }
    }
}