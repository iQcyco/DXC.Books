using DXC.Books.Domain;

namespace DXC.Books.Api.Models;

internal static class ModelExtensions
{
    internal static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Author = book.Author,
            Isbn = book.Isbn,
            Title = book.Title,
            Status = book.Status.ToDto() 
        };
    }

    internal static StatusDto ToDto(this Status status)
    {
        return status switch
        {
            Status.OnShelf => StatusDto.OnShelf,
            Status.CheckedOut => StatusDto.CheckedOut,
            Status.Returned => StatusDto.Returned,
            Status.Damaged => StatusDto.Damaged,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}