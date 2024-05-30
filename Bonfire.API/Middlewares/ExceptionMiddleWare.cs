using Bonfire.Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bonfire.API.Middlewares;

public class ExceptionMiddleWare: IExceptionHandler
{
    private readonly ILogger<ExceptionMiddleWare> _logger;

    public ExceptionMiddleWare(ILogger<ExceptionMiddleWare> logger) => _logger = logger;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BaseException baseException)
        {
            return false;
        }

        var problemDetails = new ProblemDetails
        {
            Status = (int) baseException.ErrorCode,
            Title = baseException.Message
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}