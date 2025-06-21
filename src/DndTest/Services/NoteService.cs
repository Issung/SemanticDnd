using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Data.Model.Content;
using DndTest.Helpers.Extensions;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using System.Text;

namespace DndTest.Services;

public class NoteService(
    DndDbContext dbContext,
    EmbeddingsService embeddingsService,
    FileService fileService
)
{
    public async Task Create(Note note)
    {
        dbContext.Notes.Add(note);

        //var description = string.IsNullOrWhiteSpace(note.Description) ? string.Empty : $"\nDescription:\n{note.Description}";
        //var body = string.IsNullOrWhiteSpace(note.Description) ? string.Empty : $"\Body:\n{note.Description}";
        var fields = note.CustomFieldValues
            .Select(cf =>
            {
                // TODO: Handle text output for all value types.
                var values = cf.ValueInteger?.ToString() ?? string.Join(", ", cf.Values.Select(v => v.Name));
                return $"* {cf.CustomField.Name}: {values}";
            })
            .StringJoin("\n");

        var extractedText = new ExtractedText
        {
            PageNumber = null,
            
            // TODO: Handle Description being output in text.
            Text = $"""
            # {note.Name}
            {fields}

            {note.Content}
            """,
        };

        //dbContext.ExtractedText.Add(extractedText);
        dbContext.SearchChunks.AddRange(CreateSearchChunks(note, [extractedText]));
        
        //await dbContext.SaveChangesAsync();
    }

    /*public async Task UploadPlainText(string name, string text)
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

        await CreateSearchChunks(note, [extractedText]);
    }*/

    /*public async Task<IReadOnlyList<ExtractedText>> ChunkTikaResponse(int fileId, TikaResponse response)
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

        //dbContext.ExtractedText.AddRange(chunks);

        dbContext.SearchChunks.AddRange(CreateSearchChunks(fileId, chunks));

        await dbContext.SaveChangesAsync();

        return chunks;
    }*/

    public SearchChunk[] CreateSearchChunks(Item item, IReadOnlyList<ExtractedText> extractedTexts)
    {
        return extractedTexts.Select(extractedText =>
        {
            //var embeddings = await embeddingsService.GetEmbeddingForText(extractedText.Text);
            var embeddings = new Vector(Enumerable.Repeat(0f, 768).ToArray());

            return new SearchChunk()
            {
                Item = item,
                Text = extractedText.Text,
                EmbeddingVector = embeddings,
                //EmbeddingVector = embeddings.Vector,
                PageNumber = extractedText.PageNumber,
            };
        }).ToArray();
    }
}
