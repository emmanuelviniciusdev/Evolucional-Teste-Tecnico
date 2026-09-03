import { defineConfig, devices } from '@playwright/test'

export default defineConfig({
  testDir: 'e2e',
  workers: 1,
  projects: [{ name: 'chromium', use: { ...devices['Desktop Chrome'] } }],
  webServer: [
    {
      command: 'npm run api',
      url: 'http://127.0.0.1:3001/produtos',
      timeout: 120000,
      reuseExistingServer: true
    },
    {
      command: 'npm run dev',
      url: 'http://127.0.0.1:5173',
      timeout: 120000,
      reuseExistingServer: true
    }
  ],
  use: {
    baseURL: 'http://127.0.0.1:5173',
    viewport: { width: 1280, height: 720 },
    headless: true
  }
})
