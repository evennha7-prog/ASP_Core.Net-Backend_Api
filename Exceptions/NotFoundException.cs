using System.Net;

namespace backend_api.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested resource (user, note) cannot be found.
    /// Maps to HTTP 404 Not Found.
    /// </summary>
    public class NotFoundException : AppException
    {
        /// <summary>
        /// Initializes a new instance with a custom message.
        /// </summary>
        public NotFoundException(string message)
            : base(message, HttpStatusCode.NotFound)
        {
        }

        /// <summary>
        /// Initializes a new instance specifying the entity name and key.
        /// </summary>
        public NotFoundException(string entityName, object key)
            : base($"Entity \"{entityName}\" with key ({key}) was not found.", HttpStatusCode.NotFound)
        {
        }
    }
}
