using System.Net;

namespace backend_api.Exceptions
{
    /// <summary>
    /// Exception thrown when the authenticated user does not have permission to access the requested resource.
    /// Maps to HTTP 403 Forbidden.
    /// </summary>
    public class ForbiddenException : AppException
    {
        /// <summary>
        /// Initializes a new instance with default or custom message.
        /// </summary>
        public ForbiddenException(string message = "You do not have permission to access this resource.")
            : base(message, HttpStatusCode.Forbidden)
        {
        }
    }
}
