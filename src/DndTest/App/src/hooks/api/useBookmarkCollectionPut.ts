import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import type { BookmarkCollectionPutRequest } from "./requests";
import { QueryKeys } from "./queryKeys";

export function useBookmarkCollectionPut() {
    const queryClient = useQueryClient();
    const { apiBaseUrl } = useConfigContext();

    return useMutation({
        mutationFn: async (request: BookmarkCollectionPutRequest) => {
            const response = await fetch(`${apiBaseUrl}/bookmarkCollection`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(request),
            });

            if (!response.ok) {
                throw new Error("Failed to update bookmark collection");
            }
        },
        onSuccess: (d, v, c) => {
            queryClient.invalidateQueries({ queryKey: [QueryKeys.bookmarkCollections] });
            if (v.id) {
                queryClient.invalidateQueries({ queryKey: [QueryKeys.bookmarkCollections, v.id] });
            }
        },
    });
}
