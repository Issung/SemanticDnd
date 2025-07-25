using DndTest.Data.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DndTest.Pages;

public class SearchModel(
) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }

    public IReadOnlyCollection<SearchChunk> Results { get; set; } = [];

    public async Task OnGet()
    {
        //if (!string.IsNullOrWhiteSpace(Query))
        //{
        //    Results = await documentService.HybridSearch(Query, null);
        //}
    }
}