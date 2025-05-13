using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Helpers.Extensions;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace DndTest.Services;

public class DocumentService(
    DndDbContext dbContext,
    EmbeddingsService embeddingsService
)
{
    public async Task<IReadOnlyList<ExtractedText>> ChunkTikaResponse(Guid fileId, TikaResponse response)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(response.Content);

        var pages = doc.DocumentNode.SelectNodes("//div[contains(@class, 'page')]");

        var hasPages = pages?.Count > 0;

        var text = hasPages
            ? pages!.Select(p => p.InnerText)
            : [doc.DocumentNode.SelectNodes("//body").Single().InnerText];

        var chunks = text.Select((text, index) => new ExtractedText()
        {
            FileId = fileId,
            PageNumber = hasPages ? index : null,
            Text = text,
        }).ToArray();

        dbContext.ExtractedText.AddRange(chunks);

        await dbContext.SaveChangesAsync();

        return chunks;
    }

    public async Task CreateSearchChunks(int documentId, IReadOnlyList<ExtractedText> extractedTexts)
    {
        foreach (var extractedText in extractedTexts)
        {
            var embeddings = await embeddingsService.GetEmbeddingForText(extractedText.Text);

            var searchChunk = new SearchChunk()
            {
                DocumentId = documentId,
                Text = extractedText.Text,
                EmbeddingVector = embeddings.Vector,
                PageNumber = extractedText.PageNumber,
            };

            dbContext.SearchChunks.Add(searchChunk);
        }

        await dbContext.SaveChangesAsync();
    }

    private static readonly string[] stopwords = {
        "I", "me", "my", "myself", "we", "our", "ours", "ourselves", "you", "your", "yours", "yourself", "yourselves",
        "he", "him", "his", "himself", "she", "her", "hers", "herself", "it", "its", "itself", "they", "them", "their",
        "theirs", "themselves", "what", "which", "who", "whom", "this", "that", "these", "those", "am", "is", "are",
        "was", "were", "be", "been", "being", "have", "has", "had", "having", "do", "does", "did", "doing", "a", "an",
        "the", "and", "but", "if", "or", "because", "as", "until", "while", "of", "at", "by", "for", "with", "about",
        "against", "between", "into", "through", "during", "before", "after", "above", "below", "to", "from", "up",
        "down", "in", "out", "on", "off", "over", "under", "again", "further", "then", "once", "here", "there", "when",
        "where", "why", "how", "all", "any", "both", "each", "few", "more", "most", "other", "some", "such", "no",
        "nor", "not", "only", "own", "same", "so", "than", "too", "very", "s", "t", "can", "will", "just", "don",
        "should", "now", "d", "ll", "m", "o", "re", "ve", "y", "ain", "aren", "couldn", "didn", "doesn", "hadn",
        "hasn", "haven", "isn", "ma", "mightn", "mustn", "needn", "shan", "shouldn", "wasn", "weren", "won", "wouldn"
    };

    public async Task<IReadOnlyList<SearchChunk>> HybridSearch(
        string? searchQuery,
        Category? category,
        double keywordWeight = 0.6,
        double vectorWeight = 0.4,
        int limit = 20
    )
    {
        var textQuery = (searchQuery ?? string.Empty)
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(word => !stopwords.Contains(word))
            .StringJoin(" | "); // We want to OR all the words.

        var embeddingsVector = await embeddingsService.GetEmbeddingForText(searchQuery);
        var embedding = embeddingsVector.Vector;

        // Normalize weights
        var totalWeight = keywordWeight + vectorWeight;
        keywordWeight /= totalWeight;
        vectorWeight /= totalWeight;

        var categoryFilter = category.HasValue
            ? $"AND d.\"Category\" = @category"
            : string.Empty;

        var sql = $@"
SELECT *
FROM (
    SELECT *,
        (({keywordWeight} * ts_rank(sc.""TextVector"", to_tsquery(@textQuery))) +
        ({vectorWeight} * (1 - (sc.""EmbeddingVector"" <-> @embedding)))) AS ""CombinedScore""
    FROM ""SearchChunks"" sc
    JOIN ""Documents"" d ON sc.""DocumentId"" = d.""Id""
    WHERE sc.""TextVector"" @@ to_tsquery(@textQuery)
    {categoryFilter}
) AS subquery
WHERE ""CombinedScore"" > 0
ORDER BY ""CombinedScore"" DESC
LIMIT @limit
";

        var items = await dbContext.SearchChunks
            .FromSqlRaw(sql, new NpgsqlParameter?[] {
                new("textQuery", textQuery),
                new("embedding", embedding),
                new("limit", limit),
                category == null ? null : new("category", category),
            }.Where(p => p!= null))
            .Include(sc => sc.Document)
            .ToArrayAsync();

        return items;
    }
}
