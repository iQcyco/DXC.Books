using System.Text.Json.Serialization;

namespace DXC.Books.Api.Models;

public class PagedList<T>
{
    public IReadOnlyCollection<T> Data { get; init; }
    public int CurrentPage { get; init; }

    public int PageSize { get; init; }
    public int TotalCount { get; init; }

    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public bool HasPrevious => CurrentPage > 1;

    public bool HasNext => CurrentPage < TotalPages;
    
}