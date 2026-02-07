# claude-audio-cue

A lightweight Windows system tray application that monitors Claude Desktop and plays an audio notification when Claude finishes generating a response.

## Features

- ✅ **Works out of the box** - no special flags or configuration needed
- ✅ **Windows UI Automation** - monitors Claude using accessibility APIs (FlaUI.UIA3)
- ✅ **Automatic detection** - detects when Claude finishes generating responses
- ✅ **Audio notification** - plays a selected sound when response is complete
- ✅ **System tray integration** - minimize to tray and control from there
- ✅ **Lightweight** - minimal CPU and memory usage (~10-20 MB)
- ✅ **Sound selection** - choose from Windows system sounds or add custom .wav files
- ✅ **Volume control** - adjust notification volume (0-200%)
- ✅ **Cooldown timer** - prevents spam when Claude sends multiple short responses
- ✅ **Theme support** - light and dark mode matching Windows preferences
- ✅ **Auto-start with Windows** - optional startup with Windows
- ✅ **Response duration tracking** - displays how long Claude took to respond

## Prerequisites

- **Windows 10 or later**
- **[Claude Desktop](https://claude.ai/download)** (any recent version)
- **.NET 8 Runtime** (included in published .exe)

## Installation

1. Download the latest release from [GitHub Releases](https://github.com/Looty/claude-audio-cue/releases)
2. Extract `ClaudeAudioCue.exe` to any location
3. Run `ClaudeAudioCue.exe`
4. (Optional) Enable "Start with Windows" in the Settings

That's it! The application will automatically find Claude Desktop and start monitoring.

## Building from Source

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Windows 10 or later

### Build Commands

```bash
# Restore dependencies and build
dotnet build src/ClaudeAudioCue/ClaudeAudioCue.csproj

# Run in debug mode
dotnet run --project src/ClaudeAudioCue/ClaudeAudioCue.csproj

# Publish as single-file executable
dotnet publish src/ClaudeAudioCue/ClaudeAudioCue.csproj -c Release -r win-x64 --self-contained
```

The compiled executable will be in `src/ClaudeAudioCue/bin/Release/net8.0-windows/win-x64/publish/`

## How It Works

1. **Monitors Claude Desktop** using Windows UI Automation API (FlaUI.UIA3)
2. **Detects streaming** by checking for stop/pause/cancel buttons in the UI (indicates Claude is generating)
3. **Polls every 500ms** to check for state changes (proven reliable with Electron apps)
4. **Plays audio** when streaming ends and cooldown period has elapsed
5. **Tracks response duration** and updates status in the UI

## Configuration

Settings are stored in `%LOCALAPPDATA%\ClaudeAudioCue\settings.json`:

### GUI Settings
- **Sound** - select a .wav file from Windows Media or custom sounds folder
- **Volume** - adjust notification volume (0-200%)
- **Poll Interval** - how often to check for streaming (ms, default 500)
- **Cooldown** - minimum seconds between notifications (default 3)
- **Theme** - light or dark mode
- **Start with Windows** - auto-launch on startup

### Custom Sounds

Add custom .wav files to: `%LOCALAPPDATA%\ClaudeAudioCue\Sounds\`

The application will automatically detect and list them in the sound selector.

## Troubleshooting

### Claude Window Not Found

If the application shows "Searching..." status:

1. **Ensure Claude Desktop is running** and visible (not minimized to tray)
2. **Check the Claude window title:**
   - The application searches for windows containing "Claude" in the title
   - Most versions show "Claude" or "Anthropic Claude" in the title bar

3. **Restart the application** after opening Claude

### Not Detecting Streaming

If the status shows "Monitoring" but doesn't detect when Claude is generating:

1. **Increase the poll interval** slightly (try 750ms instead of 500ms) in settings
2. **Verify Claude is actually streaming** - the status should change to "Streaming" when Claude is responding
3. **Check Claude Desktop version** - if UI changed significantly, detection might need updates

To debug, check `%LOCALAPPDATA%\ClaudeAudioCue\` for any logs or diagnostic information.

### Multiple Notifications

If you're getting multiple quick notifications:

1. **Increase the cooldown timer** in settings (try 5-10 seconds)
2. **Restart the application** to ensure settings are loaded

### Running Multiple Instances

The application prevents multiple instances from running:

- Close the existing instance (check system tray)
- Use "Exit" from the tray menu to properly shut down
- Windows/System Tray right-click → close if needed
