namespace DndTest.Data.Model.Content;

/// <summary>
/// Content that is an uploaded file.
/// </summary>
public class File : Item
{
    public string S3ObjectKey { get; set; } = default!;

    public string FileHash { get; set; } = default!;

    public long SizeBytes { get; set; } = default!;

    public string ContentType { get; set; } = default!;
}
