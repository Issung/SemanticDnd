using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndTest.Data.Model;

[Table("Files")]
[Index(nameof(Hash))]
public class File
{
    public Guid Id { get; set; }

    public string S3Key { get; set; } = default!;

    public string Hash { get; set; } = default!;

    public long SizeBytes { get; set; } = default!;

    public string ContentType { get; set; } = default!;
}
