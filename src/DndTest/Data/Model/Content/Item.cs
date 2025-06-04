using DndTest.Data.Model.CustomFields;

namespace DndTest.Data.Model.Content;

/// <summary>
/// Base class for content.
/// </summary>
public abstract class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public Folder? Parent { get; set; }
    public int? ParentId { get; set; }

    public Tenant Tenant { get; set; } = default!;
    public int TenantId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ItemCustomFieldValue> CustomFieldValues { get; set; } = default!;
}
