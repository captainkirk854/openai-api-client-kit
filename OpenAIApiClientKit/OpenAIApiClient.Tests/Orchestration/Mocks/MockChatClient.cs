// <copyright file="MockChatClient.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Tests.Orchestration.Mocks
{
    // Minimal fake ChatClient
    public sealed class MockChatClient(string apiKey, HttpClient? httpClient = null, int maxRetries = 3, int baseDelayMs = 500) : ChatClient(apiKey, httpClient, maxRetries, baseDelayMs)
    {
    }
}