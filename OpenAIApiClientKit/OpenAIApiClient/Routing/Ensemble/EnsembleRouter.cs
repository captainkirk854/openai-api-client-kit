// <copyright file="EnsembleRouter.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Routing.Ensemble
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Models.Registries;

    public sealed class EnsembleRouter(IReadOnlyDictionary<OpenAIModel, ModelDescriptor> registry)
    {
        private readonly IReadOnlyDictionary<OpenAIModel, ModelDescriptor> registry = registry;

        public EnsembleRouterResult Route(EnsembleRouterRequest request)
        {
            return request.Strategy switch
            {
                EnsembleRoutingStrategy.Reasoning => this.BuildReasoningEnsemble(),
                EnsembleRoutingStrategy.Vision => this.BuildVisionEnsemble(),
                EnsembleRoutingStrategy.CostOptimized => this.BuildCostOptimizedEnsemble(),
                EnsembleRoutingStrategy.Custom => this.BuildCustomEnsemble(request),

                _ => throw new InvalidOperationException("Unknown ensemble routing strategy.")
            };
        }

        // -------------------------
        // Ensemble Strategies
        // -------------------------

        /// <summary>
        /// Creates an ensemble of model descriptors optimized for reasoning, chat performance, and cost efficiency.
        /// </summary>
        /// <returns>An EnsembleRouterResult containing selected reasoning, fast, and critic models.</returns>
        private EnsembleRouterResult BuildReasoningEnsemble()
        {
            ModelDescriptor reasoning = this.registry.Values
                                            .Where(m => m.Capabilities.Contains(ModelCapability.Reasoning))
                                            .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                                            .First();

            ModelDescriptor fast = this.registry.Values
                                       .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                                       .OrderBy(m => m.Pricing.InputTokenCost)
                                       .First();

            ModelDescriptor critic = this.registry.Values
                                         .Where(m => m.Capabilities.Contains(ModelCapability.Reasoning))
                                         .OrderBy(m => m.Pricing.InputTokenCost)
                                         .First();

            return new EnsembleRouterResult(models:
            [
                reasoning,
                fast,
                critic,
            ]);
        }

        /// <summary>
        /// Creates an ensemble result containing the highest performing vision model and the most cost-effective chat
        /// model from the registry.
        /// </summary>
        /// <returns>An EnsembleRouterResult with selected vision and chat models.</returns>
        private EnsembleRouterResult BuildVisionEnsemble()
        {
            ModelDescriptor vision = this.registry.Values
                                         .Where(m => m.Capabilities.Contains(ModelCapability.Vision))
                                         .OrderByDescending(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                                         .First();

            ModelDescriptor fastText = this.registry.Values
                                           .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                                           .OrderBy(m => m.Pricing.InputTokenCost)
                                           .First();

            return new EnsembleRouterResult(models:
            [
                vision,
                fastText,
            ]);
        }

        /// <summary>
        /// Creates an ensemble of models optimized for cost by selecting low-cost, chat-capable, and high-performance
        /// models from the registry.
        /// </summary>
        /// <returns>An EnsembleRouterResult containing the selected models.</returns>
        private EnsembleRouterResult BuildCostOptimizedEnsemble()
        {
            ModelDescriptor low = this.registry.Values
                                      .Where(m => m.Capabilities.Contains(ModelCapability.LowCost))
                                      .OrderBy(m => m.Pricing.InputTokenCost)
                                      .First();

            ModelDescriptor mid = this.registry.Values
                                      .Where(m => m.Capabilities.Contains(ModelCapability.Chat))
                                      .OrderBy(m => m.Pricing.InputTokenCost)
                                      .Skip(1)
                                      .FirstOrDefault() ?? low;

            ModelDescriptor high = this.registry.Values
                                       .Where(m => m.Capabilities.Contains(ModelCapability.HighPerformance))
                                       .OrderBy(m => m.Pricing.InputTokenCost)
                                       .First();

            return new EnsembleRouterResult(models:
            [
                low,
                mid,
                high
            ]);
        }

        /// <summary>
        /// Creates a custom ensemble of models that match the required capabilities specified in the request.
        /// </summary>
        /// <param name="request">The ensemble router request containing required capabilities and model count.</param>
        /// <returns>An EnsembleRouterResult containing the selected models.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no required capabilities are specified or if no models match the requested capabilities.</exception>
        private EnsembleRouterResult BuildCustomEnsemble(EnsembleRouterRequest request)
        {
            if (request.RequiredCapabilities is null || request.RequiredCapabilities.Count == 0)
            {
                throw new InvalidOperationException("Custom ensemble requires capabilities.");
            }

            int count = request.ModelCount ?? 1;

            List<ModelDescriptor> models = [.. this.registry.Values
                                                   .Where(m => request.RequiredCapabilities.All(c => m.Capabilities.Contains(c)))
                                                   .OrderBy(m => m.Pricing.InputTokenCost)
                                                   .Take(count)];

            if (models.Count == 0)
            {
                throw new InvalidOperationException("No models match the requested capabilities.");
            }

            return new EnsembleRouterResult(models: models);
        }
    }
}
