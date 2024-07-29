using DXC.Books.Api.Models;
using DXC.Books.Data;
using DXC.Books.Domain;
using MediatR;

namespace DXC.Books.Api.Commands;

public record CreateBookCommand(string Isbn, string Author, string Title) : IRequest<BookDto>;

public class CreateBookCommandHandler(BooksDbContext dbContext) : IRequestHandler<CreateBookCommand, BookDto>
{
    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var entry = await dbContext.Books.AddAsync(
            new Book(request.Title, request.Author, request.Isbn, Status.OnShelf), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return entry.Entity.ToDto();
    }
}