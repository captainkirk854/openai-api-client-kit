<#
----------------------------------------------------------------------------------------------------------------------------
        Date           Reason           Purpose
        ----           ------           -------
        02-03-2026     Created          Script to list available OpenAI models from PowerShell
----------------------------------------------------------------------------------------------------------------------------
#>

. $PSScriptRoot\scripts\common\Get-OpenAIModels.ps1

# Sample usage
Get-OpenAIModels | Sort-Object -Property id