using DndTest.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DndTest.Pages;

public class DocumentsModel(
    ILogger<DocumentModel> logger,
    DndDbContext dbContext
) : PageModel
{
    //public IReadOnlyDictionary<Category, IReadOnlyList<Note>> DocumentsByCategory { get; set; } = default!;

    public async Task OnGet()
    {
        //DocumentsByCategory = await dbContext.Documents.GroupBy(d => d.Category).ToDictionaryAsync(g => g.Key, g => (IReadOnlyList<Note>)g.ToArray());
    }
}