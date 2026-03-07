// <copyright file="Program.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp
{
    using System;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.Extensions;
    using OpenAIApiClient.Models.Registries.AiModels;

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
                Console.WriteLine("1. AI Model Simple Chat Prompt Demo with Streaming and Non-Streaming Modes");
                Console.WriteLine("2. AI Model Strategy Dispatch Demo with Multiple Dispatch Strategies");
                Console.WriteLine("3. AI Model Orchestrator Demo with Single-Model and Multi-Model Strategies");
                Console.WriteLine("4. AI Simple Get Best Model Response Demo using Heuristic Consolidation");
                Console.WriteLine("5. AI Advanced Get Best Model Response Demo using Advanced Consolidation");

                Console.Write("Enter choice (1-5): ");
                string? demoChoice = Console.ReadLine();
                Console.WriteLine();

                switch (demoChoice)
                {
                    case "1":
                        await AiModelSimpleChatClientDemo(client: client, cts: cts);
                        break;

                    case "2":
                        AiModelDispatchDemo();
                        break;

                    case "3":
                        await AiModelOrchestratorDemo(client: client, cts: cts);
                        break;

                    case "4":
                        await AiSimpleGetModelBestResponseDemo(client: client, cts: cts);
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
                bool continuePrompting = SetBooleanPrompt(message: "Do you want to try again?", setTrue: 'y', setFalse: 'n', setDefault: 'y');
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
        /// A demo implementation to process a user prompt by sending it to the chat client and displaying the response, supporting both streaming and non-streaming modes.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task AiModelSimpleChatClientDemo(ChatClient client, CancellationTokenSource cts)
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

            Console.WriteLine("Which OpenAI model would you like to use?");
            Console.WriteLine("1. GPT-4o-Mini");
            Console.WriteLine("2. GPT-3.5-Turbo");
            Console.WriteLine("3. GPT-4o");
            Console.WriteLine("4. GPT-5-Mini");
            Console.WriteLine("5. GPT-5");
            Console.Write("Enter choice (1-5) [default is 1]: ");

            string? modelInput = Console.ReadLine();

            string selectedModelName = modelInput switch
            {
                "1" => "gpt-4o-mini",
                "2" => "gpt-3.5-turbo",
                "3" => "gpt-4o",
                "4" => "gpt-5-mini",
                "5" => "gpt-5",
                _ => "gpt-4o-mini",
            };

            // KEEP THIS ... NEEDS FIXING >..
            //// -------------------------------------------------------
            //// Resolve via the NEW registry.
            //AiModelPropertyRegistryModel? selectedModelInfo = ModelRegistry.TryGetByName(selectedModelName);

            //if (selectedModelInfo is null)
            //{
            //    throw new InvalidOperationException(
            //        $"The selected model '{selectedModelName}' could not be resolved using the configured model registry.");
            //}

            // If you only need the string id:
            //string selectedModelId = selectedModelInfo.Name;
            //// -------------------------------------------------------

            string selectedModel = selectedModelName;

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
            await Demos.AiModelSimpleChatClientDemo.ProcessUserPromptAsync(client: client,
                                                                           isStreaming: isStreaming,
                                                                           userPrompt: userPrompt,
                                                                           isDeterministic: isDeterministic,
                                                                           outputFormat: outputFormatChoice,
                                                                           cts: cts,
                                                                           model: selectedModel);
        }

        /// <summary>
        /// A demo implementation to showcase AI model strategy dispatch capabilities.
        /// </summary>
        private static void AiModelDispatchDemo()
        {
            Demos.AiModelDispatchDemo.Run();
        }

        /// <summary>
        /// A demo implementation to showcase AI orchestration capabilities with single-model and multi-model strategies.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task AiModelOrchestratorDemo(ChatClient client, CancellationTokenSource cts)
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
        /// A demo implementation to showcase AI orchestration capabilities with multi-model consolidation to get the best response using a simple heuristic approach.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task AiSimpleGetModelBestResponseDemo(ChatClient client, CancellationTokenSource cts)
        {
            string prompt = "explain the theory of relativity in simple terms.";
            Console.WriteLine($"Using Prompt: {prompt}");
            Console.WriteLine();

            // Get best model response for the prompt ..
            await Demos.AiSimpleEnsembleConsolidationDemo.GetBestHeuristicModelResponseAsync(client: client, prompt: prompt, cts: cts);

            Console.WriteLine("Press Enter to continue..");
            Console.ReadLine();
        }

        /// <summary>
        /// A demo implementation to showcase AI orchestration capabilities with multi-model consolidation to get the best response using a more advanced approach leveraging multiple factors and criteria for evaluation.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="cts"></param>
        /// <returns>Task.</returns>
        private static async Task AiAdvancedGetBestModelResponseDemo(ChatClient client, CancellationTokenSource cts)
        {
            string prompt = @"Explain Mahalanobis distance in simple terms suitable for a high school student.
                              Focus on: what it is, why it matters, and its application in maths and statistics.
                              If possible, include any examples of uses in fraud detection.";

            Console.WriteLine($"Using Prompt: {prompt}");
            Console.WriteLine();

            Console.WriteLine("This demo will use multiple OpenAI models to generate responses to the same prompt and then apply an advanced consolidation strategy ");
            Console.WriteLine("to evaluate and determine the best response based on multiple criteria such as relevance, coherence, creativity, and informativeness.");

            // Prompt user for streaming and reasoning options ..
            bool useStreaming = SetBooleanPrompt(message: "Use streaming mode for this demo? (y/n)", setTrue: 'y', setFalse: 'n');
            bool useReasoning = SetBooleanPrompt(message: "Use reasoning model(s) for this demo? (y/n)", setTrue: 'y', setFalse: 'n');

            // Determine call mode based on streaming choice ..
            AiCallMode callMode = useStreaming ? AiCallMode.BufferedStreaming : AiCallMode.NonStreaming;


            var registry = Registries.AiModels.Factories.OpenAIModelsFactory.Create();

            // Initialise a list of model(s) to dispatch the prompt to; can be any combination of models based on caller's needs and preferences ..
            string[] workerModels;
            if (useReasoning)
            {
                workerModels = EnsembleStrategy.ReasoningNEW.GetModelNames(registry);
            }
            else
            {
                // Define an arbitrary list of models to use as workers for response generation; in a real application, this could be based on user selection, specific model capabilities, or other criteria ..
                workerModels =
                [
                    "gpt-5.2",
                    "gpt-4o",
                    "gpt-4.1-mini",
                    "o4-mini",
                ];
            }

            // Define randomly-picked models to use for LLM-as-judge and response synthesis strategies; models should have good reasoning capabilities for best results ..
            string[] reasoningModels = EnsembleStrategy.ReasoningNEW.GetModelNames(registry);
            string judgeModel = reasoningModels[Random.Shared.Next(reasoningModels.Length)];
            string synthesisModel = reasoningModels[Random.Shared.Next(reasoningModels.Length)];

            // Run demo ..
            await Demos.AiAdvancedEnsembleConsolidationDemo.GetAdvancedResponsesAsync(client: client,
                                                                                      prompt: prompt,
                                                                                      workers: workerModels,
                                                                                      judge: judgeModel,
                                                                                      synthesiser: synthesisModel,
                                                                                      callMode: callMode,
                                                                                      cts: cts);

            Console.WriteLine();
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
            Console.Write($"{message} (default is {setDefault}) ({setTrue}/{setFalse}) ({setDefault}) ");

            // Read input and determine choice ..
            string? input = Console.ReadLine();
            string choice = string.IsNullOrEmpty(input) ? setDefault.ToString() : input;

            // Return true for positive choice, false otherwise ..
            return choice.Equals(value: setTrue.ToString(), comparisonType: StringComparison.OrdinalIgnoreCase);
        }
    }
}