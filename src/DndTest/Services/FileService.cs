using DndTest.Data;
using DndTest.Helpers.Extensions;
using File = DndTest.Data.Model.File;

namespace DndTest.Services;

public class FileService(
    DndDbContext dbContext,
    S3Service s3Service
)
{
    public async Task<File> Upload(Stream stream, string filename, string contentType)
    {
        var hash = stream.XXHash128();

        var existingRecord = dbContext.Files.SingleOrDefault(f => f.Hash == hash);

        if (existingRecord != null)
        {
            return existingRecord;
        }

        var id = Guid.NewGuid();
        var extension = Path.GetExtension(filename) ?? throw new Exception("File extension unexpectedly null.");
        var key = id + extension;

        stream.Position = 0;
        await s3Service.Put(key, stream, contentType);

        var newRecord = new File
        {
            Id = id,
            S3Key = key,
            Hash = hash,
            SizeBytes = stream.Length,
            ContentType = contentType,
        };

        dbContext.Files.Add(newRecord);

        await dbContext.SaveChangesAsync();

        return newRecord;
    }
}
