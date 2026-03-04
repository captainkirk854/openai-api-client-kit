function Invoke-OpenAIPrompt 
{
<#
  .SYNOPSIS
  Invoke OpenAI API from PowerShell with a user prompt, optional system prompt, and various parameters for different models (including GPT-5)
  .NOTES
  .LINK
#>
    [CmdletBinding()]
    param
    (
        # Required: Model
        [Parameter(Mandatory = $true)]
        [ValidateSet("gpt-5","gpt-5.1","gpt-5.2","gpt-4","gpt-4.1","gpt-4.1-mini","gpt-4.1-nano","gpt-3.5-turbo")]
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

    # Determine if the model is a GPT-5 variant (some parameters are not supported)
    $isGpt5 = $Model -like "gpt-5*"

    # GPT-5 only supports temperature = 1.0
    if ($isGpt5 -and $Temperature -ne 1.0) {
        Write-Warning "Model '$Model' only supports temperature=1.0. Overriding supplied value ($Temperature)."
        $Temperature = 1.0
    }

    # Build request body
    $body = @{
        model = $Model
        messages = $messages
        temperature = $Temperature
        n = $N
        presence_penalty = $PresencePenalty
        frequency_penalty = $FrequencyPenalty
    }

    # top_p is not supported by gpt-5 models
    if (-not $isGpt5) { $body.top_p = $TopP }

    if ($MaxTokens)                  { $body.max_completion_tokens = $MaxTokens }
    if ($Stop)                       { $body.stop = $Stop }
    if ($User)                       { $body.user = $User }
    if ($Seed)                       { $body.seed = $Seed }
    if ($LogitBias -and -not $isGpt5) { $body.logit_bias = $LogitBias } # Not supported by gpt-5
    if ($ResponseFormat)             { $body.response_format = @{ type = $ResponseFormat } }

    $json = $body | ConvertTo-Json -Depth 10

    # Invoke the API ..
    $url = "https://api.openai.com/v1/chat/completions"
    $response = Invoke-RestMethod -Uri $url -Headers $headers -Method Post -Body $json

    # Return the first completion
    return $response.choices[0].message.content
}