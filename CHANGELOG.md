# Changelog


## [1.0.2] - 2026-02-07

## Changes in this Release

* feat: enhance release workflow with version bumping and changelog upd… (#5) @Looty

**Full Changelog**: https://github.com/Looty/claude-audio-cue/compare/v2026.2.10...



## [1.0.1] - 2026-02-07

## Changes in this Release

* feat: enhance release workflow with version bumping and changelog upd… (#5) @Looty

**Full Changelog**: https://github.com/Looty/claude-audio-cue/compare/v2026.2.10...


All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Toast task notifications - Windows 10/11 notification integration
- Multiple Claude window support
- Quiet hours / Do Not Disturb mode
- Webhook/HTTP callback integration
- Global hotkey support
- First-run setup wizard

## [Latest Release] - Rewrite in C# / .NET

### Added
- **Complete rewrite in C# .NET 8** - modern, performant Windows Forms application
- Windows UI Automation monitoring (FlaUI.UIA3) - reliable Electron app detection
- System tray integration with status indicator
- Settings dialog with full configuration UI
- Sound selection dropdown with built-in Windows sounds support
- Custom sound file support (user sounds folder in AppData)
- Volume control - adjustable from 0-200%
- Cooldown timer - prevents notification spam (default 3 seconds)
- Theme support - light and dark mode matching Windows preferences
- Auto-start with Windows option via Windows startup folder
- Response duration tracking - shows how long each response took
- Multiple status states: Idle, Searching, Monitoring, Streaming, Error
- Single-instance protection - prevents running multiple copies
- Settings persistence in JSON format

### Changed
- Switched from Node.js + PowerShell to compiled .NET executable
- Improved reliability with polling-based detection (500ms default)
- Self-contained single executable with no external dependencies

### Fixed
- More reliable Claude Desktop window detection
- Consistent behavior across Windows versions
- Better error handling and recovery

### Removed
- Legacy Node.js/npm requirements
- PowerShell script execution dependency


