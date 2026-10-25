using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Products.API.Exceptions;

namespace Products.API.ExceptionHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, errorCode, errorMessage) = exception switch
            {
                NotFoundException ex =>
                    (StatusCodes.Status404NotFound, "PRD-002", ex.Message),

                BusinessRuleException ex =>
                    (StatusCodes.Status409Conflict, "PRD-005", ex.Message),

                Products.API.Exceptions.ValidationException ex =>
                    (StatusCodes.Status400BadRequest, "PRD-003", ex.Message),

                _ =>
                    (StatusCodes.Status500InternalServerError, "PRD-001",
                        "Ocurrió un error interno.")
            };

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                new
                {
                    errorCode,
                    errorMessage
                },
                cancellationToken);

            return true;
        }
    }
}
