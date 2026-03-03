function Get-OpenAIModels 
{
<#
  .SYNOPSIS
  Get a list of available OpenAI models from OpenAI API ..
  .NOTES
  .LINK
 #>
    param
    (
        [Parameter(Mandatory=$false)]
        [string]$ApiKey = $env:OPENAI_API_KEY
    )

    if (-not $ApiKey) 
    {
        throw "No API key provided. Set OPENAI_API_KEY or pass -ApiKey."
    }

    $headers = @{
        "Authorization" = "Bearer $ApiKey"
    }

    $response = Invoke-RestMethod `
        -Uri "https://api.openai.com/v1/models" `
        -Headers $headers `
        -Method GET

    return $response.data
}