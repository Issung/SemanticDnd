export class QueryKeys {
    public static bookmarkCollections = 'bookmarkCollections';
    public static bookmarkCollectionItems = 'bookmarkCollectionItems';
    public static items = 'items';
    public static search = 'search';

    /** QueryKey for an item. */
    public static item(id: number) {
        return [QueryKeys.items, id];
    }

    /** QueryKey for browsing folder `id`. */
    public static browse(id: number | undefined) {
        return ['browse', id];
    }
} 