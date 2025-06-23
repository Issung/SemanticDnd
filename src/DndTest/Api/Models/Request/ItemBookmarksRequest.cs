namespace DndTest.Api.Models.Request;

public class ItemBookmarksRequest
{
    public int ItemId { get; set; }
    public required IReadOnlyCollection<int> BookmarkCollectionIds { get; set; }
}
