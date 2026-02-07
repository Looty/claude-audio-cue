const { chromium } = require('playwright');

const CDP_URL = process.env.CDP_URL || 'http://localhost:9222';
const POLL_INTERVAL_MS = 500;

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

async function monitorClaude() {
  console.log(`[claude-audio-cue] Connecting to Claude Desktop via CDP at ${CDP_URL}...`);

  let browser;
  try {
    browser = await chromium.connectOverCDP(CDP_URL);
  } catch (err) {
    console.error(
      `[claude-audio-cue] Could not connect to Claude Desktop.\n` +
      `Make sure Claude is running with --remote-debugging-port=9222.\n` +
      `Error: ${err.message}`
    );
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
