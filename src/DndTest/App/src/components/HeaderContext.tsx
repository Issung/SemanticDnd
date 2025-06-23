import type { ReactNode } from "react";
import { createContext, useContext, useEffect, useState } from "react";

interface HeaderConfig {
    title?: string;
    back?: boolean;
    adornment?: React.ReactNode;
}

const HeaderConfigContext = createContext<HeaderConfig>({});
const SetHeaderConfigContext = createContext<(config: HeaderConfig) => void>(() => { });

export const useHeaderConfig = () => useContext(HeaderConfigContext);
export const useSetHeaderConfig = () => useContext(SetHeaderConfigContext);

export function setHeader(newConfig: HeaderConfig, deps?: React.DependencyList) {
    const setHeaderConfig = useSetHeaderConfig();

    return useEffect(() => {
        setHeaderConfig(newConfig);
    }, deps);
}

export const HeaderProvider = ({ children }: { children: ReactNode }) => {
    const [config, setConfig] = useState<HeaderConfig>({});

    return (
        <SetHeaderConfigContext.Provider value={setConfig}>
            <HeaderConfigContext.Provider value={config}>
                {children}
            </HeaderConfigContext.Provider>
        </SetHeaderConfigContext.Provider>
    );
};

