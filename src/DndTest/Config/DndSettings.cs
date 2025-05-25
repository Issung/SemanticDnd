namespace DndTest.Config;

public class DndSettings
{
    public string EmbeddingsModel { get; set; } = "nomic-embed-text";

    public int EmbeddingsSize => embeddingsSizes[EmbeddingsModel];

    public string BucketName { get; set; } = "dndtest"; // Matches string in docker-compose.

    public string TikaBaseUrl = "http://localhost:9998";

    public Uri OllamaBaseUrl = new("http://localhost:11434");

    public FrontendSettings Frontend { get; set; } = new();

    private static readonly IReadOnlyDictionary<string, int> embeddingsSizes = new Dictionary<string, int>()
    {
        ["nomic-embed-text"] = 768,
    };
}

public class FrontendSettings
{
    /// <summary>
    /// Full disk path to a build of the frontend app.
    /// </summary>
    public string? RootPath { get; set; }

    /// <summary>
    /// Should the frontend ui be served via a proxy (eg in development when using npm run dev)
    /// </summary>
    public string? SpaProxyAddress { get; set; }
}
