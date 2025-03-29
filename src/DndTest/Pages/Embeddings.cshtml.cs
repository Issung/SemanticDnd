using DndTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DndTest.Pages;

public class EmbeddingsModel(
    EmbeddingsService embeddingsService    
) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Text { get; set; }

    public IReadOnlyList<float>? Floats { get; set; }

    public async Task OnGet()
    {
        if (Text != null)
        {
            var embedding = await embeddingsService.GetEmbeddingForText(Text);
        
            Floats = embedding.Floats;
        }
    }
}
