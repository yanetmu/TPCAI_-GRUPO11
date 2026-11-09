using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode = 500;
            string errorCode = "CRT-005";
            string errorMessage = "Error interno del servidor.";

            if (exception is NotFoundException)
            {
                statusCode = 404;
                errorCode = "CRT-001";
                errorMessage = exception.Message;
            }

            if (exception is ArgumentException)
            {
                statusCode = 400;
                errorCode = "CRT-004";
                errorMessage = "La cantidad debe ser mayor a cero.";
            }

            if (exception is KeyNotFoundException)
            {
                statusCode = 404;
                errorCode = "CRT-002";
                errorMessage = "Producto no encontrado.";
            }

            if (exception is InvalidOperationException)
            {
                statusCode = 422;
                errorCode = "CRT-003";
                errorMessage = "Stock insuficiente.";
            }


            httpContext.Response.StatusCode = statusCode;

            ProblemDetails problema = new ProblemDetails();
            problema.Status = statusCode;
            problema.Title = errorMessage;
            problema.Extensions["errorCode"] = errorCode;
            problema.Extensions["errorMessage"] = errorMessage;

            await httpContext.Response.WriteAsJsonAsync(
                problema,
                cancellationToken);

            return true;
        }
    }
}
