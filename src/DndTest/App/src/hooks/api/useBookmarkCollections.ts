import { useQuery } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { BookmarkCollectionsResponse } from "./responses";

export function useBookmarkCollections() {
    const { apiBaseUrl } = useConfigContext();
    
    return useQuery({
        queryKey: [QueryKeys.bookmarkCollections],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/bookmarkCollections`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json'
                }
            })
            
            if (!response.ok) {
                throw new Error(`Failed to fetch items: ${response.status}`);
            }

            const data: BookmarkCollectionsResponse = await response.json();
            return data;
        },
    });
}