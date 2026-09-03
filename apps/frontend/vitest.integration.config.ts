import { defineConfig } from 'vitest/config'

export default defineConfig({
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: 'src/test/setup.integration.ts',
    include: ['src/test/integration/**/*.test.{ts,tsx}']
  }
})

