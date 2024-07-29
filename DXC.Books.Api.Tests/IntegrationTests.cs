using System.Net;
using System.Net.Http.Json;
using DXC.Books.Api.Commands;
using DXC.Books.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace DXC.Books.Api.Tests;

[TestCaseOrderer(ordererTypeName: "DXC.Books.Api.Tests.PriorityOrderer", ordererAssemblyName: "DXC.Books.Api.Tests")]
public class IntegrationTests : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory;

    public IntegrationTests(TestWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory, TestPriority(0)]
    [MemberData(nameof(GetData))]
    public async Task CreateResources(string url, CreateBookCommand command)
    {
        var client = _factory.CreateClient();
        var message = await client.PostAsJsonAsync(url, command);
        message.EnsureSuccessStatusCode();
    }


    [Theory, TestPriority(1)]
    [InlineData("/books")]
    [InlineData("/books/9788324144242")]
    public async Task Get_EndpointsReturnSuccess(string url)
    {
        var client = _factory.CreateClient();
        var message = await client.GetAsync(url);
        message.EnsureSuccessStatusCode();
    }

    [Fact, TestPriority(2)]
    public async Task SortingTest()
    {
        var client = _factory.CreateClient();
        var message = await client.GetAsync("/Books?sort=author");
        message.EnsureSuccessStatusCode();
        var pagedList = await message.Content.ReadFromJsonAsync<PagedList<BookDto>>();
        pagedList.Should().NotBeNull();
        pagedList.Data.Should().BeInAscendingOrder(x => x.Author);
        pagedList.TotalCount.Should().Be(4);
    }

    [Fact, TestPriority(2)]
    public async Task ValidationExceptionTest()
    {
        var client = _factory.CreateClient();
        var message = await client.PostAsJsonAsync("/Books", new CreateBookCommand("324", "A", "B"));
        message.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await message.Content.ReadAsStringAsync();
        problem.Should().NotBeNull();
    }
    
    [Fact, TestPriority(2)]
    public async Task UniqueViolationTest()
    {
        var client = _factory.CreateClient();
        var message = await client.PostAsJsonAsync("/Books", new CreateBookCommand("9788324144242", "J.R.R. Tolkien", "Drużyna Pierścienia"));
        message.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await message.Content.ReadFromJsonAsync<ProblemDetails>();
        problem.Should().NotBeNull();
        problem.Title.Should().Be("Database update error");
    }

    public static IEnumerable<object[]> GetData()
    {
        yield return ["/books", new CreateBookCommand("9788324144242", "J.R.R. Tolkien", "Drużyna Pierścienia")];
        yield return ["/books", new CreateBookCommand("9788377856819", "J.R.R. Tolkien", "Dwie wieże")];
        yield return ["/books", new CreateBookCommand("9788382024906", "J.R.R. Tolkien", "Powrót Króla")];
        yield return ["/books", new CreateBookCommand("9788381881388", "Frank Herbert", "Diuna")];
    }
}