using DndTest.Helpers.Extensions;

namespace DndTest.Api.Models.Response;

public class ItemSummary
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public ItemType Type { get; set; }
    public IEnumerable<string> PreviewFields { get; set; } = [];

    public ItemSummary(Data.Model.Content.Item item)
    {
        this.Id = item.Id;
        this.Name = item.Name;
        this.PreviewFields = item.CustomFieldValues.Select(v => v.ValueInteger?.ToString() ?? v.Values.Select(v => v.Name).StringJoin(", "));
        this.Type = item switch
        {
            Data.Model.Content.File => ItemType.File,
            Data.Model.Content.Folder => ItemType.Folder,
            Data.Model.Content.Note => ItemType.Note,
            Data.Model.Content.Shortcut => ItemType.Shortcut,
            _ => throw new Exception($"Unknown item type '{item.GetType().FullName}'."),
        };
    }

    public ItemSummary(Data.Model.SearchChunk chunk) : this(chunk.Item)
    {
    }
}
