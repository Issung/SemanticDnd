import { Button, Dialog, DialogActions, DialogContent, DialogTitle } from "@mui/material";
import BookmarkCollectionsList from "./BookmarkCollectionsList";
import { useEffect, useState } from "react";

export interface BookmarkCollectionsDialogProps {
    open: boolean;
    selectedCollectionIds: Array<number>;
    /** `undefined` indicates a cancellation, not save. */
    onClose: (selectedCollectionIds: Array<number> | undefined) => void;
}

export function BookmarkCollectionsDialog(props: BookmarkCollectionsDialogProps) {
    const [selectedIds, setSelectedIds] = useState<Array<number>>(props.selectedCollectionIds);

    // If user selects some, cancels, and opens again, need to reset the selections back.
    useEffect(() => {
        if (props.open) {
            setSelectedIds(props.selectedCollectionIds);
        }
    }, [props.open]);

    return (
        <Dialog
            fullWidth
            onClose={() => props.onClose(undefined)}
            open={props.open}
        >
            <DialogTitle>Bookmark Collections</DialogTitle>

            <DialogContent>
                <BookmarkCollectionsList checkboxMode={{
                    selectedIds: selectedIds,
                    onSelectionChanged: setSelectedIds
                }} />
            </DialogContent>

            <DialogActions>
                <Button onClick={() => props.onClose(undefined)}>Cancel</Button>
                <Button onClick={() => props.onClose(selectedIds)} variant="contained" color="primary">Save</Button>
            </DialogActions>
        </Dialog>
    );
}