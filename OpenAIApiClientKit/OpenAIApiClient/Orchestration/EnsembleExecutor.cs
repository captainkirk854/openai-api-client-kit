// <copyright file="EnsembleExecutor.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Orchestration
{
    using OpenAIApiClient.Interfaces.Orchestration;
    using OpenAIApiClient.Models.Registries;

    public sealed class EnsembleExecutor(IModelExecutor modelExecutor) : IEnsembleExecutor
    {
        private readonly IModelExecutor modelExecutor = modelExecutor;

        public async Task<IReadOnlyList<string>> ExecuteAsync(IReadOnlyList<ModelDescriptor> models, string prompt)
        {
            if (models is null || models.Count == 0)
            {
                throw new ArgumentException("Ensemble must contain at least one model.", nameof(models));
            }

            Task<string>[] tasks = [.. models
                .Select(async m =>
                {
                    try
                    {
                        return await this.modelExecutor.ExecuteAsync(model: m, prompt: prompt).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        // You can log here, or return a structured error marker
                        return $"[ERROR from {m.Name}: {ex.Message}]";
                    }
                })];

            string[] results = await Task.WhenAll(tasks).ConfigureAwait(false);

            return results;
        }
    }
}
