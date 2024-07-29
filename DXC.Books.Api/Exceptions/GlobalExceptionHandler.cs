using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DXC.Books.Api.Exceptions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            NotFoundException e => new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Not found",
                Detail = e.Message
            },
            ValidationException e => new ValidationProblemDetails()
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation error",
                Errors = e.Errors.ToDictionary(x => x.PropertyName, x => new[] { x.ErrorMessage }),
                Detail = e.Message
            },
            DbUpdateException e => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Database update error",
                Detail = e.InnerException?.Message ?? e.Message
            },
            InvalidOperationException e => new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid operation",
                Detail = e.Message
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal server error",
                Detail = exception.Message
            }
        };

        httpContext.Response.StatusCode = problem.Status.Value;

        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}