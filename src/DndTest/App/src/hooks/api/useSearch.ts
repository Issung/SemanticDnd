import { useQuery } from "@tanstack/react-query";
import { QueryKeys } from "./queryKeys";
import { SearchRequest } from "./requests";
import { SearchResponse } from "./responses";

export function useSearch(request: SearchRequest) {
    const { apiBaseUrl } = { apiBaseUrl: 'https://localhost:7223/api' }
    
    return useQuery({
        queryKey: [QueryKeys.search, ...Object.values(request)],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/search`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json'
                }
            })
            
            if (!response.ok) {
                throw new Error(`Failed to search: ${response.status}`);
            }

            const data: SearchResponse = await response.json();
            return data;
        },
    });
}