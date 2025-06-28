import {
    Button,
    Dialog,
    DialogActions,
    DialogContent,
    DialogContentText,
    DialogTitle,
} from "@mui/material";
import type { ReactNode } from "react";

export interface ConfirmDialogProps {
    open: boolean;
    title?: string;
    /** Plain string or ReactNode, to support basic text formatting like italics or bold. */
    message?: ReactNode | string;
    confirmText?: string;
    cancelText?: string;
    confirmColor?: "inherit" | "primary" | "secondary" | "success" | "error" | "info" | "warning";
    onConfirm: () => void;
    onCancel: () => void;
}

export default function ConfirmDialog({
    open,
    title = "Confirm",
    message = "Are you sure?",
    confirmText = "OK",
    cancelText = "Cancel",
    confirmColor = "primary",
    onConfirm,
    onCancel,
}: ConfirmDialogProps) {
    return (
        <Dialog open={open} onClose={onCancel}>
            <DialogTitle>{title}</DialogTitle>
            <DialogContent>
                <DialogContentText>{message}</DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={onCancel}>{cancelText}</Button>
                <Button onClick={onConfirm} color={confirmColor} variant="contained">
                    {confirmText}
                </Button>
            </DialogActions>
        </Dialog>
    );
};