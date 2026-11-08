using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Users.API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode = 500;

            string errorCode = "USR-006";
            string errorMessage = "Error inesperado del servidor";

            if (exception is UserException userException)
            {
                statusCode = userException.StatusCode;
                errorCode = userException.ErrorCode;
                errorMessage = userException.Message;
            }

            string correlationId =
                httpContext.Items["CorrelationId"]?.ToString() ?? "";

            string endpoint = httpContext.Request.Path.ToString();

            // Registrar los errores de negocio como Warning
            if (exception is UserException)
            {
                Log.Warning(
                    "Error de negocio. Endpoint: {Endpoint}, CorrelationId: {CorrelationId}, errorCode: {ErrorCode}, StatusCode: {StatusCode}",
                    endpoint, correlationId, errorCode, statusCode);
            }
            else
            {
                // Registrar los errores inesperados como Error
                Log.Error(
                    exception,
                    "Error inesperado. Endpoint: {Endpoint}, CorrelationId: {CorrelationId}, errorCode: {ErrorCode}, StatusCode: {StatusCode}",
                    endpoint, correlationId, errorCode, statusCode);
            }

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = errorMessage,
                Detail = errorMessage
            };

            problem.Extensions["errorCode"] = errorCode;
            problem.Extensions["errorMessage"] = errorMessage;
            problem.Extensions["correlationId"] = correlationId;

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);

            return true;
        }
    }
}

