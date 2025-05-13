import { useQuery } from "@tanstack/react-query";
import { QueryKeys } from "./queryKeys";
import { DocumentResponse } from "./responses";

export function useDocument(id: number) {
    const { apiBaseUrl } = { apiBaseUrl: 'https://localhost:7223/api' }
    
    if (typeof id !== 'number' || isNaN(id)) {
        throw new Error(`Document id '${id}' is not valid.`);
    }

    return useQuery({
        queryKey: [QueryKeys.documents, id],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/document/${id}`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json'
                }
            })
            
            if (!response.ok) {
                throw new Error(`Failed to fetch document ${id}: ${response.status}`);
            }

            const data: DocumentResponse = await response.json();
            return data;
        },
    });
}