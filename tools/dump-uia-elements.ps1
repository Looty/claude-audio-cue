# Diagnostic script: Dumps ALL UI Automation elements from the Claude Desktop window.
# Use this to discover element names/types for streaming detection.
#
# Usage:
#   powershell -ExecutionPolicy Bypass -File dump-uia-elements.ps1
#   powershell -ExecutionPolicy Bypass -File dump-uia-elements.ps1 -OutputFile dump.txt
#
# Run this while Claude is STREAMING and again while IDLE, then diff the outputs.

param(
    [string]$OutputFile = "",
    [int]$MaxDepth = 8
)

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

$ErrorActionPreference = 'Continue'

function Find-ClaudeWindow {
    $windowNames = @("Claude", "Anthropic Claude", "Claude Desktop")

    foreach ($name in $windowNames) {
        $condition = New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::NameProperty,
            $name
        )
        $window = [System.Windows.Automation.AutomationElement]::RootElement.FindFirst(
            [System.Windows.Automation.TreeScope]::Children,
            $condition
        )
        if ($window -ne $null) {
            return $window
        }
    }

    # Fallback: any window with "Claude" in the name
    $allWindows = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
        [System.Windows.Automation.TreeScope]::Children,
        [System.Windows.Automation.Condition]::TrueCondition
    )
    foreach ($window in $allWindows) {
        $windowName = $window.Current.Name
        if ($windowName -match "Claude") {
            return $window
        }
    }

    return $null
}

function Dump-Element {
    param(
        $Element,
        [int]$Depth = 0,
        [int]$MaxDepth = 8
    )

    if ($Depth -gt $MaxDepth) { return }

    $indent = "  " * $Depth
    $name = $Element.Current.Name
    $autoId = $Element.Current.AutomationId
    $controlType = $Element.Current.ControlType.ProgrammaticName
    $className = $Element.Current.ClassName
    $rect = $Element.Current.BoundingRectangle
    $isEnabled = $Element.Current.IsEnabled

    # Only print elements that have some identifying info
    if ($name -or $autoId -or $className) {
        $line = "${indent}[${controlType}] Name='$name' AutomationId='$autoId' Class='$className' Enabled=$isEnabled"
        if ($rect.Width -gt 0 -and $rect.Height -gt 0) {
            $line += " Rect=($([int]$rect.X),$([int]$rect.Y),$([int]$rect.Width)x$([int]$rect.Height))"
        }
        $script:output += $line + "`n"
    }

    # Recurse into children
    try {
        $children = $Element.FindAll(
            [System.Windows.Automation.TreeScope]::Children,
            [System.Windows.Automation.Condition]::TrueCondition
        )
        foreach ($child in $children) {
            Dump-Element -Element $child -Depth ($Depth + 1) -MaxDepth $MaxDepth
        }
    }
    catch {
        # Some elements may not allow child enumeration
    }
}

# Main
Write-Host "Searching for Claude Desktop window..."
$window = Find-ClaudeWindow

if ($window -eq $null) {
    Write-Host "ERROR: Claude Desktop window not found. Make sure it's running."
    exit 1
}

$windowName = $window.Current.Name
Write-Host "Found window: '$windowName'"
Write-Host "Dumping UI elements (max depth: $MaxDepth)..."
Write-Host ""

$script:output = "=== Claude UI Automation Element Dump ===`n"
$script:output += "Window: '$windowName'`n"
$script:output += "Timestamp: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')`n"
$script:output += "=========================================`n`n"

Dump-Element -Element $window -Depth 0 -MaxDepth $MaxDepth

$script:output += "`n=== End Dump ===`n"

if ($OutputFile) {
    $script:output | Out-File -FilePath $OutputFile -Encoding UTF8
    Write-Host "Output written to: $OutputFile"
}
else {
    Write-Host $script:output
}

Write-Host "`nDone. Element count: $(($script:output -split "`n" | Where-Object { $_ -match '^\s*\[' }).Count)"
