using DXC.Books.Api.Exceptions;
using DXC.Books.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Commands;

public record CheckoutBookCommand(string Isbn) : IRequest;

public class CheckoutBookCommandHandler(BooksDbContext dbContext) : IRequestHandler<CheckoutBookCommand>
{
    public async Task Handle(CheckoutBookCommand request, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.SingleOrDefaultAsync(x => x.Isbn == request.Isbn, cancellationToken);
        if (book == null)
            throw new NotFoundException("Book not found");
        book.CheckOut();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}