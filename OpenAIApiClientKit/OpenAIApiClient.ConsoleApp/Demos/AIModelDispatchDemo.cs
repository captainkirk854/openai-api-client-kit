// <copyright file="AIModelDispatchDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.Models;

    /// <summary>
    /// Single and Ensemble Model Dispatch Selection Demo.
    /// </summary>
    public static class AIModelDispatchDemo
    {
        private static readonly OpenAIModels Models = new();
        private static readonly SingleModelDispatcher SingleModelDispatcher = new(registry: Models);
        private static readonly EnsembleDispatcher EnsembleDispatcher = new(registry: Models);

        public static void Run()
        {
            Console.WriteLine("=== Model Dispatch and Selection Demo ===");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Explicit Model Selection");
            Console.WriteLine("2. Find Best Reasoning Model");
            Console.WriteLine("3. Find Lowest Cost Chat Model");
            Console.WriteLine("4. Find Best Vision Model");
            Console.WriteLine("5. Build Reasoning Ensemble");
            Console.WriteLine("6. Build Vision Ensemble");
            Console.WriteLine("7. Build Cost-Optimized Ensemble");
            Console.WriteLine("8. Build Custom Ensemble");
            Console.Write("Enter choice: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RunExplicitModelDispatch();
                    break;

                case "2":
                    RunBestReasoningModelDispatch();
                    break;

                case "3":
                    RunLowestCostChatModelDispatch();
                    break;

                case "4":
                    RunBestVisionModelDispatch();
                    break;

                case "5":
                    RunReasoningEnsembleModelDispatch();
                    break;

                case "6":
                    RunVisionEnsembleModelDispatch();
                    break;

                case "7":
                    RunCostOptimizedEnsembleModelDispatch();
                    break;

                case "8":
                    RunCustomEnsembleModelDispatch();
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        // -------------------------
        // Single Model Dispatch
        // -------------------------

        /// <summary>
        /// Run explicit model dispatch demo.
        /// </summary>
        private static void RunExplicitModelDispatch()
        {
            Console.WriteLine("Enter model name (e.g., GPT4o, GPT5_2):");
            Console.WriteLine("Options: " + string.Join(", ", Enum.GetNames(typeof(OpenAIModel))));
            string? input = Console.ReadLine();

            if (!Enum.TryParse<OpenAIModel>(input, out OpenAIModel model))
            {
                Console.WriteLine("Invalid model.");
                return;
            }

            SingleModelDispatchResult result = SingleModelDispatcher.Evaluate(new SingleModelDispatchRequest
            {
                Strategy = SingleModelStrategy.Explicit,
                ExplicitModel = model,
            });

            PrintSingleResult("Dispatch Result using Explicit Strategy and Model", result);
        }

        /// <summary>
        /// Run best reasoning model dispatch demo.
        /// </summary>
        private static void RunBestReasoningModelDispatch()
        {
            SingleModelDispatchResult result = SingleModelDispatcher.Evaluate(new SingleModelDispatchRequest
            {
                Strategy = SingleModelStrategy.BestReasoning,
            });

            PrintSingleResult("Dispatch Result for: Best Reasoning Model", result);
        }

        /// <summary>
        /// Run lowest cost chat model dispatch demo.
        /// </summary>
        private static void RunLowestCostChatModelDispatch()
        {
            SingleModelDispatchResult result = SingleModelDispatcher.Evaluate(new SingleModelDispatchRequest
            {
                Strategy = SingleModelStrategy.LowestCost,
                RequiredCapabilities = [ModelCapability.Chat],
            });

            PrintSingleResult("Dispatch Result for: Lowest Cost Chat Model", result);
        }

        /// <summary>
        /// Run best vision model dispatch demo.
        /// </summary>
        private static void RunBestVisionModelDispatch()
        {
            SingleModelDispatchResult result = SingleModelDispatcher.Evaluate(new SingleModelDispatchRequest
            {
                Strategy = SingleModelStrategy.BestVision,
            });

            PrintSingleResult("Dispatch Result for: Best Vision Model", result);
        }

        // -------------------------
        // Ensemble Routing
        // -------------------------

        /// <summary>
        /// Run reasoning ensemble dispatch demo.
        /// </summary>
        private static void RunReasoningEnsembleModelDispatch()
        {
            EnsembleDispatchResult result = EnsembleDispatcher.Evaluate(new EnsembleDispatchRequest
            {
                Strategy = EnsembleStrategy.Reasoning,
            });

            PrintEnsembleResult("Dispatch Result for: Reasoning Model Ensemble", result);
        }

        /// <summary>
        /// Run vision ensemble dispatch demo.
        /// </summary>
        private static void RunVisionEnsembleModelDispatch()
        {
            EnsembleDispatchResult result = EnsembleDispatcher.Evaluate(new EnsembleDispatchRequest
            {
                Strategy = EnsembleStrategy.Vision,
            });

            PrintEnsembleResult("Dispatch Result for: Vision Model Ensemble", result);
        }

        /// <summary>
        /// Run cost-optimized ensemble dispatch demo.
        /// </summary>
        private static void RunCostOptimizedEnsembleModelDispatch()
        {
            EnsembleDispatchResult result = EnsembleDispatcher.Evaluate(new EnsembleDispatchRequest
            {
                Strategy = EnsembleStrategy.CostOptimized,
            });

            PrintEnsembleResult("Dispatch Result for: Cost-Optimized Model Ensemble", result);
        }

        /// <summary>
        /// Run custom ensemble dispatch demo.
        /// </summary>
        private static void RunCustomEnsembleModelDispatch()
        {
            Console.WriteLine("Enter required capability(s) (comma-separated):");
            Console.WriteLine("Options: " + string.Join(", ", Enum.GetNames(typeof(ModelCapability))));

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("No capability(s) entered.");
                return;
            }

            List<string> rawParts = [.. input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())];

            List<ModelCapability> parsed = [];
            List<string> invalid = [];

            foreach (string part in rawParts)
            {
                if (Enum.TryParse(part, ignoreCase: true, out ModelCapability cap))
                {
                    parsed.Add(cap);
                }
                else
                {
                    invalid.Add(part);
                }
            }

            if (invalid.Count > 0)
            {
                Console.WriteLine($"Invalid capability(s): {string.Join(", ", invalid)}");
                return;
            }

            if (parsed.Count == 0)
            {
                Console.WriteLine("No valid capability(s) provided.");
                return;
            }

            Console.WriteLine("What's the minimum number of required model(s) to select?");
            string? countInput = Console.ReadLine();
            if (!int.TryParse(countInput, out int minRequiredCount) || minRequiredCount <= 0)
            {
                Console.WriteLine("Invalid number entered.");
                return;
            }

            // Validate that enough models exist BEFORE routing ..
            List<ModelDescriptor> matchingModels = [.. Models.GetRegistry().Values.Where(m => parsed.All(c => m.Capabilities.Contains(c)))];
            if (matchingModels.Count < minRequiredCount)
            {
                Console.WriteLine($"Only {matchingModels.Count} model(s) match those capabilities, but a minimum of {minRequiredCount} model(s) were requested.");
                return;
            }

            // Proceed with evaluation of which models are selected ..
            EnsembleDispatchResult result = EnsembleDispatcher.Evaluate(new EnsembleDispatchRequest
            {
                Strategy = EnsembleStrategy.Custom,
                ModelCount = minRequiredCount,
                RequiredCapabilities = parsed,
            });

            PrintEnsembleResult("Custom Ensemble", result);
        }

        // -------------------------
        // Output Helpers
        // -------------------------

        /// <summary>
        /// Print single model routing result.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="result"></param>
        private static void PrintSingleResult(string title, SingleModelDispatchResult result)
        {
            Console.WriteLine($"\n=== {title} ===");
            Console.WriteLine($"Model:     {result.Model.Name}");
            Console.WriteLine($"Capabilities: {string.Join(", ", result.Model.Capabilities)}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print ensemble routing result.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="result"></param>
        private static void PrintEnsembleResult(string title, EnsembleDispatchResult result)
        {
            Console.WriteLine($"\n=== {title} ===");

            foreach (ModelDescriptor model in result.Models.Distinct())
            {
                Console.WriteLine($"Model:     {model.Name}");
                Console.WriteLine($"Capabilities: {string.Join(", ", model.Capabilities)}");
                Console.WriteLine();
            }
        }
    }
}
