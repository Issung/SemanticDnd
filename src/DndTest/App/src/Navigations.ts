/** A sort of class to help reduce repition of routes stuff.. Not perfect, needs more examples. */
export default class Navigations {
    /**
     * @param replace Remove the current route (we are navigating away from) from history (useful for deletes).
     */
    public static browse(folderId?: number | undefined, options?: { replace?: boolean }) {
        return {
            to: '/browse/$folderId',
            params: { folderId: folderId ?? 0 },
            replace: options?.replace,
        };
    }
}