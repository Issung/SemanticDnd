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
import { StrictMode } from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.tsx';
import { BookmarkCollectionPage } from './BookmarkCollectionPage.tsx';
import { BookmarkCollectionsPage } from './BookmarkCollectionsPage.tsx';
import { Footer } from './components/Footer.tsx';
import Header from './components/Header.tsx';
import { HeaderProvider } from './components/HeaderContext.tsx';
import { Home } from './Home.tsx';
import ItemPage from './pages/item/ItemPage.tsx';
import SearchPage from './pages/search/SearchPage.tsx';
import reportWebVitals from './reportWebVitals.ts';
import './styles.css';
import BrowsePage from './BrowsePage.tsx';
import CreateFilePage from './pages/CreateFilePage.tsx';

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
                <HeaderProvider>
                    <Header />
                    <Box maxWidth={750} mx="auto" mt={4}>
                        <div id="content">
                            <Outlet />
                        </div>
                    </Box>
                </HeaderProvider>
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

const itemRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/item/$id',
    component: ItemPage,
    parseParams: ({ id }) => {
        const parsedId = parseInt(id)
        if (isNaN(parsedId))
        {
            throw new Error('Invalid item ID')
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

export const browseRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/browse/$folderId',
    component: BrowsePage,
    params: {
        parse: ({ folderId }) => {
            const parsedId = parseInt(folderId);
            if (isNaN(parsedId)) {
                throw new Error('Invalid folderId');
            }

            return { folderId: parsedId };
        },
    },
});

const searchRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/search',
    component: SearchPage,
    validateSearch: (search) => ({
        query: typeof search.query === 'string' ? search.query : '',
    }),
});

const bookmarkCollectionsRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/bookmarkCollections',
    component: BookmarkCollectionsPage,
});

const bookmarkCollectionRoute = createRoute({
    // getParentRoute: () => bookmarkCollectionsRoute,
    getParentRoute: () => rootRoute,
    path: '/bookmarkCollection/$id',
    component: BookmarkCollectionPage,
    parseParams: ({id}) => {
        const parsedId = parseInt(id);
        return {
            id: isNaN(parsedId) ? (() => { throw new Error()})() : parsedId,
        };
    }
});

const createFileRoute = createRoute({
    getParentRoute: () => rootRoute,
    path: '/create/file',
    component: CreateFilePage,
});

const routeTree = rootRoute.addChildren([
    indexRoute,
    itemRoute,
    homeRoute,
    browseRoute,
    bookmarkCollectionsRoute, bookmarkCollectionRoute,
    searchRoute,
    createFileRoute
])

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
