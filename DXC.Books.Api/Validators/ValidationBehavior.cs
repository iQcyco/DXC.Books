using FluentValidation;
using MediatR;

namespace DXC.Books.Api.Validators;

public class ValidationBehavior<TRequest, TResponse>(IValidator<TRequest>? requestValidator = null)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (requestValidator != null)
        {
            var validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);
        }

        return await next();
    }
}