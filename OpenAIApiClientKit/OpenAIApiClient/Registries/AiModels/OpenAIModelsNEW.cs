// <copyright file="OpenAIModelsNEW.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Registries.AiModels
{
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Interfaces.Registries;
    using OpenAIApiClient.Models.Registries.AiModels;

    /// <summary>
    /// New model registry that is constructed from a pre-built descriptor dictionary.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="OpenAIModelsNEW"/> class.
    /// </remarks>
    /// <param name="models">
    /// The descriptor dictionary keyed by <see cref="string"/>.
    /// This must contain one entry per supported model.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="models"/> is null.</exception>
    public sealed class OpenAIModelsNEW(IReadOnlyDictionary<string, AiModelPropertyRegistryModel> models) : IAiModelRegistryNEW
    {
        private readonly IReadOnlyDictionary<string, AiModelPropertyRegistryModel> models = models ?? throw new ArgumentNullException(nameof(models));

        /// <summary>
        /// Gets the complete model registry dictionary ..
        /// </summary>
        /// <returns see cref="IReadOnlyDictionary(string, AiModelPropertyRegistryModel)">.</returns>
        public IReadOnlyDictionary<string, AiModelPropertyRegistryModel> GetRegistry() => this.models;

        /// <summary>
        /// Returns all model descriptors in the registry.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{AiModelPropertyRegistryModel}"/> of matching descriptors.</returns>
        public IReadOnlyCollection<AiModelPropertyRegistryModel> GetAll() => this.models.Values.ToList();

        /// <summary>
        /// Gets the model descriptor for a specified model by its unique name, or null if not found.
        /// </summary>
        /// <param name="name"></param>
        /// <returns see cref="AiModelDescriptor">.</returns>
        public AiModelPropertyRegistryModel? TryGetByName(string name) => this.models.Values.Where(m => m.Name == name).FirstOrDefault();

        /// <summary>
        /// Gets the model descriptor for a specified model ..
        /// </summary>
        /// <param name="model">The model key.</param>
        /// <returns><see cref="AiModelDescriptor"/>.</returns>
        public AiModelPropertyRegistryModel Get(string model) => this.models[model];

        /// <summary>
        /// Returns all models that satisfy the provided predicate.
        /// </summary>
        /// <param name="predicate">Filter applied to descriptors.</param>
        /// <returns>An <see cref="IEnumerable{AiModelDescriptor}"/> of matching descriptors.</returns>
        public IEnumerable<AiModelPropertyRegistryModel> Find(Func<AiModelPropertyRegistryModel, bool> predicate)
        {
            // Guard against null predicate to prevent runtime errors.
            ArgumentNullException.ThrowIfNull(predicate);
            return this.models.Values.Where(predicate);
        }

        /// <summary>
        /// Attempts to get a descriptor for the specified model.
        /// </summary>
        /// <param name="model">The model key.</param>
        /// <param name="descriptor">The descriptor, if found.</param>
        /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
        public bool TryGet(string model, out AiModelPropertyRegistryModel? descriptor)
        {
            // Guard against invalid model keys to prevent runtime errors.
            return this.models.TryGetValue(model, out descriptor);
        }
    }
}
