using Stateless;

namespace DXC.Books.Domain;

public class Book
{
    private readonly StateMachine<Status, Trigger> _stateMachine;

    public string Title { get; private set; }
    public string Author { get; private set; }
    public string Isbn { get; private set; }
    public Status Status { get; private set; }

    public Book(string title, string author, string isbn, Status status)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(title));
        if (string.IsNullOrWhiteSpace(author))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(author));
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(isbn));
        Title = title;
        Author = author;
        Isbn = isbn;
        Status = status;
        _stateMachine = new StateMachine<Status, Trigger>(() => Status, s => Status = s);
        _stateMachine.Configure(Status.OnShelf)
            .Permit(Trigger.CheckOut, Status.CheckedOut)
            .Permit(Trigger.MarkAsDamaged, Status.Damaged);
        _stateMachine.Configure(Status.CheckedOut)
            .Permit(Trigger.Return, Status.Returned);
        _stateMachine.Configure(Status.Returned)
            .Permit(Trigger.MarkAsDamaged, Status.Damaged)
            .Permit(Trigger.PuttedOnShelf, Status.OnShelf);
        _stateMachine.Configure(Status.Damaged)
            .Permit(Trigger.PuttedOnShelf, Status.OnShelf);
    }

    public void MarkAsDamaged() => SetStatus(Trigger.MarkAsDamaged);
    public void CheckOut() => SetStatus(Trigger.CheckOut);
    public void PutOnShelf() => SetStatus(Trigger.PuttedOnShelf);
    public void Return() => SetStatus(Trigger.Return);

    private void SetStatus(Trigger trigger)
    {
        _stateMachine.Fire(trigger);
    }

    private enum Trigger
    {
        CheckOut,
        Return,
        MarkAsDamaged,
        PuttedOnShelf
    }

    public void Update(string title, string author, string isbn)
    {
        Title = title;
        Author = author;
        Isbn = isbn;
    }
}