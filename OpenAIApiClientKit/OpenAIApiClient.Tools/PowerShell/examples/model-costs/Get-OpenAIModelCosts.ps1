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
A crude, somewhat fragile script that provides some information about OpenAI model pricing.
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
    if ($Line -match '^\$') { return $false }
    #  - purely numeric
    if ($Line -match '^[0-9\.\-\,]+$') { return $false }
    # - numerous non-model texts ..
    if ($Line -match "File Search") {return $false }
    if ($Line -match "Web Search") {return $false }
    if ($Line -match ",") {return $false }

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
  Model entries frequently occur more than once (and are not always visible on the visible web-page).
  This algorithm will collect all the $prices - expect some inconsistencies - blame it on the source :)
  
  e.g.
  This algorithm expects the following order in pricing structure: [ $input | $cachedinput | $output ]
  Some tables are structured:
    [ $input | $cachedinput | $output ]             :)
    [ $input | - | $output ]                        :|
    [ $input | $output ]                            :(
    [ $training | $input | $cachedinput | $output ] GRRRRRR ...

  Tabulated data on these websites appear to be script-driven without ay of the usual table-related tags
  to attach to.
  .LINK
#>
    param
    (
        [Parameter(Mandatory = $true)]
        [string[]]$Lines,

        [Parameter(Mandatory = $false)]
        [string[]]$Url
    )

    $results    = @()
    $seenModels = @{}

    for ($i = 0; $i -lt $Lines.Count; $i++) 
    {
        $line = $Lines[$i]

        # Skip if $line is html garbage ..
        if (-not (Test-IsModelLine -Line $line)) 
        {
            continue
        }

        # Assign Model Name ..
        $modelName = $line

        # Look ahead for the next two (or three) $price lines (starting with US Dollar $ or in rare cases, no value - indicated by a '-') ..
        $inputCostLine  = $null
        $cachedInputCostLine  = $null
        $outputCostLine = $null
        for ($j = $i + 1; $j -lt [Math]::Min($i + 15, $Lines.Count); $j++) 
        {
            $candidate = $Lines[$j]

            # Look for model $pricing with this layout: [input | cachedinput | output] and ignore multi-pricings in one block e.g. $1.50/$0.65/$3.23)
            if (($candidate -match '^\$' -or ($candidate -eq '-')) -and -not $candidate.Contains('/')) 
            {
                if (-not $inputCostLine) 
                {
                    $inputCostLine = $candidate
                }
                elseif (-not $cachedInputCostLine) 
                {
                    $cachedInputCostLine = $candidate
                }
                elseif (-not $outputCostLine) 
                {
                    $outputCostLine = $candidate
                    break
                }
            }
        }

        # In those cases were CachedInputCost is valuated with '-' on the website, erroneously setting outputCostLine, reset its value ..
        if ($outputCostLine -eq '-' -and $cachedInputCostLine.Length -gt 0)
        {
          $outputCostLine = $cachedInputCostLine
        }

        # In those cases were CachedInputCost seems to be greater than OutputCost (usually because of change in order of columns on web-page), swap values ..
        if ($outputCostLine -and $cachedInputCostLine)
        {
            $numericOutputPerM = Normalise-PricePerMillion -Raw $outputCostLine
            $numericCachedPerM = Normalise-PricePerMillion -Raw $cachedInputCostLine
            if ($numericOutputPerM -ne $null -and $numericCachedPerM -ne $null -and $numericOutputPerM -lt $numericCachedPerM)
            {
                $temp = $outputCostLine
                $outputCostLine = $cachedInputCostLine
                $cachedInputCostLine = $temp
            }
        }

        # If we didn't find the two price lines we care about (so ignore cached input), skip this model block
        if (-not $inputCostLine -or -not $outputCostLine) 
        {
            continue
        }

        # Normalize the price lines to per 1M tokens numeric values ..
        $inputPerM  = Normalise-PricePerMillion -Raw $inputCostLine
        $outputPerM = Normalise-PricePerMillion -Raw $outputCostLine

        # Calculate per-token prices ..
        $inputPerTokenValue  = if ($inputPerM  -ne $null) { $inputPerM  / 1000000 } else { $null }
        $outputPerTokenValue = if ($outputPerM -ne $null) { $outputPerM / 1000000 } else { $null }

        # Format as decimal strings without scientific notation and no trailing zeros ..
        $inputPerTokenStr  = if ($inputPerTokenValue  -ne $null) { Format-Decimal -Value $inputPerTokenValue  -Decimals 12 } else { $null }
        $outputPerTokenStr = if ($outputPerTokenValue -ne $null) { Format-Decimal -Value $outputPerTokenValue -Decimals 12 } else { $null }

        # Add to results ..
        $results += [pscustomobject]@{
            Model                  = $modelName.ToLower() -replace(" ","-") -replace("\(","") -replace("\)","") # transform to standardise model name
            RawInput               = $inputCostLine
            RawOutput              = $outputCostLine
            InputTokenCost_per_1M  = $inputPerM
            OutputTokenCost_per_1M = $outputPerM
            inputTokenCost  = if ($inputPerTokenStr)  { $inputPerTokenStr  + 'm' } else { $null }
            outputTokenCost = if ($outputPerTokenStr) { $outputPerTokenStr + 'm' } else { $null }
            SourceUrl = $Url
        }

        $seenModels[$modelName] = $true
    }

    return $results
}
#---------------------------------------------------------------------------------

#---------------------------------------------------------------------------------
function Get-MostExpensiveModel 
{
    param
    (
        [Parameter(Mandatory = $true)]
        [object[]]$Group
    )

    $best = $null
    $bestScore = [double]::MinValue

    foreach ($m in $Group) 
    {
        $input  = if ($m.PSObject.Properties.Match('InputTokenCost_per_1M'))  { [double]($m.InputTokenCost_per_1M  -as [double]) } else { 0 }
        $output = if ($m.PSObject.Properties.Match('OutputTokenCost_per_1M')) { [double]($m.OutputTokenCost_per_1M -as [double]) } else { 0 }

        # Define score as: sum of input + output
        $score = $input + $output

        if ($score -gt $bestScore) 
        {
            $bestScore = $score
            $best = $m
        }
    }

    return $best
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
    $results = Get-ModelPricingFromLines -Lines $lines -Url $url
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
$cfgPickMostExpensiveModelVariant = $false # if true, will keep only the most expensive variant of each model; if false, will keep up to 5 variants per model (sorted by cost)

# Go ..
Write-Host

# Get pricing from both sources, combine and dedupe by model name (keeping the first occurrence) ..
$models1 = Get-ModelPricingFromUrl -Url "https://pricepertoken.com/pricing-page/provider/openai"
$models2 = Get-ModelPricingFromUrl -Url "https://developers.openai.com/api/docs/pricing?latest-pricing=standard"

<#
$allModels =
    $models1 + $models2 |
    Group-Object -Property model |
    ForEach-Object {
        Get-MostExpensiveModel -Group $_.Group
    } |
    Sort-Object model
#>

<#
$allModels =
    $models1 + $models2 |
    Group-Object -Property Model |
    ForEach-Object {
        # Pick the most expensive variant of this model
        $_.Group |
            Sort-Object @{
                 # Robust numeric sum; treat missing/null as 0 ..
                Expression = { 
                    ($_.InputTokenCost_per_1M  -as [double]) + 
                    ($_.OutputTokenCost_per_1M -as [double]) 
                } 
                Descending = $true # highest cost first
              } |
              Select-Object -First $(if ($cfgPickMostExpensiveModelVariant) { 1 } else { 5 } )
    }  |
    Sort-Object Model
#>


# Combine raw models from both sources (keeping duplicates / variants)
$combined = $models1 + $models2

# For each model, sort its variants by cost, assign index that resets per model
$allModels =
    $models1 + $models2 |
    Group-Object -Property model |
    ForEach-Object {
        $group = $_.Group

        # Sort variants for this model from most expensive to cheapest
        $sorted = $group | 
            Sort-Object -Property @{
                # Robust numeric sum; treat missing/null as 0 ..
                Expression = {
                    ([double]($_.InputTokenCost_per_1M  -as [double])) +
                    ([double]($_.OutputTokenCost_per_1M -as [double]))
                }
            Descending = $true
        }

        # Optionally pick only the most expensive variant (if config is set), otherwise keep some additional variants for this model
        $sorted = $sorted  |
            Select-Object -First $(if ($cfgPickMostExpensiveModelVariant) { 1 } else { 5 } )
        
        # Assign an index to each variant of this model, starting at 1 for the most expensive, that can be used to sort variants of the same model together
        $localIndex = 1
        foreach ($row in $sorted) {
            $row | Add-Member -NotePropertyName CostIndex -NotePropertyValue $localIndex -Force
            $localIndex++
            $row
        }
    } |
    Sort-Object model, CostIndex


# Grid .. 
$allModels | Out-GridView -Title "Scraped Model Costs" -PassThru | Format-Table -AutoSize

# Output as JSON (sorted by model name) ..
$json = $allModels | Sort-Object Model | ConvertTo-Json -Depth 5
$json