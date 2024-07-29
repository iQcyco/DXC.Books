using DXC.Books.Api.Exceptions;
using DXC.Books.Api.Models;
using DXC.Books.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Commands;

public record UpdateBookCommand(string Isbn, string Author, string Title) : IRequest<BookDto>;

public class UpdateBookCommandHandler(BooksDbContext dbContext) : IRequestHandler<UpdateBookCommand, BookDto>
{
    public async Task<BookDto> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == request.Isbn, cancellationToken);
        if (book == null)
            throw new NotFoundException("Book not found");
        book.Update(request.Title, request.Author, request.Isbn);
        await dbContext.SaveChangesAsync(cancellationToken);
        return book.ToDto();
    }
}