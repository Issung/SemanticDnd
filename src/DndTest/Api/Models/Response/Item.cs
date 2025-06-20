﻿using DndTest.Data.Model.Content;

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

    public IEnumerable<CustomField> CustomFields { get; set; } = default!;

    public Item(Data.Model.Content.Item item)
    {
        Id = item.Id;
        Name = item.Name;
        Description = item.Description;
        CreatedAt = item.CreatedAt;
        UpdatedAt = item.UpdatedAt;
        Text = item is Note note ? note.Content : null;
        CustomFields = item.CustomFieldValues.Select(cf => new CustomField
        {
            Name = cf.CustomField.Name,
            ValueInteger = cf.ValueInteger,
            Values = cf.Values.Select(v => v.Name),
        });
    }
}
