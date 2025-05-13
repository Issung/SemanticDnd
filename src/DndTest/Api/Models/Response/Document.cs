using DndTest.Data.Model;

namespace DndTest.Api.Models.Response;

public class Document
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public Category Category { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Text { get; set; }

    public Uri? FileAccessUrl { get; set; }

    public Document(Data.Model.Document doc)
    {
        Id = doc.Id;
        Name = doc.Name;
        Category = doc.Category;
        CreatedAt = doc.CreatedAt;
    }
}
