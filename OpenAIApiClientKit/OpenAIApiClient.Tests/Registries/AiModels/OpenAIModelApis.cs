// <copyright file="OpenAIModelApis.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Registries.AiModels
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using OpenAIApiClient.Enums;
    using OpenAIApiClient.Registries.AiModels;
    using testClass = OpenAIApiClient.Registries;

    /// <summary>
    /// Tests for the <see cref="OpenAIModelApis"/> class.
    /// </summary>
    [TestClass]
    public class OpenAIModelApis
    {
        [TestMethod]
        public void EnumToApiString_And_Back_ShouldMatch()
        {
            foreach (OpenAIModel model in Enum.GetValues(typeof(OpenAIModel)))
            {
                // Convert enum → API string
                string apiString = model.ToApiString();
                Assert.IsFalse(string.IsNullOrWhiteSpace(apiString), $"API string for {model} should not be null or empty.");

                // Convert API string → enum
                OpenAIModel parsedModel = testClass.AiModels.OpenAIModelApis.FromApiString(apiString);
                Assert.AreEqual(model, parsedModel, $"Round-trip conversion failed for {model}.");
            }
        }

        [TestMethod]
        public void InvalidApiString_ShouldThrow()
        {
            // Act + Assert
            Assert.ThrowsExactly<ArgumentException>(() => { testClass.AiModels.OpenAIModelApis.FromApiString("non-existent-model"); });
        }

        [TestMethod]
        public void InvalidEnum_ShouldThrow()
        {
            // Act - Cast an invalid enum value
            OpenAIModel invalidModel = (OpenAIModel)(-1);

            // Act + Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => { invalidModel.ToApiString(); });
        }

        [TestMethod]
        public void ListAllModels_ShouldContainAllEnumToApiStringMappings()
        {
            IEnumerable<string> allModels = testClass.AiModels.OpenAIModelApis.ListAllModels();

            foreach (OpenAIModel model in Enum.GetValues(typeof(OpenAIModel)))
            {
                string apiString = model.ToApiString();
                CollectionAssert.Contains((System.Collections.ICollection)allModels, apiString, $"ListAllModels should contain {apiString}");
            }
        }

        [TestMethod]
        public void LatestRecommendedModels_ShouldContainAllCategories()
        {
            Dictionary<string, OpenAIModel> models = testClass.AiModels.OpenAIModelApis.LatestRecommendedModels();

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
            var models = testClass.AiModels.OpenAIModelApis.LatestRecommendedModels();

            Assert.AreEqual(OpenAIModel.GPT5_2_Pro, models["Chat"], "Chat category should map to GPT5_2_Pro.");
            Assert.AreEqual(OpenAIModel.TextEmbedding_3_Large, models["Embeddings"], "Embeddings category should map to TextEmbedding_3_Large.");
            Assert.AreEqual(OpenAIModel.TTS_1_HD, models["Audio"], "Audio category should map to TTS_1_HD.");
            Assert.AreEqual(OpenAIModel.DALL_E_3, models["Image"], "Image category should map to DALL_E_3.");
            Assert.AreEqual(OpenAIModel.O1, models["Open-Weight"], "Open-Weight category should map to O1.");
        }

        [TestMethod]
        public void LatestRecommendedModels_ShouldNotBeEmpty()
        {
            Dictionary<string, OpenAIModel> models = testClass.AiModels.OpenAIModelApis.LatestRecommendedModels();
            Assert.IsNotEmpty(models, "LatestRecommendedModels should not return an empty dictionary.");
        }
    }
}
