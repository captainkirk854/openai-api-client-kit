<#
----------------------------------------------------------------------------------------------------------------------------
        Date           Reason           Purpose
        ----           ------           -------
        02-03-2026     Created          Script to scrape OpenAI model pricing from the web and output structured JSON
----------------------------------------------------------------------------------------------------------------------------
#>

<#
----------------------------------------------------------------------------------------------------------------------------
Comments
----------------------------------------------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------------------------------
#>

<#
Usage Examples:
    > .\Get-OpenAIModelCosts.ps1
#>

#---------------------------------------------------------------------------------
# FUNCTIONS
#---------------------------------------------------------------------------------

#---------------------------------------------------------------------------------
function Normalise-PricePerMillion 
{
 <#
  .SYNOPSIS
  Normalise a textual price to "per 1M tokens" numeric value
  .NOTES
  .LINK
 #>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$Raw
    )

    if (-not $Raw) 
    {
        return $null
    }

    # Extract first numeric value (e.g., 0.030)
    $numMatch = [regex]::Match($Raw, '[-+]?\d+(\.\d+)?')
    if (-not $numMatch.Success) 
    {
        return $null
    }

    $value = [double]$numMatch.Value
    $lower = $Raw.ToLowerInvariant()

    # If explicitly per 1K, scale to per 1M
    if ($lower -match '1k' -or $lower -match 'per\s*1k' -or $lower -match 'per\s*thousand' -or $lower -match '1,000') 
    {
        return $value * 1000
    }

    # If explicitly per 1M, keep as-is
    if ($lower -match '1m' -or $lower -match 'per\s*million' -or $lower -match '1,000,000') 
    {
        return $value
    }

    # Fallback: assume already per 1M tokens
    return $value
}
#---------------------------------------------------------------------------------

#---------------------------------------------------------------------------------
function Format-Decimal 
{
 <#
  .SYNOPSIS
  Format a double as a fixed decimal string and strip trailing zeros
  .NOTES
  .LINK
 #>
    param
    (
        [double]$Value,
        [int]$Decimals = 12
    )

    if ($null -eq $Value) { return $null }

    # Fixed-point string with many decimals (no scientific notation)
    $fixed = $Value.ToString("F$Decimals", [System.Globalization.CultureInfo]::InvariantCulture)

    # Strip trailing zeros and then a trailing dot if present
    # e.g. "0.000000030000" -> "0.00000003"
    #      "1.000000000000" -> "1"
    $trimmed = [regex]::Replace($fixed, '0+$', '')
    $trimmed = [regex]::Replace($trimmed, '\.$', '')

    # Edge case: if it became empty (shouldn't happen), fall back to "0"
    if ([string]::IsNullOrWhiteSpace($trimmed)) {
        $trimmed = "0"
    }

    return $trimmed
}
#---------------------------------------------------------------------------------

#---------------------------------------------------------------------------------
function Convert-HtmlToLines 
{
<#
  .SYNOPSIS
  Convert raw HTML to an array of trimmed text lines, preserving some structure by treating block-level tags as line breaks
  .NOTES
  .LINK
#>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$Html
    )

    $text = $Html

    # Replace block-level tags with line breaks
    $text = $text -replace '(?i)</?(div|p|br|li|tr|td|th|h[1-6]|section|article|span)[^>]*>', "`n"

    # Remove remaining tags
    $text = $text -replace '<[^>]+>', ''

    # Decode HTML entities
    $text = [System.Net.WebUtility]::HtmlDecode($text)

    # Split into lines, trim, filter empties
    $lines = $text -split "`n" | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne '' }

    return $lines
}
#---------------------------------------------------------------------------------

#---------------------------------------------------------------------------------
function Test-IsModelLine 
{
<#
  .SYNOPSIS
  Heuristic test if a line of text is likely to be a model name, based on presence of certain keywords and absence of others
  .NOTES
  .LINK
#>
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$Line
    )

    # Skip lines that are clearly not model names:
    #  - start with '$'
    #  - purely numeric
    if ($Line -match '^\$') { return $false }
    if ($Line -match '^[0-9\.\-\,]+$') { return $false }

    $lower = $Line.ToLowerInvariant()

    # Require some hint it's a model name
    if (
        $lower -match 'gpt'     -or
        $lower -match '\bo[0-9]' -or
        $lower -match 'nano'    -or
        $lower -match 'mini'    -or
        $lower -match 'oss'     -or
        $lower -match 'codex'   -or
        $lower -match 'search'
    ) {
        return $true
    }

    return $false
}
#---------------------------------------------------------------------------------

#---------------------------------------------------------------------------------
function Get-ModelPricingFromLines 
{
<#
  .SYNOPSIS
  Given an array of text lines, identify model blocks and extract pricing info, returning structured objects
  .NOTES
  .LINK
#>
    param
    (
        [Parameter(Mandatory = $true)]
        [string[]]$Lines
    )

    $results    = @()
    $seenModels = @{}

    for ($i = 0; $i -lt $Lines.Count; $i++) 
    {
        $line = $Lines[$i]

        if (-not (Test-IsModelLine -Line $line)) 
        {
            continue
        }

        $modelName = $line

        if ($seenModels.ContainsKey($modelName)) 
        {
            continue
        }

        # Look ahead for the next two price lines (starting with $)
        $inputLine  = $null
        $outputLine = $null
        for ($j = $i + 1; $j -lt [Math]::Min($i + 15, $Lines.Count); $j++) 
        {
            $candidate = $Lines[$j]
            if ($candidate -match '^\$') {
                if (-not $inputLine) 
                {
                    $inputLine = $candidate
                }
                elseif (-not $outputLine) 
                {
                    $outputLine = $candidate
                    break
                }
            }
        }

        # If we didn't find two price lines, skip this model block
        if (-not $inputLine -or -not $outputLine) 
        {
            continue
        }

        # Normalize the price lines to per 1M tokens numeric values ..
        $inputPerM  = Normalise-PricePerMillion -Raw $inputLine
        $outputPerM = Normalise-PricePerMillion -Raw $outputLine

        # Calculate per-token prices ..
        $inputPerTokenValue  = if ($inputPerM  -ne $null) { $inputPerM  / 1000000 } else { $null }
        $outputPerTokenValue = if ($outputPerM -ne $null) { $outputPerM / 1000000 } else { $null }

        # Format as decimal strings without scientific notation and no trailing zeros ..
        $inputPerTokenStr  = if ($inputPerTokenValue  -ne $null) { Format-Decimal -Value $inputPerTokenValue  -Decimals 12 } else { $null }
        $outputPerTokenStr = if ($outputPerTokenValue -ne $null) { Format-Decimal -Value $outputPerTokenValue -Decimals 12 } else { $null }

        # Add to results ..
        $results += [pscustomobject]@{
            Model                  = $modelName
            RawInput               = $inputLine
            RawOutput              = $outputLine
            inputTokenCost_per_1M  = $inputPerM
            outputTokenCost_per_1M = $outputPerM
            inputTokenCost  = if ($inputPerTokenStr)  { $inputPerTokenStr  + 'm' } else { $null }
            outputTokenCost = if ($outputPerTokenStr) { $outputPerTokenStr + 'm' } else { $null }
        }

        $seenModels[$modelName] = $true
    }

    return $results
}
#---------------------------------------------------------------------------------

#---------------------------------------------------------------------------------
function Get-ModelPricingFromUrl
{
    param
    (
        [Parameter(Mandatory = $true)]
        [string]$Url
    )

    # Download the page (raw HTML) ..
    $response = Invoke-WebRequest -Uri $Url -UseBasicParsing
    $html = $response.Content
    if (-not $html)
    {
        throw "Failed to download content from $Url"
    }

    # Convert HTML to plain text lines ..
    $lines = Convert-HtmlToLines -Html $html

    # Identify model blocks ..
    $results = Get-ModelPricingFromLines -Lines $lines
    if ($results.Count -eq 0) 
    {
        throw "No model blocks with two price lines were found on $Url."
    }

    return $results
}
#---------------------------------------------------------------------------------


#---------------------------------------------------------------------------------
# MAIN
#---------------------------------------------------------------------------------
# Initialise ...

# Go ..
Write-Host

# Get pricing from both sources, combine and dedupe by model name (keeping the first occurrence) ..
$models1 = Get-ModelPricingFromUrl -Url "https://pricepertoken.com/pricing-page/provider/openai"
$models2 = Get-ModelPricingFromUrl -Url "https://developers.openai.com/api/docs/pricing?latest-pricing=standard"
$allModels =
    $models1 +
    $models2 |
    Group-Object -Property Model |
    ForEach-Object {
        # Pick the most expensive variant of this model
        $_.Group |
            Sort-Object @{ Expression = { ($_.input_cost_per_1M  -as [double]) + ($_.output_cost_per_1M -as [double]) } ; Descending = $true } |
            Select-Object -First 1
    } |
    Sort-Object Model

# Grid .. 
$allModels | Out-GridView -Title "Scraped Model Costs" -OutputMode Single

# Output as JSON (sorted by model name) ..
$json = $allModels | Sort-Object Model | ConvertTo-Json -Depth 5
$json