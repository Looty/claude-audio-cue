@echo off
echo Testing Windows UI Automation setup...
echo.
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0test-find-claude.ps1"
echo.
pause
