using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Helpers.Extensions;

namespace DndTest.Services;

public class EmbeddingsService(
    IOllamaClient ollamaClient,
    DndDbContext dbContext
)
{
    public async Task<CachedEmbedding> GetEmbeddingForText(string text)
    {
        var hash = text.XxHash128();
        var existingEmbedding = dbContext.EmbeddingsCache.SingleOrDefault(e => e.TextHash == hash);

        if (existingEmbedding != null)
        {
            return existingEmbedding;
        }

        var embeddingsResponse = await ollamaClient.Embeddings(new("nomic-embed-text", text));

        var embedding = new CachedEmbedding
        {
            Text = text,
            TextHash = hash,
            Floats = embeddingsResponse.Embeddings[0],
        };

        dbContext.EmbeddingsCache.Add(embedding);

        await dbContext.SaveChangesAsync();

        return embedding;
    }
}
