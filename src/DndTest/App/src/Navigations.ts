/** A sort of class to help reduce repition of routes stuff.. Not perfect, needs more examples. */
export default class Navigations {
    public static browse(folderId?: number | undefined) {
        return { to: '/browse/$folderId', params: { folderId: folderId ?? 0 } };
    }
}