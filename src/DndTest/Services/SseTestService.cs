using System.Text.Json;

namespace DndTest.Services;

public class SseTestService(IHttpContextAccessor httpContextAccessor)
{
    public async Task Test()
    {
        var response = httpContextAccessor.HttpContext!.Response;

        response.ContentType = "text/event-stream";
        response.Headers.CacheControl = "no-cache";
        response.Headers.Connection = "keep-alive";

        const string mymessage = "My name is issung and i am vibe coding.";

        foreach (var item in mymessage.Split(" "))
        {
            // Send a message every second
            await Task.Delay(250);

            var message = new { message = item + ' ' };
            var jsonMessage = JsonSerializer.Serialize(message);

            await response.WriteAsync($"data: {jsonMessage}\n\n");
            await response.Body.FlushAsync();
        }

        // Javascript checks for this specific string to know when to stop the connection.
        await response.WriteAsync("data: { \"message\" : \"---DONE---\" }\n\n");
        await response.Body.FlushAsync();
    }
}
