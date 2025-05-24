import type { Category } from "./common";

export interface Document {
    id: number;
    name: string;
    category: Category;
    createdAt: string; // comes as ISO string
    text: string | undefined;
    fileAccessUrl: string | undefined;
}

export interface DocumentResponse {
    document: Document;
}

export interface DocumentsResponse {
    documents: Array<Document>;
}

export interface SearchHit {
    name: string;
    category: Category;
    documentId: number;
    pageNumber: number | undefined;
}

export interface SearchResponse {
    totalCount: number;
    hits: Array<SearchHit>;
}