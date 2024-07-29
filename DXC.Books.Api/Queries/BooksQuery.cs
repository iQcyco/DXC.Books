using DXC.Books.Api.Models;
using DXC.Books.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Queries;

public class BooksQuery : IRequest<PagedList<BookDto>>
{
    const int MaxPageSize = 50;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 10;

    public string? Sort { get; set; }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}

public class BooksQueryHandler(BooksDbContext dbContext) : IRequestHandler<BooksQuery, PagedList<BookDto>>
{
    public async Task<PagedList<BookDto>> Handle(BooksQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Books.Select(x => new BookDto
        {
            Author = x.Author,
            Title = x.Title,
            Isbn = x.Isbn,
            Status = x.Status.ToDto()
        });
        if (request.Sort != null)
        {
            query = query.ApplySorting(request.Sort);
        }

        return await query.AsNoTracking()
            .ToPagedListAsync(request.PageNumber, request.PageSize);
    }
}