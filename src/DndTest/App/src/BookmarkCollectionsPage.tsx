import AddIcon from '@mui/icons-material/Add';
import { IconButton } from "@mui/material";
import { useNavigate } from "@tanstack/react-router";
import { useState } from "react";
import { BookmarkCollectionDetailsDialog } from "./components/BookmarkCollectionDetailsDialog";
import BookmarkCollectionsList from "./components/BookmarkCollectionsList";
import { setHeader } from "./components/HeaderContext";

export const BookmarkCollectionsPage = () => {
    const [showCreateDialog, setShowCreateDialog] = useState(false);
    
    setHeader({
        back: false,
        title: 'Bookmark Collections',
        adornment: (
            <IconButton onClick={() => setShowCreateDialog(true)}>
                <AddIcon/>
            </IconButton>
        ),
    });
    
    const navigate = useNavigate();
    
    console.log('BookmarkCollectionsPage');
    return (
        <>
            <BookmarkCollectionsList
                onSelect={(id) => navigate({
                    to: '/bookmarkCollection/$id',
                    params: { id: id}
                })}
            />
            <BookmarkCollectionDetailsDialog
                open={showCreateDialog}
                bookmarkCollection={{name: '', description: ''}}
                onClose={() => setShowCreateDialog(false)}
            />
        </>
    );
}