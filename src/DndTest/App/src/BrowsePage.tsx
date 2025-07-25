import AddIcon from '@mui/icons-material/Add';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';
import EditIcon from '@mui/icons-material/Edit';
import ErrorIcon from '@mui/icons-material/Error';
import FileOpenIcon from '@mui/icons-material/FileOpen';
import FolderIcon from '@mui/icons-material/Folder';
import InsertDriveFileIcon from '@mui/icons-material/InsertDriveFile';
import NoteIcon from '@mui/icons-material/Note';
import { CircularProgress, IconButton, ListItemIcon, ListItemText, Menu, MenuItem } from "@mui/material";
import { useNavigate } from '@tanstack/react-router';
import { useMemo, useState } from "react";
import ConfirmDialog from './components/ConfirmDialog';
import FolderDetailsDialog from './components/FolderDetailsDialog';
import { setHeader } from "./components/HeaderContext";
import ItemList, { ItemListDisplayAdapter } from "./components/ItemsList";
import type { BrowseResponse } from './hooks/api/responses';
import { ItemType } from './hooks/api/responses';
import { useBrowse } from "./hooks/api/useBrowse";
import useFolderPut from './hooks/api/useFolderPut';
import useItemDelete from './hooks/api/useItemDelete';
import { browseRoute } from "./main";
import Navigations from './Navigations';

export default function BrowsePage() {
    const rawParams = browseRoute.useParams();
    const folderId = rawParams.folderId === 0 ? undefined : rawParams.folderId;
    const { data, isPending, isError } = useBrowse(folderId);

    // TODO: Maybe this stuff should be within HeaderAdornment.
    
    // Memoize adornment to prevent re-render of the full page every time adornment is interacted with.
    const adornment = useMemo(() => <BrowseHeaderAdornment browseResponse={data} />, [data]);

    setHeader({
        // TODO: Possibly API returns parent folder name for back button? To display in header.
        // TODO: BrowseResponse now contains folderId. It should be used here for more accurate UX.
        // TODO: Maybe we should expose options to customize the button (E.g. say "up" and change icon).
        back: Boolean(folderId),    // If within a folder, then display back button.
        title: data?.folderName ?? 'Loading...',
        adornment: adornment,
    }, [folderId, data, adornment]);

    console.log('BrowsePage', { folderId, data, isPending, isError });
    return (
        <>
            {isPending && <CircularProgress />}
            {isError && <span><ErrorIcon /> Something went wrong...</span>}
            {!isPending && !isError &&
                <>
                    <p>{data.folderDescription}</p>
                    <ItemList hits={data.items.map(ItemListDisplayAdapter.fromSummary)} />
                </>}
        </>
    );
}

function BrowseHeaderAdornment(props: {
    browseResponse: BrowseResponse | undefined;
}) {
    const navigate = useNavigate();

    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [addMenuAnchor, setAddMenuAnchor] = useState<null | HTMLElement>(null);
    const [folderDialogMode, setFolderDialogMode] = useState<"create" | "edit" | null>(null);

    const { mutate: deleteFolder, isPending: deletePending } = useItemDelete(onDeleteSuccess);
    const { mutate: folderPut, isPending: folderPutPending } = useFolderPut();
    const anythingPending = deletePending || folderPutPending;

    function createItem(type: ItemType) {
        if (!props.browseResponse) {
            return;
        }

        if (type === ItemType.Folder) {
            setFolderDialogMode("edit");
        }
        else if (type === ItemType.File) {
            navigate({to: '/create/file', search: { parentId: props.browseResponse.folderId }});
        }
        else {
            alert('Not implemented yet.');
        }

        setAddMenuAnchor(null);
    }

    async function onDeleteSuccess() {
        await navigate(Navigations.browse(props.browseResponse?.parentId, { replace: true }));
        setDeleteDialogOpen(false);
    }

    return <>
        {props.browseResponse && <>
            <IconButton onClick={() => setFolderDialogMode("edit")} disabled={anythingPending} loading={folderPutPending}>
                <EditIcon/>
            </IconButton>

            <IconButton onClick={(e) => setAddMenuAnchor(e.currentTarget)} disabled={anythingPending}>
                <AddIcon />
            </IconButton>

            {/* Can't delete root. */}
            {props.browseResponse.folderId &&
                <IconButton onClick={() => setDeleteDialogOpen(true)} disabled={anythingPending} loading={deletePending}>
                    <DeleteOutlineIcon />
                </IconButton>
            }

            <ConfirmDialog
                open={deleteDialogOpen}
                confirmColor="error"
                confirmText="Delete"
                title="Delete Folder?"
                message={<>Are you sure you want to delete folder <i>{props.browseResponse.folderName}</i>? All content within will also be deleted.</>}
                onConfirm={() => {
                    if (props.browseResponse?.folderId) {
                        deleteFolder({id: props.browseResponse.folderId, parentId: props.browseResponse.parentId });
                    }
                }}
                onCancel={() => setDeleteDialogOpen(false)}
            />

            <Menu
                anchorEl={addMenuAnchor}
                open={Boolean(addMenuAnchor)}
                onClose={() => setAddMenuAnchor(null)}
                disableScrollLock   // Locking scroll (hiding scrollbar) causes slight resize.. bad UX.
                anchorOrigin={{
                    vertical: 'bottom',
                    horizontal: 'left',
                }}
                transformOrigin={{
                    vertical: 'top',
                    horizontal: 'left',
                }}
            >
                <MenuItem onClick={() => createItem(ItemType.Note)}>
                    <ListItemIcon><NoteIcon fontSize="small" /></ListItemIcon>
                    <ListItemText primary="Note" />
                </MenuItem>
                <MenuItem onClick={() => createItem(ItemType.File)}>
                    <ListItemIcon><InsertDriveFileIcon fontSize="small" /></ListItemIcon>
                    <ListItemText primary="File" />
                </MenuItem>
                <MenuItem onClick={() => createItem(ItemType.Shortcut)}>
                    <ListItemIcon><FileOpenIcon fontSize="small" /></ListItemIcon>
                    <ListItemText primary="Shortcut" />
                </MenuItem>
                <MenuItem onClick={() => createItem(ItemType.Folder)}>
                    <ListItemIcon><FolderIcon fontSize="small" /></ListItemIcon>
                    <ListItemText primary="Folder" />
                </MenuItem>
            </Menu>

            <FolderDetailsDialog
                open={Boolean(folderDialogMode)}
                mode={folderDialogMode ?? "create"} // Just satisfy the type restraint if not open.
                details={{
                    parentId: props.browseResponse.parentId,
                    name: folderDialogMode == "create" ? '' : props.browseResponse.folderName,
                    description: folderDialogMode == "create" ? '' : props.browseResponse.folderDescription,
                }}
                onClose={(details) => {
                    if (details && props.browseResponse) {
                        folderPut({
                            id: folderDialogMode == "create" ? undefined : props.browseResponse.folderId,
                            request: details
                        })
                    }

                    setFolderDialogMode(null);
                }}
            />
        </>
        }
    </>
}