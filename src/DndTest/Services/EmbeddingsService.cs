using DndTest.Config;
using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace DndTest.Services;

public class EmbeddingsService(
    IOllamaClient ollamaClient,
    DndDbContext dbContext,
    DndSettings settings
)
{
    public async Task<EmbeddingCache> GetEmbeddingForText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) {
            text = string.Empty;
        }

        var hash = text.XxHash128();
        var cache = await dbContext.EmbeddingsCache.SingleOrDefaultAsync(e => e.TextHash == hash && e.Model == settings.EmbeddingsModel);

        if (cache != null)
        {
            return cache;
        }

        var embeddingsResponse = await ollamaClient.Embeddings(new(settings.EmbeddingsModel, text));

        var embedding = new EmbeddingCache
        {
            Text = text,
            TextHash = hash,
            Model = settings.EmbeddingsModel,
            Vector = new Vector(embeddingsResponse.Embeddings[0]),
        };

        dbContext.EmbeddingsCache.Add(embedding);

        await dbContext.SaveChangesAsync();

        return embedding;
    }
}
