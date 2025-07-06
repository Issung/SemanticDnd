import { useQuery } from "@tanstack/react-query";
import { useConfigContext } from "../configContext";
import { QueryKeys } from "./queryKeys";
import type { CustomFieldsResponse } from "./responses";

export default function useCustomFields() {
    const { apiBaseUrl } = useConfigContext();
    
    return useQuery({
        queryKey: [QueryKeys.customFields],
        queryFn: async () => {
            const response = await fetch(`${apiBaseUrl}/customFields`, {
                method: 'GET',
                headers: {
                    'content-type': 'application/json'
                }
            })
            
            if (!response.ok) {
                throw new Error(`Failed to fetch custom fields.`);
            }

            const data: CustomFieldsResponse = await response.json();
            return data;
        },
    });
}