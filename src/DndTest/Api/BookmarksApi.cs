using DndTest.Api.Models.Request;
using DndTest.Api.Models.Response;
using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Exceptions;
using DndTest.Helpers.Extensions;
using DndTest.Services;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace DndTest.Api;

public class BookmarksApi(
    DndDbContext dbContext,
    SecurityContext securityContext
)
{
    public BookmarkCollectionsResponse GetCollections()
    {
        var collections = dbContext.BookmarkCollections
            .Where(bc => bc.UserId == securityContext.UserId)
            .Select(bc => new BookmarkCollectionSummary()
            {
                Id = bc.Id,
                Name = bc.Name,
                BookmarkCount = bc.Bookmarks.Count,
            })
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
            .Include(i => i.CustomFieldValues)  // TODO: Includes all instead of just the ones we want.
                .ThenInclude(v => v.Values)
            .Select(i => new ItemSummary(i))
            .AsAsyncEnumerable();

        return new(items);
    }

    // TODO: Need a bookmark collection limit so a user can't send a request with a million collection ids.
    public async Task SetBookmarksForItem(ItemBookmarksRequest request)
    {
        var collections = await dbContext.BookmarkCollections
            .Where(c => c.UserId == securityContext.UserId)
            .Where(c => c.Bookmarks.Any(b => b.ItemId == request.ItemId) || request.BookmarkCollectionIds.Contains(c.Id))
            .Include(c => c.Bookmarks.Where(b => b.ItemId == request.ItemId))
            .ToArrayAsync();

        var missingCollectionIds = request.BookmarkCollectionIds
            .Where(id => !collections.Select(c => c.Id).Contains(id))
            .ToArray();

        if (missingCollectionIds.Length > 0)
        {
            throw new SecurityException($"Access denied for requested bookmark collection(s): [{missingCollectionIds.StringJoin(", ")}].");
        }

        foreach (var collection in collections)
        {
            var shouldContain = request.BookmarkCollectionIds.Contains(collection.Id);
            var doesContain = collection.Bookmarks.Any(b => b.ItemId == request.ItemId);

            if (shouldContain && !doesContain)
            {
                collection.Bookmarks.Add(new Bookmark
                {
                    BookmarkCollection = collection,
                    ItemId = request.ItemId,
                });
            }
            else if (!shouldContain && doesContain)
            {
                var bookmark = collection.Bookmarks.First(b => b.ItemId == request.ItemId);
                collection.Bookmarks.Remove(bookmark);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Create or update a bookmark collection.
    /// </summary>
    public async Task PutBookmarkCollection(BookmarkCollectionPutRequest request)
    {
        var bookmarkCollection = request.Id != null
            ? await GetCollectionOrNotFound(request.Id.Value)
            : new Data.Model.BookmarkCollection() { UserId = securityContext.UserId };

        bookmarkCollection.Name = request.Name;
        bookmarkCollection.Description = request.Description;

        if (request.Id == null)
        {
            dbContext.BookmarkCollections.Add(bookmarkCollection);
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task<Data.Model.BookmarkCollection> GetCollectionOrNotFound(int id)
    {
        return await dbContext.BookmarkCollections
            .Where(bc => bc.UserId == securityContext.UserId)
            .Where(bc => bc.Id == id)
            .SingleOrDefaultAsync() ?? throw new NotFoundException();
    }
}
