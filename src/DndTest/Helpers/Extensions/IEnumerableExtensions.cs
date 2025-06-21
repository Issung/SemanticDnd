namespace DndTest.Helpers.Extensions;

public static class IEnumerableExtensions
{
    public static string StringJoin(this IEnumerable<string> items, string separator)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(separator);

        return string.Join(separator, items);
    }
}
