import { useBookmarkCollections } from "@/hooks/api/useBookmarkCollections";
import { List, ListItem, ListItemText } from "@mui/material";

function BookmarkCollectionsList({
    onSelect,
}: {
    onSelect: (id: number) => void,
}) {
    const { data } = useBookmarkCollections();

    return (
        <>
            {!data
                ? "Loading..."
                : <List>
                    {data.collections.map((collection, index) => (
                        <ListItem
                            key={collection.id}
                            onClick={() => onSelect(collection.id)}
                            divider={index < data.collections.length - 1}
                            sx={{
                                cursor: 'pointer', // Show pointer cursor
                                width: '100%', // Ensure full width
                                '&:hover': {
                                    backgroundColor: 'action.hover' // Add hover effect
                                }
                            }}
                        >
                            <ListItemText
                                primary={collection.name}
                            />
                        </ListItem>
                    ))}
                </List>
            }
        </>
    );
};

export default BookmarkCollectionsList;