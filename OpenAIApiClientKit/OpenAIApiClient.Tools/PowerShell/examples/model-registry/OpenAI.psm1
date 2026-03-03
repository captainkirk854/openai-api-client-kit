# Export Module Functions for OpenAI API Client Tools

.  $PSScriptRoot\..\..\scripts\common\Get-OpenAIModels.ps1
.  $PSScriptRoot\Get-OpenAIModelInfo.ps1

Export-ModuleMember -Function Get-OpenAIModels, Get-OpenAIModelInfo

<#
Sample Usage:

    > Import-Module ./OpenAI.psm1 -Verbose
    > Get-OpenAIModelInfo | Format-Table
    > Get-OpenAIModelInfo | Format-Table Model, InputCostUSD, OutputCostUSD, CapabilitiesCSV  
#>