import { IconButton, Input, InputAdornment } from "@mui/material";
import { useEffect, useState } from "react";
import { useNavigate, useSearch as useRouteSearch } from "@tanstack/react-router";
import ItemList, { ItemListDisplayAdapter } from "@/components/ItemsList";
import { useSearch } from "@/hooks/api/useSearch";
import { useDebounce } from "@/hooks/useDebounce"; // Adjust path as needed
import ClearIcon from '@mui/icons-material/Clear';
import { setHeader } from "@/components/HeaderContext";

export default function SearchPage() {
    const { query: routeQuery } = useRouteSearch({ from: "/search" });
    const navigate = useNavigate({ from: "/search" });

    const [inputValue, setInputValue] = useState(routeQuery);

    const debouncedInput = useDebounce(inputValue, 200); // Only update after 200ms idle

    setHeader({back: false, title: 'Search'});

    // Sync input field with query param (back/forward nav)
    useEffect(() => {
        setInputValue(routeQuery);
    }, [routeQuery]);

    // Push to URL when debounced input changes
    useEffect(() => {
        navigate({ search: (prev) => ({ ...prev, query: debouncedInput }) });
    }, [debouncedInput, navigate]);

    const { data, isPending, isError } = useSearch({ query: routeQuery, category: undefined });

    const hits = data?.hits.map(ItemListDisplayAdapter.fromSearchHit);

    console.log('SearchPage', routeQuery, data, isPending, isError);
    return (
        <div>
            <Input
                sx={{ width: '100%', marginBottom: '1em' }}
                size="medium"
                type="text"
                value={inputValue}
                onChange={(e) => setInputValue(e.target.value)}
                placeholder="Search..."
                endAdornment={
                    inputValue.length > 0 &&
                    <InputAdornment position="end">
                        <IconButton
                            aria-label="clear input"
                            onClick={() => setInputValue("")}
                            edge="end"
                            size="small"
                        >
                            <ClearIcon />
                        </IconButton>
                    </InputAdornment>
                }
            />
            {isPending && <p>Loading...</p>}
            {isError && <p>Error loading search results</p>}
            {hits && <ItemList hits={hits} />}
        </div>
    );
}
