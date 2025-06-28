namespace DndTest.Api.Models.Response;

public record ItemsResponse(int Count, IAsyncEnumerable<ItemSummary> Items);