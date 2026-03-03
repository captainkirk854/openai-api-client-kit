<#
----------------------------------------------------------------------------------------------------------------------------
        Date           Reason           Purpose
        ----           ------           -------
        02-03-2026     Created          Script to invoke GPT-5 API from PowerShell
----------------------------------------------------------------------------------------------------------------------------
#>

<#
Usage Examples:
    > Invoke-GPT5Prompt -Prompt "List the planets and their diameters in order of size"
#>

#---------------------------------------------------------------------------------
function Invoke-GPT5Prompt 
{
<#
  .SYNOPSIS
  Invoke GPT-5 API from PowerShell with a user prompt and optional parameters
  .NOTES
  Some parameters (like temperature) are rejected by the GPT-5 API, but are included for consistency with other models and future compatibility.
  .LINK
#>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$Prompt,

        [string]$ApiKey = $env:OPENAI_API_KEY,

        # Optional: adjust temperature if desired
        [double]$Temperature = 1.0 # GPT-5 only supports a value of 1.0 (!)
    )

    if (-not $ApiKey) {
        throw "No API key provided. Set OPENAI_API_KEY or pass -ApiKey."
    }

    $headers = @{
        "Authorization" = "Bearer $ApiKey"
        "Content-Type"  = "application/json"
    }

    $body = @{
        model = "gpt-5"
        messages = @(
            @{
                role    = "user"
                content = $Prompt
            }
        )
        temperature = $Temperature
    } | ConvertTo-Json -Depth 5

    # Invoke the API ..
    $url = "https://api.openai.com/v1/chat/completions"
    $response = Invoke-RestMethod -Uri $url -Headers $headers -Method Post -Body $body
    return $response.choices[0].message.content
}
#---------------------------------------------------------------------------------