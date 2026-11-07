
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = errorMessage,
                Detail = errorMessage
            };

            problem.Extensions["errorCode"] = errorCode;
            problem.Extensions["errorMessage"] = errorMessage;

            httpContext.Response.StatusCode = statusCode;

            await httpContext.Response.WriteAsJsonAsync(
                problem,
                cancellationToken);

            return true;
        }
    }
}
