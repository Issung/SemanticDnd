import { useQuery } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { ItemsResponse } from "./responses";

export function useBookmarkCollectionItems(collectionId: number) {
    const { apiBaseUrl } = useConfigContext();
    
    return useQuery({
        queryKey: [QueryKeys.bookmarkCollectionItems, collectionId],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/bookmarkCollection/${collectionId}/items`, {
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