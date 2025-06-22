using DndTest.Helpers.Extensions;

namespace DndTest.Api.Models.Response;

public class ItemSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public IEnumerable<string> PreviewFields { get; set; } = [];

    public ItemSummary(Data.Model.Content.Item item)
    {
        this.Id = item.Id;
        this.Name = item.Name;
        this.PreviewFields = item.CustomFieldValues.Select(v => v.ValueInteger?.ToString() ?? v.Values.Select(v => v.Name).StringJoin(", "));
    }

    public ItemSummary(Data.Model.SearchChunk chunk)
    {
        this.Id = chunk.ItemId;
        this.Name = chunk.Item.Name;
        this.PreviewFields = chunk.Item.CustomFieldValues.Select(v => v.ValueInteger?.ToString() ?? v.Values.Select(v => v.Name).StringJoin(", "));
    }
}
