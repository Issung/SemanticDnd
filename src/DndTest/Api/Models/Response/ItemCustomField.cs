namespace DndTest.Api.Models.Response;

/// <summary>
/// A basic view of a custom field just for display with an item.
/// </summary>
public class ItemCustomField
{
    public required int Id { get; set; }  // Not needed just for display really.

    public required string Name { get; set; }

    // TODO: Implement other custom field types.

    public int? ValueInteger { get; set; }

    public IEnumerable<string>? Values { get; set; }
}
