import type { BookmarkCollectionPutRequest } from "@/hooks/api/requests";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from "@mui/material";
import { useState } from "react";
import { useBookmarkCollectionPut } from "../hooks/api/useBookmarkCollectionPut";

export interface BookmarkCollectionDetailsDialogProps {
    open: boolean;
    bookmarkCollection: BookmarkCollectionPutRequest;
    onClose: () => void;
}

export function BookmarkCollectionDetailsDialog(props: BookmarkCollectionDetailsDialogProps) {
    const putBookmarkCollection = useBookmarkCollectionPut();
    const [bookmarkCollection, setBookmarkCollection] = useState(props.bookmarkCollection);
    const isEdit = Boolean(props.bookmarkCollection.id);

    const handleSave = async () => {
        await putBookmarkCollection.mutateAsync(bookmarkCollection);
        props.onClose();
    };

    return (
        <Dialog
            fullWidth
            onClose={props.onClose}
            open={props.open}
        >
            <DialogTitle>{isEdit ? "Edit Bookmark Collection" : "Create Bookmark Collection"}</DialogTitle>

            <DialogContent>
                <TextField
                    autoFocus
                    margin="dense"
                    id="name"
                    label="Name"
                    type="text"
                    fullWidth
                    variant="standard"
                    value={bookmarkCollection.name}
                    onChange={(e) => setBookmarkCollection({...bookmarkCollection, name: e.target.value})}
                />
                <TextField
                    margin="dense"
                    id="description"
                    label="Description"
                    type="text"
                    fullWidth
                    multiline
                    rows={3}
                    variant="standard"
                    value={bookmarkCollection.description}
                    onChange={(e) => setBookmarkCollection({...bookmarkCollection, description: e.target.value})}
                />
            </DialogContent>

            <DialogActions>
                <Button onClick={props.onClose}>Cancel</Button>
                <Button onClick={handleSave} variant="contained" color="primary">{isEdit ? "Save" : "Create"}</Button>
            </DialogActions>
        </Dialog>
    );
}
