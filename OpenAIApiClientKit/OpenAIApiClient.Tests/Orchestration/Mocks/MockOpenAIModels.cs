// <copyright file="MockOpenAIModels.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries;
    using OpenAIApiClient.Registries.Models;

    /// <summary>
    /// Provides a fake read-only registry of OpenAI models and their descriptors.
    /// </summary>
    /// <param name="models">A dictionary mapping OpenAI models to their descriptors.</param>
    public sealed class MockOpenAIModels(Dictionary<OpenAIModel, ModelDescriptor> models) : IAIModelRegistry
    {
        private readonly Dictionary<OpenAIModel, ModelDescriptor> models = models;

        public Dictionary<OpenAIModel, ModelDescriptor> GetRegistry() => this.models;

        public IEnumerable<ModelDescriptor> GetAll() => this.models.Values;

        public ModelDescriptor? GetByName(string name) => this.models.Values.Where(m => m.Name.ToApiString() == name).FirstOrDefault();

        public IEnumerable<ModelDescriptor> Find(Func<ModelDescriptor, bool> predicate)
            => this.models.Values.Where(predicate);
    }
}