using System.ComponentModel.DataAnnotations.Schema;

namespace DndTest.Data.Model;

[Table("SearchChunks")]
public class SearchChunk
{
    public int Id { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public int EmbeddingId { get; set; }
    public CachedEmbedding Embedding { get; set; } = null!;

    public string Text { get; set; } = null!;

    public NpgsqlTypes.NpgsqlTsVector TextVector { get; set; } = null!;

    public IReadOnlyList<float> EmbeddingVector { get; set; } = null!;
}
