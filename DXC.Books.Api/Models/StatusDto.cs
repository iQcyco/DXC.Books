using System.Text.Json.Serialization;

namespace DXC.Books.Api.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum StatusDto
{
    OnShelf,
    CheckedOut,
    Returned,
    Damaged
}