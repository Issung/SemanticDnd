namespace DndTest.Api.Models.Response;

/// <summary>
/// Not quite the right name, it's not quite a summary either, it's just for when viewing an item.
/// </summary>
public class BookmarkCollectionSummary
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;

    public int BookmarkCount { get; set; }
}
