namespace DndTest.Helpers.Extensions;

public static class IEnumerableExtensions
{
    public static string StringJoin<T>(this IEnumerable<T> items, string separator)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(separator);

        return string.Join(separator, items);
    }
}
