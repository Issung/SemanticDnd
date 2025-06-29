namespace DndTest.Api.Models.Response;

public record BrowseResponse(
    int? ParentId,
    int? FolderId,
    string FolderName,
    string FolderDescription,
    int ItemCount,
    IAsyncEnumerable<ItemSummary> Items
);