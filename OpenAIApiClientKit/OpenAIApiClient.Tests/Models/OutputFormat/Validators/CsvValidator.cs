// <copyright file="CsvValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.OutputFormat.Validators
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using testClass = OpenAIApiClient.Models.OutputFormat.Validators.CsvValidator;

    [TestClass]
    public class CsvValidator
    {
        private readonly testClass validator = new();

        [TestMethod]
        [DataRow("Name,Age\nKlaus,42")]
        public void ValidCsv_ShouldPass(string csv)
        {
            bool result = this.validator.IsValidFormat(csv, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("Klaus,42")]
        public void MissingHeader_ShouldFail(string csv)
        {
            bool result = this.validator.IsValidFormat(csv, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }

        [TestMethod]
        [DataRow("Name\nKlaus")]
        public void SingleColumnHeader_ShouldFail(string csv)
        {
            bool result = this.validator.IsValidFormat(csv, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }
    }
}
