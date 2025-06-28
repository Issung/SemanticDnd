import { useQuery } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { ItemsResponse as ItemsResponse } from "./responses";

export function useBrowse(folderId: number | undefined) {
    const { apiBaseUrl } = useConfigContext();
    
    if (folderId === 0) {
        folderId = undefined;
    }

    return useQuery({
        queryKey: [QueryKeys.browse, folderId],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/browse/${folderId ?? ''}`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json'
                }
            })
            
            if (!response.ok) {
                throw new Error(`Browse request failed: ${response.status}.`);
            }

            const data: ItemsResponse = await response.json();
            return data;
        },
    });
}