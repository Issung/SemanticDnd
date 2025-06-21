namespace DndTest.Api.Models.Response;

public class CustomField
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    // TODO: Implement other custom field types.

    public int? ValueInteger { get; set; }

    public IEnumerable<string>? Values { get; set; }
}
