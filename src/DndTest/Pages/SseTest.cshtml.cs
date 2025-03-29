using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DndTest.Pages;

public class SseTestModel(ILogger<IndexModel> logger) : PageModel
{
    public void OnGet()
    {
    }
}
