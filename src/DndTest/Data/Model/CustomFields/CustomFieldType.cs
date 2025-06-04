namespace DndTest.Data.Model.CustomFields;

public enum CustomFieldType
{
    FreeText,

    SingleChoice,
    MultiChoice,

    /// <summary>
    /// Can pick from other existing entries or just enter your own value.
    /// </summary>
    SingleValue,

    /// <summary>
    /// Multiple free text entries (e.g. Tags).
    /// I think this should work like Multi-Choice but the UI will be different, and any user can add options not just the admin.
    /// </summary>
    MultiValue,

    Integer,
    Decimal,
    Date,
    Boolean,
}