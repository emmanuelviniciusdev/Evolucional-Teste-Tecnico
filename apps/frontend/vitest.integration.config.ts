import { defineConfig } from 'vitest/config'
import path from 'path'

export default defineConfig({
  esbuild: {
    jsx: 'automatic',
    jsxImportSource: 'react',
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
    },
  },
  define: {
    // Makes API_BASE absolute so MSW can intercept fetch calls in Node
    'import.meta.env.VITE_API_BASE': JSON.stringify('http://localhost:3001'),
  },
  test: {
    globals: true,
    environment: 'jsdom',
    setupFiles: 'src/test/setup.integration.ts',
    include: ['src/test/integration/**/*.test.{ts,tsx}'],
  },
})
