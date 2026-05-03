using System.Text.Json.Serialization;

namespace QuizNova.Application.Common.Models;

public class PaginatedList<T>
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public int TotalCount { get; init; }
    public IReadOnlyCollection<T> Items { get; init; }

    [JsonConstructor]
    public PaginatedList(IReadOnlyCollection<T> items, int totalCount, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        Items = items;

        if (pageSize <= 0)
        {
            TotalPages = 1;
        }
        else
        {
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        }
    }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
