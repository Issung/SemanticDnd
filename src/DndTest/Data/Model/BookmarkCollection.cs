namespace DndTest.Data.Model;

public class BookmarkCollection
{
    public int Id { get; set; }
    public string Name { get; set; }

    public string Description { get; set; }

    public User User { get; set; } = null!;
    public int UserId { get; set; }

    public List<Bookmark> Bookmarks = [];
}
