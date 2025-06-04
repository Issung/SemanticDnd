namespace DndTest.Data.Model.CustomFields;

public class CustomFieldCondition
{
    public int Id { get; set; }
    
    /// <summary>
    /// The CustomField this condition applies to.<br/>
    /// If this condition is me then this CustomField will be activated.
    /// </summary>
    public CustomField CustomField { get; set; } = default!;
    public int CustomFieldId { get; set; }

    /// <summary>
    /// The CustomField this condition checks/depends upon.<br/>
    /// All conditions that depend on a CustomField must be removed before a CustomField can be removed.
    /// </summary>
    public CustomField DependsOnCustomField { get; set; } = default!;
    public int DependsOnCustomFieldId { get; set; }

    /// <summary>
    /// These options are required on <see cref="DependsOnCustomField"/> for this <see cref="CustomField"/> to be made active.
    /// </summary>
    public List<CustomFieldOption> RequiredOptions { get; set; } = default!;
}
