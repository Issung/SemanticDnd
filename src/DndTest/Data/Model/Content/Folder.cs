namespace DndTest.Data.Model.Content;

/// <summary>
/// A folder, that contains other content.
/// </summary>
public class Folder : Item
{
    public ICollection<Item> Children { get; set; } = [];
}
