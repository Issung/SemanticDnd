using NpgsqlTypes;
using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndTest.Data.Model;

[Table("SearchChunks")]
public class SearchChunk
{
    public int Id { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public string Text { get; set; } = null!;

    public required int? PageNumber { get; set; }

    /// <summary>
    /// This is a computed property, do not get or set it.
    /// </summary>
    [Column(TypeName = "tsvector")]
    public NpgsqlTsVector TextVector { get; set; } = null!;

    [Column(TypeName = "vector(768)")]
    public Vector EmbeddingVector { get; set; } = null!;
}
