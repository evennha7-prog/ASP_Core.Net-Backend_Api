using System.Net;

namespace backend_api.Exceptions
{
    /// <summary>
    /// Exception thrown when an operation conflicts with the current state of a resource
    /// (e.g. attempting to register an already taken username or email address).
    /// Maps to HTTP 409 Conflict.
    /// </summary>
    public class ConflictException : AppException
    {
        /// <summary>
        /// Initializes a new instance with a custom message.
        /// </summary>
        public ConflictException(string message)
            : base(message, HttpStatusCode.Conflict)
        {
        }
    }
}
