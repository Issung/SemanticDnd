using DndTest.Api.Models.Request;
using DndTest.Api.Models.Response;
using DndTest.Services;

namespace DndTest.Api;

public class SearchApi(
    DocumentService documentService
)
{
    public async Task<SearchResponse> Search(SearchRequest request)
    {
        var results = await documentService.HybridSearch(request.Query, request.Category);
        return new(results);
    }
}
