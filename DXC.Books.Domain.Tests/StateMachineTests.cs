using FluentAssertions;

namespace DXC.Books.Domain.Tests;

public class StateMachineTests
{
    [Theory]
    [MemberData(nameof(GetHappyPathData))]
    public void HappyPath(Status initial, OperationDelegate operation, Status expected)
    {
        var sut = new Book("Drużyna Pierścienia", "J.R.R. Tolkien", "9788324144242", initial);
        operation(sut);
        sut.Status.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(GetInvalidTransitions))]
    public void IllegalTransitions(Status initial, OperationDelegate operation)
    {
        var sut = new Book("Drużyna Pierścienia", "J.R.R. Tolkien", "9788324144242", initial);
        var act = () => operation(sut);
        act.Should().Throw<InvalidOperationException>();
    }

    public static IEnumerable<object[]> GetHappyPathData()
    {
        yield return [Status.OnShelf, new OperationDelegate(book => book.CheckOut()), Status.CheckedOut];
        yield return [Status.CheckedOut, new OperationDelegate(book => book.Return()), Status.Returned];
        yield return [Status.Returned, new OperationDelegate(book => book.MarkAsDamaged()), Status.Damaged];
        yield return [Status.Returned, new OperationDelegate(book => book.PutOnShelf()), Status.OnShelf];
        yield return [Status.Damaged, new OperationDelegate(book => book.PutOnShelf()), Status.OnShelf];
    }

    public static IEnumerable<object[]> GetInvalidTransitions()
    {
        yield return [Status.CheckedOut, new OperationDelegate(book => book.MarkAsDamaged())];
        yield return [Status.Damaged, new OperationDelegate(book => book.CheckOut())];
        yield return [Status.Returned, new OperationDelegate(book => book.CheckOut())];
    }

    public delegate void OperationDelegate(Book book);
}