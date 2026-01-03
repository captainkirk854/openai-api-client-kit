// <copyright file="ToolCall.cs" company="854 Things (tm)">
// Copyright (c) 854 Things (tm). All rights reserved.
// </copyright>

namespace OpenAIApiClient.Models.Chat.Common
{
    public class ToolCall
    {
        /// <summary>
        /// Gets or sets the unique ID for the tool call (e.g., "call_123").
        /// </summary>
        required public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the tool/function being invoked.
        /// </summary>
        required public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets arguments passed to the tool.
        /// OpenAI returns these as structured JSON.
        /// </summary>
        required public Dictionary<string, object> Arguments
        {
            get;
            set;
        }
    }
}
