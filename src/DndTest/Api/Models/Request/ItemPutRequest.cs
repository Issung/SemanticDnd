namespace DndTest.Api.Models.Request;

public class ItemPutRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }

    public int? ParentId { get; set; }

    // CreatedAt & UpdatedAt handled by server.

    // public ?? CustomFields { get; set; }
}

public class FilePutRequest : ItemPutRequest
{
    // No file specific fields as of yet.. The file binary itself will come as a multi-part form request.
}

public class FolderPutRequest : ItemPutRequest
{
    // Folders don't have any additional fields as of yet.
}

public class NotePutRequest : ItemPutRequest
{
    public required string Content { get; set; }
}

public class ShortcutPutRequest : ItemPutRequest
{
    public int TargetId { get; set; }
    public int? PageNumber { get; set; }
}