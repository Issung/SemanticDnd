using DndTest.Data;
using DndTest.Helpers.Extensions;
using File = DndTest.Data.Model.Content.File;

namespace DndTest.Services;

public class FileService(
    DndDbContext dbContext,
    S3Service s3Service
)
{
    public async Task<File> Upload(Stream stream, string filename, string contentType)
    {
        var hash = stream.XXHash128();

        var existingRecord = dbContext.Files.SingleOrDefault(f => f.FileHash == hash);

        if (existingRecord != null)
        {
            return existingRecord;
        }

        // TODO: Use tenancy id within the s3 key.
        var extension = Path.GetExtension(filename) ?? throw new Exception("File extension unexpectedly null.");
        var key = Guid.NewGuid() + extension;

        stream.Position = 0;
        await s3Service.Put(key, stream, contentType);

        var newFile = new File
        {
            S3ObjectKey = key,
            FileHash = hash,
            SizeBytes = stream.Length,
            ContentType = contentType,
        };

        dbContext.Items.Add(newFile);

        await dbContext.SaveChangesAsync();

        return newFile;
    }
}
