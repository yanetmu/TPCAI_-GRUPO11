using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Orders.API.Exceptions;

namespace Orders.API.ExceptionHandlers
{
    public class OrderExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            int statusCode = 500;
            string errorCode = "ORD-007";
            string errorMessage = "Error interno al procesar la orden.";

            if (exception is OrderException orderException)
            {
                statusCode = orderException.StatusCode;
                errorCode = orderException.ErrorCode;
                errorMessage = orderException.Message;
            }

            ProblemDetails problema = new ProblemDetails
            {
                Status = statusCode,
                Title = statusCode == 500 ? "Internal Server Error" : "Error",
                Detail = errorMessage,
                Instance = httpContext.Request.Path
            };

            problema.Extensions["errorCode"] = errorCode;
            problema.Extensions["errorMessage"] = errorMessage;

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problema,
                cancellationToken);

            return true;
        }
    }
}