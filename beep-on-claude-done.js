const { chromium } = require('playwright');

const CDP_URL = process.env.CDP_URL || 'http://127.0.0.1:9222';
const POLL_INTERVAL_MS = 500;
const MAX_RETRIES = 5;
const RETRY_DELAY_MS = 2000;

// Candidate selectors for Claude Desktop's streaming indicator.
// After inspecting the DOM (chrome://inspect while Claude runs with --remote-debugging-port=9222),
// replace or extend this list with the actual selector(s) you find.
const STREAMING_SELECTORS = [
  '[data-streaming="true"]',
  '.streaming-indicator',
  '.message-streaming',
  '.typing-indicator',
  '.is-streaming',
];

let lastStreaming = false;

function beep() {
  // ASCII bell – works in most terminals
  process.stdout.write('\x07');
  console.log('[claude-audio-cue] 🔔 Response complete!');
}

async function findStreamingIndicator(page) {
  for (const selector of STREAMING_SELECTORS) {
    const found = await page.$(selector);
    if (found) return selector;
  }
  return null;
}

async function isStreaming(page, knownSelector) {
  if (knownSelector) {
    return (await page.$(knownSelector)) !== null;
  }
  // Fallback: try all selectors
  return (await findStreamingIndicator(page)) !== null;
}

async function connectWithRetry() {
  for (let attempt = 1; attempt <= MAX_RETRIES; attempt++) {
    try {
      console.log(`[claude-audio-cue] Connection attempt ${attempt}/${MAX_RETRIES} to ${CDP_URL}...`);
      const browser = await chromium.connectOverCDP(CDP_URL);
      console.log('[claude-audio-cue] ✓ Successfully connected!');
      return browser;
    } catch (err) {
      console.error(`[claude-audio-cue] ✗ Connection failed: ${err.message}`);

      if (attempt < MAX_RETRIES) {
        console.log(`[claude-audio-cue] Retrying in ${RETRY_DELAY_MS / 1000} seconds...\n`);
        await new Promise(resolve => setTimeout(resolve, RETRY_DELAY_MS));
      } else {
        console.error(
          `\n[claude-audio-cue] Could not connect after ${MAX_RETRIES} attempts.\n` +
          `Make sure Claude Desktop is running with --remote-debugging-port=9222.\n\n` +
          `Troubleshooting tips:\n` +
          `1. Check if Claude.exe is actually running (Task Manager)\n` +
          `2. Try closing Claude and running this script again\n` +
          `3. Make sure no other process is using port 9222\n`
        );
        throw err;
      }
    }
  }
}

async function monitorClaude() {
  console.log(`[claude-audio-cue] Starting monitor for Claude Desktop...`);

  let browser;
  try {
    browser = await connectWithRetry();
  } catch (err) {
    process.exit(1);
  }

  const contexts = browser.contexts();
  if (contexts.length === 0) {
    console.error('[claude-audio-cue] No browser contexts found.');
    process.exit(1);
  }

  const pages = contexts[0].pages();
  if (pages.length === 0) {
    console.error('[claude-audio-cue] No pages found in context.');
    process.exit(1);
  }

  const page = pages[0];
  console.log('[claude-audio-cue] Connected! Monitoring for streaming completion...');

  let knownSelector = null;

  setInterval(async () => {
    try {
      // Try to lock onto a specific selector once found
      if (!knownSelector) {
        knownSelector = await findStreamingIndicator(page);
        if (knownSelector) {
          console.log(`[claude-audio-cue] Detected streaming selector: ${knownSelector}`);
        }
      }

      const streaming = await isStreaming(page, knownSelector);

      if (lastStreaming && !streaming) {
        beep();
      }

      lastStreaming = streaming;
    } catch (err) {
      // Page may have navigated or closed; try to recover silently
      if (err.message.includes('Target closed') || err.message.includes('Session closed')) {
        console.log('[claude-audio-cue] Page closed or navigated. Waiting for reconnect...');
      }
    }
  }, POLL_INTERVAL_MS);
}

monitorClaude();
