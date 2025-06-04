namespace DndTest.Data.Model.CustomFields;

public class CustomField
{
    public int Id { get; set; }

    public string Name { get; set; } = default!;
    
    public CustomFieldType Type { get; set; }

    /// <summary>
    /// Only used for the SingleChoice, MultiChoice (and possibly MultiValue) types.
    /// </summary>
    public List<CustomFieldOption> Options { get; set; } = default!;

    /// <summary>
    /// Conditions for this CustomField to appear.
    /// </summary>
    public List<CustomFieldCondition> Conditions { get; set; } = default!;

    /// <summary>
    /// Conditions that depend on (check) this CustomField.
    /// </summary>
    public List<CustomFieldCondition> DependentConditions { get; set; } = default!;
}