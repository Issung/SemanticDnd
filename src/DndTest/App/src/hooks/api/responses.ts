import type { Category } from "./common";

export enum ItemType
{
    File = "File",
    Folder = "Folder",
    Note = "Note",
    Shortcut = "Shortcut",
}

export interface Item {
    id: number;
    name: string;
    category: Category;
    createdAt: string; // comes as ISO string
    bookmarkCollectionIds: Array<number>;
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
    bookmarkCount: number;
}

export interface ItemResponse {
    item: Item;
}

export interface ItemsResponse {
    count: number;
    items: Array<ItemSummary>;
}

export interface ItemSummary {
    id: number;
    name: string;
    type: ItemType;
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

export interface BrowseResponse {
    /** In the case of browsing root this will be undefined but there truly will be no parent. */
    parentId: number | undefined;
    folderId: number | undefined;
    folderName: string;
    folderDescription: string;
    itemCount: number;
    items: Array<ItemSummary>;
}