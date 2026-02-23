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
            using CancellationTokenSource cts = new(delay: TimeSpan.FromSeconds(TimeoutInSeconds));

            // Main application loop ..
            while (true)
            {
                // Reset cancellation token ..
                cts.TryReset();

                // Allow user to choose which demo to run ..
                Console.WriteLine("Welcome to the OpenAI API Client Demo Application!");
                Console.WriteLine("Which demo would you like to run?");
                Console.WriteLine("1. AI Model Prompt Demo");
                Console.WriteLine("2. AI Model Strategy Dispatch Demo");
                Console.WriteLine("3. AI Model Best Response Demo");
                Console.WriteLine("4. AI Model Orchestrator Demo");
                Console.WriteLine("5. AI Advanced Get Best Model Response Demo");

                Console.Write("Enter choice (1-5): ");
                string? demoChoice = Console.ReadLine();
                Console.WriteLine();

                switch (demoChoice)
                {
                    case "1":
                        await AIModelChatClientDemo(client: client, cts: cts);
                        break;

                    case "2":
                        AIModelDispatchDemo();
                        break;

                    case "3":
                        await AIModelBestResponseDemo(client: client, cts: cts);
                        break;

                    case "4":
                        await AIModelOrchestratorDemo(client: client, cts: cts);
                        break;

                    case "5":
                        await AiAdvancedGetBestModelResponseDemo(client: client, cts: cts);
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
        private static async Task AIModelBestResponseDemo(ChatClient client, CancellationTokenSource cts)
        {
            string prompt = "explain the theory of relativity in simple terms.";
            Console.WriteLine($"Using Prompt: {prompt}");
            Console.WriteLine();

            // Get best model response for the prompt ..
            await Demos.AiModelBestResponseDemo.GetBestModelResponseAsync(client: client, prompt: prompt, cts: cts);

            Console.WriteLine("Press Enter to continue..");
            Console.ReadLine();
        }

        /// <summary>
        /// A demo implementation to process user prompts with various options.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task AIModelChatClientDemo(ChatClient client, CancellationTokenSource cts)
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

            // Prompt user for model selection ..
            Console.WriteLine("Which OpenAI model would you like to use?");
            Console.WriteLine("1. GPT-4o-Mini");
            Console.WriteLine("2. GPT-3.5-Turbo");
            Console.WriteLine("3. GPT-4o");
            Console.WriteLine("4. GPT-5-Mini");
            Console.WriteLine("5. GPT-5");
            Console.Write("Enter choice (1-5) [default is 1]: ");
            string? modelInput = Console.ReadLine();
            OpenAIModel selectedModel = modelInput switch
            {
                "1" => OpenAIModel.GPT4o_Mini,
                "2" => OpenAIModel.GPT3_5_Turbo,
                "3" => OpenAIModel.GPT4o,
                "4" => OpenAIModel.GPT5_Mini,
                "5" => OpenAIModel.GPT5,
                _ => OpenAIModel.GPT4o_Mini,
            };

            // Ask if creativity settings should be enabled ..
            bool isDeterministic = SetBooleanPrompt(message: "Enable Creativity Settings?", setTrue: 'n', setFalse: 'y', setDefault: 'n');

            // Prompt for output format ..
            Console.WriteLine("Select Output Format:");
            Console.WriteLine("1. PlainText");
            Console.WriteLine("2. Markdown");
            Console.WriteLine("3. Html");
            Console.WriteLine("4. Json");
            Console.WriteLine("5. Xml");
            Console.WriteLine("6. Yaml");
            Console.WriteLine("7. Sql");
            Console.WriteLine("8. Tabular");
            Console.WriteLine("Press Enter to select default (PlainText).");
            Console.Write("Enter Choice (1-8) [default=1]: ");

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
            await Demos.AiModelChatClientDemo.ProcessUserPromptAsync(client: client,
                                                                     isStreaming: isStreaming,
                                                                     userPrompt: userPrompt,
                                                                     isDeterministic: isDeterministic,
                                                                     outputFormat: outputFormatChoice,
                                                                     cts: cts,
                                                                     model: selectedModel);
        }

        /// <summary>
        /// A demo implementation to showcase model routing capabilities.
        /// </summary>
        private static void AIModelDispatchDemo()
        {
            Demos.AiModelDispatchDemo.Run();
        }

        /// <summary>
        /// A demo implementation to showcase AI orchestration capabilities.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task AIModelOrchestratorDemo(ChatClient client, CancellationTokenSource cts)
        {
            string prompt = @"List the planets, dwarf planets and top 10 heaviest moons.
                              List their names along with their masses and diameters in 
                              descending order of mass with the heaviest celestial body first.";
            Console.WriteLine($"Using Prompt: {prompt}");
            Console.WriteLine();

            // Run AI Orchestrator demo ..
            await Demos.AiModelOrchestratorDemo.RunAsync(client: client, prompt: prompt, cancelToken: cts.Token);

            Console.WriteLine("Press Enter to continue..");
            Console.ReadLine();
        }

        /// <summary>
        /// A demo implementation to showcase AI orchestration capabilities with multi-model consolidation to get the best response.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task AiAdvancedGetBestModelResponseDemo(ChatClient client, CancellationTokenSource cts)
        {
            string prompt = @"Explain Mahalanobis distance in simple terms suitable for a high school student.
        //Focus on: what it is, why it matters, and its application in maths and statistics.";

            Console.WriteLine($"Using Prompt: {prompt}");
            Console.WriteLine();

            // Run AI Orchestrator demo ..
            await Demos.AiAdvancedEnsembleConsolidationDemo.GetBestModelResponseAsync(client: client, prompt: prompt, cts: cts);

            Console.WriteLine("Press Enter to continue..");
            Console.ReadLine();
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