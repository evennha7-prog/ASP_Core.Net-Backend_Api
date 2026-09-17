using System.Net;
using System.Text.Json;

namespace backend_api.Middleware
{
    /// <summary>
    /// Global exception-handling middleware that catches all unhandled exceptions occurring
    /// throughout the HTTP request processing pipeline, logs them, and formats a consistent JSON error response.
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        /// <summary>
        /// Initializes the exception middleware with the next delegate and logger.
        /// </summary>
        /// <param name="next">The next middleware in the ASP.NET pipeline.</param>
        /// <param name="logger">Logger instance for capturing error traces.</param>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Executes the middleware pipeline within a try/catch block.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Translates known exception types into appropriate HTTP status codes and serializes a JSON payload.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="exception">The unhandled exception caught.</param>
        /// <returns>A Task representing the asynchronous write operation.</returns>
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // Map custom application exception types to HTTP status codes
            var statusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401
                ApplicationException => (int)HttpStatusCode.BadRequest,          // 400
                ArgumentException => (int)HttpStatusCode.BadRequest,             // 400
                KeyNotFoundException => (int)HttpStatusCode.NotFound,            // 404
                _ => (int)HttpStatusCode.InternalServerError                     // 500
            };

            context.Response.StatusCode = statusCode;

            // Structure a clean JSON error response
            var response = new
            {
                success = false,
                statusCode = statusCode,
                message = exception.Message,
                details = statusCode == 500 ? "An unexpected server error occurred." : null
            };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(json);
        }
    }
}
