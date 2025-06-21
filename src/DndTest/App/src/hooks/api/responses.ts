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

export interface ItemResponse {
    item: Item;
}

export interface ItemsResponse {
    items: Array<Item>;
}

export interface SearchHit {
    name: string;
    /** Fields to show in preview (e.g. list view). */
    previewFields: Array<string>;
    itemId: number;
    pageNumber: number | undefined;
}

export interface SearchResponse {
    totalCount: number;
    hits: Array<SearchHit>;
}