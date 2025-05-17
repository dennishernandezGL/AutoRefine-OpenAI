import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  use: {
    baseURL: 'http://localhost:3000',
    headless: true, // ponlo en true en CI
    viewport: { width: 1280, height: 720 },
    screenshot: 'on',
    video: 'retain-on-failure'
  }
});