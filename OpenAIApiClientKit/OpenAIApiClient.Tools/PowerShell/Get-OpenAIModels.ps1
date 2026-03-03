<#
----------------------------------------------------------------------------------------------------------------------------
        Date           Reason           Purpose
        ----           ------           -------
        02-03-2026     Created          Script to list available OpenAI models from PowerShell
----------------------------------------------------------------------------------------------------------------------------
#>

#---------------------------------------------------------------------------------
function Get-OpenAIModels 
{
<#
  .SYNOPSIS
  Get a list of available OpenAI models from PowerShell
  .NOTES
  .LINK
#>
    param
    (
        [string]$ApiKey = $env:OPENAI_API_KEY
    )

    Invoke-RestMethod `
        -Uri "https://api.openai.com/v1/models" `
        -Headers @{ "Authorization" = "Bearer $ApiKey" } `
        -Method Get
}
#---------------------------------------------------------------------------------



# Sample usage
(Get-OpenAIModels).data | Sort-Object -Property id