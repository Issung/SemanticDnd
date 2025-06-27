using DndTest.Api.Models.Response;

namespace DndTest.Api.Models.Request;

public class BookmarkCollectionPutRequest : BookmarkCollection
{
    /// <summary>
    /// Null = create new collection. Set = update existing.
    /// </summary>
    new public int? Id { get; set; }
}
