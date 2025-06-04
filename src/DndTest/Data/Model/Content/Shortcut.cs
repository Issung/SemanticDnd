namespace DndTest.Data.Model.Content;

/// <summary>
/// Content that is a shortcut to another piece of content (typically so they can specify a specific page).
/// </summary>
public class Shortcut : Item
{
    public int TargetId { get; set; }
    public Item Target { get; set; } = default!;

    public int? PageNumber { get; set; }
}
