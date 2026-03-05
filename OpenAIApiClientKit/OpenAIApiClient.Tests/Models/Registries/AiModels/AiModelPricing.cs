// <copyright file="AiModelPricing.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Models.Registries.AiModels
{
    using testClass = OpenAIApiClient.Models.Registries.AiModels.AiModelPricing;

    [TestClass]
    public class AiModelPricing
    {
        [TestMethod]
        public void Ctor_WithPerTokenScheme_SetsPerTokenProperties_AndLeavesPerMillionNull()
        {
            // Arrange
            decimal inputTokenCost = 0.000001m;
            decimal outputTokenCost = 0.000002m;
            decimal cachedInputTokenCost = 0.0000005m;
            decimal? reasoningTokenCost = 0.00001m;
            decimal? toolUseTokenCost = 0.00002m;

            // Act
            testClass pricing = new(inputTokenCost: inputTokenCost,
                                    outputTokenCost: outputTokenCost,
                                    cachedInputTokenCost: cachedInputTokenCost,
                                    reasoningTokenCost: reasoningTokenCost,
                                    toolUseTokenCost: toolUseTokenCost);

            // Assert
            Assert.AreEqual(inputTokenCost, pricing.InputTokenCost);
            Assert.AreEqual(outputTokenCost, pricing.OutputTokenCost);
            Assert.AreEqual(cachedInputTokenCost, pricing.CachedInputTokenCost);
            Assert.AreEqual(reasoningTokenCost, pricing.ReasoningTokenCost);
            Assert.AreEqual(toolUseTokenCost, pricing.ToolUseTokenCost);

            Assert.IsNotNull(pricing.InputCost);
            Assert.IsNotNull(pricing.OutputCost);
            Assert.IsNotNull(pricing.CachedInputCost);
        }

        [TestMethod]
        public void Ctor_WithPerMillionScheme_ComputesPerTokenCosts()
        {
            // Arrange
            decimal inputCost = 1.50m;       // $1.50 per 1M input tokens
            decimal outputCost = 6.00m;      // $6.00 per 1M output tokens
            decimal cachedInputCost = 0.50m; // $0.50 per 1M cached input tokens

            // Act
            testClass pricing = new(inputTokenCost: 0m,
                                    outputTokenCost: 0m,
                                    cachedInputTokenCost: 0m,
                                    reasoningTokenCost: null,
                                    toolUseTokenCost: null,
                                    inputCost: inputCost,
                                    outputCost: outputCost,
                                    cachedInputCost: cachedInputCost);

            // Assert: per‑1M fields
            Assert.AreEqual(inputCost, pricing.InputCost);
            Assert.AreEqual(outputCost, pricing.OutputCost);
            Assert.AreEqual(cachedInputCost, pricing.CachedInputCost);

            // Assert: per‑token fields = per‑1M / 1_000_000
            Assert.AreEqual(inputCost / 1_000_000m, pricing.InputTokenCost);
            Assert.AreEqual(outputCost / 1_000_000m, pricing.OutputTokenCost);
            Assert.AreEqual(cachedInputCost / 1_000_000m, pricing.CachedInputTokenCost);
        }

        [TestMethod]
        public void Ctor_WithPerMillionOnlySomeValues_ComputesCorrespondingTokenCosts()
        {
            // Arrange
            decimal inputCost = 2.00m;

            // Act
            testClass pricing = new(inputTokenCost: 0m,
                                    outputTokenCost: 0m,
                                    cachedInputTokenCost: 0m,
                                    reasoningTokenCost: null,
                                    toolUseTokenCost: null,
                                    inputCost: inputCost,
                                    outputCost: null,
                                    cachedInputCost: null);

            // Assert: only input per‑1M is set
            Assert.AreEqual(inputCost, pricing.InputCost);
            Assert.IsNull(pricing.OutputCost);
            Assert.IsNull(pricing.CachedInputCost);

            // Derived token‑level values
            Assert.AreEqual(inputCost / 1_000_000m, pricing.InputTokenCost);
            Assert.AreEqual(0m, pricing.OutputTokenCost);
            Assert.AreEqual(0m, pricing.CachedInputTokenCost);
        }

        [TestMethod]
        public void Ctor_WithBothPerTokenAndPerMillion_ThrowsArgumentException()
        {
            // Arrange
            static void Create() => _ = new testClass(inputTokenCost: 0.000001m,
                                                      outputTokenCost: 0.000002m,
                                                      cachedInputTokenCost: 0.0000005m,
                                                      reasoningTokenCost: null,
                                                      toolUseTokenCost: null,
                                                      inputCost: 1.50m,
                                                      outputCost: 6.00m,
                                                      cachedInputCost: 0.50m);

            // Act & Assert
            Throws<ArgumentException>(action: Create);
        }

        [TestMethod]
        public void Ctor_WithNegativeInputTokenCost_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            static void Create() => _ = new testClass(inputTokenCost: -0.000001m,
                                                      outputTokenCost: 0.000002m,
                                                      cachedInputTokenCost: 0.0000005m);

            // Act & Assert
            Throws<ArgumentOutOfRangeException>(action: Create);
        }

        [TestMethod]
        public void Ctor_WithNegativeOutputTokenCost_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            static void Create() => _ = new testClass(inputTokenCost: 0.000001m,
                                                      outputTokenCost: -0.000002m,
                                                      cachedInputTokenCost: 0.0000005m);

            // Act & Assert
            Throws<ArgumentOutOfRangeException>(action: Create);
        }

        [TestMethod]
        public void Ctor_WithNegativeCachedInputTokenCost_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            static void Create() => _ = new testClass(inputTokenCost: 0.000001m,
                                                      outputTokenCost: 0.000002m,
                                                      cachedInputTokenCost: -0.0000005m);

            // Act & Assert
            Throws<ArgumentOutOfRangeException>(action: Create);
        }

        [TestMethod]
        public void Ctor_WithNegativeInputCost_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            static void Create() => _ = new testClass(inputTokenCost: 0m,
                                                      outputTokenCost: 0m,
                                                      cachedInputTokenCost: 0m,
                                                      reasoningTokenCost: null,
                                                      toolUseTokenCost: null,
                                                      inputCost: -1m,
                                                      outputCost: null,
                                                      cachedInputCost: null);

            // Act & Assert
            Throws<ArgumentOutOfRangeException>(action: Create);
        }

        [TestMethod]
        public void Ctor_WithNegativeOutputCost_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            static void Create() => _ = new testClass(inputTokenCost: 0m,
                                                      outputTokenCost: 0m,
                                                      cachedInputTokenCost: 0m,
                                                      reasoningTokenCost: null,
                                                      toolUseTokenCost: null,
                                                      inputCost: null,
                                                      outputCost: -1m,
                                                      cachedInputCost: null);

            // Act & Assert
            Throws<ArgumentOutOfRangeException>(action: Create);
        }

        [TestMethod]
        public void Ctor_WithNegativeCachedInputCost_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            static void Create() => _ = new testClass(inputTokenCost: 0m,
                                                      outputTokenCost: 0m,
                                                      cachedInputTokenCost: 0m,
                                                      reasoningTokenCost: null,
                                                      toolUseTokenCost: null,
                                                      inputCost: null,
                                                      outputCost: null,
                                                      cachedInputCost: -1m);

            // Act & Assert
            Throws<ArgumentOutOfRangeException>(action: Create);
        }

        /// <summary>
        /// Helper method that asserts that executing the specified action throws an exception of
        /// type TException and returns the thrown exception.
        /// </summary>
        /// <typeparam name="TException">The type of exception expected to be thrown.</typeparam>
        /// <param name="action">The action expected to throw the exception.</param>
        /// <returns>The exception of type TException that was thrown.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the expected exception is not thrown by the action.</exception>
        private static TException Throws<TException>(Action action)
            where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException ex)
            {
                return ex; // exception was thrown as expected
            }
            catch (Exception ex)
            {
                Assert.Fail($"Expected exception of type {typeof(TException).Name}, " +
                            $"but got {ex.GetType().Name} instead.");
            }

            Assert.Fail($"Expected exception of type {typeof(TException).Name} was not thrown.");

            // This will never be reached, but compiler requires a return
            throw new InvalidOperationException();
        }
    }
}
