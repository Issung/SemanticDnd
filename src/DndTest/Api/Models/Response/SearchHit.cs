using DndTest.Data.Model;

namespace DndTest.Api.Models.Response;

public class SearchHit
{
    public string Name { get; set; } = default!;

    public int DocumentId { get; set; }

    public int? PageNumber { get; set; }

    public SearchHit(SearchChunk chunk)
    {
        this.Name = chunk.Item.Name;
        this.DocumentId = chunk.ItemId;
        this.PageNumber = chunk.PageNumber;
    }
}
