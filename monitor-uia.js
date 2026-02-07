const { spawn } = require('child_process');
const path = require('path');

// Configuration
const POLL_INTERVAL_MS = 500;

let isStreaming = false;

const SOUND_FILE = process.env.CLAUDE_CUE_SOUND || 'C:\\Windows\\Media\\tada.wav';

function beep() {
  spawn('powershell.exe', [
    '-NoProfile', '-Command',
    `(New-Object Media.SoundPlayer '${SOUND_FILE}').PlaySync()`
  ], { stdio: 'ignore' });
  console.log('[claude-audio-cue] 🔔 Response complete!');
}

function startPowerShellMonitor() {
  console.log('[claude-audio-cue] Starting Windows UI Automation monitor for Claude Desktop...');

  const psScript = path.join(__dirname, 'monitor-claude-uia.ps1');

  const ps = spawn('powershell.exe', [
    '-NoProfile',
    '-ExecutionPolicy', 'Bypass',
    '-File', psScript,
    '-PollIntervalMs', POLL_INTERVAL_MS.toString()
  ], {
    stdio: ['ignore', 'pipe', 'pipe']
  });

  ps.stdout.on('data', (data) => {
    const output = data.toString().trim();

    for (const line of output.split(/\r?\n/)) {
      const trimmed = line.trim();
      if (!trimmed) continue;

      if (trimmed.includes('[STREAMING_START]')) {
        if (!isStreaming) {
          console.log('[claude-audio-cue] Claude is generating a response...');
          isStreaming = true;
        }
      } else if (trimmed.includes('[STREAMING_STOP]')) {
        if (isStreaming) {
          console.log('[claude-audio-cue] Response complete.');
          beep();
          isStreaming = false;
        }
      } else if (trimmed.includes('[DEBUG]') || trimmed.includes('[INFO]') || trimmed.includes('[ERROR]')) {
        console.log(trimmed);
      }
    }
  });

  ps.stderr.on('data', (data) => {
    console.error(`[PowerShell Error] ${data.toString().trim()}`);
  });

  ps.on('exit', (code) => {
    console.error(`[claude-audio-cue] PowerShell monitor exited with code ${code}`);
    console.log('[claude-audio-cue] Restarting monitor in 3 seconds...');
    isStreaming = false;
    setTimeout(() => startPowerShellMonitor(), 3000);
  });
}

// Handle graceful shutdown
process.on('SIGINT', () => {
  console.log('\n[claude-audio-cue] Shutting down...');
  process.exit(0);
});

console.log('[claude-audio-cue] Claude Desktop Audio Cue Monitor');
console.log('[claude-audio-cue] Using Windows UI Automation API');
console.log('[claude-audio-cue] Press Ctrl+C to stop\n');

startPowerShellMonitor();
