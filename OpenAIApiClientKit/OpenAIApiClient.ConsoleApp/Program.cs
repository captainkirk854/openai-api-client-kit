// <copyright file="Program.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp
{
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

            // Initialise OpenAI Chat client instance ..
            ChatClient client = new(apiKey: apiKey, maxRetries: 5, baseDelayMs: 1000);

            // Initialise a cancellation token with a timeout ..
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(TimeoutInSeconds));

            // Main application loop ..
            while (true)
            {
                // Reset cancellation token ..
                cts.TryReset();

                // Allow user to choose which demo to run ..
                Console.WriteLine("Welcome to the OpenAI API Client Demo Application!");
                Console.WriteLine("Which demo would you like to run?");
                Console.WriteLine("1. Model Prompt Demo");
                Console.WriteLine("2. Best Model Response Demo");
                Console.WriteLine("3. Model Routing Demo");

                Console.Write("Enter choice (1-3): ");
                string? demoChoice = Console.ReadLine();
                Console.WriteLine();

                switch (demoChoice)
                {
                    case "1":
                        await ModelPromptDemo(client: client, cts: cts);
                        break;

                    case "2":
                        await BestModelResponseDemo(client: client, cts: cts);
                        break;

                    case "3":
                        ModelRoutingDemo();
                        break;

                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }

                // Prompt to continue or exit ..
                Console.WriteLine();
                bool continuePrompting = SetBooleanPrompt(message: "Do you want to try again?", setTrue: 'y', setFalse: 'n');
                if (!continuePrompting)
                {
                    break;
                }

                // Clear console for better readability ..
                Console.Clear();
            }

            // Final newline for better console readability ..
            Console.WriteLine();
            Console.WriteLine("Exiting application. Goodbye!");
        }

        /// <summary>
        /// A demo implementation to get the best model response for a given prompt.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task BestModelResponseDemo(ChatClient client, CancellationTokenSource cts)
        {
            string prompt = "explain the theory of relativity in simple terms.";
            Console.WriteLine($"Using Prompt: {prompt}");
            Console.WriteLine();

            // Get best model response for the prompt ..
            await Demos.BestModelResponseDemo.GetBestModelResponseAsync(client: client, prompt: prompt, cts: cts);

            Console.WriteLine("Press Enter to continue..");
            Console.ReadLine();
        }

        /// <summary>
        /// A demo implementation to process user prompts with various options.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task ModelPromptDemo(ChatClient client, CancellationTokenSource cts)
        {
            // Run regular demo starting with whether to use streaming or non-streaming modes ..
            bool isStreaming = SetBooleanPrompt(message: "Use streaming mode?", setTrue: 'y', setFalse: 'n');

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
            await Demos.ModelPromptDemo.ProcessUserPromptAsync(client: client, isStreaming: isStreaming, userPrompt: userPrompt, isDeterministic: isDeterministic, outputFormat: outputFormatChoice, cts: cts);
        }

        private static void ModelRoutingDemo()
        {
            Demos.ModelRoutingDemo.Run();
        }

        /// <summary>
        /// Helper Method Console Prompt helper method.
        /// </summary>
        /// <param name="message">Prompt message.</param>
        /// <param name="setTrue">Positive response.</param>
        /// <param name="setFalse">Negative response.</param>
        /// <param name="setDefault">Default response.</param>
        /// <returns><see cref="bool"/>.</returns>
        private static bool SetBooleanPrompt(string message, char setTrue, char setFalse, char setDefault = 'n')
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