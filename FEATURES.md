# Features

## Implemented ✅

### Core
- **Windows UI Automation monitoring** - detects Claude Desktop window and streaming activity
- **Audio notifications** - plays .wav file when Claude finishes responding
- **Windows system tray integration** - minimize to tray, control from notification area
- **Single-instance protection** - prevents running multiple copies

### Customization
- **Sound selection** - choose from Windows system sounds or custom .wav files
- **Volume control** - adjustable volume (0-200%)
- **Polling interval configuration** - adjust detection sensitivity (default 500ms)
- **Cooldown timer** - prevents spam from multiple quick responses (default 3 seconds)
- **Theme support** - light and dark mode

### Windows Integration
- **Auto-start with Windows** - optional startup on login
- **Window position saving** - remembers main window location

### Monitoring & User Feedback
- **Status indicator** - shows "Searching", "Monitoring", "Streaming", or "Error" status
- **Response duration tracking** - displays how long each Claude response took
- **Settings persistence** - stores preferences in JSON config file

## Planned 🚀

### Quality of Life
- Toast task notifications - Windows 10/11 toast notifications alongside audio
- Notification history - log of recent responses with timestamps

### Power User
- Per-window tracking - support multiple Claude Desktop windows simultaneously
- Quiet hours / Do Not Disturb - schedule times when notifications are suppressed
- Custom hotkeys - global keyboard shortcuts for app control
- Response statistics - track response times, frequency, etc.

### Reliability & Improvement
- Auto-reconnect - automatic recovery if Claude Desktop is restarted
- Dynamic UI element detection - adapt to Claude UI changes automatically
- Better error reporting - detailed logs for troubleshooting

### Integration
- System event notifications - integrate with Windows notification system

### Polish
- First-run setup wizard - guided initial configuration
- Update checker - automatic notification of new releases
- Portable mode - no installation required
- Settings UI improvements - context help and tooltips
