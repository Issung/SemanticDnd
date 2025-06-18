using DndTest.Data.Model.Content;
using NpgsqlTypes;
using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace DndTest.Data.Model;

[Table("SearchChunks")]
public class SearchChunk
{
    public int Id { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = null!;

    public string Text { get; set; } = null!;

    /// <summary>
    /// Indexed from 0 so remember to add 1 for display.
    /// TODO: Maybe save them indexed from 1 upwards.
    /// </summary>
    public required int? PageNumber { get; set; }

    /// <summary>
    /// This is a computed property, do not get or set it.
    /// </summary>
    [Column(TypeName = "tsvector")]
    public NpgsqlTsVector TextVector { get; set; } = null!;

    [Column(TypeName = "vector(768)")]
    public Vector EmbeddingVector { get; set; } = null!;
}
