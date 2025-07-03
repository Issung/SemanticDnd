import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import type { FilePutRequest } from "./requests";
import { QueryKeys } from "./queryKeys";

export function useItemPut() {
    const queryClient = useQueryClient();
    const { apiBaseUrl } = useConfigContext();

    return useMutation({
        mutationFn: async (request: FilePutRequest & { id?: number, file?: File | null }) => {
            const { id, file, ...body } = request;

            const formData = new FormData();
            formData.append('name', body.name);
            formData.append('description', body.description);
            if (body.parentId) {
                formData.append('parentId', body.parentId.toString());
            }
            if (file) {
                formData.append('file', file);
            }

            const response = await fetch(`${apiBaseUrl}/item/file${id ? `/${id}` : ''}`, {
                method: "PUT",
                body: formData,
            });

            if (!response.ok) {
                throw new Error("Failed to put item.");
            }

            const b = await response.text()
            const i = b.replaceAll('"', '');
            return parseInt(i);
        },
        onSuccess: (_d, v, _c) => {
            queryClient.invalidateQueries({ queryKey: [QueryKeys.items] });
            if (v.id) {
                queryClient.invalidateQueries({ queryKey: [QueryKeys.item(v.id)] });
            }
            // Invalidate browse view for folder we are in.
            queryClient.invalidateQueries({ queryKey: [QueryKeys.browse(v.parentId)] });
        },
    });
}
