namespace DndTest.Data.Model.Content;

/// <summary>
/// Content that is a document that is editable within the application.
/// </summary>
public class Note : Item
{
    /// <summary>
    /// Markdown or rich text??
    /// </summary>
    public string Content { get; set; } = default!;
}
