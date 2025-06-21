using DndTest.Data.Model;
using DndTest.Helpers.Extensions;

namespace DndTest.Api.Models.Response;

public class SearchHit
{
    public string Name { get; set; } = default!;

    public IEnumerable<string> PreviewFields { get; set; } = [];

    public int ItemId { get; set; }

    public int? PageNumber { get; set; }

    public SearchHit(SearchChunk chunk)
    {
        this.Name = chunk.Item.Name;
        this.PreviewFields = chunk.Item.CustomFieldValues.Select(v => v.ValueInteger?.ToString() ?? v.Values.Select(v => v.Name).StringJoin(", "));
        this.ItemId = chunk.ItemId;
        this.PageNumber = chunk.PageNumber;
    }
}
