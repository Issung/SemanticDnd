using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Tesseract;

namespace DndTest.Pages;

public class ReadModel(ILogger<ReadModel> logger) : PageModel
{
    [BindProperty]
    public IFormFile File { get; set; }

    public string OutputText { get; set; }

    public void OnGet()
    {
    }

    public async Task OnPostAsync()
    {
        using var memoryStream = new MemoryStream();
        await File.CopyToAsync(memoryStream);
        var fileBytes = memoryStream.ToArray();
        using var img = Pix.LoadFromMemory(fileBytes);

        using var engine = new TesseractEngine("./tessdata", "eng", EngineMode.Default);
        using var page = engine.Process(img);

        var text = page.GetText();
        logger.LogInformation("Mean confidence: {0}", page.GetMeanConfidence());

        logger.LogInformation("Text (GetText): \r\n{0}", text);
        logger.LogInformation("Text (iterator):");

        using var iter = page.GetIterator();
        iter.Begin();

        OutputText = text;

        /*var text = "";

        do
        {
            do
            {
                do
                {
                    do
                    {
                        if (iter.IsAtBeginningOf(PageIteratorLevel.Block))
                        {
                            text += "<BLOCK>";
                            logger.LogInformation("<BLOCK>");
                        }

                        var t = iter.GetText(PageIteratorLevel.Word);

                        logger.LogInformation(t);
                        logger.LogInformation(" ");

                        text += t + " ";

                        if (iter.IsAtFinalOf(PageIteratorLevel.TextLine, PageIteratorLevel.Word))
                        {
                            text += '\n';
                            logger.LogInformation("");
                        }
                    } while (iter.Next(PageIteratorLevel.TextLine, PageIteratorLevel.Word));

                    if (iter.IsAtFinalOf(PageIteratorLevel.Para, PageIteratorLevel.TextLine))
                    {
                        text += '\n';
                        logger.LogInformation("");
                    }
                } while (iter.Next(PageIteratorLevel.Para, PageIteratorLevel.TextLine));
            } while (iter.Next(PageIteratorLevel.Block, PageIteratorLevel.Para));
        } while (iter.Next(PageIteratorLevel.Block));*/
    }

    /*public async Task<IActionResult> OnPostAsync()
    {
        if (File != null && File.Length > 0)
        {
            // Convert image to Base64
            using (var memoryStream = new MemoryStream())
            {
                await File.CopyToAsync(memoryStream);
                byte[] fileBytes = memoryStream.ToArray();
                ImageBase64 = Convert.ToBase64String(fileBytes);
            }

            // Call the Llava API
            var client = httpClientFactory.CreateClient();
            var requestData = new
            {
                model = "llava",
                prompt = "Convert the content of the provided image into Markdown format, ensuring accurate formatting and no extra text.",
                images = new[] { ImageBase64 }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync("http://localhost:11434/api/generate", content);

            if (response.IsSuccessStatusCode)
            {
                // Handle SSE streaming response
                using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                {
                    var sb = new StringBuilder();
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        try
                        {
                            // Parse the JSON line from the SSE stream
                            using (JsonDocument doc = JsonDocument.Parse(line))
                            {
                                var message = doc.RootElement.GetProperty("response").GetString();
                                if (message != null)
                                {
                                    sb.Append(message);
                                    StreamResponse = sb.ToString();  // Update the UI incrementally
                                                                        // You could also consider saving or further processing the data
                                }
                            }
                        }
                        catch (JsonException ex)
                        {
                            // Handle parsing errors here
                            ApiResponse = $"Error parsing JSON: {ex.Message}";
                        }
                    }
                }
            }
            else
            {
                ApiResponse = "Error calling API: " + response.StatusCode;
            }
        }

        return Page();
    }*/
}