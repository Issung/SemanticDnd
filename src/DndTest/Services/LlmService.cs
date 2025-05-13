using DndTest.Config;
using DndTest.Helpers.Extensions;
using System.Text.Json;

namespace DndTest.Services;

public class LlmService(
    IHttpClientFactory httpClientFactory,
    DndSettings settings,
    DocumentService documentService
)
{
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public async Task<IAsyncEnumerable<ResponsePart>> Question(string question)
    {
        var chunks = await documentService.HybridSearch(question);

        var documentsString = chunks
            .Select((chunk, index) => $"""
            ## {chunk.Document.Name} {(chunk.PageNumber == null ? string.Empty : $"(Page {chunk.PageNumber + 1})")}
            
            {chunk.Text}
            """)
            .StringJoin("\n\n");

        var prompt = $"""
        You are a large language model assitant that helps a player with their 2nd Edition Dungeons & Dragons related questions.
        Below is the user's question and a series of related documents, ordered by rough relevance.
        Answer the question as directly & simply as possible, with only information provided in the documents.
        You must not use general knowledge about the world or Dungeons & Dragons or the player may fail in-game as a result.
        If you are not confident you can answer with the information provided, simply tell the player to refer to the relevant documents.

        # Player's Question
        {question}

        # Related Documents

        {documentsString}
        """;

        // TODO: Return the relevant documents as a response item to be displayed on the UI.

        return StreamLlmResponse(prompt);
    }

    private async IAsyncEnumerable<ResponsePart> StreamLlmResponse(string prompt) // TODO: Cancellation
    {
        var httpClient = httpClientFactory.CreateClient();
        var request = new
        {
            model = "llama2",
            prompt
        };
        var content = new StringContent(JsonSerializer.Serialize(request), System.Text.Encoding.UTF8, "application/json");
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(settings.OllamaBaseUrl, "/api/generate"))
        {
            Content = content
        };

        using var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        using var stream = await response.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            var line = (await reader.ReadLineAsync()) ?? throw new Exception("Line unexpectedly null.");

            yield return JsonSerializer.Deserialize<ResponsePart>(line, jsonOptions) ?? throw new Exception("Deserialized line in response unexpectedly null.");
        }
    }

    /// <summary>
    /// {"model":"llama2","created_at":"2025-04-05T07:05:35.12293Z","response":"\n","done":false}
    /// 
    /// Commented out shit we don't care about (yet).
    /// </summary>
    public class ResponsePart
    {
        //public string Model { get; set; }
        //public DateTime Created_At { get; set; }
        //public string DoneReason
        //public int[] Context
        public string Response { get; set; }
        public bool Done { get; set; }
    }

}
