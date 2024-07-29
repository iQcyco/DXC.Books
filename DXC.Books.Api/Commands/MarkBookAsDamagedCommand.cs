using DXC.Books.Api.Exceptions;
using DXC.Books.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Commands;

public record MarkBookAsDamagedCommand(string Isbn) : IRequest;

public class MarkBookAsDamagedCommandHandler(BooksDbContext dbContext) : IRequestHandler<MarkBookAsDamagedCommand>
{
    public async Task Handle(MarkBookAsDamagedCommand request, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == request.Isbn, cancellationToken);
        if (book == null)
            throw new NotFoundException("Book not found");
        book.MarkAsDamaged();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}