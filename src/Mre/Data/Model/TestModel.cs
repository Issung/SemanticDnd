using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mre.Data.Model;

/// <summary>
/// This table is not searched on, just here to cache generated embedding floats.
/// </summary>
[Table("EmbeddingCache")]
public class TestModel
{
    public int Id { get; set; }

    [Column(TypeName = "vector(768)")]
    public Vector Vector { get; set; } = default!;
}
