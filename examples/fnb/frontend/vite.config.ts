import { defineConfig, loadEnv } from 'vite';
import react from '@vitejs/plugin-react';

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, '.', 'BACKEND_');
  const backendUrl = env.BACKEND_URL || 'http://localhost:5080';

  return {
    plugins: [react()],
    server: {
      port: 5173,
      proxy: { '/api': backendUrl, '/webhooks': backendUrl },
    },
  };
});
