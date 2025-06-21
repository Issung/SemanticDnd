using DndTest.Data;
using DndTest.Data.Model;
using DndTest.Helpers.Extensions;
using Microsoft.EntityFrameworkCore;

namespace DndTest.Services;

public class SearchService(
    DndDbContext dbContext
)
{
    private static readonly string[] stopwords = [
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
    ];

    private static readonly int[] includeCustomFieldIds = [
        //1, // Type
        2, // Class
        8, // Level
    ];

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
            .Include(sc => sc.Item)
            .ThenInclude(i => i.CustomFieldValues.Where(v => includeCustomFieldIds.Contains(v.CustomFieldId)))
            .ThenInclude(v => v.Values)
            .Where(sc => !hasTextQuery || sc.TextVector.Matches(EF.Functions.ToTsQuery(textQuery)));

        if (hasTextQuery)
        {
            searchQuery = searchQuery
                .OrderByDescending(sc => EF.Functions.ILike(sc.Item.Name, $"%{query}%"))
                .ThenByDescending(sc => sc.TextVector.Rank(EF.Functions.ToTsQuery(textQuery)));
        }
        else
        {
            searchQuery = searchQuery.OrderBy(sc => sc.Item.Name);
        }

        var results = await searchQuery
            .Take(limit)
            .ToArrayAsync();

        return results;
    }
}
