<#
----------------------------------------------------------------------------------------------------------------------------
        Date           Reason           Purpose
        ----           ------           -------
        02-03-2026     Created          Script to invoke OpenAI API from PowerShell with support for multiple models and parameters
----------------------------------------------------------------------------------------------------------------------------
#>

. $PSScriptRoot\scripts\common\Invoke-OpenAIModel.ps1

# Sample usage

<#
# Basic
$response = Invoke-OpenAIPrompt -Model "gpt-4.1-mini" `
                                -Prompt "Explain the theory of relativity in simple terms."
$response

Write-Host "---------------------------------------------------------------------------------------------"

# With system prompt and additional parameters
$response = Invoke-OpenAIPrompt -Model "gpt-4.1-mini" `
                                -Prompt "Summarize the key points of the formula E=MC^2." `
                                -SystemPrompt "You are a concise summarizer." `
                                -MaxTokens 150 `
                                -Temperature 1.0 `
                                -TopP 0.9
$response

Write-Host "---------------------------------------------------------------------------------------------"

# With logit bias and response format
$response = Invoke-OpenAIPrompt -Model "gpt-3.5-turbo" `
                                -Prompt "Generate a JSON object with user details." `
                                -LogitBias @{ "50256" = -100 } `
                                -ResponseFormat "json_object"
$response
Write-Host "---------------------------------------------------------------------------------------------"
Write-Host
#>