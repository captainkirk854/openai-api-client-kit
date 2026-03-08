// <copyright file="OrchestratorBuilder.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration.Factories
{
    using OpenAIApiClient;
    using OpenAIApiClient.Helpers;
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Interfaces.Orchestration.Dispatch;
    using OpenAIApiClient.Interfaces.Orchestration.Execution;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Orchestration;
    using OpenAIApiClient.Orchestration.Dispatch;
    using OpenAIApiClient.Orchestration.Execution;

    /// <summary>
    /// Provides a builder for configuring and creating an Orchestrator instance with customizable components.
    /// </summary>
    /// <remarks>
    /// Usage Patterns:
    ///
    /// 1. Minimal Configuration:
    /// var orchestrator = new OrchestratorBuilder()
    ///                        .WithClient(client)
    ///                        .WithResponseHandler(handler)
    ///                        .Build();.
    ///
    /// 2. Override the Model Registry:
    /// var orchestrator = new OrchestratorBuilder()
    ///                        .WithClient(client)
    ///                        .WithResponseHandler(handler)
    ///                        .WithModelRegistry(new CustomModelRegistry())
    ///                        .Build();.
    ///
    /// 3. Override the Request Builder:
    /// var orchestrator = new OrchestratorBuilder()
    ///                        .WithClient(client)
    ///                        .WithResponseHandler(handler)
    ///                        .WithRequestBuilder(() => new <see cref="ChatClientRequestBuilder"/>().WithDefaults().UsingMaxTokens(2000))
    ///                        .Build();.
    ///
    /// 4. Override everything (full control):
    /// var orchestrator = new OrchestratorBuilder()
    ///                        .WithClient(client)
    ///                        .WithResponseHandler(handler)
    ///                        .WithModelRegistry(myRegistry)
    ///                        .WithRequestBuilder(() => new <see cref="ChatClientRequestBuilder"/>().WithMyConfig())
    ///                        .Build();.
    /// <./remarks>
    public sealed class OrchestratorBuilder
    {
        private ChatClient client = default!;
        private IAiModelResponseHandler responseHandler = default!;
        private IAiModelRegistry? registry;
        private Func<ChatClientRequestBuilder>? requestBuilderFactory;
        private ISingleAiModelDispatcher? singleModelDispatcher;
        private IEnsembleDispatcher? ensembleDispatcher;
        private ISingleAiModelExecutor? singleModelExecutor;
        private IEnsembleExecutor? ensembleExecutor;

        /// <summary>
        /// The <see cref="ChatClient"/> instance the orchestrator should use to send requests to the OpenAI API.
        /// </summary>
        /// <remarks>
        /// This is required, and must be set by the caller (or an exception will be thrown when Build() is called).
        /// The builder does not create a default ChatClient because it has required constructor parameters (e.g. API key)
        /// that the builder cannot know how to populate.
        /// </remarks>
        /// <param name="client">The <see cref="ChatClient"/> instance to use for API calls.</param>
        /// <returns see cref="OrchestratorBuilder">The builder instance, for chaining.</returns>
        public OrchestratorBuilder WithClient(ChatClient client)
        {
            this.client = client;
            return this;
        }

        /// <summary>
        /// The <see cref="IAiModelResponseHandler"/> the orchestrator should use to process model responses before returning them to the caller.
        /// </summary>
        /// <param name="handler">The <see cref="IAiModelResponseHandler"/> instance to use for processing model responses. If not set, a default handler that returns raw model responses will be used.</param>
        /// <returns see cref="OrchestratorBuilder">The builder instance, for chaining.</returns>
        public OrchestratorBuilder WithResponseHandler(IAiModelResponseHandler handler)
        {
            this.responseHandler = handler;
            return this;
        }

        /// <summary>
        /// The <see cref="IAiModelRegistry"/> the orchestrator should use to look up model capabilities and metadata when dispatching requests.
        /// </summary>
        /// <param name="registry">The <see cref="IAiModelRegistry"/> instance to use for model lookups. If not set, a default registry containing OpenAI models will be used.</param>
        /// <returns see cref="OrchestratorBuilder">The builder instance, for chaining.</returns>
        public OrchestratorBuilder WithModelRegistry(IAiModelRegistry registry)
        {
            this.registry = registry;
            return this;
        }

        /// <summary>
        /// The factory used to produce a fresh <see cref="ChatClientRequestBuilder"/> for each request processed by the orchestrator.
        /// </summary>
        /// <remarks>
        /// Providing a factory ensures that each call to <c>ProcessAsync</c> starts with a clean builder, preventing
        /// messages or state from leaking between requests.
        /// </remarks>
        /// <param name="builderFactory">A factory that returns a new, pre-configured <see cref="ChatClientRequestBuilder"/> each time it is called/invoked. If not set, a default factory is used.</param>
        /// <returns see cref="OrchestratorBuilder">The builder instance, for chaining.</returns>
        public OrchestratorBuilder WithRequestBuilderFactory(Func<ChatClientRequestBuilder> builderFactory)
        {
            this.requestBuilderFactory = builderFactory;
            return this;
        }

        /// <summary>
        /// The <see cref="ISingleAiModelDispatcher"/> the orchestrator should use to determine which model to send single-model requests
        /// to, and how to structure those requests.
        /// </summary>
        /// <param name="dispatcher">The <see cref="ISingleAiModelDispatcher"/> instance to use for dispatching single-model requests. If not set, a default dispatcher that uses the model registry to find the most capable model for the request will be used.</param>
        /// <returns see cref="OrchestratorBuilder">The builder instance, for chaining.</returns>
        public OrchestratorBuilder WithSingleModelDispatcher(ISingleAiModelDispatcher dispatcher)
        {
            this.singleModelDispatcher = dispatcher;
            return this;
        }

        /// <summary>
        /// The <see cref="IEnsembleDispatcher"/> the orchestrator should use to determine which models to send ensemble requests to, how to
        /// structure those requests, and how to combine the results.
        /// </summary>
        /// <param name="dispatcher">The <see cref="IEnsembleDispatcher"/> instance to use for dispatching ensemble requests. If not set, a default dispatcher that uses the model registry to find a set of complementary models for the request and combines their results using a simple heuristic will be used.</param>
        /// <returns see cref="OrchestratorBuilder">The builder instance, for chaining.</returns>
        public OrchestratorBuilder WithEnsembleDispatcher(IEnsembleDispatcher dispatcher)
        {
            this.ensembleDispatcher = dispatcher;
            return this;
        }

        /// <summary>
        /// The <see cref="ISingleAiModelExecutor"/> the orchestrator should use to execute requests that have been dispatched to a single model.
        /// </summary>
        /// <param name="executor">The <see cref="ISingleAiModelExecutor"/> instance to use for executing single-model requests. If not set, a default executor that sends requests to the OpenAI API using the provided ChatClient will be used.</param>
        /// <returns see cref="OrchestratorBuilder">The builder instance, for chaining.</returns>
        public OrchestratorBuilder WithSingleModelExecutor(ISingleAiModelExecutor executor)
        {
            this.singleModelExecutor = executor;
            return this;
        }

        /// <summary>
        /// The <see cref="IEnsembleExecutor"/> the orchestrator should use to execute requests that have been dispatched to multiple models as an ensemble.
        /// </summary>
        /// <param name="executor">The <see cref="IEnsembleExecutor"/> instance to use for executing ensemble requests. If not set, a default executor that sends requests to the OpenAI API using the provided ChatClient and combines results using a simple heuristic will be used.</param>
        /// <returns see cref="OrchestratorBuilder">The builder instance, for chaining.</returns>
        public OrchestratorBuilder WithEnsembleExecutor(IEnsembleExecutor executor)
        {
            this.ensembleExecutor = executor;
            return this;
        }

        /// <summary>
        /// Creates and configures a new <see cref="Orchestrator"/> instance using the specified or default components.
        /// </summary>
        /// <returns see cref="Orchestrator">A configured Orchestrator instance.</returns>
        public Orchestrator Build()
        {
            if (this.client is null)
            {
                throw new InvalidOperationException("A <ChatClient> must be provided.");
            }

            if (this.responseHandler is null)
            {
                throw new InvalidOperationException("A <IAiModelResponseHandler> must be provided.");
            }

            // Use factory defaults if caller didn’t override ..
            IAiModelRegistry registry = this.registry ?? AiModelRegistryFactory.Create();
            Func<ChatClientRequestBuilder> requestBuilderFactory = this.requestBuilderFactory ?? ChatClientRequestBuilderFactory.CreateDefaultFactory();

            // Create Dispatchers - Use overrides if provided, otherwise fall back to DispatcherFactory ..
            (SingleAiModelDispatcher factorySingleModelDispatcher, EnsembleDispatcher defaultEnsembleDispatcher) = DispatcherFactory.Create(registry: registry);
            ISingleAiModelDispatcher singleModelDispatcher = this.singleModelDispatcher ?? factorySingleModelDispatcher;
            IEnsembleDispatcher ensembleDispatcher = this.ensembleDispatcher ?? defaultEnsembleDispatcher;

            // Create Executors - Use overrides if provided, otherwise fall back to ExecutorFactory ..
            (SingleAiModelExecutor factorySingleModelExecutor, EnsembleExecutor defaultEnsembleExecutor) = ExecutorFactory.Create(client: this.client);
            ISingleAiModelExecutor singleModelExecutor = this.singleModelExecutor ?? factorySingleModelExecutor;
            IEnsembleExecutor ensembleExecutor = this.ensembleExecutor ?? defaultEnsembleExecutor;

            // With all the components gathered (either from the caller or from defaults), create the Orchestrator instance ..
            return new Orchestrator(requestBuilderFactory,
                                    singleModelDispatcher,
                                    ensembleDispatcher,
                                    singleModelExecutor,
                                    ensembleExecutor,
                                    this.responseHandler);
        }
    }
}
