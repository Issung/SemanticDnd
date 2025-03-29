using DndTest.Data;
using DndTest.Helpers.Extensions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DndTest.Services;

public class TikaService(
    IHttpClientFactory httpClientFactory,
    DndDbContext dbContext
)
{
    private static readonly string tikaServerBaseUrl = "http://localhost:9998";
    private static readonly JsonSerializerOptions jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new TikaIntConverter(),
            new TikaLongConverter(),
            new TikaDateTimeConverter(),
        }
    };

    public async Task<TikaResponse> GetMetadata(Stream fileStream)
    {
        var fileHash = fileStream.XXHash128();

        var existingMetadata = dbContext.TikaCache.SingleOrDefault(m => m.FileHash == fileHash);

        using var request = new HttpRequestMessage(HttpMethod.Put, $"{tikaServerBaseUrl}/tika");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        request.Content = content;

        var response = await httpClientFactory.CreateClient().SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<TikaResponse>(responseStream, jsonOptions) ?? throw new Exception("Deserialized TikaResponse unexpectedly null.");
    }
}

