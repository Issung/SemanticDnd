import path from 'node:path';
import viteReact from "@vitejs/plugin-react";
import { defineConfig } from "vite";
import { VitePWA } from 'vite-plugin-pwa';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        viteReact(),
        VitePWA({   // https://adueck.github.io/blog/caching-everything-for-totally-offline-pwa-vite-react/
            // add this to cache all the imports
            workbox: {
                globPatterns: ["**/*"],
            },
            // add this to cache all the
            // static assets in the public folder
            includeAssets: [
                "**/*",
            ],
            registerType: 'autoUpdate'
        })
    ],
    test: {
        globals: true,
        environment: "jsdom",
    },
    resolve: {
        alias: {
            '@': path.resolve(__dirname, './src'),
        },
    },
    server: {
        // Host on IPs not just localhost, so we can connect on phone.
        host: true,
    }
});
