import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";

export function useBookmarkCollectionDelete(onSuccess?: () => Promise<void> | void) {
    const queryClient = useQueryClient();
    const { apiBaseUrl } = useConfigContext();

    return useMutation({
        mutationFn: async (collectionId: number) => {
            const response = await fetch(`${apiBaseUrl}/bookmarkCollection/${collectionId}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                }
            });

            if (!response.ok) {
                throw new Error("Failed to delete bookmark collection.");
            }
        },
        onSuccess: async (d, collectionId, c) => {
            // Must `await` navigation away first before invalidating queries, so the collection that
            // was just deleted we don't try to refetch the query that was just deleted.

            await onSuccess?.();

            queryClient.invalidateQueries({ queryKey: [QueryKeys.bookmarkCollections] });
            queryClient.invalidateQueries({ queryKey: [QueryKeys.bookmarkCollections, collectionId] });
        },
    });
}
