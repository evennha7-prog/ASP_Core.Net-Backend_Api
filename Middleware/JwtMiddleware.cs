using System.Security.Claims;
using backend_api.Helpers;

namespace backend_api.Middleware
{
    /// <summary>
    /// Middleware that inspects incoming HTTP requests for an "Authorization: Bearer [token]" header,
    /// verifies the token, and attaches the authenticated user's ClaimsPrincipal and UserId to the HttpContext.
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Initializes the JWT middleware.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Intercepts the request, validates the Bearer token, and populates HttpContext.User and HttpContext.Items["UserId"].
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        /// <param name="jwtHelper">Injected JWT helper for token validation.</param>
        public async Task InvokeAsync(HttpContext context, JwtHelper jwtHelper)
        {
            // Extract the Authorization header
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var token = authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true
                ? authHeader.Substring("Bearer ".Length).Trim()
                : null;

            // If a token was supplied, validate it
            if (!string.IsNullOrEmpty(token))
            {
                var principal = jwtHelper.ValidateToken(token);
                if (principal != null)
                {
                    // Attach user principal to HttpContext
                    context.User = principal;

                    // Extract the UserId claim and store in HttpContext.Items for fast access in controllers
                    var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(userIdClaim, out var userId))
                    {
                        context.Items["UserId"] = userId;
                    }
                }
            }

            // Continue through the pipeline
            await _next(context);
        }
    }
}
