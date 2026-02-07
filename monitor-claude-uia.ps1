# PowerShell script to monitor Claude Desktop using UI Automation
param(
    [int]$PollIntervalMs = 500
)

# Add UI Automation assembly
Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

$ErrorActionPreference = 'Continue'

Write-Host "[INFO] Initializing UI Automation..."

# Function to find Claude Desktop window
function Find-ClaudeWindow {
    try {
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

        # Fallback: find any window with "Claude" in the name
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
    catch {
        Write-Host "[ERROR] Error finding window: $_"
        return $null
    }
}

# Function to check if Claude is streaming by looking for a Stop button
function Test-IsStreaming {
    param($window)

    try {
        # Look for buttons - the Stop/pause button only appears while streaming
        $buttonCondition = New-Object System.Windows.Automation.PropertyCondition(
            [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
            [System.Windows.Automation.ControlType]::Button
        )

        $buttons = $window.FindAll(
            [System.Windows.Automation.TreeScope]::Descendants,
            $buttonCondition
        )

        foreach ($button in $buttons) {
            $name = $button.Current.Name.ToLower()
            # Match stop/pause/cancel buttons that appear during streaming
            if ($name -match 'stop|pause|cancel generat|stop generat|abort') {
                return $true
            }
        }

        return $false
    }
    catch {
        Write-Host "[ERROR] Error checking streaming: $_"
        return $false
    }
}

# One-time dump of all interactive elements for debugging
function Dump-UIElements {
    param($window)

    Write-Host "[DEBUG] === UI Element Dump ==="

    # Dump buttons
    $buttonCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::Button
    )
    $buttons = $window.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $buttonCondition
    )
    Write-Host "[DEBUG] Buttons ($($buttons.Count)):"
    foreach ($button in $buttons) {
        $name = $button.Current.Name
        $autoId = $button.Current.AutomationId
        if ($name -or $autoId) {
            Write-Host "[DEBUG]   Button: Name='$name' AutomationId='$autoId'"
        }
    }

    # Dump other potentially useful elements (groups, custom controls)
    $groupCondition = New-Object System.Windows.Automation.PropertyCondition(
        [System.Windows.Automation.AutomationElement]::ControlTypeProperty,
        [System.Windows.Automation.ControlType]::Group
    )
    $groups = $window.FindAll(
        [System.Windows.Automation.TreeScope]::Descendants,
        $groupCondition
    )
    Write-Host "[DEBUG] Groups ($($groups.Count)):"
    foreach ($group in $groups) {
        $name = $group.Current.Name
        $autoId = $group.Current.AutomationId
        if ($name -or $autoId) {
            Write-Host "[DEBUG]   Group: Name='$name' AutomationId='$autoId'"
        }
    }

    Write-Host "[DEBUG] === End UI Element Dump ==="
}

Write-Host "[INFO] Searching for Claude Desktop window..."

$claudeWindow = $null
$wasStreaming = $false
$lastSearchTime = Get-Date
$hasDumped = $false

# Main monitoring loop
while ($true) {
    try {
        # Find Claude window if not already found
        if ($claudeWindow -eq $null) {
            $claudeWindow = Find-ClaudeWindow

            if ($claudeWindow -ne $null) {
                $windowName = $claudeWindow.Current.Name
                Write-Host "[INFO] Found Claude window: $windowName"
            }
            else {
                if ((Get-Date).Subtract($lastSearchTime).TotalSeconds -gt 10) {
                    Write-Host "[INFO] Waiting for Claude Desktop to start..."
                    $lastSearchTime = Get-Date
                }
                Start-Sleep -Milliseconds 1000
                continue
            }
        }

        # Check if window is still valid
        try {
            $windowName = $claudeWindow.Current.Name
        }
        catch {
            Write-Host "[INFO] Claude window closed, searching for new window..."
            $claudeWindow = $null
            $hasDumped = $false
            continue
        }

        # One-time dump of UI elements for debugging
        if (-not $hasDumped) {
            Dump-UIElements -window $claudeWindow
            $hasDumped = $true
        }

        # Check streaming state
        $isStreaming = Test-IsStreaming -window $claudeWindow

        if ($isStreaming -and -not $wasStreaming) {
            Write-Host "[STREAMING_START] Claude started streaming"
        }
        elseif (-not $isStreaming -and $wasStreaming) {
            Write-Host "[STREAMING_STOP] Claude stopped streaming"
        }

        $wasStreaming = $isStreaming

        Start-Sleep -Milliseconds $PollIntervalMs
    }
    catch {
        Write-Host "[ERROR] Monitoring error: $_"
        Start-Sleep -Milliseconds 1000
    }
}
