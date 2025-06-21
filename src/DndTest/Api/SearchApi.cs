using DndTest.Api.Models.Request;
using DndTest.Api.Models.Response;
using DndTest.Services;

namespace DndTest.Api;

public class SearchApi(
    SearchService searchService
)
{
    public async Task<SearchResponse> TradSearch(SearchRequest request)
    {
        var results = await searchService.TradSearch(request.Query);
        var hits = results.Select(r => new SearchHit(r));

        return new(999, hits);
    }

    //public async Task<SearchResponse> HybridSearch(SearchRequest request)
    //{
    //    var results = await documentService.HybridSearch(request.Query);
    //    var hits = results.Select(r => new SearchHit(r));
    //
    //    return new(999, hits);
    //}
}
