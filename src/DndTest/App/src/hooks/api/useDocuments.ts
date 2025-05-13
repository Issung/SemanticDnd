import { useQuery } from "@tanstack/react-query";
import { QueryKeys } from "./queryKeys";
import { DocumentsResponse } from "./responses";

export function useDocuments() {
    const { apiBaseUrl } = { apiBaseUrl: 'https://localhost:7223/api' }
    
    return useQuery({
        queryKey: [QueryKeys.documents],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/documents`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json'
                }
            })
            
            if (!response.ok) {
                throw new Error(`Failed to fetch documents: ${response.status}`);
            }

            const data: DocumentsResponse = await response.json();
            return data;
        },
    });
}