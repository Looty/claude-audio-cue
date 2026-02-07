#!/bin/bash
# Start Claude Desktop with remote debugging enabled, then launch the monitor.

# Detect platform
if [[ "$OSTYPE" == "darwin"* ]]; then
    CLAUDE_EXE="/Applications/Claude.app/Contents/MacOS/Claude"
else
    CLAUDE_EXE="/usr/bin/claude"
fi

if [ ! -f "$CLAUDE_EXE" ]; then
    echo "Claude Desktop not found at $CLAUDE_EXE"
    echo "Please update the CLAUDE_EXE path in this script."
    exit 1
fi

echo "Starting Claude Desktop with remote debugging on port 9222..."
"$CLAUDE_EXE" --remote-debugging-port=9222 &

echo "Waiting for Claude to start..."
sleep 3

echo "Starting audio-cue monitor..."
node beep-on-claude-done.js
