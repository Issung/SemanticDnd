using DndTest.Data.Model.Content;

namespace DndTest.Api.Models.Response;

public enum ItemType
{
    File,
    Folder,
    Note,
    Shortcut,
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public string? Text { get; set; }

    public Uri? FileAccessUrl { get; set; }

    /// <summary>
    /// This item is bookmarked in these collections.
    /// </summary>
    public IEnumerable<BookmarkCollectionSummary> BookmarkCollections { get; set; }

    public IEnumerable<CustomField> CustomFields { get; set; }

    public Item(Data.Model.Content.Item item)
    {
        Id = item.Id;
        Name = item.Name;
        Description = item.Description;
        CreatedAt = item.CreatedAt;
        UpdatedAt = item.UpdatedAt;
        Text = item is Note note ? note.Content : null;
        BookmarkCollections = item.Bookmarks
            .Select(b => b.BookmarkCollection)
            .Select(bc => new BookmarkCollectionSummary
            {
                Id = bc.Id,
                Name = bc.Name,
            });
        CustomFields = item.CustomFieldValues.Select(cf => new CustomField
        {
            Id = cf.Id,
            Name = cf.CustomField.Name,
            ValueInteger = cf.ValueInteger,
            Values = cf.Values.Select(v => v.Name),
        });
    }
}
