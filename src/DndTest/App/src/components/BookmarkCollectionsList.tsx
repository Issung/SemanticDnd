import type { BookmarkCollectionSummary } from "@/hooks/api/responses";
import { useBookmarkCollections } from "@/hooks/api/useBookmarkCollections";
import EditIcon from '@mui/icons-material/Edit';
import {
    Checkbox,
    IconButton,
    List,
    ListItem,
    ListItemButton,
    ListItemText
} from "@mui/material";

function BookmarkCollectionsList({
    onSelect,
    onEdit,
    checkboxMode,
}: {
    onSelect?: (id: number) => void,
    onEdit?: (collection: BookmarkCollectionSummary) => void,
    checkboxMode?: {
        selectedIds: Array<number>,
        onSelectionChanged: (newSelectedIds: Array<number>) => void,
    }
}) {
    const { data } = useBookmarkCollections();

    function toggleCheckbox(id: number) {
        if (checkboxMode)
        {
            if (checkboxMode.selectedIds.includes(id))
            {
                const newList = checkboxMode.selectedIds.filter(i => i != id);
                checkboxMode.onSelectionChanged(newList);
            }
            else
            {
                const newList = [...checkboxMode.selectedIds, id];
                checkboxMode.onSelectionChanged(newList);
            }
        }
    }

    return (
        <>
            {!data
                ? "Loading..."
                : <List>
                    {data.collections.map((collection, index) => (
                        <ListItem
                            key={collection.id}
                            divider={index < data.collections.length - 1}
                            secondaryAction={
                                <>
                                    <span>{collection.bookmarkCount}</span>
                                    {onEdit && (
                                        <IconButton onClick={() => onEdit(collection)}>
                                            <EditIcon />
                                        </IconButton>
                                    )}
                                </>
                            }
                            sx={{
                                cursor: 'pointer',
                                width: '100%',
                                '&:hover': {
                                    backgroundColor: 'action.hover'
                                }
                            }}
                            disablePadding  // Fixes div alignment for ripple animation not going over into the secondaryAction area.
                            onClick={() => {
                                if (!checkboxMode)
                                {
                                    onSelect?.(collection.id);
                                }
                            }}
                        >
                            <ListItemButton onClick={() => toggleCheckbox(collection.id)}>
                                {checkboxMode && (
                                    <Checkbox
                                        edge="start"
                                        checked={checkboxMode.selectedIds.includes(collection.id)}
                                    />
                                )}
                                <ListItemText primary={collection.name} />
                            </ListItemButton>
                        </ListItem>
                    ))}
                </List>
            }
        </>
    );
}

export default BookmarkCollectionsList;
