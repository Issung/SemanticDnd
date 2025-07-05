import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";

interface DeleteItemParams {
    id: number;
    /** Used to invalidate the browse queries of the parent id, so the deleted item does not appear. */
    parentId?: number;
}

export default function useItemDelete(onSuccess?: () => Promise<void> | void) {
    const queryClient = useQueryClient();
    const { apiBaseUrl } = useConfigContext();

    return useMutation({
        mutationFn: async (params: DeleteItemParams) => {
            const response = await fetch(`${apiBaseUrl}/item/${params.id}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error(`Failed to delete item ${params.id}.`);
            }
        },
        onSuccess: async (_d, params, _c) => {
            await onSuccess?.();
            // We don't know what item type the id was but invalidating extra query keys doesn't matter.
            queryClient.invalidateQueries({ queryKey: QueryKeys.item(params.id) });
            queryClient.invalidateQueries({ queryKey: QueryKeys.browse(params.id) });
            // TODO: Invalidate browse of the folder the item was within.

            if (params.parentId) {
                queryClient.invalidateQueries({ queryKey: QueryKeys.browse(params.parentId) })
            }
        },
    });
}
