import AddIcon from '@mui/icons-material/Add';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';
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
    const { mutate: folderPut } = useFolderPut();
    const [folderCreateDialogOpen, setFolderCreateDialogOpen] = useState(false);

    // Memoize adornment to prevent re-render of the full page every time adornment is interacted with.
    const adornment = useMemo(() => <HeaderAdornment browseResponse={data} create={createItem} />, [folderId, data]);

    function createItem(type: ItemType) {
        if (type === ItemType.Folder) {
            setFolderCreateDialogOpen(true);
        }
        else {
            alert('Not implemented yet.');
        }
    }

    setHeader({
        // TODO: Possibly API returns parent folder name for back button? To display in header.
        // TODO: BrowseResponse now contains folderId. It should be used here for more accurate UX.
        // TODO: Maybe we should expose options to customize the button (E.g. say "up" and change icon).
        back: Boolean(folderId),    // If within a folder, then display back button.
        title: data?.folderName ?? 'Loading...',
        adornment: adornment,
    }, [folderId, data, adornment]);

    console.log('BrowsePage', folderId);
    return (
        <>
            {isPending && <CircularProgress />}
            {isError && <span><ErrorIcon /> Something went wrong...</span>}
            {!isPending && !isError &&
                <>
                    <ItemList hits={data.items.map(ItemListDisplayAdapter.fromSummary)} />

                    <FolderDetailsDialog
                        mode="create"
                        details={{ parentId: folderId, name: '', description: '' }}
                        open={folderCreateDialogOpen}
                        onClose={(details) => {
                            if (details) {
                                folderPut({ id: undefined, request: details })
                            }

                            setFolderCreateDialogOpen(false);
                        }}
                    />
                </>}
        </>
    );
}

function HeaderAdornment(props: {
    browseResponse: BrowseResponse | undefined;
    create: (type: ItemType) => void;
}) {
    const navigate = useNavigate();
    const { mutate: deleteFolder } = useItemDelete(onDeleteSuccess);
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [addMenuAnchor, setAddMenuAnchor] = useState<null | HTMLElement>(null);

    async function onDeleteSuccess() {
        await navigate(Navigations.browse(props.browseResponse?.parentId));
        setDeleteDialogOpen(false);
    }

    return <>
        {props.browseResponse && <>
            {props.browseResponse.folderId &&
                <IconButton onClick={() => setDeleteDialogOpen(true)}>
                    <DeleteOutlineIcon />
                </IconButton>
            }

            {/* TODO: Implement edit, using the same folder dialog and put mutation. */}

            <IconButton onClick={(e) => setAddMenuAnchor(e.currentTarget)}>
                <AddIcon />
            </IconButton>

            <ConfirmDialog
                open={deleteDialogOpen}
                confirmColor="error"
                confirmText="Delete"
                title="Delete Folder?"
                message={<>Are you sure you want to delete folder <i>{props.browseResponse.folderName}</i>? All content within will also be deleted.</>}
                onConfirm={() => {
                    if (props.browseResponse?.folderId) {
                        deleteFolder(props.browseResponse.folderId);
                    }
                }}
                onCancel={() => setDeleteDialogOpen(false)}
            />

            <Menu
                anchorEl={addMenuAnchor}
                open={Boolean(addMenuAnchor)}
                onClose={() => setAddMenuAnchor(null)}
                disableScrollLock   // Locking scroll (hidig scrollbar) causes slight resize.. bad UX.
                anchorOrigin={{
                    vertical: 'bottom',
                    horizontal: 'left',
                }}
                transformOrigin={{
                    vertical: 'top',
                    horizontal: 'left',
                }}
            >
                <MenuItem onClick={() => props.create(ItemType.Note)}>
                    <ListItemIcon><NoteIcon fontSize="small" /></ListItemIcon>
                    <ListItemText primary="Note" />
                </MenuItem>
                <MenuItem onClick={() => props.create(ItemType.File)}>
                    <ListItemIcon><InsertDriveFileIcon fontSize="small" /></ListItemIcon>
                    <ListItemText primary="File" />
                </MenuItem>
                <MenuItem onClick={() => props.create(ItemType.Shortcut)}>
                    <ListItemIcon><FileOpenIcon fontSize="small" /></ListItemIcon>
                    <ListItemText primary="Shortcut" />
                </MenuItem>
                <MenuItem onClick={() => props.create(ItemType.Folder)}>
                    <ListItemIcon><FolderIcon fontSize="small" /></ListItemIcon>
                    <ListItemText primary="Folder" />
                </MenuItem>
            </Menu>
        </>
        }
    </>
}