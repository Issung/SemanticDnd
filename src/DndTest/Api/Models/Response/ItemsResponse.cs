namespace DndTest.Api.Models.Response;

public record ItemsResponse(IAsyncEnumerable<ItemSummary> Items);