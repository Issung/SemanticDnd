using DndTest.Data.Model;
using DndTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DndTest.Pages;

public class SearchModel(
    DocumentService documentService
) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Query { get; set; }

    public IReadOnlyList<SearchChunk> Results { get; set; } = [];

    public async Task OnGet()
    {
        if (!string.IsNullOrWhiteSpace(Query))
        {
            Results = await documentService.HybridSearch(Query);
        }
    }
}