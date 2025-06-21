import { useQuery } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { ItemsResponse } from "./responses";

export function useItems() {
    const { apiBaseUrl } = useConfigContext();
    
    return useQuery({
        queryKey: [QueryKeys.items],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/items`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json'
                }
            })
            
            if (!response.ok) {
                throw new Error(`Failed to fetch items: ${response.status}`);
            }

            const data: ItemsResponse = await response.json();
            return data;
        },
    });
}