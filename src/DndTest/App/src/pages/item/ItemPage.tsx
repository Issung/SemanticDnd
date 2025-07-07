import { BookmarkCollectionsDialog } from '@/components/BookmarkCollectionsDialog';
import ConfirmDialog from '@/components/ConfirmDialog';
import { CustomFields } from '@/components/CustomFields';
import { setHeader } from '@/components/HeaderContext';
import type { ItemResponse } from '@/hooks/api/responses';
import { useItem } from '@/hooks/api/useItem';
import useItemDelete from '@/hooks/api/useItemDelete';
import { useSetItemBookmarks } from '@/hooks/api/useSetItemBookmarks';
import Navigations from '@/Navigations';
import BookmarkIcon from '@mui/icons-material/Bookmark';
import BookmarkBorderIcon from '@mui/icons-material/BookmarkBorder';
import DeleteOutlineIcon from '@mui/icons-material/DeleteOutline';
import EditIcon from '@mui/icons-material/Edit';
import { IconButton } from '@mui/material';
import { useNavigate, useParams } from '@tanstack/react-router';
import { useEffect, useMemo, useState } from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';

export default function ItemPage() {
    const { id } = useParams({ strict: false }) // will be typed later with route param
    const [fileType, setFileType] = useState<null | 'text' | 'image' | 'pdf'>(null);
    const [fileContent, setFileContent] = useState<string | Blob | null>(null);
    const [fileError, setFileError] = useState<string | null>(null);
    const { data, isPending, isError } = useItem(id!);

    const adornment = useMemo(() => <HeaderAdornment data={data} />, [data]);

    setHeader({
        back: true,
        adornment: adornment
    }, [data]);

    useEffect(() => {
        const load = async () => {
            if (data?.item.fileAccessUrl)
            {
                setFileContent(null);
                setFileError(null);
                const res = await fetch(data.item.fileAccessUrl);

                if (!res.ok)
                {
                    setFileError('Failed to fetch file. Status: ' + res.status);
                }

                const ct = res.headers.get('Content-Type') ?? '';
                if (ct.startsWith('text/plain'))
                {
                    const text = await res.text();
                    setFileType('text');
                    setFileContent(text);
                }
                else if (ct.startsWith('image/') || ct.startsWith('application/pdf'))
                {
                    const blob = await res.blob();
                    setFileType(ct.startsWith('image/') ? 'image' : 'pdf');
                    setFileContent(blob);
                }
                else
                {
                    setFileError('Unsupported file type: ' + ct);
                }
            }
        }

        load();
    }, [data?.item.fileAccessUrl]);

    // TODO: Disable all fields while `isPending` (save is in progress).
    return <>
        <div>
            {isPending && <p>Loading...</p>}
            {isError && <p>Error loading item</p>}
            {data && (
                <div>
                    <h1>{data.item.name}</h1>
                    <p>{data.item.description}</p>
                    <CustomFields fields={data.item.customFields} />
                    {data.item.text && <ReactMarkdown remarkPlugins={[remarkGfm]}>{data.item.text}</ReactMarkdown>}
                    {data.item.fileAccessUrl && (
                        <>
                            <a href={data.item.fileAccessUrl} target="_blank" rel="noopener noreferrer">
                                Download File
                            </a>
                            <div style={{ marginTop: '1em' }}>
                                <h3>File Contents:</h3>
                                {fileError && <p style={{ color: 'red' }}>Error: {fileError}</p>}
                                {fileContent === null && !fileError && <p>Loading file...</p>}
                                {fileContent !== null && (
                                    fileType === 'image' ? (
                                        <img src={URL.createObjectURL(fileContent as Blob)} />
                                    ) : fileType === 'pdf' ? (
                                        <embed src={URL.createObjectURL(fileContent as Blob)} />
                                    ) : fileType === 'text' ? (
                                        <pre style={{ padding: '1em', textWrap: 'wrap' }}>{fileContent as string}</pre>
                                    ) : <></>
                                )}
                            </div>
                        </>
                    )}
                </div>
            )}
        </div>
    </>
}

function HeaderAdornment({ data }: { data: ItemResponse | undefined }) {
    const navigate = useNavigate();
    const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
    const [bookmarkDialogOpen, setBookmarkDialogOpen] = useState(false);
    const { mutate: mutateBookmarks, isPending: mutateBookmarksPending } = useSetItemBookmarks();

    // TODO: There are multiple routes the user could have taken to get here, e.g. via browse or bookmarks.
    // We need some state to know where to go back to. We can't use history.back() because it is not awaitable. 
    // This causes the page to fruitlessly attempt reloading the item after we invalidate the queries.
    const { mutate: deleteItem, isPending: deletePending } = useItemDelete(async () => await navigate(Navigations.browse(data?.item.parentId, { replace: true })));

    const pending = mutateBookmarksPending || deletePending;

    return !data
        ? <></>
        : <>
            <IconButton onClick={() => setBookmarkDialogOpen(true)} loading={pending}>
                {data.item.bookmarkCollectionIds.length == 0
                    ? <BookmarkBorderIcon />
                    : <BookmarkIcon />
                }
            </IconButton>
            <IconButton loading={pending}>
                <EditIcon />
            </IconButton>
            <IconButton loading={pending} onClick={() => setDeleteDialogOpen(true)}>
                <DeleteOutlineIcon />
            </IconButton>

            <ConfirmDialog
                confirmColor='error'
                confirmText='Delete'
                title='Delete Item?'
                message={<>Are you sure you want to delete item <i>{data.item.name}</i>? This action cannot be undone.</>}
                open={deleteDialogOpen}
                onCancel={() => setDeleteDialogOpen(false)}
                onConfirm={() => deleteItem({id: data.item.id, parentId: data.item.parentId})}
            />

            <BookmarkCollectionsDialog
                open={bookmarkDialogOpen}
                selectedCollectionIds={data.item.bookmarkCollectionIds}
                onClose={(collectionIds) => {
                    setBookmarkDialogOpen(false);

                    // If ids are set then the user hit save (didn't cancel).
                    if (collectionIds)
                    {
                        mutateBookmarks({ itemId: data.item.id, bookmarkCollectionIds: collectionIds });
                    }
                }}
            />
        </>
}
