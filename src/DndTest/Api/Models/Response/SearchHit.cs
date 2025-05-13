namespace DndTest.Api.Models.Response;

public class SearchHit
{
    public string Name { get; set; } = default!;
    public int DocumentId { get; set; }

    public int? PageNumber { get; set; }
}
