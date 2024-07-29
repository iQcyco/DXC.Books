using System.Net.Mime;
using DXC.Books.Api.Commands;
using DXC.Books.Api.Models;
using DXC.Books.Api.Queries;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DXC.Books.Api.Controllers;

[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[Route("[controller]")]
public class BooksController(ISender sender) : ControllerBase
{
    [HttpGet("", Name = nameof(GetBooks))]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PagedList<BookDto>))]
    public Task<PagedList<BookDto>> GetBooks([FromQuery] BooksQuery parameters)
    {
        return sender.Send(parameters, HttpContext.RequestAborted);
    }

    [HttpPost("")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(BookDto))]
    public async Task<IActionResult> AddBook([FromBody] CreateBookCommand command)
    {
        await sender.Send(command);
        var book = await sender.Send(new BookByIsbnQuery(command.Isbn));
        return CreatedAtAction(nameof(GetByIsbn), new { book.Isbn }, book);
    }

    [HttpGet("{isbn}")]
    public async Task<BookDto?> GetByIsbn([FromRoute] string isbn)
    {
        return await sender.Send(new BookByIsbnQuery(isbn), HttpContext.RequestAborted);
    }

    [HttpPost("{isbn}/Checkout")]
    public async Task<IActionResult> Checkout([FromRoute] string isbn)
    {
        await sender.Send(new CheckoutBookCommand(isbn));
        return AcceptedAtAction(nameof(GetByIsbn), new { isbn }, null);
    }

    [HttpPost("{isbn}/Return")]
    public async Task<IActionResult> Return([FromRoute] string isbn)
    {
        await sender.Send(new ReturnBookCommand(isbn));
        return AcceptedAtAction(nameof(GetByIsbn), new { isbn }, null);
    }

    [HttpPost("{isbn}/PutOnShelf")]
    public async Task<IActionResult> PutOnShelf([FromRoute] string isbn)
    {
        await sender.Send(new PutBookOnShelfCommand(isbn));
        return AcceptedAtAction(nameof(GetByIsbn), new { isbn }, null);
    }

    [HttpPost("{isbn}/MarkAsDamaged")]
    public async Task<IActionResult> MarkAsDamaged([FromRoute] string isbn)
    {
        await sender.Send(new MarkBookAsDamagedCommand(isbn));
        return AcceptedAtAction(nameof(GetByIsbn), new { isbn }, null);
    }

    [HttpPut("{isbn}")]
    public async Task<IActionResult> UpdateBook([FromRoute] string isbn, UpdateBookCommand command)
    {
        await sender.Send(command, HttpContext.RequestAborted);
        return AcceptedAtAction(nameof(GetByIsbn), new { isbn }, null);
    }

    [HttpDelete("{isbn}")]
    public async Task<IActionResult> DeleteBook([FromRoute] string isbn)
    {
        await sender.Send(new DeleteBookCommand(isbn));
        return NoContent();
    }
}