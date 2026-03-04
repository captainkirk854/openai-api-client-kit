function Get-OpenAIModelInfo 
{
    param
    (
        [Parameter(Mandatory=$false)]
        [string]$ApiKey = $env:OPENAI_API_KEY,

        [Parameter(Mandatory=$false)]
        [string]$RegistryPath = "$PSScriptRoot/model-registry.json"
    )

    $apiModels = Get-OpenAIModels -ApiKey $ApiKey
    $registry = Get-Content $RegistryPath | ConvertFrom-Json

    $merged = foreach ($m in $apiModels) 
    {
        $id = $m.id
        $meta = $registry.$id

        if ($meta) {
            [PSCustomObject]@{
                Model            = $id
                OwnedBy          = $m.owned_by
                ContextWindow    = $meta.context_window
                MaxOutputTokens  = $meta.max_output_tokens
                InputCostUSD     = $meta.input_cost
                OutputCostUSD    = $meta.output_cost
                Capabilities     = $meta.capabilities
                UseCases         = $meta.use_cases
                CapabilitiesCSV  = ($meta.capabilities -join ", ")
                UseCasesCSV      = ($meta.use_cases -join ", ")
                ReleaseYear      = $meta.release_year
                Notes            = $meta.notes
            }
        }
    }

    return $merged
}