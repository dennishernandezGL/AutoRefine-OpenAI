import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  use: {
    baseURL: 'http://localhost:5175/',
    headless: false, // ponlo en true en CI
    viewport: { width: 1280, height: 720 },
    screenshot: 'on',
    video: 'retain-on-failure'
  }
});