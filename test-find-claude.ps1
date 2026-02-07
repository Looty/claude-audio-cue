# Quick test script to verify UI Automation can find Claude Desktop

Add-Type -AssemblyName UIAutomationClient
Add-Type -AssemblyName UIAutomationTypes

Write-Host "====================================="
Write-Host "Claude Desktop Window Finder"
Write-Host "====================================="
Write-Host ""

Write-Host "Searching for all windows..."
Write-Host ""

try {
    $allWindows = [System.Windows.Automation.AutomationElement]::RootElement.FindAll(
        [System.Windows.Automation.TreeScope]::Children,
        [System.Windows.Automation.Condition]::TrueCondition
    )

    $claudeFound = $false
    $windowList = @()

    foreach ($window in $allWindows) {
        try {
            $windowName = $window.Current.Name
            $processId = $window.Current.ProcessId

            if ($windowName) {
                $windowList += [PSCustomObject]@{
                    Name = $windowName
                    ProcessId = $processId
                    IsClaude = $windowName -match "Claude"
                }

                if ($windowName -match "Claude") {
                    $claudeFound = $true
                }
            }
        }
        catch {
            # Skip windows that can't be accessed
        }
    }

    # Show all windows
    Write-Host "Found $($windowList.Count) accessible windows:"
    Write-Host ""

    $windowList | Sort-Object -Property IsClaude -Descending | ForEach-Object {
        if ($_.IsClaude) {
            Write-Host "[✓] " -ForegroundColor Green -NoNewline
            Write-Host "$($_.Name) " -ForegroundColor Green -NoNewline
            Write-Host "(PID: $($_.ProcessId))" -ForegroundColor Gray
        }
        else {
            Write-Host "    $($_.Name) (PID: $($_.ProcessId))" -ForegroundColor DarkGray
        }
    }

    Write-Host ""
    Write-Host "====================================="

    if ($claudeFound) {
        Write-Host "✓ SUCCESS: Claude Desktop window found!" -ForegroundColor Green
        Write-Host "The monitor should work correctly." -ForegroundColor Green
    }
    else {
        Write-Host "✗ ERROR: Claude Desktop window not found!" -ForegroundColor Red
        Write-Host ""
        Write-Host "Possible solutions:" -ForegroundColor Yellow
        Write-Host "1. Make sure Claude Desktop is running" -ForegroundColor Yellow
        Write-Host "2. Make sure the Claude window is open (not minimized to tray)" -ForegroundColor Yellow
        Write-Host "3. Try restarting Claude Desktop" -ForegroundColor Yellow
    }

    Write-Host "====================================="
}
catch {
    Write-Host "ERROR: $_" -ForegroundColor Red
    exit 1
}
