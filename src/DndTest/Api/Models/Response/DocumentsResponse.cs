namespace DndTest.Api.Models.Response;

public record DocumentsResponse(IAsyncEnumerable<Item> Documents);
