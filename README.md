# claude-audio-cue

Plays an audio cue (beep) when Claude Desktop finishes streaming a response.

Uses Playwright to connect to Claude Desktop via Chrome DevTools Protocol (CDP) and polls the DOM for a streaming indicator.

## Prerequisites

- [Node.js](https://nodejs.org/) (v18+)
- [Claude Desktop](https://claude.ai/download)

## Setup

1. Install dependencies:

```bash
npm install
```

2. Find Claude's streaming DOM selector (one-time):

   - Launch Claude Desktop with debugging: `Claude.exe --remote-debugging-port=9222`
   - Open Chrome and go to `chrome://inspect`
   - Click **inspect** under the Claude Desktop target
   - Start a conversation and look for the element that appears while streaming (e.g. `.streaming-indicator`, `[data-streaming="true"]`)
   - Update the `STREAMING_SELECTORS` array in `beep-on-claude-done.js`

3. Run the monitor:

```bash
# Option A: use the startup script (launches Claude + monitor)
# Windows
start-claude.bat

# macOS / Linux
chmod +x start-claude.sh
./start-claude.sh

# Option B: if Claude is already running with --remote-debugging-port=9222
npm start
```

## Configuration

| Environment Variable | Default                  | Description           |
| -------------------- | ------------------------ | --------------------- |
| `CDP_URL`            | `http://localhost:9222`  | CDP endpoint URL      |

## How It Works

1. Connects to Claude Desktop's Electron app via CDP (port 9222)
2. Polls the DOM every 500 ms for a streaming indicator element
3. When streaming transitions from active to inactive, plays an ASCII bell beep

## Customising the Sound

Replace the `beep()` function in `beep-on-claude-done.js` with any sound playback method you prefer (e.g. `play-sound` npm package, PowerShell media player, etc.).
