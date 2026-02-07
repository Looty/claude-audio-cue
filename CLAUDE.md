# Claude Audio Cue

## What This Is
A lightweight Windows system tray app that monitors Claude Desktop and plays an audio notification when Claude finishes generating a response. Works with both regular chat and Claude Code inside the Desktop app.

## Tech Stack
- **C# / .NET 8** (LTS) — WinForms
- **FlaUI.UIA3** — managed wrapper for Windows UI Automation (UIA3 COM)
- **System.Media.SoundPlayer** — .wav playback
- Publishes as a single self-contained `.exe`

## Project Structure
```
src/ClaudeAudioCue/
  Program.cs              # Entry point, single-instance mutex
  MainForm.cs             # WinForms UI + system tray (NotifyIcon)
  MainForm.Designer.cs    # Designer-generated layout
  ClaudeMonitor.cs        # Background STA thread, state machine, polling loop
  StreamingDetector.cs    # UIA3 window finding + streaming detection
  AudioPlayer.cs          # SoundPlayer wrapper + C:\Windows\Media\*.wav enumeration
  AppSettings.cs          # JSON settings persisted to %LOCALAPPDATA%\ClaudeAudioCue\
  Resources/app-icon.ico  # App/tray icon

tools/
  dump-uia-elements.ps1  # Diagnostic: dumps all UIA elements from Claude window

legacy/                   # Original Node.js + PowerShell implementation (reference only)
```

## Build Commands
```bash
# Restore + build
dotnet build src/ClaudeAudioCue/ClaudeAudioCue.csproj

# Publish single-file exe (trimming not in csproj — WinForms doesn't support it at build time)
dotnet publish src/ClaudeAudioCue/ClaudeAudioCue.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

Output lands in `src/ClaudeAudioCue/bin/Release/net8.0-windows/win-x64/publish/`.

## Architecture Notes

### Threading
- **UI thread** (STA) — runs WinForms, receives status updates via `IProgress<MonitorStatus>`
- **Monitor thread** (STA) — runs `ClaudeMonitor.MonitorLoop()`, required STA for UIA3 COM interop
- Communication: `IProgress<T>` pattern auto-marshals callbacks to UI thread

### Streaming Detection
- **Polling** (500ms) — UIA automation events are unreliable with Electron apps
- Finds Claude window by title: "Claude" / "Anthropic Claude" / "Claude Desktop"
- Detects streaming by scanning descendant buttons for stop/pause/cancel/interrupt patterns
- On streaming → not-streaming transition: plays the selected .wav sound

### State Machine (ClaudeMonitor)
`Idle → Searching → Monitoring → StreamingDetected → Monitoring`

### Settings
Stored as JSON at `%LOCALAPPDATA%\ClaudeAudioCue\settings.json`:
- `SelectedSound` — filename from C:\Windows\Media
- `PollIntervalMs` — polling interval (default 500)

## Debugging UI Automation
If streaming detection stops working (e.g., Claude Desktop UI changes):
```powershell
# Dump all UIA elements from the Claude window while it's streaming
powershell -ExecutionPolicy Bypass -File tools/dump-uia-elements.ps1
```
Compare idle vs streaming dumps to find the differentiating elements.

## Key Design Decisions
- **FlaUI.UIA3 over System.Windows.Automation** — UIA3 is more reliable with Electron apps than the old UIA2
- **Polling over events** — UIA events are flaky with Electron; 500ms polling is proven lightweight
- **No trimming** — WinForms doesn't support IL trimming; publish as single-file without trimming
- **Minimize to tray on close** — app stays resident until explicitly exited from tray menu

## Legacy Files
The `legacy/` folder contains the original Node.js + PowerShell implementation:
- `monitor-uia.js` — Node.js wrapper that spawned PowerShell
- `monitor-claude-uia.ps1` — PowerShell UI Automation monitor
- `beep-on-claude-done.js` — deprecated CDP-based approach
These are kept for reference only.
