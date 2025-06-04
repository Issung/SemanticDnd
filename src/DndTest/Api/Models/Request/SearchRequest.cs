using DndTest.Data.Model;

namespace DndTest.Api.Models.Request;

public class SearchRequest
{
    public string? Query { get; set; }
    //public SearchSort SortBy { get; set; } = SearchSort.Relevancy;
    //public SearchOrder? SortOrder { get; set; }
}

//public enum SearchSort
//{
//    Relevancy,
//    Name,
//    CreationDate,
//}

//public enum SearchOrder
//{
//    Ascending,
//    Descending,
//}