namespace DndTest.Data.Model.CustomFields;

public class CustomFieldOption
{
    public int Id { get; set; }

    public int CustomFieldId { get; set; }
    public CustomField CustomField { get; set; } = default!;

    public string Name { get; set; } = default!;

    /// <summary>
    /// The values (items) that this option is used on.
    /// </summary>
    public List<ItemCustomFieldValue> SelectedOn { get; set; } = default!;
}
