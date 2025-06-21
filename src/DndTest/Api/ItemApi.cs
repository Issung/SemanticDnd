using DndTest.Api.Models.Response;
using DndTest.Data;
using DndTest.Data.Model.Content;
using DndTest.Services;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Api;

public class ItemApi(
    DndDbContext dbContext,
    S3Service s3Service
)
{
    public ItemsResponse GetAll()
    {
        var e = dbContext.Items
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.CustomField)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.Values)
            .Select(i => new Models.Response.Item(i))
            .AsAsyncEnumerable();

        return new(e);
    }

    public async Task<ItemResponse> Get(int id)
    {
        var item = await dbContext.Items
            .Include(i => i.Parent)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.CustomField)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.Values)
            .SingleAsync(d => d.Id == id);

        var fileUrl = await MaybeGetFileUrl(item);

        var model = new Models.Response.Item(item)
        {
            FileAccessUrl = fileUrl,
            
            Text = item is Note note ? note.Content : null
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
