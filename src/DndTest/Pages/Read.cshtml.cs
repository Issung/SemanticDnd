using DndTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DndTest.Pages;

public class ReadModel(
    TikaService tikaService
) : PageModel
{
    [BindProperty]
    public IFormFile File { get; set; }

    public string OutputText { get; set; }

    public void OnGet()
    {
    }

    public async Task OnPostAsync()
    {
        var tikaResponse = await tikaService.Process(File.OpenReadStream());

        OutputText = tikaResponse.Content;
    }
}