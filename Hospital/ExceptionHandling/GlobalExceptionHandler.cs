using System.Net;
using BLL.DTOs.Exception;
using BLL.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace PL.ExceptionHandling;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
        Exception exception, CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            InvalidOperationException => HttpStatusCode.BadRequest,
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            IdentityValidationException => HttpStatusCode.BadRequest,
            _ => HttpStatusCode.InternalServerError
        };
        
        httpContext.Response.StatusCode = (int)statusCode;

        if (exception is IdentityValidationException ex)
        {
            await httpContext.Response.WriteAsJsonAsync(new ErrorResponse
            {
                Message = "Validation failed",
                Errors = new Dictionary<string, string[]>
                    { { "password", ex.Errors.Select(e => e.Description).ToArray() } },
                StatusCode = (int) HttpStatusCode.BadRequest
            });
            
            return true;
        }
        
        await httpContext.Response.WriteAsJsonAsync(new ErrorResponse
        {
            Message = exception.Message,
            StatusCode = (int)statusCode
        });

        return true;
    }
}