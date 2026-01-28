// <copyright file="ModelRoutingDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries;
    using OpenAIApiClient.Routing.Ensemble;
    using OpenAIApiClient.Routing.SingleModel;

    /// <summary>
    /// Ensemble and Single Model Routing Demo.
    /// </summary>
    public static class ModelRoutingDemo
    {
        private static readonly OpenAIModelRegistry ModelRegistry = new();
        private static readonly SingleModelRouter SingleRouter = new(modelRegistry: ModelRegistry.Registry);
        private static readonly EnsembleRouter EnsembleRouter = new(modelRegistry: ModelRegistry.Registry);

        public static void Run()
        {
            Console.WriteLine("=== OpenAI Model Routing Demo ===");
            Console.WriteLine("Choose an option:");
            Console.WriteLine("1. Explicit Model Routing");
            Console.WriteLine("2. Best Reasoning Model");
            Console.WriteLine("3. Lowest Cost Chat Model");
            Console.WriteLine("4. Best Vision Model");
            Console.WriteLine("5. Reasoning Ensemble");
            Console.WriteLine("6. Vision Ensemble");
            Console.WriteLine("7. Cost-Optimized Ensemble");
            Console.WriteLine("8. Custom Ensemble");
            Console.Write("Enter choice: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    RunExplicitRouting();
                    break;

                case "2":
                    RunBestReasoning();
                    break;

                case "3":
                    RunLowestCostChat();
                    break;

                case "4":
                    RunBestVision();
                    break;

                case "5":
                    RunReasoningEnsemble();
                    break;

                case "6":
                    RunVisionEnsemble();
                    break;

                case "7":
                    RunCostOptimizedEnsemble();
                    break;

                case "8":
                    RunCustomEnsemble();
                    break;

                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        // -------------------------
        // Single Model Routing
        // -------------------------

        /// <summary>
        /// Run explicit model routing demo.
        /// </summary>
        private static void RunExplicitRouting()
        {
            Console.WriteLine("Enter model name (e.g., GPT4o, GPT5_2):");
            Console.WriteLine("Options: " + string.Join(", ", Enum.GetNames(typeof(OpenAIModel))));
            string? input = Console.ReadLine();

            if (!Enum.TryParse<OpenAIModel>(input, out OpenAIModel model))
            {
                Console.WriteLine("Invalid model.");
                return;
            }

            SingleModelRouterResult result = SingleRouter.Route(new SingleModelContext
            {
                Strategy = ModelRoutingStrategy.Explicit,
                ExplicitModel = model,
            });

            PrintSingleResult("Explicit Routing", result);
        }

        /// <summary>
        /// Run best reasoning model routing demo.
        /// </summary>
        private static void RunBestReasoning()
        {
            SingleModelRouterResult result = SingleRouter.Route(new SingleModelContext
            {
                Strategy = ModelRoutingStrategy.BestReasoning,
            });

            PrintSingleResult("Best Reasoning Model", result);
        }

        /// <summary>
        /// Run lowest cost chat model routing demo.
        /// </summary>
        private static void RunLowestCostChat()
        {
            SingleModelRouterResult result = SingleRouter.Route(new SingleModelContext
            {
                Strategy = ModelRoutingStrategy.LowestCost,
                RequiredCapabilities = [ModelCapability.Chat],
            });

            PrintSingleResult("Lowest Cost Chat Model", result);
        }

        /// <summary>
        /// Run best vision model routing demo.
        /// </summary>
        private static void RunBestVision()
        {
            SingleModelRouterResult result = SingleRouter.Route(new SingleModelContext
            {
                Strategy = ModelRoutingStrategy.BestVision,
            });

            PrintSingleResult("Best Vision Model", result);
        }

        // -------------------------
        // Ensemble Routing
        // -------------------------

        /// <summary>
        /// Run reasoning ensemble routing demo.
        /// </summary>
        private static void RunReasoningEnsemble()
        {
            EnsembleRouterResult result = EnsembleRouter.Route(new EnsembleContext
            {
                Strategy = EnsembleRoutingStrategy.Reasoning,
            });

            PrintEnsembleResult("Reasoning Ensemble", result);
        }

        /// <summary>
        /// Run vision ensemble routing demo.
        /// </summary>
        private static void RunVisionEnsemble()
        {
            EnsembleRouterResult result = EnsembleRouter.Route(new EnsembleContext
            {
                Strategy = EnsembleRoutingStrategy.Vision,
            });

            PrintEnsembleResult("Vision Ensemble", result);
        }

        /// <summary>
        /// Run cost-optimized ensemble routing demo.
        /// </summary>
        private static void RunCostOptimizedEnsemble()
        {
            EnsembleRouterResult result = EnsembleRouter.Route(new EnsembleContext
            {
                Strategy = EnsembleRoutingStrategy.CostOptimized,
            });

            PrintEnsembleResult("Cost-Optimized Ensemble", result);
        }

        /// <summary>
        /// Run custom ensemble routing demo.
        /// </summary>
        private static void RunCustomEnsemble()
        {
            Console.WriteLine("Enter required capabilities (comma-separated):");
            Console.WriteLine("Options: " + string.Join(", ", Enum.GetNames(typeof(ModelCapability))));

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("No capabilities entered.");
                return;
            }

            List<string> rawParts = [.. input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())];

            List<ModelCapability> parsed = [];
            List<string> invalid = [];

            foreach (var part in rawParts)
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
                Console.WriteLine($"Invalid capabilities: {string.Join(", ", invalid)}");
                return;
            }

            if (parsed.Count == 0)
            {
                Console.WriteLine("No valid capabilities provided.");
                return;
            }

            Console.WriteLine("How many models do you want?");
            var countInput = Console.ReadLine();

            if (!int.TryParse(countInput, out var count) || count <= 0)
            {
                Console.WriteLine("Invalid number.");
                return;
            }

            // Validate that enough models exist BEFORE routing
            List<ModelDescriptor> matchingModels = [.. ModelRegistry.Registry.Values.Where(m => parsed.All(c => m.Capabilities.Contains(c)))];

            if (matchingModels.Count < count)
            {
                Console.WriteLine($"Only {matchingModels.Count} models match those capabilities, but {count} were requested.");
                return;
            }

            EnsembleRouterResult result = EnsembleRouter.Route(new EnsembleContext
            {
                Strategy = EnsembleRoutingStrategy.Custom,
                ModelCount = count,
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
        private static void PrintSingleResult(string title, SingleModelRouterResult result)
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
        private static void PrintEnsembleResult(string title, EnsembleRouterResult result)
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
