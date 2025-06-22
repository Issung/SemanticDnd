using DndTest.Data.Model.Content;

namespace DndTest.Data.Model;

public class Bookmark
{
    public Item Item { get; set; } = null!;
    public int ItemId { get; set; }

    public BookmarkCollection BookmarkCollection { get; set; } = null!;
    public int BookmarkCollectionId { get; set; }
}
