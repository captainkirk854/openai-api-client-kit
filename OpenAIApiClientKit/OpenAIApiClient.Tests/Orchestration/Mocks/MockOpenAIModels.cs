// <copyright file="MockOpenAIModels.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;
    using OpenAIApiClient.Registries.AiModels;

    /// <summary>
    /// Provides a fake read-only registry of OpenAI models and their descriptors.
    /// </summary>
    /// <param name="models">A dictionary mapping OpenAI models to their descriptors.</param>
    public sealed class MockOpenAIModels(Dictionary<OpenAIModel, AiModelDescriptor> models) : IAiModelRegistry
    {
        private readonly Dictionary<OpenAIModel, AiModelDescriptor> models = models;

        public Dictionary<OpenAIModel, AiModelDescriptor> GetRegistry() => this.models;

        public IEnumerable<AiModelDescriptor> GetAll() => this.models.Values;

        public AiModelDescriptor? GetByName(string name) => this.models.Values.Where(m => m.Name.ToApiString() == name).FirstOrDefault();

        public IEnumerable<AiModelDescriptor> Find(Func<AiModelDescriptor, bool> predicate)
            => this.models.Values.Where(predicate);
    }
}