using System.Net;
using System.Text.Json;
using backend_api.DTOs.Common;
using backend_api.Exceptions;

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
        private readonly IHostEnvironment _env;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        /// <summary>
        /// Initializes the exception middleware with dependencies.
        /// </summary>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Executes the middleware pipeline within a try/catch block.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Translates known exception types into standard HTTP status codes and serializes an ErrorResponse.
        /// </summary>
        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message, errors) = exception switch
            {
                ValidationException valEx => (
                    (int)HttpStatusCode.BadRequest,
                    valEx.Message,
                    valEx.Errors
                ),
                UnauthorizedException unauthEx => (
                    (int)HttpStatusCode.Unauthorized,
                    unauthEx.Message,
                    null
                ),
                UnauthorizedAccessException unauthAccEx => (
                    (int)HttpStatusCode.Unauthorized,
                    unauthAccEx.Message,
                    null
                ),
                ForbiddenException forbEx => (
                    (int)HttpStatusCode.Forbidden,
                    forbEx.Message,
                    null
                ),
                NotFoundException notFoundEx => (
                    (int)HttpStatusCode.NotFound,
                    notFoundEx.Message,
                    null
                ),
                ConflictException confEx => (
                    (int)HttpStatusCode.Conflict,
                    confEx.Message,
                    null
                ),
                KeyNotFoundException keyEx => (
                    (int)HttpStatusCode.NotFound,
                    keyEx.Message,
                    null
                ),
                ArgumentException argEx => (
                    (int)HttpStatusCode.BadRequest,
                    argEx.Message,
                    null
                ),
                ApplicationException appEx => (
                    (int)HttpStatusCode.BadRequest,
                    appEx.Message,
                    null
                ),
                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    "An unexpected error occurred. Please try again later.",
                    null
                )
            };

            context.Response.StatusCode = statusCode;

            var errorResponse = new ErrorResponse
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Errors = errors,
                Details = _env.IsDevelopment() && statusCode == (int)HttpStatusCode.InternalServerError
                    ? exception.ToString()
                    : null
            };

            var json = JsonSerializer.Serialize(errorResponse, JsonOptions);
            return context.Response.WriteAsync(json);
        }
    }
}
