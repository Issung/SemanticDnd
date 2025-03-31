using DndTest.Config;
using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Helpers.Extensions;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DndTest.Services;

public class TikaService(
    IHttpClientFactory httpClientFactory,
    DndDbContext dbContext,
    DndSettings settings
)
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new TikaIntConverter(),
            new TikaLongConverter(),
            new TikaDateTimeConverter(),
        },
    };

    static TikaService()
    {
        JsonOptions.MakeReadOnly(populateMissingResolver: true);
    }

    public async Task<TikaResponse> Process(Stream fileStream)
    {
        var fileHash = fileStream.XXHash128();

        var cache = dbContext.TikaCache.SingleOrDefault(m => m.FileHash == fileHash);

        if (cache != null)
        {
            return cache.TikaResponse;
        }
        
        var (json, response) = await SendToTika(fileStream);

        dbContext.TikaCache.Add(new TikaCache
        {
            FileHash = fileHash,
            TikaResponseJson = json,
        });
        await dbContext.SaveChangesAsync();

        return response;
    }

    private async Task<(string Json, TikaResponse response)> SendToTika(Stream fileStream)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, new Uri(new(settings.TikaBaseUrl), "/tika").AbsoluteUri);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        fileStream.Position = 0;
        var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        request.Content = content;

        var httpResponse = await httpClientFactory.CreateClient().SendAsync(request);
        httpResponse.EnsureSuccessStatusCode();

        var json = await httpResponse.Content.ReadAsStringAsync();
        var response = JsonSerializer.Deserialize<TikaResponse>(json, JsonOptions) ?? throw new Exception("Deserialized TikaResponse unexpectedly null.");

        return (json, response);
    }
}

