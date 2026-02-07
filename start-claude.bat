@echo off
REM Start Claude Desktop with remote debugging enabled, then launch the monitor.
REM Adjust the Claude.exe path if your installation differs.

set CLAUDE_EXE=C:\Users\erezm\AppData\Local\AnthropicClaude\claude.exe

if not exist "%CLAUDE_EXE%" (
    echo Claude Desktop not found at %CLAUDE_EXE%
    echo Please update the CLAUDE_EXE path in this script.
    pause
    exit /b 1
)

echo Starting Claude Desktop with remote debugging on port 9222...
start "" "%CLAUDE_EXE%" --remote-debugging-port=9222

echo Waiting for Claude to start...
timeout /t 5 /nobreak >nul

echo Checking if CDP port is available...
node check-cdp-port.js

echo.
echo Starting audio-cue monitor...
set CDP_URL=http://127.0.0.1:9222
node beep-on-claude-done.js
