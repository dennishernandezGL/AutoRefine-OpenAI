import { defineConfig } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  use: {
    baseURL: process.env.BASE_URL || 'http://localhost:3000', // Default to localhost if no base URL is provided
    headless: true, // ponlo en true en CI
    viewport: { width: 1280, height: 720 },
    screenshot: 'on',
    video: 'retain-on-failure'
  },
  projects: [
    {
      name: 'chromium',
      use: { browserName: 'chromium' },
    },
  ],

});