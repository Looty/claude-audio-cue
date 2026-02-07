const http = require('http');

const CDP_PORT = 9222;

function checkPort(host, port) {
  return new Promise((resolve) => {
    const req = http.get(`http://${host}:${port}/json/version`, (res) => {
      let data = '';
      res.on('data', (chunk) => { data += chunk; });
      res.on('end', () => {
        console.log(`✓ Successfully connected to ${host}:${port}`);
        console.log('Response:', data);
        resolve(true);
      });
    });

    req.on('error', (err) => {
      console.log(`✗ Failed to connect to ${host}:${port}`);
      console.log(`  Error: ${err.message}`);
      resolve(false);
    });

    req.setTimeout(2000, () => {
      console.log(`✗ Timeout connecting to ${host}:${port}`);
      req.destroy();
      resolve(false);
    });
  });
}

async function main() {
  console.log('Checking if Chrome DevTools Protocol is available...\n');

  console.log('Testing IPv4 (127.0.0.1):');
  await checkPort('127.0.0.1', CDP_PORT);

  console.log('\nTesting IPv6 (::1):');
  await checkPort('::1', CDP_PORT);

  console.log('\nTesting localhost:');
  await checkPort('localhost', CDP_PORT);
}

main();
