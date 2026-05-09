[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ToolName,

    [Parameter(Mandatory = $true)]
    [string]$PayloadJson,

    [switch]$PassThru
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function ConvertTo-PlainObject {
    param(
        [Parameter(Mandatory = $true)]
        [object]$InputObject
    )

    if ($null -eq $InputObject) {
        return @{}
    }

    if ($InputObject -is [hashtable]) {
        return $InputObject
    }

    if ($InputObject -is [System.Collections.IDictionary]) {
        $table = @{}
        foreach ($key in $InputObject.Keys) {
            $table[$key] = $InputObject[$key]
        }

        return $table
    }

    $tableFromProperties = @{}
    foreach ($property in $InputObject.PSObject.Properties) {
        $tableFromProperties[$property.Name] = $property.Value
    }

    return $tableFromProperties
}

function Assert-RequiredTextField {
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Payload,

        [Parameter(Mandatory = $true)]
        [string]$FieldName,

        [Parameter(Mandatory = $true)]
        [string]$Tool
    )

    if (-not $Payload.ContainsKey($FieldName)) {
        throw "Blocked call: '$Tool' requires field '$FieldName'. Payload keys: [$($Payload.Keys -join ', ')]"
    }

    $value = $Payload[$FieldName]
    if ($null -eq $value -or [string]::IsNullOrWhiteSpace([string]$value)) {
        throw "Blocked call: '$Tool' requires non-empty '$FieldName'."
    }
}

# Endpoints that fail with 400 when query is missing or empty.
$requiredTextFieldsByTool = @{
    "mcp_umbraco-mcp_get-item-member-search" = @("query")
    "mcp_umbraco-mcp_get-template-search" = @("query")
    "mcp_umbraco-mcp_get-data-type-search" = @("query")
    "mcp_umbraco-mcp_get-document-type-tree-search" = @("query")
}

try {
    $payloadObject = ConvertFrom-Json -InputObject $PayloadJson -ErrorAction Stop
}
catch {
    throw "Payload is not valid JSON. $_"
}

$payload = ConvertTo-PlainObject -InputObject $payloadObject

if ($requiredTextFieldsByTool.ContainsKey($ToolName)) {
    foreach ($field in $requiredTextFieldsByTool[$ToolName]) {
        Assert-RequiredTextField -Payload $payload -FieldName $field -Tool $ToolName
    }
}

Write-Host "Payload validation passed for $ToolName" -ForegroundColor Green

if ($PassThru) {
    $payload | ConvertTo-Json -Depth 16
}
