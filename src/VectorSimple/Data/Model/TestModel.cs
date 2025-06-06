using Pgvector;
using System.ComponentModel.DataAnnotations.Schema;

namespace VectorSimple.Data.Model;

public class TestModel
{
    public int Id { get; set; }

    [Column(TypeName = "vector(768)")]
    public required Vector Embedding { get; set; }
}