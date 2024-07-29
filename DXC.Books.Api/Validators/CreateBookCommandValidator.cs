using DXC.Books.Api.Commands;
using FluentValidation;

namespace DXC.Books.Api.Validators;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Isbn)
            .NotEmpty()
            .Matches(@"^\d{9}[0-9xX]|\d{13}$")
            .WithMessage("{PropertyName} have to be valid ISBN number");
        RuleFor(x => x.Author)
            .NotEmpty()
            .MaximumLength(500);
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);
    }
}