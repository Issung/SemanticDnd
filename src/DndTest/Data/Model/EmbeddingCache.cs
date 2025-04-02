using Microsoft.EntityFrameworkCore;
using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndTest.Data.Model;

/// <summary>
/// This table is not searched on, just here to cache generated embedding floats.
/// </summary>
[Table("EmbeddingCache")]
[Index(nameof(TextHash), nameof(Model))]
public class EmbeddingCache
{
    public int Id { get; set; }

    public string Text { get; set; } = default!;

    /// <summary>
    /// Xxhash128 hash of the text.
    /// </summary>
    public string TextHash { get; set; } = default!;

    public string Model { get; set; } = default!;

    public Vector Vector { get; set; } = default!;
}
