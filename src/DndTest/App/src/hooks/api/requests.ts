import type { Category } from "./common";

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