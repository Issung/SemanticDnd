using DndTest.Api.Models.Response;
using DndTest.Data;
using DndTest.Data.Model.Content;
using DndTest.Exceptions;
using DndTest.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql.PostgresTypes;

namespace DndTest.Api;

public class ItemApi(
    DndDbContext dbContext,
    S3Service s3Service,
    SecurityContext securityContext
)
{
    /// <param name="folderId">Null for root of tenancy.</param>
    /// <exception cref="NotFoundException"/>
    /// <exception cref="BadRequestException"/>
    public async Task<ItemsResponse> Browse(int? folderId)
    {
        if (folderId.HasValue)
        {
            var folder = await dbContext.Items
                .Where(i => i.TenantId == securityContext.TenancyId)
                .Where(i => i.Id == folderId)
                .SingleOrDefaultAsync();

            if (folder == null)
            {
                throw new NotFoundException("Folder not found.");
            }

            if (folder is not Folder)
            {
                throw new BadRequestException("Can only browse into a folder.");
            }
        }

        var query = dbContext.Items
            .Where(i => i.TenantId == securityContext.TenancyId)
            .Where(i => i.ParentId == folderId);

        var count = await query.CountAsync();

        var items = query
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.CustomField)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.Values)
            .OrderByDescending(i => i is Folder)    // Place folders at top.
            .ThenBy(i => i.Name)
            .Select(i => new ItemSummary(i))
            .AsAsyncEnumerable();

        return new(count, items);
    }

    public async Task<ItemResponse> Get(int id)
    {
        var item = await dbContext.Items
            .Include(i => i.Parent)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.CustomField)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.Values)
            .Include(i => i.Bookmarks.Where(b => b.BookmarkCollection.UserId == securityContext.UserId))
            .SingleAsync(d => d.Id == id);

        var fileUrl = await MaybeGetFileUrl(item);

        var model = new Models.Response.Item(item)
        {
            FileAccessUrl = fileUrl,
            Text = item is Note note ? note.Content : null,
        };

        return new(model);
    }

    private async Task<Uri?> MaybeGetFileUrl(Data.Model.Content.Item doc)
    {
        if (doc is Data.Model.Content.File file)
        {
            return await s3Service.GetAccessUrl(file.S3ObjectKey);
        }

        return null;
    }
}
