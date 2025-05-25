import { useQuery } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { DocumentsResponse } from "./responses";

export function useDocuments() {
    const { apiBaseUrl } = useConfigContext();
    
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