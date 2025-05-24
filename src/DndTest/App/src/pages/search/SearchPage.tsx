import { Input } from "@mui/material";
import { useState } from "react";
import ItemList from "@/components/ItemsList";
import { useSearch } from "@/hooks/api/useSearch";

export default function SearchPage() {
    const [query, setQuery] = useState<string>('');
    const { data, isPending, isError } = useSearch({query, category: undefined});

    return <>
        <div>
            <Input
                sx={{ width: '100%', marginBottom: '1em' }}
                size="medium"
                type="text"
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                placeholder="Search..."
            />
            {isPending && <p>Loading...</p>}
            {isError && <p>Error loading search results</p>}
            {data && <ItemList hits={data.hits} />}
        </div>
    </>
}