import { Category } from "./common";

export interface SearchRequest {
    query: string | undefined;
    category: Category | undefined;
}