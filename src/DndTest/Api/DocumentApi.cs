using DndTest.Api.Models.Response;
using DndTest.Data;
using DndTest.Services;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Api;

public class DocumentApi(
    DndDbContext dbContext,
    S3Service s3Service
)
{
    public DocumentsResponse GetAll()
    {
        var e = dbContext.Items.Select(i => new Item(i)).AsAsyncEnumerable();
        return new(e);
    }

    public async Task<DocumentResponse> Get(int id)
    {
        var doc = await dbContext.Items
            .Include(i => i.Parent)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.CustomField)
            .Include(i => i.CustomFieldValues)
                .ThenInclude(c => c.Values)
            .SingleAsync(d => d.Id == id);

        var fileUrl = await MaybeGetFileUrl(doc);

        return new(new Item(doc) { FileAccessUrl = fileUrl });
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
