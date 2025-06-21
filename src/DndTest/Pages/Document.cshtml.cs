using DndTest.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Pages;

public class DocumentModel(
    DndDbContext dbContext
) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? Id{ get; set; }

    [BindProperty]
    public string? Name { get; set; }

    [BindProperty]
    new public IFormFile? File { get; set; }

    public string? Text { get; set; }

    //public static readonly IReadOnlyList<SelectListItem> CategoryOptions = Enum.GetValues<Category>()
    //    .Select(c => new SelectListItem(c.ToString(), c.ToString()))
    //    .ToList();

    public async Task OnGet()
    {
        //if (Id.HasValue)
        //{
        //    var doc = await dbContext.Documents.Include(d => d.File).SingleAsync(d => d.Id == Id);
        //    Name = doc.Name;

        //    var cachedTikaResponse = await dbContext.TikaCache.SingleOrDefaultAsync(t => t.FileHash == doc.File.Hash);
        //    Text = cachedTikaResponse?.TikaResponse.Content;
        //}
    }

    public async Task OnPostAsync()
    {
        //ArgumentNullException.ThrowIfNull(Name, nameof(Name));
        //ArgumentNullException.ThrowIfNull(Category, nameof(Category));
        //ArgumentNullException.ThrowIfNull(File, nameof(File));

        //using var stream = new MemoryStream();
        //await File.CopyToAsync(stream);

        //stream.Position = 0;
        //var file = await fileService.Upload(stream, File.FileName, File.ContentType);

        //var document = new Note
        //{
        //    Name = Name,
        //    Category = Category.Value,
        //    File = file,
        //    CreatedAt = DateTime.UtcNow,
        //};

        //dbContext.Documents.Add(document);

        //await dbContext.SaveChangesAsync();

        //stream.Position = 0;
        //var tikaResponse = await tikaService.Process(stream);

        //var extractedTexts = await documentService.ChunkTikaResponse(file.Id, tikaResponse);

        //await documentService.CreateSearchChunks(document.Id, extractedTexts);

        //Response.Redirect($"/Document?Id={document.Id}");
    }

    public async Task<IActionResult> OnPostDelete(int id)
    {
        // Remove the item from the database or list
        await dbContext.Notes.Where(i => i.Id == id).ExecuteDeleteAsync();
        return RedirectToPage("/Documents");
    }

}