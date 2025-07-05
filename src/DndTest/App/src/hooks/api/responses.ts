import type { Category } from "./common";

export enum CustomFieldType {
    FreeText = "FreeText",

    SingleChoice = "SingleChoice",
    MultiChoice = "MultiChoice",

    /**
     * Can pick from other existing entries or just enter your own value.
     */
    SingleValue = "SingleValue",

    /**
     * Multiple free text entries (e.g. Tags).
     * I think this should work like Multi-Choice but the UI will be different,
     * and any user can add options not just the admin.
     */
    MultiValue = "MultiValue",

    Integer = "Integer",
    Decimal = "Decimal",
    Date = "Date",
    Boolean = "Boolean",
}

export enum ItemType {
    File = "File",
    Folder = "Folder",
    Note = "Note",
    Shortcut = "Shortcut",
}

export interface Item {
    id: number;
    parentId: number | undefined;
    name: string;
    category: Category;
    createdAt: string; // comes as ISO string
    bookmarkCollectionIds: Array<number>;
    customFields: Array<ItemCustomField>;
    text: string | undefined;
    fileAccessUrl: string | undefined;
}

export interface ItemCustomField {
    // id: number;
    name: string;
    valueInteger: number | undefined;
    // TODO: Other value types.
    values: Array<string> | undefined;
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

export interface CustomFieldsResponse {
  customFields: Array<CustomField>;
}

export interface CustomField {
  id: number;
  name: string;
  type: CustomFieldType; // You may want to decouple this from DB-level enum
  valueInteger?: number;
  options?: Array<CustomFieldOption>;
  conditions?: Array<CustomFieldCondition>;
}

export interface CustomFieldOption {
  id: number;
  name: string;
}

export interface CustomFieldCondition {
  dependsOnCustomFieldId: number;
  requiredOptionIds: Array<number>;
}
