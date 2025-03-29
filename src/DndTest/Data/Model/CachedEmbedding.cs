using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndTest.Data.Model;

/// <summary>
/// This table is not searched on, just here to cache generated embedding floats.
/// </summary>
[Table("CachedEmbeddings")]
[PrimaryKey(nameof(Id))]
[Index(nameof(TextHash))]
public class CachedEmbedding
{
    public int Id { get; set; }

    public string Text { get; set; } = default!;

    /// <summary>
    /// Xxhash128 hash of the text.
    /// </summary>
    public string TextHash { get; set; } = default!;

    public IReadOnlyList<float> Floats { get; set; } = default!;
}
