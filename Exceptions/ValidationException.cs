using System.Net;

namespace backend_api.Exceptions
{
    /// <summary>
    /// Exception thrown when domain validation rules or request payloads fail validation.
    /// Maps to HTTP 400 Bad Request.
    /// </summary>
    public class ValidationException : AppException
    {
        /// <summary>
        /// Optional field-specific validation errors.
        /// </summary>
        public IDictionary<string, string[]>? Errors { get; }

        /// <summary>
        /// Initializes a new instance with a custom validation message.
        /// </summary>
        public ValidationException(string message)
            : base(message, HttpStatusCode.BadRequest)
        {
        }

        /// <summary>
        /// Initializes a new instance with field-level errors dictionary.
        /// </summary>
        public ValidationException(string message, IDictionary<string, string[]> errors)
            : base(message, HttpStatusCode.BadRequest)
        {
            Errors = errors;
        }
    }
}
