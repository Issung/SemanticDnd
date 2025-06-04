using DndTest.Data.Model.Content;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Data.Model.CustomFields;

/// <summary>
/// Represents a CustomField's value on an Item.<br/>
/// Can have multiple values via <see cref="Values"/>.
/// </summary>
[Index(nameof(ItemId), nameof(CustomFieldId), IsUnique = true)] // Unique constraint - An item can only have 1 definition for a given custom field.
public class ItemCustomFieldValue
{
    public int Id { get; set; }

    public int ItemId { get; set; }
    public Item Item { get; set; } = default!;

    public int CustomFieldId { get; set; }
    public CustomField CustomField { get; set; } = default!;

    // Has either a freely entered value or a reference to a CustomFieldOption

    public bool? ValueBoolean { get; set; }
    public DateTime? ValueDateTime { get; set; }
    public double? ValueDouble { get; set; }
    public string? ValueFreeText { get; set; }
    public int? ValueInteger { get; set; }

    /// <summary>Used for both single and multi-choice custom fields.</summary>
    public List<CustomFieldOption> Values { get; set; } = default!;
}
