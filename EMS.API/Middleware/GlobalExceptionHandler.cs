using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EMS.Application.Exceptions;

namespace EMS.API.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler                                     // Custom middleware class that implements IExceptionHandler(.NET 8’s built-in contract) to handle exceptions for the entire application
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)                   // ILogger is the framework’s built-in logging service, and we use it to record serious errors. This logger is injected into the middleware through dependency injection.
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(                                            // The method ASP.NET Core calls automatically whenever any exception escapes any controller. It receives the request context and the exception that was thrown.
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, message) = exception switch                                        // A switch expression that picks a status code and message based on the exception’s type, like a sorting desk reading a label and sending each problem to the right counter.
            {
                ConflictException => (StatusCodes.Status409Conflict, exception.Message),                                    // If the exception is a ConflictException, set the status code to 409 Conflict and use the exception's message.
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message),                      // If the exception is an UnauthorizedAccessException means wrong login details, set the status code to 401 Unauthorized and use the exception's message.
                ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),                                  // If the exception is an ArgumentException means invalid attendance status, invalid month, etc., set the status code to 400 Bad Request and use the exception's message.
                DbUpdateException => (StatusCodes.Status409Conflict,                                                        // If the exception is a DbUpdateException means a database update error (EF Core throws this when MySQL rejects a save), set the status code to 409 Conflict and use a custom message indicating that the change could not be saved due to potential duplication or reference issues.
                    "The change could not be saved. A value may be duplicated or refer to a record that does not exist."),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")                            // For any other exception type(_), set the status code to 500 Internal Server Error and use a generic error message.
            };

            if (statusCode == StatusCodes.Status500InternalServerError)                         // If the status code is 500 Internal Server Error, log the exception details for further investigation.
                _logger.LogError(exception, "Unhandled exception");

            httpContext.Response.StatusCode = statusCode;                                       // sets the HTTP status.
            await httpContext.Response.WriteAsJsonAsync(new { message }, cancellationToken);    // writes a JSON response({ "message": "..." }) containing the error message to the HTTP response body.

            return true;                                                                        // tells ASP.NET Core “I handled this, stop looking for other handlers”
        }
    }
}