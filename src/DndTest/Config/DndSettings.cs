namespace DndTest.Config;

public class DndSettings
{
    public string EmbeddingsModel { get; set; } = "nomic-embed-text";

    public int EmbeddingsSize => embeddingsSizes[EmbeddingsModel];

    public string BucketName { get; set; } = "dndtest"; // Matches string in docker-compose.

    public string TikaBaseUrl = "http://localhost:9998";

    private static readonly IReadOnlyDictionary<string, int> embeddingsSizes = new Dictionary<string, int>()
    {
        ["nomic-embed-text"] = 768,
    };
}
