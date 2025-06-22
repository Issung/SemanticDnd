using DndTest.Data.Model;

namespace DndTest.Api.Models.Response;

public class SearchHit
{
    public ItemSummary Item { get; set; }
    public int? PageNumber { get; set; }

    public SearchHit(SearchChunk chunk)
    {
        this.Item = new(chunk.Item);
        this.PageNumber = chunk.PageNumber;
    }
}
