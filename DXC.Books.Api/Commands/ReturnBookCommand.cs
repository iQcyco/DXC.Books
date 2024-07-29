using DXC.Books.Api.Exceptions;
using DXC.Books.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Commands;

public record ReturnBookCommand(string Isbn) : IRequest;

public class ReturnBookCommandHandler(BooksDbContext dbContext) : IRequestHandler<ReturnBookCommand>
{
    public async Task Handle(ReturnBookCommand request, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == request.Isbn, cancellationToken);
        if (book == null)
            throw new NotFoundException("Book not found");
        book.Return();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}