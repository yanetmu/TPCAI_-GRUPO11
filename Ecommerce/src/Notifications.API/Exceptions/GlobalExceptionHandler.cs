
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Notifications.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode = 500;
            string codigo = "NTF-004";
            string mensaje = "Error interno del servidor.";

            if (exception is ValidationException)
            {
                statusCode = 400;
                codigo = "NTF-002";
                mensaje = exception.Message;
            }
            if (exception is NotFoundException)
            {
                NotFoundException error = (NotFoundException)exception;

                statusCode = 404;
                codigo = error.ErrorCode;
                mensaje = error.Message;
            }

            if (statusCode == 500)
            {
                Log.Error(exception, "Error interno. Código: {ErrorCode}", codigo);
            }
            else
            {
                Log.Warning("Error controlado. Código: {ErrorCode}. Mensaje: {ErrorMessage}",
                    codigo, mensaje);
            }
            httpContext.Response.StatusCode = statusCode;

            ProblemDetails problema = new ProblemDetails();
            problema.Type = "about:blank";
            problema.Status = statusCode;
            problema.Title = "Error";
            problema.Detail = mensaje;
            problema.Instance = httpContext.Request.Path;
            problema.Extensions["errorCode"] = codigo;
            problema.Extensions["errorMessage"] = mensaje;
            problema.Extensions["correlationId"] =
    httpContext.Items["CorrelationId"]?.ToString() ?? "";

            await httpContext.Response.WriteAsJsonAsync(
                problema, cancellationToken);

            return true;
        }
    }
}
