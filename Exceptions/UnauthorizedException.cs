using System.Net;

namespace backend_api.Exceptions
{
    /// <summary>
    /// Exception thrown when authentication fails or valid credentials are not provided.
    /// Maps to HTTP 401 Unauthorized.
    /// </summary>
    public class UnauthorizedException : AppException
    {
        /// <summary>
        /// Initializes a new instance with default or custom message.
        /// </summary>
        public UnauthorizedException(string message = "Invalid credentials or unauthenticated.")
            : base(message, HttpStatusCode.Unauthorized)
        {
        }
    }
}
