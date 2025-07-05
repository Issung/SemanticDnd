using DndTest.Data.Model.Content;

namespace DndTest.Api.Models.Response;

public class Item
{
    public int Id { get; set; }

    public int? ParentId { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string? Text { get; set; }

    public Uri? FileAccessUrl { get; set; }

    /// <summary>
    /// This item is bookmarked in these collections (for the fetching user).
    /// </summary>
    public IEnumerable<int> BookmarkCollectionIds { get; set; }

    public IEnumerable<ItemCustomField> CustomFields { get; set; }

    public Item(Data.Model.Content.Item item)
    {
        Id = item.Id;
        ParentId = item.ParentId;
        Name = item.Name;
        Description = item.Description;
        CreatedAt = item.CreatedAt;
        UpdatedAt = item.UpdatedAt;
        Text = item is Note note ? note.Content : null;
        BookmarkCollectionIds = item.Bookmarks.Select(b => b.BookmarkCollectionId);
        CustomFields = item.CustomFieldValues.Select(cf => new ItemCustomField
        {
            //Id = cf.Id,
            Name = cf.CustomField.Name,
            ValueInteger = cf.ValueInteger,
            Values = cf.Values.Select(v => v.Name),
        });
    }
}
