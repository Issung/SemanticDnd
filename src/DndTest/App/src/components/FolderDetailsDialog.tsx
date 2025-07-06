import type { FolderPutRequest } from "@/hooks/api/requests";
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, TextField } from "@mui/material";
import { useEffect, useState } from "react";

interface FolderDetailsDialogProps {
    open: boolean;
    mode: "create" | "edit";
    details: FolderPutRequest;
    /** Undefined indicates cancellation (no API request required). */
    onClose: (details: FolderPutRequest | undefined) => void;
}

export default function FolderDetailsDialog(props: FolderDetailsDialogProps) {
    const [details, setDetails] = useState(props.details);

    useEffect(() => {
        setDetails(props.details);
    }, [props.details]);

    function cancel() {
        props.onClose(undefined);
    }

    function save() {
        props.onClose(details);
    };

    // console.log('FolderDetailsDialog', props.details, details);
    return (
        <Dialog
            fullWidth
            onClose={cancel}
            open={props.open}
            disableScrollLock
        >
            <DialogTitle>{props.mode === "create" ? "Create Folder" : "Edit Folder"}</DialogTitle>

            <DialogContent>
                <TextField
                    autoFocus
                    margin="dense"
                    id="name"
                    label="Name"
                    type="text"
                    fullWidth
                    variant="outlined"
                    value={details.name}
                    onChange={(e) => setDetails({ ...details, name: e.target.value })}
                />
                <TextField
                    margin="dense"
                    id="description"
                    label="Description"
                    type="text"
                    fullWidth
                    multiline
                    rows={3}
                    variant="outlined"
                    value={details.description}
                    onChange={(e) => setDetails({ ...details, description: e.target.value })}
                />
            </DialogContent>

            <DialogActions>
                <Button onClick={cancel}>Cancel</Button>
                <Button onClick={save} variant="contained" color="primary">
                    {props.mode === "create" ? "Create" : "Save"}
                </Button>
            </DialogActions>
        </Dialog>
    );
}
