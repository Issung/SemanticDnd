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
        var e = dbContext.Documents.Select(d => new Document(d)).AsAsyncEnumerable();
        return new(e);
    }

    public async Task<DocumentResponse> Get(int id)
    {
        var doc = await dbContext.Documents.Include(d => d.File).SingleAsync(d => d.Id == id);
        var fileUrl = await s3Service.GetAccessUrl(doc.File.S3Key);
        
        return new(new Document(doc) { FileAccessUrl = fileUrl });
    }
}
