using DXC.Books.Api.Exceptions;
using DXC.Books.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Commands;

public record PutBookOnShelfCommand(string Isbn) : IRequest;

public class PutBookOnShelfCommandHandler(BooksDbContext dbContext) : IRequestHandler<PutBookOnShelfCommand>
{
    public async Task Handle(PutBookOnShelfCommand request, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == request.Isbn, cancellationToken);
        if (book == null)
            throw new NotFoundException("Book not found");
        book.PutOnShelf();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}