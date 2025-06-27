using Bonfire.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Middlewares;

public class ExceptionMiddleware : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        switch (exception)
            {
            // Обработка ваших кастомных исключений
            case BaseException baseException:
                var baseProblemDetails = new ProblemDetails
                {
                    Status = (int)baseException.ErrorCode,
                    Title = baseException.Message
                };
                httpContext.Response.StatusCode = baseProblemDetails.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(baseProblemDetails, cancellationToken);
                return true;

            // Обработка ошибок валидации FluentValidation
            case ValidationException validationException:
                var validationProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = validationException.Errors.FirstOrDefault().ErrorMessage,
                };
                httpContext.Response.StatusCode = validationProblemDetails.Status.Value;
                await httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
                return true;

            default:
                return false;
        }
    }
}