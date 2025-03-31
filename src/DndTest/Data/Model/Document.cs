using System.ComponentModel.DataAnnotations.Schema;

namespace DndTest.Data.Model;

[Table("Documents")]
public class Document
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public Category Category { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid FileId { get; set; }
    public File File { get; set; } = default!;
}
