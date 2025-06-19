import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';
import { Box, CssBaseline, ThemeProvider, createTheme } from '@mui/material';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import {
    Outlet,
    RouterProvider,
    createRootRoute,
    createRoute,
    createRouter
} from '@tanstack/react-router';
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools';
import { StrictMode } from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.tsx';
import { Bookmarks } from './Bookmarks.tsx';
import { Footer } from './components/Footer.tsx';
import Header from './components/Header.tsx';
import { Content } from './Content.tsx';
import { Home } from './Home.tsx';
import DocumentPage from './pages/document/DocumentPage.tsx';
import SearchPage from './pages/search/SearchPage.tsx';
import reportWebVitals from './reportWebVitals.ts';
import './styles.css';

const theme = createTheme({
    palette: {
        mode: 'dark',
    },
});

const rootRoute = createRootRoute({
    component: () => (
        <>
            <ThemeProvider theme={theme}>
                <CssBaseline />
                <Header />
                <Box maxWidth={750} mx="auto" mt={4}>
                    <div id="content">
                        <Outlet />
                    </div>
                </Box>
                <Footer />
                {/* <TanStackRouterDevtools /> */}
            </ThemeProvider>
        </>
    ),
})

const indexRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/',
    component: App,
})

const documentRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/document/$id',
    component: DocumentPage,
    parseParams: ({ id }) => {
        const parsedId = parseInt(id)
        if (isNaN(parsedId))
        {
            throw new Error('Invalid document ID')
        }
        return { id: parsedId }
    },
    stringifyParams: ({ id }) => ({ id: String(id) }),
})

const homeRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/home',
    component: Home
});

const contentRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/content',
    component: Content
});

const searchRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/search',
    component: SearchPage
});

const settingsRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/bookmarks',
    component: Bookmarks
});

const routeTree = rootRoute.addChildren([indexRoute, documentRoute, homeRoute, contentRoute, settingsRoute, searchRoute])

const router = createRouter({
    routeTree,
    context: {},
    defaultPreload: 'intent',
    scrollRestoration: true,
    defaultStructuralSharing: true,
    defaultPreloadStaleTime: 0,
    basepath: '/app',   // Must match what's in vite.config.ts.
})

declare module '@tanstack/react-router' {
    interface Register {
        router: typeof router
    }
}

const rootElement = document.getElementById('app')
const queryClient = new QueryClient()

if (rootElement && !rootElement.innerHTML)
{
    const root = ReactDOM.createRoot(rootElement)
    root.render(
        <StrictMode>
            <QueryClientProvider client={queryClient}>
                <RouterProvider router={router} />
            </QueryClientProvider>
        </StrictMode>,
    )
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals()
