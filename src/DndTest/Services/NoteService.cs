using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Data.Model.Content;
using DndTest.Helpers.Extensions;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using Pgvector;
using System.Text;

namespace DndTest.Services;

public class NoteService(
    DndDbContext dbContext,
    EmbeddingsService embeddingsService,
    FileService fileService
)
{
    public async Task UploadPlainText(string name, string text)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));

        var file = await fileService.Upload(stream, name, "text/plain");

        var note = new Note
        {
            Name = name,
            Content = text,
        };

        dbContext.Items.Add(note);

        await dbContext.SaveChangesAsync();

        var extractedText = new ExtractedText
        {
            
            PageNumber = null,
            Text = text,
        };

        dbContext.ExtractedText.Add(extractedText);
        await dbContext.SaveChangesAsync();

        await CreateSearchChunks(note.Id, [extractedText]);
    }

    public async Task<IReadOnlyList<ExtractedText>> ChunkTikaResponse(int fileId, TikaResponse response)
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
            //var embeddings = await embeddingsService.GetEmbeddingForText(extractedText.Text);
            var embeddings = new Vector(Enumerable.Repeat(0f, 768).ToArray());

            var searchChunk = new SearchChunk()
            {
                DocumentId = documentId,
                Text = extractedText.Text,
                EmbeddingVector = embeddings,
                //EmbeddingVector = embeddings.Vector,
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

    public async Task<IReadOnlyCollection<SearchChunk>> TradSearch(
        string? query,
        int limit = 20
    )
    {
        var textQuery = (query ?? string.Empty)
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(word => !stopwords.Contains(word))
            .StringJoin(" | ");

        var hasTextQuery = !string.IsNullOrWhiteSpace(textQuery);

        var searchQuery = dbContext.SearchChunks
            .Include(sc => sc.Document)
            .Where(sc => !hasTextQuery || sc.TextVector.Matches(EF.Functions.ToTsQuery(textQuery)));

        if (hasTextQuery)
        {
            searchQuery = searchQuery
                .OrderByDescending(sc => EF.Functions.ILike(sc.Document.Name, $"%{query}%"))
                .ThenByDescending(sc => sc.TextVector.Rank(EF.Functions.ToTsQuery(textQuery)));
        }
        else
        {
            searchQuery = searchQuery.OrderBy(sc => sc.Document.Name);
        }

        var results = await searchQuery
            .Take(limit)
            .ToArrayAsync();

        return results;
    }
}
