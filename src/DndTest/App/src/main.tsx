import { Button } from '@mui/material'
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import {
    Outlet,
    RouterProvider,
    createRootRoute,
    createRoute,
    createRouter,
} from '@tanstack/react-router'
import { TanStackRouterDevtools } from '@tanstack/react-router-devtools'
import { StrictMode } from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import DocumentView from './pages/document/DocumentView.tsx'
import reportWebVitals from './reportWebVitals.ts'
import './styles.css'
import '@fontsource/roboto/300.css';
import '@fontsource/roboto/400.css';
import '@fontsource/roboto/500.css';
import '@fontsource/roboto/700.css';

const rootRoute = createRootRoute({
    component: () => (
        <>
            <div id="header">
                <Button size="small" variant="contained" color="primary" startIcon={<ArrowBackIcon/>}>Back</Button>
                <h1 style={{margin: '0 0 0 10px', height: '100%'}}>
                    Header Here
                </h1>
            </div>
            <Outlet />
            <TanStackRouterDevtools />
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
    component: DocumentView,
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

const routeTree = rootRoute.addChildren([indexRoute, documentRoute])

const router = createRouter({
    routeTree,
    context: {},
    defaultPreload: 'intent',
    scrollRestoration: true,
    defaultStructuralSharing: true,
    defaultPreloadStaleTime: 0,
})

declare module '@tanstack/react-router' {
    interface Register {
        router: typeof router
    }
}

const rootElement = document.getElementById('app')
if (rootElement && !rootElement.innerHTML)
{
    const root = ReactDOM.createRoot(rootElement)
    root.render(
        <StrictMode>
            <RouterProvider router={router} />
        </StrictMode>,
    )
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals()
