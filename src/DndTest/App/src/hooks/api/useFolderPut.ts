import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { FolderPutRequest } from "./requests";

export default function useFolderPut() {
    const queryClient = useQueryClient();
    const { apiBaseUrl } = useConfigContext();

    return useMutation({
        mutationFn: async (params: {id: number | undefined, request: FolderPutRequest}) => {
            const response = await fetch(`${apiBaseUrl}/item/folder/${params.id ?? ''}`, {
                method: "PUT",
                body: JSON.stringify(params.request),
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error("Failed to put bookmark collection.");
            }
        },
        onSuccess: (_d, params, _c) => {
            if (params.id) {
                // We could use `setQueryData` to update the query stored instead of invalidating & refetching. 
                queryClient.invalidateQueries({ queryKey: QueryKeys.browse(params.id) });
            }
            queryClient.invalidateQueries({ queryKey: QueryKeys.browse(params.request.parentId) });
        },
    });
}
