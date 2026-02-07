# claude-audio-cue

Plays an audio cue (beep) when Claude Desktop finishes streaming a response.

**Now uses Windows UI Automation API** - works without requiring special debugging flags!

## Features

- ✅ **No special flags needed** - works with Claude Desktop out of the box
- ✅ **Windows UI Automation** - monitors Claude using accessibility APIs
- ✅ **Automatic detection** - detects when Claude finishes generating responses
- ✅ **Audio notification** - plays a beep sound when response is complete
- ✅ **Lightweight** - minimal CPU and memory usage

## Prerequisites

- [Node.js](https://nodejs.org/) (v18+)
- [Claude Desktop](https://claude.ai/download)

## Setup

1. Install dependencies:

```bash
npm install
```

2. Run the monitor:

```bash
# Option A: use the startup script (launches Claude + monitor)
start-claude-uia.bat

# Option B: if Claude is already running
npm start
```

That's it! The monitor will automatically find Claude Desktop and start monitoring for activity.

## How It Works

1. Uses Windows UI Automation API to find the Claude Desktop window
2. Monitors UI element changes, text updates, and accessibility events
3. Detects when Claude is actively generating responses
4. Waits for activity to stop (2 second timeout)
5. Plays an ASCII bell beep when the response is complete

## Configuration

You can adjust these settings in `monitor-uia.js`:

| Setting | Default | Description |
| ------- | ------- | ----------- |
| `POLL_INTERVAL_MS` | 500 | How often to check for UI changes (milliseconds) |
| `ACTIVITY_TIMEOUT_MS` | 2000 | How long to wait after last activity before beeping (milliseconds) |

## Customising the Sound

Replace the `beep()` function in `monitor-uia.js` with any sound playback method you prefer:

```javascript
function beep() {
  // Example: Play a WAV file using PowerShell
  const { execSync } = require('child_process');
  execSync('powershell -c (New-Object Media.SoundPlayer "C:\\Windows\\Media\\chimes.wav").PlaySync()');
  console.log('[claude-audio-cue] 🔔 Response complete!');
}
```

## Troubleshooting

### Claude Window Not Found

If the monitor can't find the Claude Desktop window:

1. **Make sure Claude Desktop is running:**
   - Open Task Manager and look for `claude.exe`
   - The window must be open (not just in system tray)

2. **Check the window title:**
   - The script looks for windows with "Claude" in the title
   - If Claude uses a different window title, you may need to adjust `Find-ClaudeWindow` in `monitor-claude-uia.ps1`

3. **Run Claude first, then the monitor:**
   ```bash
   # Start Claude Desktop manually, then run:
   npm start
   ```

### PowerShell Execution Policy Error

If you see "execution of scripts is disabled", run this in PowerShell as Administrator:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Monitor Not Detecting Activity

If the monitor runs but doesn't detect when Claude is responding:

1. **Adjust the timeout:**
   - Edit `ACTIVITY_TIMEOUT_MS` in `monitor-uia.js`
   - Increase it if beeps happen too early
   - Decrease it if beeps happen too late

2. **Check PowerShell output:**
   - Look for `[ACTIVITY]` messages in the console
   - If you see these, the detection is working
   - If not, Claude's UI might not trigger the expected events

3. **Try the event-based monitor:**
   - Edit `monitor-uia.js` and change the PowerShell script to `monitor-claude-uia-events.ps1`
   - This uses a different detection method

### False Positives

If you get beeps when Claude isn't actually done:

1. **Increase `ACTIVITY_TIMEOUT_MS`** in `monitor-uia.js` (try 3000-5000ms)
2. **Adjust `POLL_INTERVAL_MS`** to check less frequently (try 1000ms)

## Legacy CDP Method

The original Chrome DevTools Protocol method is still available in `beep-on-claude-done.js`, but Claude Desktop doesn't support the required `--remote-debugging-port` flag. If you're using a different Electron app that supports this flag:

```bash
npm run monitor-cdp
```
