using System.ComponentModel.DataAnnotations.Schema;

namespace DndTest.Data.Model;

[Table("Documents")]
public class Document
{
    public int Id { get; set; }
    public string Name { get; set; }
}
