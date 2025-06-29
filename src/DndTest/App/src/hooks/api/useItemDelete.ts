import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";

export default function useItemDelete(onSuccess?: () => Promise<void> | void) {
    const queryClient = useQueryClient();
    const { apiBaseUrl } = useConfigContext();

    return useMutation({
        mutationFn: async (id: number) => {
            const response = await fetch(`${apiBaseUrl}/item/${id}`, {
                method: "DELETE",
                headers: {
                    "Content-Type": "application/json",
                },
            });

            if (!response.ok) {
                throw new Error(`Failed to delete item ${id}.`);
            }
        },
        onSuccess: async (_d, id, _c) => {
            await onSuccess?.();
            // We don't know what item type the id was but invalidating extra query keys doesn't matter.
            queryClient.invalidateQueries({ queryKey: QueryKeys.item(id) });
            queryClient.invalidateQueries({ queryKey: QueryKeys.browse(id) });
        },
    });
}
