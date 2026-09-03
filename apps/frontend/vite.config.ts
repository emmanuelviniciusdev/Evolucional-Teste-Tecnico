import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from 'path'

export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src')
    }
  },
  server: {
    host: '127.0.0.1',
    port: 5173,
    strictPort: true,
    proxy: {
      '/produtos': {
        target: 'http://127.0.0.1:3001',
        changeOrigin: true,
        secure: false,
        bypass(req) {
          if (req.headers.accept?.includes('text/html')) return '/index.html'
        }
      }
    }
  }
})
