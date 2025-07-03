using DndTest.Data.Model.CustomFields;

namespace DndTest.Api.Models.Response;

public class CustomFieldsResponse
{
    public IEnumerable<CustomField> CustomFields { get; set; } = default!;

    public class CustomField
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public CustomFieldType Type { get; set; }   // TODO: Separate this enum from database enum - maybe?

        public int? ValueInteger { get; set; }

        /// <summary>
        /// Only set if this custom field is a type that takes values.<br/>
        /// Options that can be used for this custom field.
        /// </summary>
        public IEnumerable<CustomFieldOption>? Options { get; set; }

        /// <summary>
        /// If set, a condition that must be satisfied for this custom field to appear.
        /// </summary>
        public IEnumerable<CustomFieldCondition>? Conditions { get; set; }
    }

    public class CustomFieldOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class CustomFieldCondition
    {
        public int DependsOnCustomFieldId { get; set; }

        public IEnumerable<int> RequiredOptionIds { get; set; } = default!;
    }
}
