using DndTest.Api.Models.Response;
using DndTest.Data;
using DndTest.Services;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Api;

public class BookmarksApi(
    DndDbContext dbContext,
    SecurityContext securityContext
)
{
    public BookmarkCollectionsResponse GetAll()
    {
        var collections = dbContext.BookmarkCollections
            .Where(bc => bc.UserId == securityContext.UserId)
            .Select(bc => new BookmarkCollectionSummary() { Id = bc.Id, Name = bc.Name })
            .AsAsyncEnumerable();

        return new()
        {
            Collections = collections
        };
    }

    public async Task<BookmarkCollectionResponse> GetBookmarkCollection(int collectionId)
    {
        var bc = await dbContext.BookmarkCollections
            .Where(bc => bc.Id == collectionId && bc.UserId == securityContext.UserId)
            .SingleAsync();

        return new()
        {
            BookmarkCollection = new()
            {
                Id = bc.Id,
                Name = bc.Name,
                Description = bc.Description,
            }
        };
    }

    public ItemsResponse GetBookmarkCollectionItems(int collectionId)
    {
        var items = dbContext.BookmarkCollections
            .Where(bc => bc.Id == collectionId && bc.UserId == securityContext.UserId)
            .SelectMany(bc => bc.Bookmarks.Select(b => b.Item))
            .Include(i => i.CustomFieldValues)
                .ThenInclude(v => v.Values)
            .Select(i => new ItemSummary(i))   // TODO: Doesn't include preview fields for nice display in UI.
            .AsAsyncEnumerable();

        return new(items);
    }
}
