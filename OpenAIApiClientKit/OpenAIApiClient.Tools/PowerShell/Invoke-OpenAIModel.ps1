<#
----------------------------------------------------------------------------------------------------------------------------
        Date           Reason           Purpose
        ----           ------           -------
        02-03-2026     Created          Script to invoke OpenAI API from PowerShell with support for multiple models and parameters
----------------------------------------------------------------------------------------------------------------------------
#>

#---------------------------------------------------------------------------------
function Invoke-OpenAIPrompt 
{
<#
  .SYNOPSIS
  Invoke OpenAI API from PowerShell with a user prompt, optional system prompt, and various parameters for different models (including GPT-5)
  .NOTES
  .LINK
#>
    [CmdletBinding()]
    param(
        # Required: Model
        [Parameter(Mandatory = $true)]
        [string]$Model,
        
        # Required: User prompt
        [Parameter(Mandatory = $true)]
        [string]$Prompt,

        # Optional: System prompt
        [string]$SystemPrompt,

        # Optional: API key (defaults to environment variable)
        [string]$ApiKey = $env:OPENAI_API_KEY,

        # Optional: OpenAI parameters
        [double]$Temperature = 1.0,
        [double]$TopP = 1.0,
        [int]$MaxTokens,
        [double]$PresencePenalty = 0.0,
        [double]$FrequencyPenalty = 0.0,
        [string[]]$Stop,
        [string]$User,
        [int]$Seed,
        [int]$N = 1,

        # Optional: JSON object for logit bias
        [hashtable]$LogitBias,

        # Optional: response format (e.g., "json_object")
        [string]$ResponseFormat
    )

    if (-not $ApiKey) {
        throw "No API key provided. Set OPENAI_API_KEY or pass -ApiKey."
    }

    $headers = @{
        "Authorization" = "Bearer $ApiKey"
        "Content-Type"  = "application/json"
    }

    # Build messages array
    $messages = @()

    if ($SystemPrompt) {
        $messages += @{
            role    = "system"
            content = $SystemPrompt
        }
    }

    $messages += @{
        role    = "user"
        content = $Prompt
    }

    # Build request body
    $body = @{
        model = $Model
        messages = $messages
        temperature = $Temperature # GPT-5 only supports 1.0
        top_p = $TopP              # Not supported by gpt-5
        n = $N
        presence_penalty = $PresencePenalty
        frequency_penalty = $FrequencyPenalty
    }

    if ($MaxTokens)      { $body.max_completion_tokens = $MaxTokens }
    if ($Stop)           { $body.stop = $Stop }
    if ($User)           { $body.user = $User }
    if ($Seed)           { $body.seed = $Seed }
    if ($LogitBias)      { $body.logit_bias = $LogitBias } # Not supported by gpt-5
    if ($ResponseFormat) { $body.response_format = @{ type = $ResponseFormat } }

    $json = $body | ConvertTo-Json -Depth 10

    # Invoke the API ..
    $url = "https://api.openai.com/v1/chat/completions"
    $response = Invoke-RestMethod -Uri $url -Headers $headers -Method Post -Body $json

    # Return the first completion
    return $response.choices[0].message.content
}
#---------------------------------------------------------------------------------



# Sample usage

# Basic
$response = Invoke-OpenAIPrompt -Model "gpt-5" `
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