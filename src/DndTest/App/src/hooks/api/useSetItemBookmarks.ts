import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { ItemBookmarksRequest } from "./requests";
import type { ItemResponse } from "./responses";

export function useSetItemBookmarks() {
    const queryClient = useQueryClient();
    const { apiBaseUrl } = useConfigContext();
    
    return useMutation({
        mutationFn: async (itemBookmarks: ItemBookmarksRequest) => {
            console.log("Posting", itemBookmarks)
            const response = await fetch(`${apiBaseUrl}/bookmarks/`, {
                method: 'POST',
                body: JSON.stringify(itemBookmarks),
                headers: {
                    'content-type': 'application/json'
                },
            })

            if (!response.ok) {
                throw new Error(`Failed to set bookmarks for item.`);
            }

            // Update item in query client (updating bookmark display in header).
            const itemQueryKey = QueryKeys.item(itemBookmarks.itemId);
            const itemResponse = queryClient.getQueryData<ItemResponse>(itemQueryKey);

            if (!itemResponse) {
                return;
            }

            const updated: ItemResponse = {
                item: {
                    ...itemResponse.item,
                    bookmarkCollectionIds: itemBookmarks.bookmarkCollectionIds,
                }
            };

            queryClient.setQueryData(itemQueryKey, updated);
        },
    });
}