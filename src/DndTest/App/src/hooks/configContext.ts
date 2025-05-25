import { createContext, useContext } from "react";

interface ConfigContext {
    /**
     * The base URL to use for all API calls. Must not end with any trailing slashes..
     */
    apiBaseUrl: string;
}

export const ConfigContext = createContext<ConfigContext>({
    apiBaseUrl: "/api"
});

export const useConfigContext = () => {
    return useContext(ConfigContext);
};