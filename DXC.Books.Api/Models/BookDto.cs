namespace DXC.Books.Api.Models;

public class BookDto
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Isbn { get; set; }
    public StatusDto Status { get; set; }
}