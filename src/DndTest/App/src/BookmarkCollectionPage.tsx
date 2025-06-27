import { useParams } from "@tanstack/react-router";
import { useBookmarkCollection } from "./hooks/api/useBookmarkCollection";
import { useBookmarkCollectionItems } from "./hooks/api/useBookmarkCollectionItems";
import ItemList, { ItemListDisplay } from "./components/ItemsList";
import { setHeader } from "./components/HeaderContext";
import { IconButton } from "@mui/material";
import EditIcon from '@mui/icons-material/Edit';
import { BookmarkCollectionDetailsDialog } from "./components/BookmarkCollectionDetailsDialog";
import { useState } from "react";

export function BookmarkCollectionPage() {
    setHeader({
        back: true,
        adornment: (
            <IconButton onClick={() => setShowEditDialog(true)}>
                <EditIcon/>
            </IconButton>
        ),
    });

    const { id } = useParams({ strict: false }) // will be typed later with route param
    const {data: collectionData} = useBookmarkCollection(id!);
    const {data: itemsData} = useBookmarkCollectionItems(id!);
    const [showEditDialog, setShowEditDialog] = useState(false);

    const items = itemsData?.items.map(ItemListDisplay.fromSummary);

    return (
        <>
            {!collectionData
                ? <h1>Bookmark Collection</h1>
                : <>
                    <h1>{collectionData.bookmarkCollection.name}</h1>
                    <p>{collectionData.bookmarkCollection.description}</p>
                    {!items
                        ? "Items loading..."
                        : <ItemList hits={items}/>
                    }
                    <BookmarkCollectionDetailsDialog
                        open={showEditDialog}
                        bookmarkCollection={collectionData.bookmarkCollection}
                        onClose={() => setShowEditDialog(false)}
                    />
                </>
            }
        </>
    );
}