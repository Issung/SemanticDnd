import type { Category } from "./common";

export interface Item {
    id: number;
    name: string;
    category: Category;
    createdAt: string; // comes as ISO string
    customFields: Array<CustomField>;
    text: string | undefined;
    fileAccessUrl: string | undefined;
}

export interface CustomField {
    id: number;
    name: string;
    valueInteger: number | undefined;
    values: Array<string>;
}

export interface BookmarkCollectionsResponse {
    collections: Array<BookmarkCollectionSummary>;
}

export interface BookmarkCollectionResponse {
    bookmarkCollection: BookmarkCollection;
}

export interface BookmarkCollection {
    id: number;
    name: string;
    description: string;
}

export interface BookmarkCollectionSummary {
    id: number;
    name: string;
}

export interface ItemResponse {
    item: Item;
}

export interface ItemsResponse {
    items: Array<ItemSummary>;
}

export interface ItemSummary {
    id: number;
    name: string;
    /** Fields to show in previews (e.g. list view). */
    previewFields: Array<string>;
}

export interface SearchHit {
    item: ItemSummary;
    pageNumber: number | undefined;
}

export interface SearchResponse {
    totalCount: number;
    hits: Array<SearchHit>;
}