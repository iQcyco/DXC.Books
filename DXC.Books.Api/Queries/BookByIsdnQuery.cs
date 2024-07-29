using DXC.Books.Api.Models;
using DXC.Books.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Queries;

public record BookByIsbnQuery(string Isbn) : IRequest<BookDto?>;

public class BookByIsbnQueryHandler(BooksDbContext dbContext) : IRequestHandler<BookByIsbnQuery, BookDto?>
{
    public async Task<BookDto?> Handle(BookByIsbnQuery request, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books
            .SingleOrDefaultAsync(x => x.Isbn == request.Isbn, cancellationToken);
        return book?.ToDto();
    }
}