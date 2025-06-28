export class QueryKeys {
    public static bookmarkCollections = 'bookmarkCollections';
    public static bookmarkCollectionItems = 'bookmarkCollectionItems';
    public static browse = 'browse';
    public static items = 'items';
    public static search = 'search';

    /** QueryKey for an item. */
    public static item(id: number) {
        return [QueryKeys.items, id];
    }
} 