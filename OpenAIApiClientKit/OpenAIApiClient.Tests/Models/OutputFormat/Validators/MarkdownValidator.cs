// <copyright file="MarkdownValidator.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.OutputFormat.Validators
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using testClass = OpenAIApiClient.Models.OutputFormat.Validators.MarkdownValidator;

    [TestClass]
    public class MarkdownValidator
    {
        private readonly testClass validator = new();

        [TestMethod]
        [DataRow("# Heading")]
        [DataRow("* List item")]
        [DataRow("| Col1 | Col2 |")]
        public void ValidMarkdown_ShouldPass(string md)
        {
            bool result = this.validator.IsValidFormat(md, out var error);

            Assert.IsTrue(result);
            Assert.IsNull(error);
        }

        [TestMethod]
        [DataRow("This is plain text with no markdown")]
        public void InvalidMarkdown_ShouldFail(string md)
        {
            bool result = this.validator.IsValidFormat(md, out var error);

            Assert.IsFalse(result);
            Assert.IsNotNull(error);
        }
    }
}
