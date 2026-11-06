
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
            int statusCode;
            string errorCode;
            string errorMessage;
            string titulo;
            string detalle;
            string tipo;

            if (exception is NotFoundException)
            {
                statusCode = 404;
                errorCode = "PRD-001";
                errorMessage = exception.Message;
                titulo = "Not Found";
                detalle = "El recurso solicitado no fue encontrado.";
                tipo = "https://tools.ietf.org/html/rfc7231#section-6.5.4";
            }
            else if (exception is BusinessRuleException)
            {
                BusinessRuleException error = (BusinessRuleException)exception;

                statusCode = 409;
                errorCode = error.ErrorCode;
                errorMessage = error.Message;
                titulo = "Conflict";

                if (errorCode == "PRD-004")
                {
                    detalle = "No se puede eliminar el recurso.";
                }
                else
                {
                    detalle = "Ya existe un recurso con esos datos.";
                }

                tipo = "https://tools.ietf.org/html/rfc7231#section-6.5.9";
            }
            else if (exception is Products.API.Exceptions.ValidationException)
            {
                statusCode = 400;
                errorCode = "PRD-002";
                errorMessage = exception.Message;
                titulo = "Bad Request";
                detalle = "Los datos del producto son inválidos.";
                tipo = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
            }
            else
            {
                statusCode = 500;
                errorCode = "PRD-005";
                errorMessage = "Error interno al procesar el producto.";
                titulo = "Internal Server Error";
                detalle = "Ocurrió un error interno.";
                tipo = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            }

            var logger = httpContext.RequestServices
                .GetRequiredService<ILogger<GlobalExceptionHandler>>();

            if (statusCode == 500)
            {
                logger.LogError(exception,
                    "Error interno {ErrorCode}", errorCode);
            }
            else
            {
                logger.LogWarning(
                    "Error de negocio {ErrorCode}: {ErrorMessage}",
                    errorCode, errorMessage);
            }

            var problem = new ProblemDetails();

            problem.Type = tipo;
            problem.Title = titulo;
            problem.Status = statusCode;
            problem.Detail = detalle;
            problem.Instance = httpContext.Request.Path;

            problem.Extensions["errorCode"] = errorCode;
            problem.Extensions["errorMessage"] = errorMessage;
            problem.Extensions["correlationId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken: cancellationToken);

            return true;
        }
    }
}
