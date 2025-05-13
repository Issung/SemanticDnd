namespace DndTest.Api.Models.Response;

public record SearchResponse(
    int TotalCount,
    IEnumerable<SearchHit> Hits
);
