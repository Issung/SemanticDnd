using Microsoft.EntityFrameworkCore;

namespace DndTest.Data.Model;

[PrimaryKey(nameof(FileHash))]
public class TikaCache
{
    public string FileHash { get; set; } = null!;

    public string TikaResponseJson { get; set; } = null!;
}
