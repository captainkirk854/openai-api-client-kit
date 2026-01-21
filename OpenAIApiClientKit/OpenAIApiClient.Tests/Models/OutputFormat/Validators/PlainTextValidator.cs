// <copyright file="PlainTextValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.OutputFormat.Validators
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using testClass = OpenAIApiClient.Models.OutputFormat.Validators.PlainTextValidator;

    [TestClass]
    public class PlainTextValidator
    {
        private readonly testClass validator = new();

        [TestMethod]
        [DataRow("Hello world")]
        [DataRow("Don't Panic")]
        public void ValidTxt_ShouldPass(string input)
        {
            bool result = this.validator.IsValidFormat(content: input, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("<html>nope</html>")]
        [DataRow("{ \"json\": true }")]
        [DataRow("[1,2,3]")]
        public void InvalidTxt_ShouldFail(string input)
        {
            bool result = this.validator.IsValidFormat(content: input, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }
    }
}
