import type { Category } from "./common";
import type { BookmarkCollection } from "./responses";

export interface SearchRequest {
    query: string | undefined;
    category: Category | undefined;
}

export interface ItemBookmarksRequest {
    itemId: number;
    bookmarkCollectionIds: Array<number>;
}

export interface CreateBookmarkCollectionRequest {
    name: string;
    description?: string;
}

export interface BookmarkCollectionPutRequest extends Omit<BookmarkCollection, 'id'>{
    /** Undefined = create new collection. Set = update existing. */
    id?: number | undefined;
}

export interface ItemPutRequest {
    parentId?: number;
    name: string;
    description: string;
}

export interface FilePutRequest extends ItemPutRequest {
    // No additional fields for files yet.
}

export interface FolderPutRequest extends ItemPutRequest {
    // No additional fields for folders yet.
}

export interface NotePutRequest extends ItemPutRequest {
    content: string;
}

export interface ShortcutPutRequest extends ItemPutRequest {
    targetId: number;
    pageNumber?: number;
}
