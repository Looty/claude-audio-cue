# Enhanced PowerShell script using UI Automation Events
# Monitors Claude Desktop for text/content changes

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes
Add-Type -AssemblyName WindowsBase

$ErrorActionPreference = 'Continue'

Write-Host "[INFO] Initializing UI Automation with event-based monitoring..."

# Track activity
$script:lastEventTime = [DateTime]::MinValue
$script:eventCount = 0

# Function to find Claude Desktop window
function Find-ClaudeWindow {
    try {
        $allWindows = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
            [System.Windows.Automation.TreeScope]::Children,
            [System.Windows.Automation.Condition]::TrueCondition
        )

        foreach ($window in $allWindows) {
            $windowName = $window.Current.Name
            if ($windowName -match "Claude") {
                Write-Host "[INFO] Found Claude window: $windowName"
                return $window
            }
        }

        return $null
    }
    catch {
        Write-Host "[ERROR] Error finding window: $_"
        return $null
    }
}

# Event handler for structure changes
$structureChangedHandler = {
    param($sender, $e)
    $script:lastEventTime = Get-Date
    $script:eventCount++
    Write-Host "[ACTIVITY] UI structure changed (event #$($script:eventCount))"
}

# Event handler for property changes
$propertyChangedHandler = {
    param($sender, $e)
    $script:lastEventTime = Get-Date
    $script:eventCount++
    Write-Host "[ACTIVITY] Property changed (event #$($script:eventCount))"
}

# Event handler for text changes
$textChangedHandler = {
    param($sender, $e)
    $script:lastEventTime = Get-Date
    $script:eventCount++
    Write-Host "[ACTIVITY] Text changed (event #$($script:eventCount))"
}

Write-Host "[INFO] Searching for Claude Desktop window..."

# Find Claude window
$claudeWindow = Find-ClaudeWindow

if ($claudeWindow -eq $null) {
    Write-Host "[ERROR] Could not find Claude Desktop window."
    Write-Host "[INFO] Please make sure Claude Desktop is running and try again."
    exit 1
}

Write-Host "[INFO] Setting up UI Automation event listeners..."

try {
    # Register for structure changed events
    [System.Windows.Automation.Automation]::AddStructureChangedEventHandler(
        $claudeWindow,
        [System.Windows.Automation.TreeScope]::Descendants,
        $structureChangedHandler
    )

    # Register for property changed events (for text content)
    [System.Windows.Automation.Automation]::AddAutomationPropertyChangedEventHandler(
        $claudeWindow,
        [System.Windows.Automation.TreeScope]::Descendants,
        $propertyChangedHandler,
        @([System.Windows.Automation.AutomationElement]::NameProperty)
    )

    Write-Host "[INFO] Event listeners registered successfully."
    Write-Host "[INFO] Monitoring Claude Desktop for activity..."
    Write-Host "[INFO] Press Ctrl+C to stop."
    Write-Host ""

    # Keep script running
    while ($true) {
        Start-Sleep -Milliseconds 100

        # Periodically check if window still exists
        try {
            $null = $claudeWindow.Current.Name
        }
        catch {
            Write-Host "[INFO] Claude window closed."
            break
        }
    }
}
catch {
    Write-Host "[ERROR] Error setting up event handlers: $_"
    exit 1
}
finally {
    Write-Host "[INFO] Cleaning up event handlers..."
    try {
        [System.Windows.Automation.Automation]::RemoveAllEventHandlers()
    }
    catch {
        # Ignore cleanup errors
    }
}
