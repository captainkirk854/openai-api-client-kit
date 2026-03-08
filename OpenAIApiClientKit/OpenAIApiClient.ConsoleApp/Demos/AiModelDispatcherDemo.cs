// <copyright file="AiModelDispatcherDemo.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.ConsoleApp.Demos
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.Extensions;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Registries.AiModels;

    /// <summary>
    /// Single and Ensemble Model Dispatch Selection Demo.
    /// </summary>
    public static class AiModelDispatcherDemo
    {
        private static readonly OpenAIModelRegistry Models = new();
        private static readonly SingleAiModelDispatcher SingleModelDispatcher = new(registry: Models);
        private static readonly EnsembleDispatcher EnsembleDispatcher = new(modelRegistry: Models);

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
            Console.WriteLine("Enter model name (e.g., gpt-4o, gpt-3.5-turbo, whisper-1):");
            Console.WriteLine("Available models:" + Environment.NewLine + string.Join(Environment.NewLine, Models.GetAll().Select(m => $"- {m.Name}")));
            string? model = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(model))
            {
                Console.WriteLine("No model name entered.");
                return;
            }
            if (Models.IsExists(name: model))
            {
                Console.WriteLine($"Model '{model}' not found in registry.");
                return;
            }

            SingleAiModelDispatchResult result = SingleModelDispatcher.Evaluate(new SingleAiModelDispatchRequest
            {
                Strategy = AiModelStrategy.SingleAiModel.Explicit,
                ExplicitModel = model,
            });

            PrintSingleResult("Dispatch Result using Explicit Strategy and Model", result);
        }

        /// <summary>
        /// Run best reasoning model dispatch demo.
        /// </summary>
        private static void RunBestReasoningModelDispatch()
        {
            SingleAiModelDispatchResult result = SingleModelDispatcher.Evaluate(new SingleAiModelDispatchRequest
            {
                Strategy = AiModelStrategy.SingleAiModel.BestReasoning,
            });

            PrintSingleResult("Dispatch Result for: Best Reasoning Model", result);
        }

        /// <summary>
        /// Run lowest cost chat model dispatch demo.
        /// </summary>
        private static void RunLowestCostChatModelDispatch()
        {
            SingleAiModelDispatchResult result = SingleModelDispatcher.Evaluate(new SingleAiModelDispatchRequest
            {
                Strategy = AiModelStrategy.SingleAiModel.LowestCost,
                RequiredCapabilities = [AiModelCapability.Chat],
            });

            PrintSingleResult("Dispatch Result for: Lowest Cost Chat Model", result);
        }

        /// <summary>
        /// Run best vision model dispatch demo.
        /// </summary>
        private static void RunBestVisionModelDispatch()
        {
            SingleAiModelDispatchResult result = SingleModelDispatcher.Evaluate(new SingleAiModelDispatchRequest
            {
                Strategy = AiModelStrategy.SingleAiModel.BestVision,
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
                Strategy = AiModelStrategy.Ensemble.Reasoning,
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
                Strategy = AiModelStrategy.Ensemble.Vision,
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
                Strategy = AiModelStrategy.Ensemble.CostOptimized,
            });

            PrintEnsembleResult("Dispatch Result for: Cost-Optimized Model Ensemble", result);
        }

        private static void RunCustomEnsembleModelDispatch()
        {
            Console.WriteLine("Enter required capability(s) (comma-separated):");
            Console.WriteLine("Options: " + string.Join(", ", Enum.GetNames(typeof(AiModelCapability))));

            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("No capability(s) entered.");
                return;
            }

            List<string> rawParts = [.. input.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim())];

            List<AiModelCapability> parsedCapabilities = [];
            List<string> invalid = [];

            foreach (string part in rawParts)
            {
                if (Enum.TryParse(part, ignoreCase: true, out AiModelCapability cap))
                {
                    parsedCapabilities.Add(cap);
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

            if (parsedCapabilities.Count == 0)
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

            // Validate that enough models exist BEFORE routing, using registry and leaf capability checks.
            IReadOnlyCollection<AiModelDescriptor> allModels = Models.GetAll();

            List<AiModelDescriptor> matchingModels = [.. allModels.Where(model => parsedCapabilities.All(capability => model.HasCapability(capability: capability)))];

            if (matchingModels.Count < minRequiredCount)
            {
                Console.WriteLine($"Only {matchingModels.Count} model(s) match those capabilities, but a minimum of {minRequiredCount} model(s) were requested.");
                return;
            }

            // Proceed with evaluation of which models are selected using the NEW dispatcher and request type.
            EnsembleDispatchResult result = EnsembleDispatcher.Evaluate(
                new EnsembleDispatchRequest
                {
                    Strategy = AiModelStrategy.Ensemble.Custom,
                    ModelCount = minRequiredCount,
                    RequiredCapabilities = parsedCapabilities,
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
        private static void PrintSingleResult(string title, SingleAiModelDispatchResult result)
        {
            Console.WriteLine($"\n=== {title} ===");
            Console.WriteLine($"Model:     {result.Model.Name}");
            PrintHighLevelScoresAndCosts(model: result.Model);
            Console.WriteLine();
        }

        /// <summary>
        /// Print ensemble dispatch result.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="result"></param>
        private static void PrintEnsembleResult(string title, EnsembleDispatchResult result)
        {
            Console.WriteLine($"\n=== {title} ===");

            foreach (AiModelDescriptor model in result.Models.Distinct())
            {
                Console.WriteLine($"Model:     {model.Name}");
                PrintHighLevelScoresAndCosts(model: model);
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Writes the total scores for core, advanced, performance, and operational capabilities to the console.
        /// </summary>
        /// <param name="model"></param>
        private static void PrintHighLevelScoresAndCosts(AiModelDescriptor model)
        {
            AiModelCapabilityScores caps = model.Capabilities;

            Console.WriteLine($"Capabilities:");
            Console.WriteLine($"- Core:        {caps.Core.Total}");
            Console.WriteLine($"- Advanced:    {caps.Advanced.Total}");
            Console.WriteLine($"- Performance: {caps.Performance.Total}");
            Console.WriteLine($"- Operational: {caps.Operational.Total}");

            Console.WriteLine("Costs:");
            Console.WriteLine($"- Cost(per token):        {model.Pricing.TotalCostInTokens}");
            Console.WriteLine($"- Cost(per 1mill tokens): {model.Pricing.TotalCostPerMillionTokens}");
        }
    }
}
