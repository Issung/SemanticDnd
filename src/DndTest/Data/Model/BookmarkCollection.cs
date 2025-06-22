namespace DndTest.Data.Model;

public class BookmarkCollection
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public required string Description { get; set; }

    public User User { get; set; } = null!;
    public int UserId { get; set; }

    public List<Bookmark> Bookmarks = [];
}
