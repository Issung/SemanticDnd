using DndTest.Data;
using DndTest.Data.Migrations;
using DndTest.Data.Model;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;
using System.Text.Json;
using File = DndTest.Data.Model.File;

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

    public async Task<IReadOnlyList<SearchChunk>> HybridSearchAsync(
        string searchQuery,
        double keywordWeight = 0.4,
        double vectorWeight = 0.6,
        int limit = 20
    )
    {
        var embeddingsVector = await embeddingsService.GetEmbeddingForText(searchQuery);
        //var floats = JsonSerializer.Serialize(embeddingsVector.Floats);
        var embedding = embeddingsVector.Vector;

        // Normalize weights
        var totalWeight = keywordWeight + vectorWeight;
        keywordWeight /= totalWeight;
        vectorWeight /= totalWeight;

        /*
         SELECT *,
           ((0.4 * ts_rank("SearchChunks"."TextVector", plainto_tsquery('english', 'glantri'))) +
           (0.6 * (1 - ("SearchChunks"."EmbeddingVector" <-> '[0]')))) AS CombinedScore
        FROM "SearchChunks"
        WHERE "SearchChunks"."TextVector" @@ plainto_tsquery('english', 'glantri')
        ORDER BY CombinedScore DESC
        LIMIT 
         */

        //return await dbContext.SearchChunks
        //    .FromSqlInterpolated($@"
        //        SELECT *, 
        //            (({keywordWeight} * ts_rank(""SearchChunks"".""TextVector"", plainto_tsquery('english', {searchQuery}))) + 
        //            ({vectorWeight} * (1 - (""SearchChunks"".""EmbeddingVector"" <-> {floats})))) AS ""CombinedScore""
        //        FROM ""SearchChunks""
        //        WHERE ""SearchChunks"".""TextVector"" @@ plainto_tsquery('english', {searchQuery})
        //        ORDER BY CombinedScore DESC
        //        LIMIT {limit}"
        //    )
        //    .ToArrayAsync();

        var items = await dbContext.SearchChunks
            .OrderBy(x => x.EmbeddingVector.L2Distance(embedding))
            .Take(10)
            .ToArrayAsync();

        return items;
    }
}
