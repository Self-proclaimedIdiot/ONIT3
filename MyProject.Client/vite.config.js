import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
console.log("------------------- КОНФИГ ЗАГРУЗИЛСЯ -------------------")
export default defineConfig(
    {
    plugins: [react()],
    server: {
        proxy: {
            // Все запросы к /api теперь пойдут на бэкенд
            '^/api': {
                target: 'http://localhost:5159', // ПОРТ ТВОЕГО API (посмотри в launchSettings.json бэкенда)
                changeOrigin: true,
                secure: false,
            }
        }
    }
})