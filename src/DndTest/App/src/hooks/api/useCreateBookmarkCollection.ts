import { useMutation, useQueryClient } from "@tanstack/react-query";
import type { CreateBookmarkCollectionRequest } from "./requests";
import type { BookmarkCollectionResponse } from "./responses";
import { QueryKeys } from "./queryKeys";
import { useConfigContext } from "../configContext";

export function useCreateBookmarkCollection() {
    const queryClient = useQueryClient();
    const { apiBaseUrl } = useConfigContext();

    return useMutation({
        mutationFn: async (request: CreateBookmarkCollectionRequest) => {
            const response = await fetch(`${apiBaseUrl}/bookmarkCollection`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(request),
            });

            if (!response.ok) {
                throw new Error("Failed to create bookmark collection");
            }

            return response.json() as Promise<BookmarkCollectionResponse>;
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: [QueryKeys.bookmarkCollections] });
        },
    });
}
