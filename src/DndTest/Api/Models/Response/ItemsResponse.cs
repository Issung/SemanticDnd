namespace DndTest.Api.Models.Response;

public record ItemsResponse(IAsyncEnumerable<Item> Items);
