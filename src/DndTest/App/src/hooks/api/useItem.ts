import { useQuery } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { ItemResponse } from "./responses";

export function useItem(id: number) {
    const { apiBaseUrl } = useConfigContext();
    
    if (typeof id !== 'number' || isNaN(id)) {
        throw new Error(`Item id '${id}' is not valid.`);
    }

    return useQuery({
        queryKey: QueryKeys.item(id),
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/item/${id}`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json'
                }
            })
            
            if (!response.ok) {
                throw new Error(`Failed to fetch item ${id}: ${response.status}`);
            }

            const data: ItemResponse = await response.json();
            return data;
        },
    });
}