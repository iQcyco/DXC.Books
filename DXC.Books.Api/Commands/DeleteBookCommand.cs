using DXC.Books.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Commands;

public record DeleteBookCommand(string Isbn) : IRequest;

public class DeleteBookCommandHandler(BooksDbContext dbContext) : IRequestHandler<DeleteBookCommand>
{
    public async Task Handle(DeleteBookCommand request, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == request.Isbn, cancellationToken);
        if (book != null)
            dbContext.Books.Remove(book);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}