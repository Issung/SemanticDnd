import { useQuery } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { DocumentResponse } from "./responses";

export function useDocument(id: number) {
    const { apiBaseUrl } = useConfigContext();
    
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