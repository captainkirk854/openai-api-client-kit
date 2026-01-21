// <copyright file="JsonValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.OutputFormat.Validators
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using testClass = OpenAIApiClient.Models.OutputFormat.Validators.JsonValidator;

    [TestClass]
    public class JsonValidator
    {
        private readonly testClass validator = new();

        [TestMethod]
        [DataRow("{ \"Name\": \"Klaus Schulze\" }")]
        [DataRow("{ \"Name\": \"Jean-Michel Jarre\" }")]
        public void ValidJson_ShouldPass(string json)
        {
            bool result = this.validator.IsValidFormat(json, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("{ invalid json }")]
        [DataRow("{ \"missing\": \"comma\" \"oops\": 1 }")]
        public void InvalidJson_ShouldFail(string json)
        {
            bool result = this.validator.IsValidFormat(json, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }
    }
}
