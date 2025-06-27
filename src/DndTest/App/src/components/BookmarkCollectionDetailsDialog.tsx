import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from "@mui/material";
import { useState } from "react";
import { useCreateBookmarkCollection } from "../hooks/api/useCreateBookmarkCollection";
import type { BookmarkCollection } from "../hooks/api/responses";

export interface BookmarkCollectionDetailsDialogProps {
    bookmarkCollection?: BookmarkCollection;
    onClose: () => void;
}

export function BookmarkCollectionDetailsDialog(props: BookmarkCollectionDetailsDialogProps) {
    const [name, setName] = useState(props.bookmarkCollection?.name || "");
    const [description, setDescription] = useState(props.bookmarkCollection?.description || "");
    const createMutation = useCreateBookmarkCollection();

    const handleSave = async () => {
        await createMutation.mutateAsync({
            name: name,
            description: description || undefined,
        });
        props.onClose();
    };

    return (
        <Dialog
            fullWidth
            onClose={props.onClose}
            open={true}
        >
            <DialogTitle>{props.bookmarkCollection ? "Edit Bookmark Collection" : "Create New Bookmark Collection"}</DialogTitle>

            <DialogContent>
                <TextField
                    autoFocus
                    margin="dense"
                    id="name"
                    label="Name"
                    type="text"
                    fullWidth
                    variant="standard"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
                <TextField
                    margin="dense"
                    id="description"
                    label="Description"
                    type="text"
                    fullWidth
                    multiline
                    rows={4}
                    variant="standard"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                />
            </DialogContent>

            <DialogActions>
                <Button onClick={props.onClose}>Cancel</Button>
                <Button onClick={handleSave} variant="contained" color="primary">Save</Button>
            </DialogActions>
        </Dialog>
    );
}
