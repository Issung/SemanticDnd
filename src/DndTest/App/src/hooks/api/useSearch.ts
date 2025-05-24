import { useQuery } from "@tanstack/react-query";
import { QueryKeys } from "./queryKeys";
import type { SearchRequest } from "./requests";
import type { SearchResponse } from "./responses";

export function useSearch(request: SearchRequest) {
    const { apiBaseUrl } = { apiBaseUrl: 'https://localhost:7223/api' }
    
    return useQuery({
        queryKey: [QueryKeys.search, ...Object.values(request)],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/tradsearch`, {
                method: 'POST',
                body: JSON.stringify(request),
                headers: {
                    'content-type': 'application/json'
                },
            })
            
            if (!response.ok) {
                throw new Error(`Failed to search: ${response.status}`);
            }

            const data: SearchResponse = await response.json();
            return data;
        },
    });
}