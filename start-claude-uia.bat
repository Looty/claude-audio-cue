@echo off
REM Start Claude Desktop monitor using Windows UI Automation
REM This version doesn't require --remote-debugging-port flag

set CLAUDE_EXE=C:\Users\erezm\AppData\Local\AnthropicClaude\claude.exe

echo ================================================
echo Claude Desktop Audio Cue Monitor
echo Using Windows UI Automation API
echo ================================================
echo.

REM Check if Claude is already running
tasklist /FI "IMAGENAME eq claude.exe" 2>NUL | find /I /N "claude.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo Claude Desktop is already running.
    echo.
    echo Starting monitor...
    node monitor-uia.js
) else (
    if not exist "%CLAUDE_EXE%" (
        echo Claude Desktop not found at %CLAUDE_EXE%
        echo Please update the CLAUDE_EXE path in this script.
        pause
        exit /b 1
    )

    echo Starting Claude Desktop...
    start "" "%CLAUDE_EXE%"

    echo Waiting for Claude to start...
    timeout /t 5 /nobreak >nul

    echo.
    echo Starting audio-cue monitor...
    node monitor-uia.js
)
