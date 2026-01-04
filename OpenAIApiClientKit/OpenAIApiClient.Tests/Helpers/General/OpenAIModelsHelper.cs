// <copyright file="OpenAIModelsHelper.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Helpers.General
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Helpers.General;
    using testClass = OpenAIApiClient.Helpers.General;

    [TestClass]
    public class OpenAIModelsHelper
    {
        [TestMethod]
        public void EnumToApiString_And_Back_ShouldMatch()
        {
            foreach (OpenAIModels model in Enum.GetValues(typeof(OpenAIModels)))
            {
                // Convert enum → API string
                string apiString = model.ToApiString();
                Assert.IsFalse(string.IsNullOrWhiteSpace(apiString), $"API string for {model} should not be null or empty.");

                // Convert API string → enum
                OpenAIModels parsedModel = testClass.OpenAIModelsHelper.FromApiString(apiString);
                Assert.AreEqual(model, parsedModel, $"Round-trip conversion failed for {model}.");
            }
        }

        [TestMethod]
        public void InvalidApiString_ShouldThrow()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => { testClass.OpenAIModelsHelper.FromApiString("non-existent-model"); });
        }

        [TestMethod]
        public void InvalidEnum_ShouldThrow()
        {
            // Act - Cast an invalid enum value
            OpenAIModels invalidModel = (OpenAIModels)(-1);

            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => { invalidModel.ToApiString(); });
        }

        [TestMethod]
        public void ListAllModels_ShouldContainAllMappings()
        {
            IEnumerable<string> allModels = testClass.OpenAIModelsHelper.ListAllModels();

            foreach (OpenAIModels model in Enum.GetValues(typeof(OpenAIModels)))
            {
                string apiString = model.ToApiString();
                CollectionAssert.Contains((System.Collections.ICollection)allModels, apiString, $"ListAllModels should contain {apiString}");
            }
        }

        [TestMethod]
        public void LatestRecommendedModels_ShouldContainAllCategories()
        {
            Dictionary<string, OpenAIModels> models = testClass.OpenAIModelsHelper.LatestRecommendedModels();

            // Expected categories
            List<string> expectedCategories =
            [
                "Chat",
                "Embeddings",
                "Audio",
                "Image",
                "Open-Weight"
            ];

            foreach (var category in expectedCategories)
            {
                Assert.IsTrue(models.ContainsKey(category), $"Category '{category}' should exist in LatestRecommendedModels.");
            }
        }

        [TestMethod]
        public void LatestRecommendedModels_ShouldReturnCorrectEnumValues()
        {
            var models = testClass.OpenAIModelsHelper.LatestRecommendedModels();

            Assert.AreEqual(OpenAIModels.GPT5_2_Pro, models["Chat"], "Chat category should map to GPT5_2_Pro.");
            Assert.AreEqual(OpenAIModels.TextEmbedding_3_Large, models["Embeddings"], "Embeddings category should map to TextEmbedding_3_Large.");
            Assert.AreEqual(OpenAIModels.TTS_1_HD, models["Audio"], "Audio category should map to TTS_1_HD.");
            Assert.AreEqual(OpenAIModels.DALL_E_3, models["Image"], "Image category should map to DALL_E_3.");
            Assert.AreEqual(OpenAIModels.O1, models["Open-Weight"], "Open-Weight category should map to O1.");
        }

        [TestMethod]
        public void LatestRecommendedModels_ShouldNotBeEmpty()
        {
            Dictionary<string, OpenAIModels> models = testClass.OpenAIModelsHelper.LatestRecommendedModels();
            Assert.IsNotEmpty(models, "LatestRecommendedModels should not return an empty dictionary.");
        }
    }
}
