namespace DndTest.Api.Models.Response;

public class BookmarkCollectionsResponse
{
    public required IAsyncEnumerable<BookmarkCollectionSummary> Collections { get; set; }
}
