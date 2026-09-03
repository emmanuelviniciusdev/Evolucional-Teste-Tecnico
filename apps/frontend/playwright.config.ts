import { defineConfig } from '@playwright/test'

export default defineConfig({
  testDir: 'e2e',
  webServer: {
    command: 'npm run dev:all',
    url: 'http://localhost:5173',
    timeout: 120000,
    reuseExistingServer: true
  },
  use: {
    baseURL: 'http://localhost:5173',
    viewport: { width: 1280, height: 720 },
    headless: true
  }
})

